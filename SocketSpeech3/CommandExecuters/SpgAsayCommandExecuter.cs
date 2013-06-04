using System;
using Robotics.API;

namespace SocketSpeech3
{
	public class SpgAsayCommandExecuter : AsyncCommandExecuter
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
		public SpgAsayCommandExecuter(SpeechGenerator speechGenerator)
			: base("spg_asay")
		{
			if (speechGenerator == null)
				throw new ArgumentNullException();
			this.spGen = speechGenerator;
		}

		/// <summary>
		/// Initializes a new instance of
		/// </summary>
		/// <param name="compatibleCommandName">The command name (provided for compatibility)</param>
		/// <param name="speechGenerator">Speech generator object used for TTS</param>
		public SpgAsayCommandExecuter(string compatibleCommandName, SpeechGenerator speechGenerator)
			: base(compatibleCommandName)
		{
			if (speechGenerator == null)
				throw new ArgumentNullException();
			this.spGen = speechGenerator;
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
		/// Executes the asay command
		/// </summary>
		/// <param name="command">Command object which contains the command to be executed</param>
		/// <returns>The Response object result of provided command execution. If no response is required, must return null</returns>
		/// <remarks>If the command execution is aborted the execution of this method is
		/// canceled and a failure response is sent if required</remarks>
		protected override Response AsyncTask(Command command)
		{
			bool result = spGen.SpeakAsync(command.Parameters);
			return Response.CreateFromCommand(command, result);
		}

		/// <summary>
		/// When overriden, receives the parameters of an analyzed command by the default Signature object as an array of strings
		/// </summary>
		/// <param name="parameters">Array of strings containing the parameters of the command</param>
		public override void DefaultParameterParser(string[] parameters)
		{
			return;
		}

		#endregion
	}
}
