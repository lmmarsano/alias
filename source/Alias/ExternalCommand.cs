using System;
using SCG = System.Collections.Generic;

namespace Alias {
	/**
	 * <summary>
	 * An external command built from a base command and additional arguments.
	 * </summary>
	 */
	class ExternalCommand: ICommand {
		/**
		 * <summary>
		 * Additional arguments.
		 * </summary>
		 * <value>Arguments to append to a base command.</value>
		 */
		public SCG.IEnumerable<string> Arguments { get; }
		/**
		 * <summary>
		 * An base command to run externally.
		 * </summary>
		 * <value>Command string matching native console conventions.</value>
		 */
		public string BaseCommand { get; }
		/**
		 * <summary>
		 * Run external command: run the command string as is with appended arguments.
		 * </summary>
		 * <returns>The external command’s exit code.</returns>
		 */
		public ExitCode Run() {
			throw new NotImplementedException();
		}
		/**
		 * <summary>
		 * Construct an external command from a base command and arguments.
		 * </summary>
		 * <param name="arguments">Additional arguments following the base command.</param>
		 * <param name="baseCommand">A command string.</param>
		 */
		public ExternalCommand(SCG.IEnumerable<string> arguments, string baseCommand) {
			Arguments = arguments;
			BaseCommand = baseCommand;
		}
	}
}
