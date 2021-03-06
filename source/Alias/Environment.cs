﻿using S = System;
using SD = System.Diagnostics;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using ST = LMMarsano.SumType;

namespace Alias {
	class Environment: IEnvironment {
		/// <summary>Default configuration file name.</summary>
		const string _configurationFileName = @"alias.conf";
		/// <inheritdoc/>
		public string ApplicationDirectory { get; }
		/// <inheritdoc/>
		public string ApplicationName { get; }
		/// <inheritdoc/>
		public IFileInfo ApplicationFile { get; }
		/// <inheritdoc/>
		public string ConfigurationFilePath { get; }
		/// <inheritdoc/>
		public IFileInfo ConfigurationFile { get; }
		/// <inheritdoc/>
		public SCG.IEnumerable<string> Arguments { get; }
		/// <inheritdoc/>
		public string WorkingDirectory { get; }
		/// <inheritdoc/>
		public SIO.TextReader StreamIn { get; } = S.Console.In;
		/// <inheritdoc/>
		public SIO.TextWriter StreamOut { get; } = S.Console.Out;
		/// <inheritdoc/>
		public SIO.TextWriter StreamError { get; } = S.Console.Error;
		/// <inheritdoc/>
		public IEffect Effect { get; } = new Effect();
		/**
		 * <summary>Initialize properties about the application path and name, configuration path and file information, and main arguments.</summary>
		 * <param name="arguments">Main arguments.</param>
		 * <exception cref="TerminalFileException">Failure accessing application/configuration file information or current directory.</exception>
		 */
		public Environment(SCG.IEnumerable<string> arguments) {
			try {
				WorkingDirectory = S.Environment.CurrentDirectory;
			} catch (S.Exception error) {
				throw TerminalFileException.CurrentDirectoryUnavailable(ConfigurationFilePath, error);
			}
			using (var currentProcess = SD.Process.GetCurrentProcess()) {
				ApplicationName = currentProcess.ProcessName;
				using var mainModule = currentProcess.MainModule;
				// No exceptions should get thrown on a process necessarily running to execute this code.
				ApplicationFile = GetFileInfo(mainModule.FileName);
			}
			ApplicationDirectory = ApplicationFile.DirectoryName;
			ConfigurationFilePath = SIO.Path.Combine(ApplicationDirectory, _configurationFileName);
			ConfigurationFile = GetFileInfo(ConfigurationFilePath);
			Arguments = arguments;
		}
		/**
		 * <summary>Initialize properties about the application path and name, configuration path and file information, main arguments, and IO streams.</summary>
		 * <param name="arguments">Main arguments.</param>
		 * <param name="streamIn">Input stream.</param>
		 * <param name="streamOut">Output stream.</param>
		 * <param name="streamError">Error stream.</param>
		 * <exception cref="TerminalFileException">Configuration file information failure or current directory failure.</exception>
		 */
		public Environment(SCG.IEnumerable<string> arguments, SIO.TextReader streamIn, SIO.TextWriter streamOut, SIO.TextWriter streamError) : this(arguments) {
			StreamIn = streamIn;
			StreamOut = streamOut;
			StreamError = streamError;
		}
		/**
		 * <summary>
		 * Get file information.
		 * </summary>
		 * <param name="path">Path to file.</param>
		 * <exception cref='TerminalFileException'>Inaccessible path.</exception>
		 * <returns>File information.</returns>
		 */
		FileInfo GetFileInfo(string path) {
			try {
				return new FileInfo(path);
			} catch (S.Exception error) {
				throw TerminalFileException.InaccessiblePath(path, error);
			}
		}
		/**
		 * <summary>
		 * Get an error stream from environment with default fallback.
		 * </summary>
		 * <param name="maybeEnvironment">Optional environment.</param>
		 * <returns>An error output stream.</returns>
		 */
		public static SIO.TextWriter GetErrorStream(ST.Maybe<IEnvironment> maybeEnvironment)
		=> maybeEnvironment is ST.Just<IEnvironment>(var environment)
		 ? environment.StreamError
		 : S.Console.Error;
	}
}
