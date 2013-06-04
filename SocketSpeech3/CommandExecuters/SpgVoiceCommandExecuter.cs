using System;
using Robotics.API;

namespace SocketSpeech3
{
	/// <summary>
	/// Implements a synchronous command execuer for the voice command
	/// </summary>
	public class SpgVoiceCommandExecuter : SyncCommandExecuter
	{
		#region Variables

		/// <summary>
		/// Speech generator used for Speech Synthesis
		/// </summary>
		private readonly SpeechGenerator spGen;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of
		/// </summary>
		/// <param name="speechGenerator">Speech generator object used for TTS</param>
		public SpgVoiceCommandExecuter(SpeechGenerator speechGenerator)
			: base("spg_voice")
		{
			if (speechGenerator == null)
				throw new ArgumentNullException();
			this.spGen = speechGenerator;
		}

		#endregion

		#region Events
		#endregion

		#region Properties
		#endregion

		#region Methodos
		#endregion

		#region Inherited Methodos

		/// <summary>
		/// Executes the voice command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
		/// <remarks>If the command execution is aborted the execution of this method is
		/// canceled and a failure response is sent if required</remarks>
		protected override Response SyncTask(Command command)
		{
			if((!command.HasParams) || (command.Parameters.ToLower() == "get"))
			{
				command.Parameters = spGen.SelectedVoiceName;
				return Response.CreateFromCommand(command, true);
			}

			if (!spGen.VoiceNames.Contains(command.Parameters))
				return Response.CreateFromCommand(command, false);

			try
			{
				spGen.SelectedVoiceName = command.Parameters;
				return Response.CreateFromCommand(command, true);
			}
			catch
			{
				return Response.CreateFromCommand(command, false);
			}
		}

		#endregion
	}
}
