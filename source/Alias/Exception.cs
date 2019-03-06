using S = System;
using SIO = System.IO;
using SSP = System.Security.Permissions;
using NJL = Newtonsoft.Json.Linq;

namespace Alias {
	/**
	 * <summary>
	 * Interface for exceptions originating from <c cref='Alias'>Alias</c>.
	 * </summary>
	 */
	public interface IException {}
	/**
	 * <summary>
	 * Interface for exceptions terminating program.
	 * </summary>
	 */
	interface ITerminalException: IException {}
	/**
	 * <summary>
	 * Error attempting to access file.
	 * </summary>
	 */
	class FileException: S.IO.IOException, ITerminalException {
		public string FilePath { get; }
		public SSP.FileIOPermissionAccess Type { get; }
		public FileException(string filePath, SSP.FileIOPermissionAccess type) {
			FilePath = filePath;
			Type = type;
		}
		public FileException(string filePath, SSP.FileIOPermissionAccess type, string message): base(message) {
			FilePath = filePath;
			Type = type;
		}
		public FileException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException): base(message, innerException) {
			FilePath = filePath;
			Type = type;
		}
	}
	/**
	 * <summary>
	 * Error deserializing file.
	 * </summary>
	 */
	class DeserialException: S.IO.IOException, ITerminalException {
		public SIO.FileInfo File { get; }
		public DeserialException(SIO.FileInfo file) {
			File = file;
		}
		public DeserialException(SIO.FileInfo file, string message): base(message) {
			File = file;
		}
		public DeserialException(SIO.FileInfo file, string message, S.Exception innerException): base(message, innerException) {
			File = file;
		}
	}
	/**
	 * <summary>
	 * Thrown when execution encounters a <c cref='NJL.JToken'>JToken</c>-derived object the program is unable to process.
	 * </summary>
	 */
	public class UnhandledJsonTokenException: S.InvalidOperationException, IException {
		/**
		 * <summary>A <c cref='NJL.JToken'>JSON token</c> with unhandled runtime type.</summary>
		 * <value>A <c cref='NJL.JToken'>JSON token</c> the program is unable to process: this object has an unrecognized runtime type.</value>
		 */
		public NJL.JToken JsonToken { get; }
		/**
		 * <summary>
		 * Initializes the exception for <c cref='NJL.JToken'>JSON tokens</c> that cannot be processed.
		 * Includes <c cref='NJL.JToken'>JSON token</c> causing this exception.
		 * </summary>
		 * <param name="jsonToken">The <c cref='NJL.JToken'>JSON token</c> causing the runtime exception.</param>
		 * <param name="message">Error description.</param>
		 */
		public UnhandledJsonTokenException(NJL.JToken jsonToken) {
			JsonToken = jsonToken;
		}
		/**
		 * <summary>
		 * Initializes the exception for <c cref='NJL.JToken'>JSON tokens</c> that cannot be processed.
		 * Includes error message and <c cref='NJL.JToken'>JSON token</c> causing this exception.
		 * </summary>
		 * <param name="jsonToken">The <c cref='NJL.JToken'>JSON token</c> causing the runtime exception.</param>
		 * <param name="message">Error description.</param>
		 */
		public UnhandledJsonTokenException(NJL.JToken jsonToken, string message): base(message) {
			JsonToken = jsonToken;
		}
		/**
		 * <summary>
		 * Initializes the exception for <c cref='NJL.JToken'>JSON tokens</c> that cannot be processed.
		 * Includes error message and references to the inner exception and <c cref='NJL.JToken'>JSON token</c> causing this exception.
		 * </summary>
		 * <param name="jsonToken">The <c cref='NJL.JToken'>JSON token</c> causing the runtime exception.</param>
		 * <param name="message">Error description.</param>
		 * <param name="innerException">The <c cref='S.Exception'>exception</c> causing this runtime exception.</param>
		 */
		public UnhandledJsonTokenException(NJL.JToken jsonToken, string message, S.Exception innerException): base(message, innerException) {
			JsonToken = jsonToken;
		}
	}
}
