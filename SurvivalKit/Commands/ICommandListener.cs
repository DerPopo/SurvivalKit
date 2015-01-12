using System;

namespace SurvivalKit.Interfaces
{
	/// <summary>
	/// The CommandListener interface is used in command listening classes.
	/// </summary>
	public interface ICommandListener
	{
		/// <summary>
		/// Called when a <see cref="SurvivalKit.Permissions.CommandSender"/> sent a command.
		/// </summary>
		/// <returns><c>true</c>, if the search for a CommandListener should cancel, <c>false</c> otherwise.</returns>
		/// <param name="sender">The sender of the command.</param>
		/// <param name="command">The original command name.</param>
		/// <param name="alias">The alias used by the CommandSender.</param>
		/// <param name="args">The arguments.</param>
		bool onCommand(Permissions.CommandSender sender, string command, string alias, string[] args);
	}
}

