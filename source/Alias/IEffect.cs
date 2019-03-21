using SIO = System.IO;
using F = Functional;
using Command = System.String;
using Arguments = System.String;
using WorkingDirectory = System.String;

namespace Alias {
	/**
	 * <summary>
	 * Effects needed to perform operations.
	 * </summary>
	 */
	interface IEffect {
		/**
		 * <summary>
		 * Copy file to destination.
		 * </summary>
		 * <param name="file">File to copy.</param>
		 * <param name="destination">Destination to copy file to.</param>
		 * <returns>Result of nothing or error.</returns>
		 */
		F.Result<F.Nothing> CopyFile(IFileInfo file, string destination);
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