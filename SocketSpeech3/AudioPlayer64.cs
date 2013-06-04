using System;
using System.Collections.Generic;
using System.Media;
using System.Threading;
using Robotics;

namespace SocketSpeech3
{
	/// <summary>
	/// Implements an audio player
	/// </summary>
	public class AudioPlayer64 : IAudioPlayer
	{
		#region Variables

		/// <summary>
		/// The audio file which is currently being played.
		/// </summary>
		private SoundPlayer playingAudio;

		/// <summary>
		/// Thread to control aync play 
		/// </summary>
		private Thread playThread;

		/// <summary>
		/// Queue for store pending audio files
		/// </summary>
		private ProducerConsumer<PlayAudioTask> playQueue;

		/// <summary>
		/// Stores the result of the last play operation
		/// </summary>
		private PlayOperationResult lastPlayResult;

		/// <summary>
		/// Flag that indicates when the audio player is working
		/// </summary>
		private bool playing;

		/// <summary>
		/// The path of the currently payed audio file
		/// </summary>
		private string playingFilePath;

		/// <summary>
		/// Flag that indicates if the main thread is running
		/// </summary>
		private bool running;

		/// <summary>
		/// Object used for synchronization
		/// </summary>
		private object syncObj;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of AudioPlayer
		/// </summary>
		public AudioPlayer64()
		{
			syncObj = new Object();
			playQueue = new ProducerConsumer<PlayAudioTask>();
			playThread = new Thread(new ThreadStart(PlayAudioThreadTask));
			playThread.IsBackground = true;
			playThread.Start();
		}

		/// <summary>
		/// Destructor. It will stop threads and cancel playing operations
		/// </summary>
		~AudioPlayer64()
		{
			running = false;
			if (this.playingAudio != null)
			{
				try
				{
					playingAudio.Stop();
				}
				catch { }
			}
			if ((this.playThread != null) && (this.playThread.IsAlive))
			{
				if (this.playThread.ThreadState == ThreadState.WaitSleepJoin)
				{
					this.playThread.Interrupt();
					this.playThread.Join(100);
				}
				if (this.playThread.IsAlive)
					this.playThread.Abort();
			}
		}

		#endregion

		#region Events

		/// <summary>
		/// Occurs when the AudioPlayer begins to play an audio file
		/// </summary>
		public event StartingEventHandler Starting;
		/// <summary>
		/// Occurs when the AudioPlayer stops playing an audio file
		/// </summary>
		public event StoppingEventHandler Stopping;

		#endregion

		#region Properties

		/// <summary>
		/// Gets a value that indicates if the Speech Synthesizer is working
		/// </summary>
		public bool IsPlaying
		{
			get { return playing; }
		}

		/// <summary>
		/// Gets the number of pending asynchronous speech tasks
		/// </summary>
		public int Pending
		{
			get { return playQueue.Count; }
		}

		/// <summary>
		/// Gets the path of the currently played audio
		/// </summary>
		private string PlayingFilePath
		{
			get { return playingFilePath; }
		}

		#endregion

		#region Methodos

		/// <summary>
		/// Cancels current audio play operations
		/// </summary>
		public void Stop()
		{
			/*
			if (playingAudio != null)
			{
				try
				{
					lock (playingAudio)
					{
						playingAudio.Pause();
						playingAudio.Stop();
					}
				}
				catch { }
				lastPlayResult = PlayOperationResult.Canceled;
			}
			*/
		}

		/// <summary>
		/// Cancels all audio play operations
		/// </summary>
		public void StopAll()
		{
			/*
			playQueue.Clear();
			if (playingAudio != null)
			{
				try
				{
					lock (playingAudio)
					{
						playingAudio.Pause();
						playingAudio.Stop();
					}
				}
				catch { }
				lastPlayResult = PlayOperationResult.Canceled;
			}
			*/
		}

		/// <summary>
		/// Cancels all audio play operations
		/// </summary>
		public void ShutUp()
		{
			StopAll();
		}

