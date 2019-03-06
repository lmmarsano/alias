using SCG = System.Collections.Generic;

namespace Alias {
	interface IEnvironment {
		/**
		 * <summary>Path to the application’s parent directory.</summary>
		 */
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
	}
}