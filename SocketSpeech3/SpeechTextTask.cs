using System;
using System.Collections.Generic;
using System.Text;


namespace SocketSpeech3
{
	/// <summary>
	/// Gets the status of a SpeechTextTask
	/// </summary>
	public enum SpeechTextTaskStatus
	{
		/// <summary>
		/// The speech task is pending for execution
		/// </summary>
		Pending,
		/// <summary>
		/// The speech task is being executed
		/// </summary>
		Speaking,
		/// <summary>
		/// The speech task was completed successfully
		/// </summary>
		CompletedSuccessfully,
		/// <summary>
		/// The speech task was completed but failed
		/// </summary>
		CompleteFailed
	}

	/// <summary>
	/// Encapsulates text to synthetize
	/// </summary>
	public class SpeechTextTask
	{
		/// <summary>
		/// The text to be spoken
		/// </summary>
		private string tts;

		/// <summary>
		/// Status of the speech task
		/// </summary>
		private SpeechTextTaskStatus status;

		/// <summary>
		/// Initializes a new instance of SpeechTextTask
		/// </summary>
		/// <param name="tts">The name of the file source of the audio</param>
		/// <param name="loop">Indicates if the amount of times the audio must loop </param>
		public SpeechTextTask(string tts)
		{
			this.tts = PhrasePronunciation.ReplacePronunciations(tts);
			this.status = SpeechTextTaskStatus.Pending;
		}

		/// <summary>
		/// Gets the name of the file source of the audio
		/// </summary>
		public string TTS
		{
			get
			{
				return this.tts;
			}
		}

		/// <summary>
		/// Gets or sets the status of the speech task
		/// </summary>
		public SpeechTextTaskStatus Status
		{
			get
			{
				return this.status;
			}
			set { this.status = value; }
		}

		/*
		public static implicit operator string(SpeechTextTask task)
		{
			return task == null ? null : task.audio;
		}

		public static implicit operator SpeechTextTask(string tts)
		{
			return audio == null ? null : new SpeechTextTask(tts);
		}
		*/
	}


}