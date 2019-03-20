using Command = System.String;
using Arguments = System.String;
using WorkingDirectory = System.String;
using F = Functional;

namespace Alias {
	interface IEffectRunCommand {
		/**
		 * <summary>
		 * Run command from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <returns>Command’s exit code.</returns>
		 */
		F.Result<ExitCode> RunCommand(WorkingDirectory workingDirectory, Command command);
		/**
		 * <summary>
		 * Run command with arguments from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <param name="arguments">Command’s arguments.</param>
		 * <returns>Command’s exit code.</returns>
		 */
		F.Result<ExitCode> RunCommand(WorkingDirectory workingDirectory, Command command, Arguments arguments);
	}
}