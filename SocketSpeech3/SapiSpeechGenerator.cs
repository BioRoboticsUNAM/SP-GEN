using System;
using System.Threading;
using System.Speech.Synthesis;

namespace SocketSpeech3
{

	/// <summary>
	/// Implements a Speech Generator
	/// </summary>
	public class SapiSpeechGenerator : SpeechGenerator
	{
		#region Variables

		/// <summary>
		/// Index of selected voice
		/// </summary>
		private int selectedVoiceIndex;

		/// <summary>
		/// Name of selected voice
		/// </summary>
		private string selectedVoiceName;

		/// <summary>
		/// Array of installed voices
		/// </summary>
		private InstalledVoice[] voices;

		/// <summary>
		/// The speech synthethizer used for synthesis
		/// </summary>
		private SpeechSynthesizer speechSynth;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SapiSpeechGenerator
		/// </summary>
		public SapiSpeechGenerator()
		{
			selectedVoiceIndex = -1;
			try
			{
				speechSynth = new SpeechSynthesizer();
				speechSynth.SpeakCompleted += new EventHandler<SpeakCompletedEventArgs>(speechSynth_SpeakCompleted);
				speechSynth.SpeakStarted += new EventHandler<SpeakStartedEventArgs>(speechSynth_SpeakStarted);
				//speechSynth.SpeakProgress += new EventHandler<SpeakProgressEventArgs>(speechSynth_SpeakProgress);
			}
			catch { speechSynth = null; }
			LoadVoices();
		}

