using System;
using System.Collections.Generic;
using Microsoft.DirectX.AudioVideoPlayback;

namespace SocketSpeech3
{
	/// <summary>
	/// Encapsulates data for an audio play task
	/// </summary>
	public class PlayAudioTask
	{
		/// <summary>
		/// The name of the file source of the audio
		/// </summary>
		private string fileName;
		/// <summary>
		/// The audio to be played
		/// </summary>
		private Audio audio;
		/// <summary>
		/// Indicates if the amount of times the audio must loop 
		/// </summary>
		private int loop;

		/// <summary>
		/// Initializes a new instance of PlayAudioTask
		/// </summary>
		/// <param name="fileName">The name of the file source of the audio</param>
		/// <param name="loop">Indicates if the amount of times the audio must loop </param>
		public PlayAudioTask(string fileName)
		{
			this.fileName = fileName;
			audio = new Audio(fileName);
			this.loop = 1;
		}

		/// <summary>
		/// Initializes a new instance of PlayAudioTask
		/// </summary>
		/// <param name="fileName">The name of the file source of the audio</param>
		/// <param name="audio">The audio to be played</param>
		/// <param name="loop">Indicates if the amount of times the audio must loop </param>
		public PlayAudioTask(string fileName, Audio audio)
		{
			this.fileName = fileName;
			this.audio = audio;
			this.loop = 1;
		}

		/// <summary>
		/// Initializes a new instance of PlayAudioTask
		/// </summary>
		/// <param name="fileName">The name of the file source of the audio</param>
		/// <param name="loop">Indicates if the amount of times the audio must loop </param>
		public PlayAudioTask(string fileName, int loop)
		{
			this.fileName = fileName;
				audio = new Audio(fileName);
			this.loop = loop;
		}

		/// <summary>
		/// Initializes a new instance of PlayAudioTask
		/// </summary>
		/// <param name="fileName">The name of the file source of the audio</param>
		/// <param name="audio">The audio to be played</param>
		/// <param name="loop">Indicates if the amount of times the audio must loop </param>
		public PlayAudioTask(string fileName, Audio audio, int loop)
		{
			this.fileName = fileName;
			this.audio = audio;
			this.loop = loop;
		}

		/// <summary>
		/// Gets the name of the file source of the audio
		/// </summary>
		public string FileName
		{
			get
			{
				return this.fileName;
			}
		}
		/// <summary>
		/// Gets the audio to be played
		/// </summary>
		public Audio Audio
		{
			get
			{
				return this.audio;
			}
		}
		/// <summary>
		/// Indicates if the amount of times the audio must loop 
		/// </summary>
		public int Loop
		{
			get
			{
				return this.loop;
			}
		}
		/*
		public static implicit operator Audio(PlayAudioTask task)
		{
			return task == null ? null : task.audio;
		}

		public static implicit operator PlayAudioTask(Audio audio)
		{
			return audio == null ? null : new PlayAudioTask(audio);
		}
		*/
	}
}
