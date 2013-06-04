using System;
using System.Threading;
using System.Speech.Synthesis;
using LTTS7Lib;

namespace SocketSpeech3
{

	/// <summary>
	/// Implements a Speech Generator
	/// </summary>
	public class Loq7SpeechGenerator : SpeechGenerator
	{
		#region Variables

		/// <summary>
		/// Name of selected voice
		/// </summary>
		private string selectedVoiceName;

		/// <summary>
		/// The speech synthethizer used for synthesis
		/// </summary>
		private LTTS7Class speechSynth;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SapiSpeechGenerator
		/// </summary>
		public Loq7SpeechGenerator()
		{
			this.selectedVoiceName = String.Empty;
			try
			{
				speechSynth = new LTTS7Class();
			}
			catch { speechSynth = null; }
			LoadVoices();
		}

		/// <summary>
		/// Destructor. It will stop threads and cancel speech
		/// </summary>
		~Loq7SpeechGenerator()
		{
			if (this.speechSynth != null)
			{
				try
				{
				
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
			get { return this.voiceNames.IndexOfKey(selectedVoiceName); }
			set
			{
				if ((value < 0) || (value >= voiceNames.Count))
					throw new ArgumentOutOfRangeException();
				this.SelectedVoiceName = voiceNames.Keys[value];
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
				selectedVoiceName = value;
				OnVoiceChanged();
			}
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
				string voiceName;
				int i = 0;

				// Retrieves the first voice of the Loquendo TTS found on the system
				voiceName = speechSynth.EnumFirstVoice(String.Empty);
				while (!String.IsNullOrEmpty(voiceName))
				{
					if (voiceNames.ContainsKey(voiceName))
						continue;
					voiceNames.Add(voiceName, i++);
					voiceName = speechSynth.EnumNextVoice();
				}

				string lang = speechSynth.EnumFirstLanguage(String.Empty);
				while (!String.IsNullOrEmpty(lang))
					lang = speechSynth.EnumNextLanguage();
				
				if (voiceNames.ContainsKey("Susan"))
					SelectedVoiceIndex = voiceNames["Susan"];
				else
					SelectedVoiceIndex = 0;
			}
			catch
			{
				voiceNames.Clear();
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
			if(speechSynth != null)
			speechSynth.Stop();
		}

		/// <summary>
		/// Executes a TTS sinchronously
		/// </summary>
		/// <param name="textToSay">text to speech out</param>
		/// <returns>true if provided text was spoken, false otherwise</returns>
		public override bool SpeakSync(string textToSay)
		{
			return false;
			//if (speechSynth == null)
			//    return false;
			//return base.SpeakSync(textToSay);
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

			//if (!Monitor.TryEnter(speechSynth))
			//    return false;
			//locked = true;
			//Speaking = true;
			SpokenString = original;
			speechTask.Status = SpeechTextTaskStatus.Speaking;
			lastSpeechResult = SpeakOperationResult.Error;

			try
			{
				OnSpeakStarted(tts);
				if (speechSynth.Voice != selectedVoiceName)
					speechSynth.Voice = selectedVoiceName;
				speechSynth.Language = "EnglishUs";
				speechSynth.Read(tts);
				//Monitor.Wait(speechSynth);
				//Monitor.Exit(speechSynth);
				//locked = false;
			}
			catch
			{
				//if (locked)
				//    Monitor.Exit(speechSynth);
				////Speaking = ((speechSynth.State == SynthesizerState.Speaking) || (speechQueue.Count != 0));
				//Speaking = (speechQueue.Count != 0);
				speechTask.Status = SpeechTextTaskStatus.CompleteFailed;
				return false;
			}
			//speechTask.Status = (lastSpeechResult == SpeakOperationResult.Succeeded) ? SpeechTextTaskStatus.CompletedSuccessfully : SpeechTextTaskStatus.CompleteFailed;
			//return (lastSpeechResult == SpeakOperationResult.Succeeded);
			speechTask.Status = SpeechTextTaskStatus.CompletedSuccessfully;
			return true;
		}

		/// <summary>
		/// Converts the TTS generated from provided text to a wavefile
		/// </summary>
		/// <param name="textToSay">Text to speech out</param>
		/// <param name="path">Path of the file to save waveform</param>
		/// <returns>true if file was generated successfully</returns>
		public override bool SaveTextToWav(string textToSay, string path)
		{
			LTTS7Class synth;

			textToSay = textToSay.Replace("robot", "row-bought");
			//textToSay = Regex.Replace(textToSay, "robot", "row-bought", RegexOptions.IgnoreCase);
			//textToSay = Regex.Replace(textToSay, "robot", "robought", RegexOptions.IgnoreCase);

			try
			{
				synth = new LTTS7Class();
				synth.Voice = selectedVoiceName;
				synth.Record(textToSay, path);
				return true;
			}
			catch
			{
				return false;
			}
		}

		protected override void SpeechThreadTask()
		{
			base.SpeechThreadTask();
		}

		#endregion

		#region Event Handlers

		#endregion

		#region Static Properties

		public static bool LoquendoInstalled
		{
			get
			{
				try{

					System.Reflection.Assembly loqAssembly = System.Reflection.Assembly.GetAssembly(typeof(LTTS7Lib.LTTS7Class));
					string strDllPath = loqAssembly.CodeBase.Substring(8);
					return System.IO.File.Exists(strDllPath);

				}catch{return false;}
			}
		}

		#endregion
	}
}
