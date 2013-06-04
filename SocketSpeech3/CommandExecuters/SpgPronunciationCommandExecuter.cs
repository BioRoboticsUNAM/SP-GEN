using System;
using System.Text.RegularExpressions;
using Robotics.API;

namespace SocketSpeech3
{
	/// <summary>
	/// Implements a synchronous command execuer for the voice command
	/// </summary>
	public class SpgPronunciation : SyncCommandExecuter
	{
		#region Variables

		private Regex rx;

		#endregion

		#region Constructors

		/// <summary>
		/// Initializes a new instance of SpgPronunciation
		/// </summary>
		public SpgPronunciation()
			: base("spg_pronunciation")
		{
			rx = new Regex(@"(?<mode>\w+)\s+(((?<phrase>""[^""]+"")\s+(?<pron>""[^""]+""))|((?<phrase>\S+)\s+(?<pron>\S+)))", RegexOptions.Compiled);
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
			Match m;
			string mode;
			string phrase;
			string pronunciation;

			m = rx.Match(command.Parameters);
			if (!m.Success)
				return Response.CreateFromCommand(command, false);

			mode = m.Result("${mode}");
			phrase = m.Result("${phrase}");
			pronunciation = m.Result("${pron}");
			if(String.Compare(mode, "add", true) == 0)
			{
				//PhrasePronunciation.AddP
			}
			else if(String.Compare(mode, "remove", true) == 0)
			{
				
			}
			else
				return Response.CreateFromCommand(command, false);
			return Response.CreateFromCommand(command, true);
		}

		#endregion
	}
}
