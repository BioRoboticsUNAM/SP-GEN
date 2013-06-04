using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.DirectX.AudioVideoPlayback;
using Robotics;

namespace SocketSpeech3
{

	/// <summary>
	/// Enumerates the result of an audio play operation
	/// </summary>
	public enum PlayOperationResult
	{
		/// <summary>
		/// Audio file played successfully
		/// </summary>
		Succeeded,
		/// <summary>
		/// Play canceled
		/// </summary>
		Canceled,
		/// <summary>
		/// While playing audio file
		/// </summary>
		Error

	};

	public delegate void StartingEventHandler(IAudioPlayer sender, Audio audio);

	public delegate void StoppingEventHandler(IAudioPlayer sender, PlayOperationResult result);

	/// <summary>
	/// Implements an audio player
	/// </summary>
	public interface IAudioPlayer
	{
		#region Events

		/// <summary>
		/// Occurs when the AudioPlayer begins to play an audio file
		/// </summary>
		event StartingEventHandler Starting;
		/// <summary>
		/// Occurs when the AudioPlayer stops playing an audio file
		/// </summary>
		event StoppingEventHandler Stopping;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the Speech Synthesizer is working
		/// </summary>
		bool IsPlaying
		{
			get;
		}

		/// <summary>
		/// Gets the number of pending asynchronous speech tasks
		/// </summary>
		int Pending
		{
			get;
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Cancels current audio play operations
		/// </summary>
		void Stop();

		/// <summary>
		/// Cancels all audio play operations
		/// </summary>
		void StopAll();

		/// <summary>
		/// Cancels all audio play operations
		/// </summary>
		void ShutUp();

		/// <summary>
		/// Plays an audio file synchronously
		/// </summary>
		/// <param name="audioFilePath">Path to the audio file to play</param>
		/// <returns>true if audio file was loaded and enqueued successfully, false otherwise</returns>
		bool PlaySync(string audioFilePath);

		/// <summary>
		/// Plays an audio file asynchronously
		/// </summary>
		/// <param name="audioFilePath">Audio object to play</param>
		/// <returns>true if audio file was loaded and enqueued successfully, false otherwise</returns>
		bool PlayAsync(string audioFilePath);

		/// <summary>
		/// Plays an audio file asynchronously to the infinity
		/// </summary>
		/// <param name="audioFilePath">Audio object to play</param>
		/// <returns>true if audio file was loaded and enqueued successfully, false otherwise</returns>
		bool PlayLoop(string audioFilePath);

		#endregion
	}
}
