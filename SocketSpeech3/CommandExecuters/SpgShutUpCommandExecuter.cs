using System;
using Robotics.API;

namespace SocketSpeech3
{
	/// <summary>
	/// Implements a synchronous command execuer for the voice command
	/// </summary>
	public class SpgShutUpCommandExecuter : SyncCommandExecuter
	{
		#region Variables

		/// <summary>
		/// Speech generator used for Speech Synthesis
		/// </summary>
		private readonly SpeechGenerator spGen;
		/// <summary>
		/// Audio player used to play sounds
		/// </summary>
		private readonly IAudioPlayer audioPlayer;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of
		/// </summary>
		/// <param name="speechGenerator">Speech generator object used for TTS</param>
		public SpgShutUpCommandExecuter(SpeechGenerator speechGenerator, IAudioPlayer audioPlayer)
			: base("spg_shutup")
		{
			if ((speechGenerator == null)|| (audioPlayer == null))
				throw new ArgumentNullException();
			this.spGen = speechGenerator;
			this.audioPlayer = audioPlayer;
		}

		#endregion

		#region Events
		#endregion

		#region Properties

		public override bool Busy
		{
			get
			{
				return false;
			}
		}

		#endregion

		#region Methodos
		#endregion

		#region Inherited Methodos

		/// <summary>
		/// Executes the shutup command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
		/// <remarks>If the command execution is aborted the execution of this method is
		/// canceled and a failure response is sent if required</remarks>
		protected override Response SyncTask(Command command)
		{
			int canceled;

			switch (command.Parameters.ToLower())
			{
				case "all":
				default:
					spGen.ShutUp();
					audioPlayer.StopAll();
					return Response.CreateFromCommand(command, true);

				case "sound":
					canceled = audioPlayer.Pending;
					if (audioPlayer.IsPlaying)
						++canceled;

					audioPlayer.StopAll();
					command.Parameters = "sound " + canceled.ToString();
					return Response.CreateFromCommand(command, true);

				case "voice":
					canceled = spGen.Pending;
					if (spGen.Speaking)
						++canceled;

					spGen.ShutUp();
					command.Parameters = "voice " + canceled.ToString();
					return Response.CreateFromCommand(command, true);
			}
			//return Response.CreateFromCommand(command, false);
		}

		#endregion
	}
}
