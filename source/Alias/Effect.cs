using S = System;
using SIO = System.IO;
using SD = System.Diagnostics;
using SSP = System.Security.Permissions;
using F = Functional;
using static Functional.Extension;
using Command = System.String;
using Arguments = System.String;
using WorkingDirectory = System.String;
using System.IO;

namespace Alias {
	class Effect: IEffect {
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
		public F.Result<ExitCode> RunCommand(WorkingDirectory workingDirectory, Command command)
		=> RunProcess(new SD.ProcessStartInfo(command) { WorkingDirectory = workingDirectory });
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
		public F.Result<ExitCode> RunCommand(WorkingDirectory workingDirectory, Command command, Arguments arguments)
		=> RunProcess(new SD.ProcessStartInfo(command, arguments) { WorkingDirectory = workingDirectory });
		/**
		 * <summary>
		 * Copy file to destination.
		 * </summary>
		 * <param name="file">File to copy.</param>
		 * <param name="destination">Destination to copy file to.</param>
		 * <returns>True unless destination already exists.</returns>
		 * <exception cname='OperationIOException'>Unable to copy file. Inner exception specifies cause.</exception>
		 */
		public F.Result<F.Nothing> CopyFile(IFileInfo file, string destination)
		=> ( new FileInfo(destination).Exists
		   ? OperationIOException.FileExists(destination)
		   : F.Factory.Result(destination)
		   )
		   .SelectMany
		    (_
		     => F.Factory.Try
		         ( () => file.CopyTo(destination)
		         , OperationIOException.CopyErrorMap(destination)
		         )
		    )
		   .Combine<F.Nothing>(F.Nothing.Value);
	}
}