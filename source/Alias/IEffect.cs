using STT = System.Threading.Tasks;
using F = Functional;
using AC = Alias.ConfigurationData;
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
		 * Delete file.
		 * </summary>
		 * <param name="files">File information.</param>
		 * <returns>Task yielding nothing result.</returns>
		 * <exception cref='OperationIOException'>Unable to access file for deletion.</exception>
		 */
		F.Result<STT.Task> DeleteFile(IFileInfo file);
		/**
		 * <summary>
		 * Attempt to retrieve the configuration. If the configuration is missing, yield nothing.
		 * </summary>
		 * <param name="file">File containing configuration.</param>
		 * <returns>Possible task retrieving an optional configuration.</returns>
		 * <exception cref="TerminalFileException">The file exists and cannot be read.</exception>
		 * <exception cref="DeserialException">Processing file into configuration fails.</exception>
		 * <exception cref='UnhandledCaseException'>An alternative that shouldn’t exist occurred.</exception>
		 */
		F.Result<STT.Task<F.Maybe<AC.Configuration>>> TryGetConfiguration(IFileInfo file);
		/**
		 * <summary>
		 * Write configuration to file.
		 * </summary>
		 * <param name="configuration">Alias configuration.</param>
		 * <param name="file">Configuration file.</param>
		 * <returns>Resulting task to write configuration.</returns>
		 * <exception cref="TerminalFileException">Unable to write configuration to file.</exception>
		 * <exception cref="SerializerException">Unable to serialize configuration.</exception>
		 */
		F.Result<STT.Task> WriteConfiguration(AC.Configuration configuration, IFileInfo file);
		/**
		 * <summary>
		 * Copy file to destination.
		 * </summary>
		 * <param name="file">File to copy.</param>
		 * <param name="destination">Destination to copy file to.</param>
		 * <returns>Result of task to copy file.</returns>
		 * <exception cref='OperationIOException'>Unable to read source, create destination, or perform copy.</exception>
		 */
		F.Result<STT.Task> CopyFile(IFileInfo file, string destination);
		/**
		 * <summary>
		 * Run command with arguments from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <param name="maybeArguments">Optional command arguments.</param>
		 * <returns>Command’s exit code.</returns>
		 */
		F.Result<STT.Task<ExitCode>> RunCommand(WorkingDirectory workingDirectory, Command command, F.Maybe<Arguments> maybeArguments);
	}
}
