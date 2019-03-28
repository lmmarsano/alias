using S = System;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using F = Functional;
using AC = Alias.Configuration;
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
		 * <param name="files">Fil information.</param>
		 * <returns>Task yielding nothing result.</returns>
		 */
		F.Result<STT.Task> DeleteFile(FileInfo file);
		/**
		 * <summary>
		 * Write configuration to file.
		 * </summary>
		 * <param name="configuration">Alias configuration.</param>
		 * <param name="file">Configuration file.</param>
		 * <returns>Resulting task to write configuration.</returns>
		 * <exception cref="SerializerException">Configuration could not be serialized.</exception>
		 */
		F.Result<STT.Task> WriteConfiguration(AC.Configuration configuration, IFileInfo file);
		/**
		 * <summary>
		 * Copy file to destination.
		 * </summary>
		 * <param name="file">File to copy.</param>
		 * <param name="destination">Destination to copy file to.</param>
		 * <returns>Result of task to copy file.</returns>
		 * <exception cref='OperationIOException'>Destination exists or copy failed.</exception>
		 */
		F.Result<STT.Task> CopyFile(IFileInfo file, string destination);
		/**
		 * <summary>
		 * Run command from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <returns>Task returning command’s exit code result.</returns>
		 */
		F.Result<STT.Task<ExitCode>> RunCommand(WorkingDirectory workingDirectory, Command command);
		/**
		 * <summary>
		 * Run command with arguments from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <param name="arguments">Command’s arguments.</param>
		 * <returns>Command’s exit code.</returns>
		 */
		F.Result<STT.Task<ExitCode>> RunCommand(WorkingDirectory workingDirectory, Command command, Arguments arguments);
	}
}