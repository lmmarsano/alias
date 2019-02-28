using S = System;
using NJL = Newtonsoft.Json.Linq;

namespace Alias {
	/**
	 * <summary>
	 * Base class for exceptions originating from <c cref='Alias'>Alias</c>.
	 * </summary>
	 */
	public abstract class Exception: S.Exception {
		public Exception() {}
		public Exception(string message) : base(message) {}
		public Exception(string message, S.Exception innerException): base(message, innerException) {}
	}
	/**
	 * <summary>
	 * Thrown when execution encounters a <c cref='NJL.JToken'>JToken</c>-derived object the program is unable to process.
	 * </summary>
	 */
	public class UnhandledJsonTokenException: Exception {
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