		/// <summary>
		/// Plays an audio file synchronously
		/// </summary>
		/// <param name="audioFilePath">Path to the audio file to play</param>
		/// <returns>true if audio file was loaded and enqueued successfully, false otherwise</returns>
		public bool PlaySync(string audioFilePath)
		{
			/*
			PlayAudioTask audioTask;
			try
			{
				audioTask = new PlayAudioTask(audioFilePath);
			}
			catch { return false; }
			return Play(audioTask);
			*/
			return false;
		}

		/// <summary>
		/// Plays an audio file asynchronously
		/// </summary>
		/// <param name="audioFilePath">Audio object to play</param>
		/// <returns>true if audio file was loaded and enqueued successfully, false otherwise</returns>
		public bool PlayAsync(string audioFilePath)
		{
			/*
			if (playQueue.Count >= playQueue.Capacity)
				return false;
			PlayAudioTask audioTask;
			try
			{
				audioTask = new PlayAudioTask(audioFilePath);
			}
			catch { return false; }
			if (audioTask != null)
			{
				playQueue.Produce(audioTask);
				return true;
			}
			*/
			return false;
		}

		/// <summary>
		/// Plays an audio file asynchronously to the infinity
		/// </summary>
		/// <param name="audioFilePath">Audio object to play</param>
		/// <returns>true if audio file was loaded and enqueued successfully, false otherwise</returns>
		public bool PlayLoop(string audioFilePath)
		{
			/*
			if (playQueue.Count >= playQueue.Capacity)
				return false;
			PlayAudioTask audio;
			try
			{
				audio = new PlayAudioTask(audioFilePath, -1);
			}
			catch { return false; }
			if (audio != null)
			{
				playQueue.Produce(audio);
				return true;
			}
			*/
			return false;
		}

		/// <summary>
		/// Plays an audio file
		/// </summary>
		/// <param name="audio">Audio to play out</param>
		/// <returns>true if audio was played, false otherwise</returns>
		private bool Play(PlayAudioTask audioTask)
		{
			/*
			if (playing || !Monitor.TryEnter(syncObj))
				return false;
			if (audioTask == null)
				return false;

			Audio audio;
			int remaining;
			bool complete;

			if ((audio = audioTask.Audio) == null)
				return false;
			remaining = audioTask.Loop;
			complete = false;
			playingFilePath = audioTask.FileName;
			playing = true;
			playingAudio = audio;
			lastPlayResult = PlayOperationResult.Error;

			try
			{
				do
				{
					complete = false;
					audio.Play();
					while (audio.Playing)
					{
						if (audio.CurrentPosition >= audio.Duration)
						{
							audio.Stop();
							complete = true;
							break;
						}
						Thread.Sleep(10);
					}
				} while (complete && ((audioTask.Loop == -1) || (--remaining > 0)));
				lastPlayResult = PlayOperationResult.Succeeded;
			}
			catch
			{
				lastPlayResult = PlayOperationResult.Error;
				return false;
			}

			try { audio.Dispose(); }
			catch { }

			playingAudio = null;
			playing = false;
			Monitor.Exit(syncObj);
			OnStopping();
			return true;
			*/
			return false;
		}

		protected void OnStarting(string textToSay)
		{
			/*
			if (Starting != null)
				Starting(this, playingAudio);
			*/
		}

		protected void OnStopping()
		{
			if (Stopping != null)
				Stopping(this, lastPlayResult);
		}

		private void PlayAudioThreadTask()
		{
			PlayAudioTask audioTask;
			running = true;

			while (running)
			{
				try
				{
					audioTask = playQueue.Consume();
					if ((audioTask == null) || (audioTask.Audio == null))
					    continue;
					if (!Play(audioTask))
					    playQueue.Produce(audioTask);
				}
				catch (ThreadAbortException taex)
				{
					Console.WriteLine(taex.Message);
					playQueue.Clear();
					return;
				}
				catch (ThreadInterruptedException tiex)
				{
					Console.WriteLine(tiex.Message);
					continue;
				}
				catch
				{
					Thread.Sleep(100);
					continue;
				}

			}
		}

		#endregion
	}
}