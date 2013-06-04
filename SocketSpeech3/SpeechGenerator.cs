using System;
using System.Collections.Generic;
using System.Threading;
using Robotics;

namespace SocketSpeech3
{
	/// <summary>
	/// Enumerates the result of a speech synthesis operation
	/// </summary>
	public enum SpeakOperationResult
	{
		/// <summary>
		/// Speech synthesis completed successfully
		/// </summary>
		Succeeded,
		/// <summary>
		/// Speech synthesis canceled
		/// </summary>
		Canceled,
		/// <summary>
		/// Error durin speech synthesis operation
		/// </summary>
		Error

	};

	public delegate void SpeakStartedEventHandler(SpeechGenerator sender, string textToSay);

	public delegate void SpeakCompletedEventHandler(SpeechGenerator sender, string spokenString, SpeakOperationResult result);

	public delegate void VoiceChangedEventHandler(SpeechGenerator sender, string voiceName);

	//public delegate void SpeakProgressEventHandler(SpeechGenerator sender, SpeakProgressEventArgs e, double percentage);
	public delegate void SpeakProgressEventHandler(SpeechGenerator sender, double percentage);

	/// <summary>
	/// Implements a Speech Generator
	/// </summary>
	public abstract class SpeechGenerator
	{
		#region Variables

		/// <summary>
		/// Correspondence list between voice names and voices
		/// </summary>
		protected readonly SortedList<string, int> voiceNames;

		/// <summary>
		/// Stores a list of replacements used as pronunciation rules
		/// </summary>
		protected readonly PronunciationRuleList pronunciation;

		/// <summary>
		/// Thread to control aync speech 
		/// </summary>
		protected readonly Thread speechThread;

		/// <summary>
		/// Queue for store pending text to be spoken
		/// </summary>
		protected readonly ProducerConsumer<SpeechTextTask> speechQueue;

		/// <summary>
		/// Stores the result of the last speech synthesis operation
		/// </summary>
		protected SpeakOperationResult lastSpeechResult;

		/// <summary>
		/// Flag that indicates when the Speech Synthesizer is working
		/// </summary>
		private bool speaking;

		/// <summary>
		/// Gets the string which is currently spoken
		/// </summary>
		private string spokenString;

		/// <summary>
		/// Indicates if the SpeechThread is running
		/// </summary>
		protected bool running;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SpeechGenerator
		/// </summary>
		public SpeechGenerator()
		{
			this.voiceNames = new SortedList<string, int>(20, StringComparer.OrdinalIgnoreCase);
			this.pronunciation = new PronunciationRuleList(200);
			this.speechQueue = new ProducerConsumer<SpeechTextTask>(100);
			this.speechThread = new Thread(new ThreadStart(SpeechThreadTask));
			this.speechThread.IsBackground = true;
			this.speechThread.Start();
		}

