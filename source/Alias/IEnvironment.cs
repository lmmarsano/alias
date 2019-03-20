﻿using SCG = System.Collections.Generic;
using SIO = System.IO;

namespace Alias {
	interface IEnvironment {
		/// <summary>Path to the application’s parent directory.</summary>
		string ApplicationDirectory { get; }
		/**
		 * <summary>The application’s base file name.</summary>
		 * <value>The application’s file name without the last file extension (if any).</value>
		 */
		string ApplicationName { get; }
		/**
		 * <summary>Command arguments.</summary>
		 * <value>Arguments passed from main invocation.</value>
		 */
		SCG.IEnumerable<string> Arguments { get; }
		/**
		 * <summary>Path to the configuration file.</summary>
		 * <value>The configuration file path, which normally shares parent directory with the application.</value>
		 */
		string ConfigurationFilePath { get; }
		/// <summary>Configuration file information.</summary>
		IFileInfo ConfigurationFile { get; }
		/**
		<summary>Application’s working directory.</summary>
		<value>Fully qualified path of the current working directory.</value>
		 */
		string WorkingDirectory { get; }
		/**
		 * <summary>
		 * Input stream.
		 * </summary>
		 * <value>Stream for application input.</value>
		 */
		SIO.TextReader StreamIn { get; }
		/**
		 * <summary>
		 * Output stream.
		 * </summary>
		 * <value>Stream for application normal output.</value>
		 */
		SIO.TextWriter StreamOut { get; }
		/**
		 * <summary>
		 * Error stream.
		 * </summary>
		 * <value>Stream for application error output.</value>
		 */
		SIO.TextWriter StreamError { get; }
		/**
		 * <summary>
		 * An effect instance.
		 * </summary>
		 * <value>An instance providing methods producing effects.</value>
		 */
		IEffect Effect { get; }
	}
}