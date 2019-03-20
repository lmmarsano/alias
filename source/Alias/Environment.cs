using S = System;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using SSP = System.Security.Permissions;

namespace Alias {
	class Environment: IEnvironment {
		/// <summary>Default configuration file name.</summary>
		const string ConfigurationFileName = @"alias.conf";
		/// <inheritdoc/>
		public string ApplicationDirectory { get; }
		/// <inheritdoc/>
		public string ApplicationName { get; }
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
		 * <summary>Initialize properties about the application path and name, configuration path and file  information, and main arguments.</summary>
		 * <param name="arguments">Main arguments.</param>
		 * <exception cref="TerminalFileException">Configuration file information failure or current directory  failure.</exception>
		 */
		public Environment(SCG.IEnumerable<string> arguments) {
			try {
				WorkingDirectory = S.Environment.CurrentDirectory;
			} catch (System.Exception error) {
				throw TerminalFileException.CurrentDirectoryUnavailable(ConfigurationFilePath, error);
			}
			var appDomain = S.AppDomain.CurrentDomain;
			// Accessing appDomain's properties shouldn't raise exceptions: CurrentDomain is always loaded.
			ApplicationDirectory = appDomain.BaseDirectory;
			ConfigurationFilePath = SIO.Path.Combine(ApplicationDirectory, ConfigurationFileName);
			try {
				ConfigurationFile = new FileInfo(ConfigurationFilePath);
			} catch (S.Exception error) {
				throw TerminalFileException.InaccessiblePath(ConfigurationFilePath, error);
			}
			ApplicationName = SIO.Path.GetFileNameWithoutExtension(appDomain.FriendlyName);
			Arguments = arguments;
		}
		/**
		 * <summary>Initialize properties about the application path and name, configuration path and file  information, main arguments, and IO streams.</summary>
		 * <param name="arguments">Main arguments.</param>
		 * <param name="streamIn">Input stream.</param>
		 * <param name="streamOut">Output stream.</param>
		 * <param name="streamError">Error stream.</param>
		 * <exception cref="TerminalFileException">Configuration file information failure or current directory failure.</exception>
		 */
		public Environment(SCG.IEnumerable<string> arguments, SIO.TextReader streamIn, SIO.TextWriter streamOut, SIO.TextWriter streamError): this(arguments) {
			StreamIn = streamIn;
			StreamOut = streamOut;
			StreamError = streamError;
		}
	}
}