		/// <summary>
		/// Destructor. It will stop threads and cancel speech
		/// </summary>
		~SpeechGenerator()
		{
			running = false;
			ShutUp();
			if (this.speechThread != null)
			{
				if (speechThread.ThreadState == ThreadState.WaitSleepJoin)
				{
					speechThread.Interrupt();
					speechThread.Join(102);
				}

				if (this.speechThread.IsAlive)
					this.speechThread.Abort();
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the SpeechGenerator begins to speak
		/// </summary>
		public event SpeakStartedEventHandler SpeakStarted;
		/// <summary>
		/// Occurs when the SpeechGenerator completes a speaking operation
		/// </summary>
		public event SpeakCompletedEventHandler SpeakCompleted;
		/// <summary>
		/// Occurs when the selected voice is changed
		/// </summary>
		public event VoiceChangedEventHandler VoiceChanged;
		public event SpeakProgressEventHandler SpeakProgress;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the Speech Synthesizer is working
		/// </summary>
		public bool Speaking
		{
			get { return speaking; }
			protected set
			{
				this.speaking = value;
			}
		}

		/// <summary>
		/// Gets the number of pending asynchronous speech tasks
		/// </summary>
		public int Pending
		{
			get
			{
				int count = 0;
				if (speaking)
					++count;
				count += speechQueue.Count;
				return count;
			}
		}

		/// <summary>
		/// Gets a list of replacements used as pronunciation rules
		/// </summary>
		public PronunciationRuleList Pronunciation
		{
			get { return this.pronunciation; }
		}

		/// <summary>
		/// Gets or sets the index of selected voice
		/// </summary>
		public abstract int SelectedVoiceIndex
		{
			get;
			set;

		}

		/// <summary>
		/// Gets or sets the name of selected voice
		/// </summary>
		public virtual string SelectedVoiceName
		{
			get { return VoiceNames[SelectedVoiceIndex]; }
			set
			{
				int index = VoiceNames.IndexOf(value);
				if(index == -1)
					throw new ArgumentException("Invalid voice name");
				SelectedVoiceIndex = index;
			}
		}

		/// <summary>
		/// Gets a list of strings containing the voice names
		/// </summary>
		public IList<string> VoiceNames
		{
			get { return this.voiceNames.Keys; }
		}

		/// <summary>
		/// Gets the current string sent to tts
		/// </summary>
		public string SpokenString
		{
			get { return spokenString; }
			protected set { this.spokenString = value; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Loads the voices installed on the system
		/// </summary>
		protected abstract void LoadVoices();

		/// <summary>
		/// Cancels all speech synthesis operations
		/// </summary>
		public abstract void ShutUp();

		/// <summary>
		/// Executes a TTS sinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was spoken, false otherwise</returns>
		public virtual bool SpeakSync(string textToSay)
		{
			return Speak(new SpeechTextTask(textToSay));
		}

		/// <summary>
		/// Enqueues provided text to perform a TTS asinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was enqueued, false otherwise</returns>
		public virtual bool SpeakAsync(string textToSay)
		{
			//if (speechQueue.Count >= speechQueue.Capacity)
			//	return false;
			speechQueue.Produce(new SpeechTextTask(textToSay));
			return true;
		}

		/// <summary>
		/// Enqueues provided SpeechTextTask to perform a TTS asinchronously
		/// </summary>
		/// <param name="textToSay">SpeechTextTask cotaining the text to speech out</param>
		/// <returns>true if provided text was enqueued, false otherwise</returns>
		public virtual bool SpeakAsync(SpeechTextTask task)
		{
			if (task == null)
				return false;
			//if (speechQueue.Count >= speechQueue.Capacity)
			//	return false;
			speechQueue.Produce(task);
			return true;
		}

		/// <summary>
		/// Raises the SpeakStarted event
		/// </summary>
		/// <param name="textToSay"></param>
		protected void OnSpeakStarted(string textToSay)
		{
			if (SpeakStarted != null)
				SpeakStarted(this, textToSay);
		}

		/// <summary>
		/// Raises the SpeakCompleted event
		/// </summary>
		/// <param name="spokenString"></param>
		/// <param name="operationResult"></param>
		protected void OnSpeakCompleted(string spokenString, SpeakOperationResult operationResult)
		{
			if (SpeakCompleted != null)
				SpeakCompleted(this, spokenString, operationResult);
		}

		/// <summary>
		/// Raises the VoiceChanged event
		/// </summary>
		protected void OnVoiceChanged()
		{
			if (VoiceChanged != null)
				VoiceChanged(this, this.SelectedVoiceName);
		}

		/// <summary>
		/// Raises the SpeakProgress event
		/// </summary>
		/// <param name="e"></param>
		//protected void OnSpeakProgress(SpeakProgressEventArgs e)
		protected void OnSpeakProgress(double percentage)
		{
			SpeakProgress(this, percentage);
		}

		/// <summary>
		/// Converts the TTS generated from provided text to a wavefile
		/// </summary>
		/// <param name="textToSay">Text to speech out</param>
		/// <param name="path">Path of the file to save waveform</param>
		/// <returns>true if file was generated successfully</returns>
		public abstract bool SaveTextToWav(string textToSay, string path);
		
		/// <summary>
		/// Executes a TTS sinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was spoken, false otherwise</returns>
		protected abstract bool Speak(SpeechTextTask speechTask);

		protected virtual void SpeechThreadTask()
		{
			running = true;
			#region Voices Initialization
			// Voices Initialization
			//while (running && (voiceNames.Count < 1))
			//{
			//    Thread.Sleep(100);
			//    LoadVoices();
			//}
			#endregion

			while (running)
			{
				try
				{
					SpeechTextTask stt = speechQueue.Consume(100);
					if (stt == null)
						continue;
					Speak(stt);
				}
				catch (ThreadInterruptedException tiex)
				{
					Console.WriteLine("SpeechThread aborted: " + tiex.ToString());
					return;
				}
				catch (ThreadAbortException taex)
				{
					Console.WriteLine("SpeechThread aborted: " + taex.ToString());
					ShutUp();
					return;
				}
				catch { continue; }
			}
			ShutUp();
			speechQueue.Clear();
			
		}

		#endregion
	}
}
