using SCG = System.Collections.Generic;

namespace Alias {
	/**
	 * <summary>
	 * Type for runnable commands with arguments.
	 * </summary>
	 */
	interface ICommand {
		/**
		 * <summary>
		 * Arguments to pass to a command.
		 * </summary>
		 * <value>Sequence of arguments.</value>
		 */
		SCG.IEnumerable<string> Arguments { get; }
		/**
		 * <summary>
		 * Run the command.
		 * </summary>
		 * <returns>Exit code for program.</returns>
		 */
		ExitCode Run();
	}
}
