using S = System;
using SIO = System.IO;
using SD = System.Diagnostics;
using STT = System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;
using F = Functional;
using AC = Alias.Configuration;
using Command = System.String;
using Arguments = System.String;
using WorkingDirectory = System.String;

namespace Alias {
	class Effect: IEffect {
		const int fileStreamBufferSize = 0x1000;
		/**
		 * <summary>
		 * Run process to completion.
		 * </summary>
		 * <param name="processStartInfo">Information to start process.</param>
		 * <returns>Result of the process’s exit code or error.</returns>
		 * <exception cref='S.InvalidOperationException'>No file name was specified in the <see cref='SD.ProcessStartInfo.FileName'/> property of <paramref name="processStartInfo"/>.</exception>
		 * <exception cref='S.ArgumentNullException'><paramref name="processStartInfo"/> is null.</exception>
		 * <exception cref='S.ObjectDisposedException'>The process object has already been disposed.</exception>
		 * <exception cref='SIO.FileNotFoundException'>The file specified in the <see cref='SD.ProcessStartInfo.FileName'/> property of <paramref name="processStartInfo"/> could not be found.</exception>
		 * <exception cref='S.ComponentModel.Win32Exception'>
		 * <list>
		 * <item><description>An error occurred when opening the associated file.</description></item>
		 * <item><description>The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: 'The data area passed to a system call is too small.' or 'Access is denied.'</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.PlatformNotSupportedException'>Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
		 */
		static F.Result<ExitCode> RunProcess(SD.ProcessStartInfo processStartInfo)
		=> F.Factory.Try
		   ( ()
		     => F.Disposable.Using
		        ( SD.Process.Start(processStartInfo)
		        , process => {
		          	process.WaitForExit();
		          	return (ExitCode)process.ExitCode;
		          }
		        )
		   );
		/**
		 * <summary>
		 * Asynchronously run process to completion.
		 * </summary>
		 * <param name="processStartInfo">Information to start process.</param>
		 * <returns>Result of the process’s exit code or error.</returns>
		 * <exception cref='S.InvalidOperationException'>No file name was specified in the <see cref='SD.ProcessStartInfo.FileName'/> property of <paramref name="processStartInfo"/>.</exception>
		 * <exception cref='S.ArgumentNullException'><paramref name="processStartInfo"/> is null.</exception>
		 * <exception cref='S.ObjectDisposedException'>The process object has already been disposed.</exception>
		 * <exception cref='SIO.FileNotFoundException'>The file specified in the <see cref='SD.ProcessStartInfo.FileName'/> property of <paramref name="processStartInfo"/> could not be found.</exception>
		 * <exception cref='S.ComponentModel.Win32Exception'>
		 * <list>
		 * <item><description>An error occurred when opening the associated file.</description></item>
		 * <item><description>The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: 'The data area passed to a system call is too small.' or 'Access is denied.'</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.PlatformNotSupportedException'>Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
		 */
		static async STT.Task<ExitCode> RunProcessAsync(SD.ProcessStartInfo processStartInfo) {
			using var process = new SD.Process { StartInfo = processStartInfo };
			return (ExitCode)await process.RunAsync();
		}
		/**
		 * <summary>
		 * Run command from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <returns>Command’s exit code.</returns>
		 * <exception cref='S.InvalidOperationException'>No file name was specified in <paramref name="command"/>.</exception>
		 * <exception cref='S.ObjectDisposedException'>The process object has already been disposed.</exception>
		 * <exception cref='SIO.FileNotFoundException'>The file specified in the <paramref name="command"/> could not be found.</exception>
		 * <exception cref='S.ComponentModel.Win32Exception'>
		 * <list>
		 * <item><description>An error occurred when opening the associated file.</description></item>
		 * <item><description>The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: 'The data area passed to a system call is too small.' or 'Access is denied.'</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.PlatformNotSupportedException'>Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
		 */
		public F.Result<STT.Task<ExitCode>> RunCommand(WorkingDirectory workingDirectory, Command command)
		=> RunProcessAsync(new SD.ProcessStartInfo(command) { WorkingDirectory = workingDirectory });
		/**
		 * <summary>
		 * Run command with arguments from working directory.
		 * </summary>
		 * <param name="workingDirectory">New process’s working directory.</param>
		 * <param name="command">Command for new process.</param>
		 * <param name="arguments">Command’s arguments.</param>
		 * <returns>Command’s exit code.</returns>
		 * <exception cref='S.InvalidOperationException'>No file name was specified in <paramref name="command"/>.</exception>
		 * <exception cref='S.ObjectDisposedException'>The process object has already been disposed.</exception>
		 * <exception cref='SIO.FileNotFoundException'>The file specified in the <paramref name="command"/> could not be found.</exception>
		 * <exception cref='S.ComponentModel.Win32Exception'>
		 * <list>
		 * <item><description>An error occurred when opening the associated file.</description></item>
		 * <item><description>The sum of the length of the arguments and the length of the full path to the process exceeds 2080. The error message associated with this exception can be one of the following: 'The data area passed to a system call is too small.' or 'Access is denied.'</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.PlatformNotSupportedException'>Method not supported on operating systems without shell support such as Nano Server (.NET Core only).</exception>
		 */
		public F.Result<STT.Task<ExitCode>> RunCommand(WorkingDirectory workingDirectory, Command command, Arguments arguments)
		=> RunProcessAsync(new SD.ProcessStartInfo(command, arguments) { WorkingDirectory = workingDirectory });
		/**
		 * <summary>
		 * Create a file stream. Default: asynchronous, available for shared reading.
		 * </summary>
		 * <param name="path">Path to file.</param>
		 * <param name="mode">File opening mode.</param>
		 * <param name="access">Access permitted to other FileStream objects.</param>
		 * <returns>An asynchronous file stream available for shared reading.</returns>
		 * <exception cref='S.ArgumentNullException'>path is null.</exception>
		 * <exception cref='S.ArgumentException'>
		 * <list>
		 * <item><description>path is an empty string (""), contains only white space, or contains one or more invalid characters.</description></item>
		 * <item><description>path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in an NTFS environment.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.NotSupportedException'>path refers to a non-file device, such as "con:", "com1:", "lpt1:", etc. in a non-NTFS environment.</exception>
		 * <exception cref='S.ArgumentOutOfRangeException'>
		 * <list>
		 * <item><description>bufferSize is negative or zero.</description></item>
		 * <item><description>mode, access, or share contain an invalid value.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='SIO.FileNotFoundException'>The file cannot be found, such as when mode is FileMode.Truncate or FileMode.Open, and the file specified by path does not exist. The file must already exist in these modes.</exception>
		 * <exception cref='SIO.IOException'>
		 * <list>
		 * <item><description>An I/O error, such as specifying FileMode.CreateNew when the file specified by path already exists, occurred.</description></item>
		 * <item><description>The stream has been closed.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>
		 * <list>
		 * <item><description>The access requested is not permitted by the operating system for the specified path, such as when access is Write or ReadWrite and the file or directory is set for read-only access.</description></item>
		 * <item><description>System.IO.FileOptions.Encrypted is specified for options, but file encryption is not supported on the current platform.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='SIO.PathTooLongException'>The specified path, file name, or both exceed the system-defined maximum length.</exception>
		 */
		public static SIO.FileStream GetFileStream(string path, SIO.FileMode mode, SIO.FileAccess access, SIO.FileShare fileShare=SIO.FileShare.Read, SIO.FileOptions fileOptions=SIO.FileOptions.Asynchronous | SIO.FileOptions.SequentialScan)
		=> new SIO.FileStream(path, mode, access, fileShare, fileStreamBufferSize, fileOptions);
		/// <inheritdoc/>
		public F.Result<STT.Task> CopyFile(IFileInfo file, string destination)
		=> F.Factory.Try
		   ( () => GetFileStream(file.FullName, SIO.FileMode.Open, SIO.FileAccess.Read)
		   , OperationIOException.ReadErrorMap(file.FullName)
		   )
		   .SelectMany
		    ( (SIO.FileStream source)
		      => F.Factory.Try
		         // file exists exceptions happen regardless of checking with File.Exists due to race conditions & async IO: just catch exceptions that happen
		         ( () => GetFileStream(destination, SIO.FileMode.CreateNew, SIO.FileAccess.Write)
		         , OperationIOException.CreateErrorMap(destination)
		         )
		         .Catch(error => {
		          	source.Dispose();
		          	return error;
		          })
		         .SelectMany
		          ( destinationStream
		            => F.Factory.Try
		               ( async () => {
		                 	using (source)
		                 	using (destinationStream) {
		                 		await source.CopyToAsync(destinationStream).ConfigureAwait(false);
		                 	}
		                 }
		               , OperationIOException.CopyErrorMap(destination)
		               )
		          )
		    );
		/// <inheritdoc/>
		public F.Result<STT.Task> DeleteFile(FileInfo file) {
			throw new S.NotImplementedException();
			return F.Factory.Try
			( ()
			  => GetFileStream(file.FullName, SIO.FileMode.Open, SIO.FileAccess.Write, fileOptions: SIO.FileOptions.Asynchronous | SIO.FileOptions.DeleteOnClose | SIO.FileOptions.SequentialScan).DisposeAsync().AsTask()
			, OperationIOException.DeleteErrorMap(file.FullName)
			);
		}
		/// <inheritdoc/>
		public F.Result<STT.Task> WriteConfiguration(AC.Configuration configuration, IFileInfo file) {
			throw new S.NotImplementedException();
			return F.Factory.Try
			 ( () => {
			   	using var stream = file.OpenWrite();
			   	using var textWriter = new SIO.StreamWriter(stream);
			   	configuration.Serialize(textWriter);
			   }
			 )
			.ToTask;
		}
	}
}