		/// <summary>
		/// Destructor. It will stop threads and cancel speech
		/// </summary>
		~SapiSpeechGenerator()
		{
			if (this.speechSynth != null)
			{
				try
				{
					speechSynth.SpeakAsyncCancelAll();
					speechSynth.SetOutputToNull();
				}
				catch { }
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the index of selected voice
		/// </summary>
		public override int SelectedVoiceIndex
		{
			get { return this.selectedVoiceIndex; }
			set
			{
				if ((value < 0) || (value >= voices.Length))
					throw new ArgumentOutOfRangeException();
				if (this.selectedVoiceIndex == value) return;
				this.selectedVoiceIndex = value;
				selectedVoiceName = voices[this.selectedVoiceIndex].VoiceInfo.Name;
				OnVoiceChanged();
			}

		}

		/// <summary>
		/// Gets or sets the name of selected voice
		/// </summary>
		public override string SelectedVoiceName
		{
			get { return this.selectedVoiceName; }
			set
			{
				if (value == null)
					throw new ArgumentNullException();
				if (!voiceNames.ContainsKey(value))
					throw new ArgumentException("Invalid voice name");
				if (selectedVoiceName == value) return;

				this.selectedVoiceIndex = voiceNames[value];
				selectedVoiceName = voices[this.selectedVoiceIndex].VoiceInfo.Name;
				OnVoiceChanged();
			}
		}

		/// <summary>
		/// Gets the array of installed voices
		/// </summary>
		public InstalledVoice[] Voices
		{
			get { return this.voices; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Loads the voices installed on the system
		/// </summary>
		protected override void LoadVoices()
		{
			if (speechSynth == null)
				return;
			try
			{
				System.Collections.ObjectModel.ReadOnlyCollection<InstalledVoice> collection = speechSynth.GetInstalledVoices();
				if (collection.Count == 0)
				{
					voices = null;
					return;
				}
				voices = new InstalledVoice[collection.Count];
				//voiceNames = new SortedList<string, int>(collection.Count, StringComparer.OrdinalIgnoreCase);
				for (int i = 0; i < collection.Count; ++i)
				{
					voices[i] = collection[i];
					if (!voiceNames.ContainsKey(voices[i].VoiceInfo.Name))
						voiceNames.Add(voices[i].VoiceInfo.Name, i);
					else
					{
						voiceNames[voices[i].VoiceInfo.Name] = i;
					}

				}
				if (voiceNames.ContainsKey("Susan"))
					SelectedVoiceIndex = voiceNames["Susan"];
				else
					SelectedVoiceIndex = 0;
			}
			catch
			{
				voices = null;
			}
		}

		/// <summary>
		/// Raises the SpeakProgress event
		/// </summary>
		/// <param name="e"></param>
		protected void OnSpeakProgress(SpeakProgressEventArgs e)
		{
			double percentage = (double)SpokenString.Length / (double)e.CharacterCount;
			base.OnSpeakProgress(percentage);
		}

		/// <summary>
		/// Cancels all speech synthesis operations
		/// </summary>
		public override void ShutUp()
		{
			speechQueue.Clear();
			try
			{
				speechSynth.SpeakAsyncCancelAll();
			}
			catch (ObjectDisposedException) { }
		}
		
		/// <summary>
		/// Executes a TTS sinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was spoken, false otherwise</returns>
		public override bool SpeakSync(string textToSay)
		{
			if (speechSynth == null)
				return false;
			return base.SpeakSync(textToSay);
		}

		/// <summary>
		/// Enqueues provided text to perform a TTS asinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was enqueued, false otherwise</returns>
		public override bool SpeakAsync(string textToSay)
		{
			if (speechSynth == null)
				return false;
			return base.SpeakAsync(textToSay);
		}

		/// <summary>
		/// Enqueues provided SpeechTextTask to perform a TTS asinchronously
		/// </summary>
		/// <param name="textToSay">SpeechTextTask cotaining the text to speech out</param>
		/// <returns>true if provided text was enqueued, false otherwise</returns>
		public override bool SpeakAsync(SpeechTextTask task)
		{
			if (speechSynth == null)
				return false;
			return base.SpeakAsync(task);
		}

		/// <summary>
		/// Executes a TTS sinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was spoken, false otherwise</returns>
		protected override bool Speak(SpeechTextTask speechTask)
		{
			bool locked;
			if (speechSynth == null)
				return false;
			if (speechTask == null)
				return false;

			string original = speechTask.TTS;
			string tts = speechTask.TTS;
			//if (selectedVoiceName == "Susan")
			//{
			//    tts = tts.Replace("robot", "row-bought");
			//    tts = tts.Replace("_", " ");
			//    tts = tts.Replace("stop", "stup");
			//}
			for (int i = 0; i < this.pronunciation.Count; ++i)
			{
				tts = this.pronunciation[i].ReplaceWordByPronunciation(tts);
			}

			if ((speechSynth.State != SynthesizerState.Ready) || !Monitor.TryEnter(speechSynth))
				return false;
			locked = true;
			Speaking = true;
			SpokenString = original;
			speechTask.Status = SpeechTextTaskStatus.Speaking;
			lastSpeechResult = SpeakOperationResult.Error;

			try
			{
				OnSpeakStarted(tts);
				if (speechSynth.Voice.Name != selectedVoiceName)
					speechSynth.SelectVoice(selectedVoiceName);
				speechSynth.SpeakAsync(tts);
				Monitor.Wait(speechSynth);
				Monitor.Exit(speechSynth);
				locked = false;
			}
			catch
			{
				if (locked)
					Monitor.Exit(speechSynth);
				Speaking = ((speechSynth.State == SynthesizerState.Speaking) || (speechQueue.Count != 0));
				speechTask.Status = SpeechTextTaskStatus.CompleteFailed;
				return false;
			}
			speechTask.Status = (lastSpeechResult == SpeakOperationResult.Succeeded) ? SpeechTextTaskStatus.CompletedSuccessfully : SpeechTextTaskStatus.CompleteFailed;
			return (lastSpeechResult == SpeakOperationResult.Succeeded);

		}

		/// <summary>
		/// Converts the TTS generated from provided text to a wavefile
		/// </summary>
		/// <param name="textToSay">Text to speech out</param>
		/// <param name="path">Path of the file to save waveform</param>
		/// <returns>true if file was generated successfully</returns>
		public override bool SaveTextToWav(string textToSay, string path)
		{
			SpeechSynthesizer synth;

			textToSay = textToSay.Replace("robot", "row-bought");
			//textToSay = Regex.Replace(textToSay, "robot", "row-bought", RegexOptions.IgnoreCase);
			//textToSay = Regex.Replace(textToSay, "robot", "robought", RegexOptions.IgnoreCase);

			try
			{
				using (synth = new SpeechSynthesizer())
				{
					synth.SelectVoice(voices[selectedVoiceIndex].VoiceInfo.Name);
					synth.SetOutputToWaveFile(path);
					synth.Speak(textToSay);
					return true;
				}

			}
			catch
			{
				return false;
			}
		}

		protected override void SpeechThreadTask()
		{
			base.SpeechThreadTask();
			speechSynth.Dispose();
		}

		#endregion

		#region Event Handlers

		private void speechSynth_SpeakStarted(object sender, SpeakStartedEventArgs e)
		{
			Monitor.Enter(speechSynth);
			Speaking = true;
		}

		private void speechSynth_SpeakCompleted(object sender, SpeakCompletedEventArgs e)
		{
			if (e.Cancelled)
				lastSpeechResult = SpeakOperationResult.Canceled;
			else if (e.Error != null)
				lastSpeechResult = SpeakOperationResult.Error;
			else
				lastSpeechResult = SpeakOperationResult.Succeeded;
			Speaking = ((speechSynth.State != SynthesizerState.Ready) || (speechQueue.Count > 1));
			Monitor.PulseAll(speechSynth);
			Monitor.Exit(speechSynth);
			string tts = SpokenString;
			OnSpeakCompleted(tts, lastSpeechResult);
			SpokenString = null;
		}

		void speechSynth_SpeakProgress(object sender, SpeakProgressEventArgs e)
		{
			OnSpeakProgress(e);
		}

		#endregion
	}
}
