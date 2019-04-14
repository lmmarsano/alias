using S = System;
using SIO = System.IO;
using SSP = System.Security.Permissions;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using static System.Linq.Enumerable;
using NJL = Newtonsoft.Json.Linq;
using AC = Alias.ConfigurationData;
using ST = LMMarsano.SumType;

namespace Alias {
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
	public abstract class TerminalException: S.Exception, IException {
		protected TerminalException() {}
		protected TerminalException(string message): base(message) {}
		protected TerminalException(string message, S.Exception innerException): base(message, innerException) {}
		public virtual SCG.IEnumerable<string> ErrorMessage => Utility.GetErrorMessage(this);
	}
	/**
	 * <summary>
	 * Error attempting to access file.
	 * </summary>
	 */
	abstract class FileException: TerminalException {
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
		protected FileException(string filePath, SSP.FileIOPermissionAccess type) {
			FilePath = filePath;
			Type = type;
		}
		protected FileException(string filePath, SSP.FileIOPermissionAccess type, string message) : base(message) {
			FilePath = filePath;
			Type = type;
		}
		protected FileException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException) : base(message, innerException) {
			FilePath = filePath;
			Type = type;
		}
	}
	/**
	 * <summary>
	 * Default switch branches that should never happen.
	 * </summary>
	 */
	class UnhandledCaseException: TerminalException {
		UnhandledCaseException(string message) : base(message) {}
		public static UnhandledCaseException Error { get; }
		= new UnhandledCaseException(@"Unhandled case encountered: the program cannot continue.");
	}
	/**
	 * <summary>
	 * Program terminating error attempting to access file.
	 * </summary>
	 */
	class TerminalFileException: TerminalException {
		public string FilePath { get; }
		public SSP.FileIOPermissionAccess Type { get; }
		TerminalFileException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException) : base(message, innerException) {
			FilePath = filePath;
			Type = type;
		}
		public static TerminalFileException CurrentDirectoryUnavailable(string configurationFilePath, S.Exception error)
		=> new TerminalFileException(configurationFilePath, SSP.FileIOPermissionAccess.PathDiscovery, @"Current directory unavailable.", error);
		public static TerminalFileException InaccessiblePath(string path, S.Exception error)
		=> new TerminalFileException(path, SSP.FileIOPermissionAccess.PathDiscovery, @"Inaccessible file path.", error);
		public static TerminalFileException ReadErrorMap(string path, S.Exception error)
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
	class DeserialException: TerminalException {
		public IFileInfo File { get; }
		DeserialException(IFileInfo file, string message, S.Exception innerException) : base(message, innerException) {
			File = file;
		}
		public static S.Func<S.Exception, DeserialException> FailureMap(IFileInfo file)
		=> (error)
		=> new DeserialException(file, $@"Unable to process file: {file.FullName}", error);
	}
	/**
	 * <summary>
	 * Error operating serializer.
	 * </summary>
	 */
	class SerializerException: TerminalException {
		public string JsonPath { get; }
		SerializerException(string jsonPath, string message, S.Exception innerException) : base(message, innerException) {
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
	class UnhandledJsonTokenException: S.InvalidOperationException, IException {
		/**
		 * <summary>A <c cref='NJL.JToken'>JSON token</c> with unhandled runtime type.</summary>
		 * <value>A <c cref='NJL.JToken'>JSON token</c> the program is unable to process: this object has an unrecognized runtime type.</value>
		 */
		public NJL.JToken JsonToken { get; }
		/**
		 * <summary>
		 * Initializes the exception for <c cref='NJL.JToken'>JSON tokens</c> that cannot be processed.
		 * Includes error message and <c cref='NJL.JToken'>JSON token</c> causing this exception.
		 * </summary>
		 * <param name="jsonToken">The <c cref='NJL.JToken'>JSON token</c> causing the runtime exception.</param>
		 * <param name="message">Error description.</param>
		 */
		UnhandledJsonTokenException(NJL.JToken jsonToken, string message) : base(message) {
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
	interface INonTerminalException: IException {}
	/**
	 * <summary>
	 * Error attempting to access file not requiring program termination.
	 * </summary>
	 */
	class OperationIOException: FileException, INonTerminalException {
		public OperationIOException(string filePath, SSP.FileIOPermissionAccess type, string message, S.Exception innerException) : base(filePath, type, message, innerException) {}
		public static S.Func<S.Exception, OperationIOException> ReadErrorMap(string destination)
		=> (error)
		=> new OperationIOException(destination, SSP.FileIOPermissionAccess.Read, "Unable to open file for reading.", error);
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
	/**
	 * <summary>
	 * Exception for an argument containing invalid data.
	 * </summary>
	 */
	class InvalidOptionException: TerminalException {
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
		/**
		 * <summary>
		 * The name of the parameter that causes this exception.
		 * </summary>
		 */
		public string ParamName { get; }
		InvalidOptionException(string context, string paramName, string value, string message, S.Exception inner) : base(message, inner) {
			Context = context;
			ParamName = paramName;
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
		public static S.Func<S.Exception, InvalidOptionException> InvalidAliasName(string class_, string property, string value)
		=> (error)
		=> new InvalidOptionException(class_, property, value, @"Invalid alias name.", error);
		public static S.Func<S.Exception, InvalidOptionException> InvalidAliasCommand(string class_, string property, string value)
		=> (error)
		=> new InvalidOptionException(class_, property, value, @"Invalid alias command.", error);
	}
	/**
	 * <summary>
	 * Exception for unparsable arguments.
	 * </summary>
	 */
	class UnparsableOptionException: TerminalException {
		const string _message = @"Unable to parse arguments.";
		/**
		 * <summary>
		 * Unparsable arguments.
		 * </summary>
		 * <value>Command invocation arguments that could not be parsed.</value>
		 */
		public Arguments Arguments { get; }
		UnparsableOptionException(Arguments arguments, string message) : base(message) {
			Arguments = arguments;
		}
		UnparsableOptionException(Arguments arguments, string message, S.Exception inner) : base(message, inner) {
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
	 * Bindings contain a resolution cycle.
	 * </summary>
	 */
	class CyclicBindingException: TerminalException {
		/// <inheritdoc/>
		public override SCG.IEnumerable<string> ErrorMessage
		=> new []
		   { Message
		   , string.Join
		     ( " → "
		     , Configuration.Binding.GetResolutionSequence(CyclicEntry.Command)
		       .TakeWhile(entry => !object.ReferenceEquals(CyclicEntry, entry))
		       .Select(entry => entry.Command)
		       .Prepend(CyclicEntry.Command)
		       .Append(CyclicEntry.Command)
		     )
		   , "Try editing the alias configuration to eliminate cycles."
		   };
		public AC.Configuration Configuration { get; }
		/**
		 * <summary>
		 * Member of a binding cycle.
		 * </summary>
		 */
		public AC.CommandEntry CyclicEntry { get; }
		CyclicBindingException(AC.Configuration configuration, AC.CommandEntry cyclicEntry, string message) : base(message) {
			Configuration = configuration;
			CyclicEntry = cyclicEntry;
		}
		/**
		 * <summary>
		 * Create error for unresolvable commands.
		 * </summary>
		 * <param name="cyclicEntry">A cycle member.</param>
		 * <returns>Error for unresolvable commands.</returns>
		 */
		public static CyclicBindingException CommandError(AC.Configuration configuration, AC.CommandEntry cyclicEntry)
		=> new CyclicBindingException(configuration, cyclicEntry, @"Unable to resolve command due to a cycle in alias resolution.");
	}
	/**
	 * <summary>
	 * Exceptions for failure to perform an option’s operations.
	 * </summary>
	 * <typeparam name="T">The option type.</typeparam>
	 */
	abstract class OperationException<T>: TerminalException where T : AbstractOption {
		/**
		 * <summary>
		 * The options associated with exception.
		 * </summary>
		 */
		public T Option { get; }
		protected OperationException(T option) {
			Option = option;
		}
		protected OperationException(T option, string message): base(message) {
			Option = option;
		}
		protected OperationException(T option, string message, S.Exception innerException): base(message, innerException) {
			Option = option;
		}
	}
	/**
	 * <summary>
	 * Exception for external command failure.
	 * </summary>
	 */
	class ExternalOperationException: OperationException<External> {
		/**
		 * <summary>
		 * Invocation arguments line if any.
		 * </summary>
		 */
		public ST.Maybe<string> Arguments { get; }
		ExternalOperationException(External option, ST.Maybe<string> arguments, string message, S.Exception inner): base(option, message, inner) {
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
		public static S.Func<S.Exception, ExternalOperationException> GetRunFailureMap(External option, ST.Maybe<string> maybeArgumentLine)
		=> (inner)
		=> new ExternalOperationException(option, maybeArgumentLine, @"Unable to run external command.", inner);
	}
	/**
	 * <summary>
	 * Exception for list command failure.
	 * </summary>
	 */
	class ListOperationException: OperationException<List> {
		ListOperationException(List option, string message, S.Exception inner) : base(option, message, inner) {}
		/**
		 * <summary>
		 * Create an exception map for output failures.
		 * </summary>
		 * <param name="option">The list command options.</param>
		 * <returns>An exception map from original exception to exception indicating output failure.</returns>
		 */
		public static S.Func<S.Exception, ListOperationException> OutputFailureMap(List option)
		=> (inner)
		=> new ListOperationException(option, @"Unable to output list command.", inner);
	}
	/**
	 * <summary>
	 * Exception for unset command failure.
	 * </summary>
	 */
	class UnsetOperationException: OperationException<Unset> {
		UnsetOperationException(Unset option, string message) : base(option, message) {}
		/**
		 * <summary>
		 * Create an exception for non-existent aliases.
		 * </summary>
		 * <param name="option">The unset command options.</param>
		 * <returns>An exception for non-existent aliases.</returns>
		 */
		public static UnsetOperationException AliasUndefined(Unset option)
		=> new UnsetOperationException(option, $@"Unable to remove non-existent alias: {option.Name}.");
	}
}
