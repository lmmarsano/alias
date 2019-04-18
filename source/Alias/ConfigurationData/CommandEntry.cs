#nullable enable
using S = System;
using NJ = Newtonsoft.Json;
using Command = System.String;
using Arguments = System.String;

namespace Alias.ConfigurationData {
	/**
	 * <summary>
	 * Components of a command invocation: command & optional arguments.
	 * </summary>
	 */
	public class CommandEntry: S.IEquatable<CommandEntry> {
		/**
		<summary>
			Command to execute.
		</summary>
		<value>A command without arguments.</value>
		*/
		[NJ.JsonProperty("command")]
		public Command Command { get; set; }
		/**
		<summary>
			Command arguments.
		</summary>
		<value>Arguments for a command.</value>
		*/
		[NJ.JsonProperty("arguments")]
		public Arguments? Arguments { get; set; }
		/**
		<summary>
			Initialize a <c cref='CommandEntry'>command entry</c>.
		</summary>
		<param name="command">A commandline with arguments.</param>
		*/
		public CommandEntry(Command command, Arguments? arguments) {
			Command = command.Trim();
			Arguments = string.IsNullOrWhiteSpace(arguments)
								? null
								: arguments.Trim();
		}
		/// <inheritdoc/>
		public override string ToString()
		=> Utility.SafeQuote(Command)
		 + ( string.IsNullOrWhiteSpace(Arguments)
			 ? string.Empty
			 : @" " + Arguments
			 );
		public bool Equals(CommandEntry commandEntry)
		=> Command == commandEntry.Command
		&& Arguments == commandEntry.Arguments;
		public override bool Equals(object obj)
		=> (obj as CommandEntry)?.Equals(this) == true;
		public override int GetHashCode() => S.HashCode.Combine(Command, Arguments);
	}
}
