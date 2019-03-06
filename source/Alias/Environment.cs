using S = System;
using SIO = System.IO;
using SCG = System.Collections.Generic;

namespace Alias {
	class Environment: IEnvironment {
		/**
		 * <summary>Default configuration file name.</summary>
		 */
		const string ConfigurationFileName = "alias.conf";
		/**
		 * <summary>Path to the application’s parent directory.</summary>
		 */
		public string ApplicationDirectory { get; }
		/**
		 * <summary>The application’s base file name.</summary>
		 * <value>The application’s file name without the last file extension (if any).</value>
		 */
		public string ApplicationName { get; }
		/**
		 * <summary>Path to the configuration file.</summary>
		 * <value>The configuration file path, which normally shares parent directory with the application.</value>
		 */
		public string ConfigurationFilePath { get; }
		/**
		 * <summary>Command arguments.</summary>
		 * <value>Arguments passed from main invocation.</value>
		 */
		public SCG.IEnumerable<string> Arguments { get; }
		/**
		 * <summary>Initialize properties.</summary>
		 */
		public Environment(SCG.IEnumerable<string> arguments) {
			var appDomain = S.AppDomain.CurrentDomain;
			// Accessing appDomain's shouldn't raise exceptions: CurrentDomain is always loaded.
			ApplicationDirectory = appDomain.BaseDirectory;
			ApplicationName = SIO.Path.GetFileNameWithoutExtension(appDomain.FriendlyName);
			ConfigurationFilePath = SIO.Path.Combine(ApplicationDirectory, ConfigurationFileName);
			Arguments = arguments;
		}
	}
}
