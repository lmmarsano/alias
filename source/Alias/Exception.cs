using S = System;
using SIO = System.IO;
using SSP = System.Security.Permissions;
using SCG = System.Collections.Generic;
using NJL = Newtonsoft.Json.Linq;
using F = Functional;
using static Functional.Extension;

namespace Alias {
	using System.Threading.Tasks;
	using Alias.Configuration;
	using Alias.Option;
	using Arguments = SCG.IEnumerable<string>;
	/**
	 * <summary>
	 * Interface for exceptions originating from <c cref='Alias'>Alias</c>.
	 * </summary>
	 */
	interface IException {
		string Message { get; }
	}
	/**
	 * <summary>
	 * Interface for exceptions terminating program.
	 * </summary>
	 */
	interface ITerminalException : IException {}
	/**
	 * <summary>
	 * Error attempting to access file.
	 * </summary>
	 */
	abstract class FileException : SIO.IOException, ITerminalException {
		/**
		 * <summary>
		 * File path triggering IO exception.
		 * </summary>
		 * <value>A file path.</value>
		 */
		public string FilePath { get; }
		/**
		 * <summary>
		 * Type of IO access triggering exception.
		 * </summary>
		 * <value>Permission associated with type of IO access.</value>
		 */
		public SSP.FileIOPermissionAccess Type { get; }
		public FileException(string filePath, SSP.FileIOPermissionAccess type) {
			FilePath = filePath;
			Type = type;
		}
		public FileException(string filePath, SSP.FileIOPermissionAccess type, string message) : base(message) {
			FilePath = filePath;
			Type = type;
		}
		public FileException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException) : base(message, innerException) {
			FilePath = filePath;
			Type = type;
		}
	}
	/**
	 * <summary>
	 * Default switch branches that should never happen.
	 * </summary>
	 */
	class UnhandledCaseException: S.NotImplementedException, ITerminalException {
		UnhandledCaseException(string message): base(message) {}
		public static UnhandledCaseException Error { get; }
		= new UnhandledCaseException(@"Unhandled case encountered: the program cannot continue.");
	}
	/**
	 * <summary>
	 * Program terminating error attempting to access file.
	 * </summary>
	 */
	class TerminalFileException : FileException, ITerminalException {
		public TerminalFileException(string filePath, SSP.FileIOPermissionAccess type) : base(filePath, type) {}
		public TerminalFileException(string filePath, SSP.FileIOPermissionAccess type, string message) : base(filePath, type, message) {}
		public TerminalFileException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException) : base(filePath, type, message, innerException) {}
		public static TerminalFileException CurrentDirectoryUnavailable(string configurationFilePath, S.Exception error)
		=> new TerminalFileException(configurationFilePath, SSP.FileIOPermissionAccess.PathDiscovery, @"Current directory unavailable.", error);
		public static TerminalFileException InaccessiblePath(string path, S.Exception error)
		=> new TerminalFileException(path, SSP.FileIOPermissionAccess.PathDiscovery, @"Inaccessible file path.", error);
		public static S.Func<S.Exception, TerminalFileException> ReadErrorMap(string path)
		=> (error)
		=> new TerminalFileException(path, SSP.FileIOPermissionAccess.Read, @"Unable to open file for reading.", error);
		public static S.Func<S.Exception, S.Exception> WriteErrorMap(string path)
		=> (error)
		=> new TerminalFileException(path, SSP.FileIOPermissionAccess.Write, @"Unable to create or write file.", error);
	}
	/**
	 * <summary>
	 * Error deserializing file.
	 * </summary>
	 */
	class DeserialException : S.InvalidOperationException, ITerminalException {
		public IFileInfo File { get; }
		public DeserialException(IFileInfo file) {
			File = file;
		}
		public DeserialException(IFileInfo file, string message) : base(message) {
			File = file;
		}
		public DeserialException(IFileInfo file, string message, S.Exception innerException) : base(message, innerException) {
			File = file;
		}
		public static S.Func<S.Exception, DeserialException> FailureMap(IFileInfo file)
		=> (error)
		=> new DeserialException(file, @"Deserialization failure.", error);
	}
	/**
	 * <summary>
	 * Error operating serializer.
	 * </summary>
	 */
	class SerializerException : S.InvalidOperationException, ITerminalException {
		public string JsonPath { get; }
		public SerializerException(string jsonPath) {
			JsonPath = jsonPath;
		}
		public SerializerException(string jsonPath, string message) : base(message) {
			JsonPath = jsonPath;
		}
		public SerializerException(string jsonPath, string message, S.Exception innerException) : base(message, innerException) {
			JsonPath = jsonPath;
		}
		public static SerializerException Failure(string jsonPath, S.Exception error)
		=> new SerializerException(jsonPath, @"Serializer failure.", error);
	}
	/**
	 * <summary>
	 * Thrown when execution encounters a <c cref='NJL.JToken'>JToken</c>-derived object the program is unable to process.
	 * </summary>
	 */
	class UnhandledJsonTokenException : S.InvalidOperationException, IException {
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
		public UnhandledJsonTokenException(NJL.JToken jsonToken, string message) : base(message) {
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
		public UnhandledJsonTokenException(NJL.JToken jsonToken, string message, S.Exception innerException) : base(message, innerException) {
			JsonToken = jsonToken;
		}
		public static UnhandledJsonTokenException GetUnknown(NJL.JToken jToken)
		=> throw new UnhandledJsonTokenException(jToken, $"Factory for {jToken.GetType()} not implemented.");
	}
	/**
	 * <summary>
	 * Interface for exceptions not requiring program termination.
	 * </summary>
	 */
	interface INonTerminalException : IException {}
	/**
	 * <summary>
	 * Error attempting to access file not requiring program termination.
	 * </summary>
	 */
	class OperationIOException : FileException, INonTerminalException {
		public OperationIOException(string filePath, SSP.FileIOPermissionAccess type) : base(filePath, type) {}
		public OperationIOException(string filePath, SSP.FileIOPermissionAccess type, string message) : base(filePath, type, message) {}
		public OperationIOException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException) : base(filePath, type, message, innerException) {}
		public static S.Func<S.Exception, OperationIOException> ReadErrorMap(string destination)
		=> (error)
		=> new OperationIOException(destination, SSP.FileIOPermissionAccess.Read, "Unable to open file for reading.", error);
		public static S.Func<S.Exception, OperationIOException> WriteErrorMap(string destination)
		=> (error)
		=> new OperationIOException(destination, SSP.FileIOPermissionAccess.Write, "Unable to write configuration file.", error);
		public static S.Func<S.Exception, OperationIOException> CreateErrorMap(string destination)
		=> (error)
		=> new OperationIOException(destination, SSP.FileIOPermissionAccess.Write, "Unable to create file for writing.", error);
		public static S.Func<S.Exception, OperationIOException> CopyErrorMap(string destination)
		=> (error)
		=> new OperationIOException(destination, SSP.FileIOPermissionAccess.Write, "File copy error.", error);
		public static S.Func<S.Exception, OperationIOException> DeleteErrorMap(string destination)
		=> (error)
		=> new OperationIOException(destination, SSP.FileIOPermissionAccess.Write, "Unable to access file for deletion.", error);
	}
	class HelpException: S.Exception, INonTerminalException {
		public static HelpException HelpRequest { get; } = new HelpException(@"Help requested.");
		public HelpException() {}
		public HelpException(string message): base(message) {}
		public HelpException(string message, S.Exception inner): base(message, inner) {}
	}
	/**
	 * <summary>
	 * Exception for an argument containing invalid data.
	 * </summary>
	 */
	class InvalidOptionException: S.ArgumentException, ITerminalException {
		/**
		 * <summary>
		 * Invalid value.
		 * </summary>
		 * <value>Value of the invalid argument.</value>
		 */
		public string Value { get; }
		/**
		 * <summary>
		 * Class or method name.
		 * </summary>
		 * <value>Class or method associated with invalid argument.</value>
		 */
		public string Context { get; }
		public InvalidOptionException(string context, string paramName, string value): base(null, paramName) {
			Context = context;
			Value = value;
		}
		public InvalidOptionException(string context, string paramName, string value, string message) : base(message, paramName) {
			Context = context;
			Value = value;
		}
		public InvalidOptionException(string context, string paramName, string value, string message, S.Exception inner) : base(message, paramName, inner) {
			Context = context;
			Value = value;
		}
		/**
		 * <summary>
		 * Produce an exception generator for invalid alias names.
		 * </summary>
		 * <param name="class_">Class name.</param>
		 * <param name="property">Property name.</param>
		 * <param name="value">Invalid value.</param>
		 * <returns>A map from originating exceptions to exceptions for invalid alias names.</returns>
		 */
		public static S.Func<S.Exception,InvalidOptionException> InvalidAliasName(string class_, string property, string value)
		=> (error)
		=> new InvalidOptionException(class_, property, value, @"Invalid alias name.", error);
	}
	/**
	 * <summary>
	 * Exception for unparsable arguments.
	 * </summary>
	 */
	class UnparsableOptionException: S.Exception, ITerminalException {
		const string _message = @"Unable to parse arguments.";
		/**
		 * <summary>
		 * Unparsable arguments.
		 * </summary>
		 * <value>Command invocation arguments that could not be parsed.</value>
		 */
		public Arguments Arguments { get; }
		public UnparsableOptionException(Arguments arguments) {
			Arguments = arguments;
		}
		public UnparsableOptionException(Arguments arguments, string message) : base(message) {
			Arguments = arguments;
		}
		public UnparsableOptionException(Arguments arguments, string message, S.Exception inner) : base(message, inner) {
			Arguments = arguments;
		}
		public static S.Func<S.Exception, UnparsableOptionException> UnparsableMap(Arguments arguments)
		=> (error)
		=> new UnparsableOptionException(arguments, _message, error);
		public static UnparsableOptionException Unparsable(Arguments arguments)
		=> new UnparsableOptionException(arguments, _message);
	}
	/**
	 * <summary>
	 * Exceptions for failure to perform an option’s operations.
	 * </summary>
	 * <typeparam name="T">The option type.</typeparam>
	 */
	interface IOperationException<T>: ITerminalException where T: Option.AbstractOption {
		/**
		 * <summary>
		 * The options associated with exception.
		 * </summary>
		 */
		T Option { get; }
	}
	/**
	 * <summary>
	 * Exception for external command failure.
	 * </summary>
	 */
	class ExternalOperationException: S.Exception, IOperationException<Option.External> {
		/// <inheritdoc/>
		public External Option { get; }
		/**
		 * <summary>
		 * Invocation arguments line if any.
		 * </summary>
		 */
		public F.Maybe<string> Arguments { get; }
		public ExternalOperationException(External option, F.Maybe<string> arguments) {
			Option = option;
			Arguments = arguments;
		}
		public ExternalOperationException(External option, F.Maybe<string> arguments, string message): base(message) {
			Option = option;
			Arguments = arguments;
		}
		public ExternalOperationException(External option, F.Maybe<string> arguments, string message, S.Exception inner): base(message, inner) {
			Option = option;
			Arguments = arguments;
		}
		/**
		 * <summary>
		 * Create an exception map for run failures.
		 * </summary>
		 * <param name="option">The external command options.</param>
		 * <param name="maybeArgumentLine">The argument line if any for command.</param>
		 * <returns>An exception map from original exception to exception indicating run failure.</returns>
		 */
		public static S.Func<S.Exception, ExternalOperationException> GetRunFailureMap(Option.External option, F.Maybe<string> maybeArgumentLine)
		=> (inner)
		=> new ExternalOperationException(option, maybeArgumentLine, @"Unable to run external command.", inner);
	}
	/**
	 * <summary>
	 * Exception for list command failure.
	 * </summary>
	 */
	class ListOperationException : SIO.IOException, IOperationException<Option.List> {
		/// <inheritdoc/>
		public Option.List Option { get; }
		public ListOperationException(Option.List option) {
			Option = option;
		}
		public ListOperationException(Option.List option, string message) : base(message) {
			Option = option;
		}
		public ListOperationException(Option.List option, string message, S.Exception inner) : base(message, inner) {
			Option = option;
		}
		/**
		 * <summary>
		 * Create an exception map for output failures.
		 * </summary>
		 * <param name="option">The list command options.</param>
		 * <returns>An exception map from original exception to exception indicating output failure.</returns>
		 */
		public static S.Func<S.Exception, ListOperationException> OutputFailureMap(Option.List option)
		=> (inner)
		=> new ListOperationException(option, @"Unable to output list command.", inner);
	}
	/**
	 * <summary>
	 * Exception for unset command failure.
	 * </summary>
	 */
	class UnsetOperationException: S.Exception, IOperationException<Option.Unset> {
		/// <inheritdoc/>
		public Unset Option { get; }
		public UnsetOperationException(Unset option) {
			Option = option;
		}
		public UnsetOperationException(Unset option, string message): base(message) {
			Option = option;
		}
		public UnsetOperationException(Unset option, string message, S.Exception inner): base(message, inner) {
			Option = option;
		}
		/**
		 * <summary>
		 * Create an exception for non-existent aliases.
		 * </summary>
		 * <param name="option">The unset command options.</param>
		 * <returns>An exception for non-existent aliases.</returns>
		 */
		public static UnsetOperationException AliasUndefined(Option.Unset option)
		=> new UnsetOperationException(option, $@"Unable to remove non-existent alias: {option.Name}.");
	}
}