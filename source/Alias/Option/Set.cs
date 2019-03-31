using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using CL = CommandLine;
using F = Functional;
using Name = System.String;
using Command = System.String;
using Argument = System.String;

namespace Alias.Option {
	using Arguments = SCG.IEnumerable<Argument>;
	/**
	 * <summary>
	 * Type of file to create when adding an alias.
	 * </summary>
	 *
	enum SetFileType
	{ None
	, Symlink
	, Hardlink
	, Copy
	}
	 */
	// TODO handle file type option
	/**
	 * <summary>
	 * Set command options.
	 * </summary>
	 */
	[CL.Verb(@"set", HelpText = @"Add or change an alias.")]
	sealed class Set: AbstractOption, S.IEquatable<Set> {
		[CL.Value(0, Required = true, HelpText = @"Name for the alias.")]
		public Name Name { get; }
		[CL.Value(1, Required = true, HelpText = @"Command the alias invokes.")]
		public Command Command { get; }
		[CL.Value(2, HelpText = @"Arguments alias invokes with command.")]
		public Arguments Arguments { get; }
		/**
		 * <summary>
		 * Arguments as nullable, space separated argument string as the serializer requires.
		 * </summary>
		 * <value>An argument string or null (no arguments).</value>
		 */
		public string? ArgumentString
		=> Arguments.Any()
		 ? string.Join(' ', Arguments.Select(Utility.SafeQuote))
		 : null;
		/// <inheritdoc/>
		public override F.Result<AbstractOption> Validation
		=> Utility.ValidateFileName(Name)
		   .SelectError(InvalidOptionException.InvalidAliasName(nameof(Unset), nameof(Name), Name))
		   .Combine
		    ( Utility.ValidatePath(Command)
		      .SelectError
		       ( error
		         => new InvalidOptionException
		            (nameof(Set), nameof(Command), Command, @"Invalid alias command.", error)
		       )
		    )
		   .Combine(F.Factory.Result<AbstractOption>(this));
		/**
		 * <summary>
		 * Construct parsed options for set verb.
		 * </summary>
		 * <param name="name">The name to set.</param>
		 * <param name="command">The command to set for name.</param>
		 * <param name="arguments">Arguments to include with command.</param>
		 * <exception cref='InvalidOptionException'>The operating system cannot accept <paramref name='name'/> or <paramref name='command'/> as a valid filename or path.</exception>
		 */
		public Set(Name name, Command command, Arguments arguments) {
			Name = name;
			Command = command;
			Arguments = arguments;
		}
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">The other operand.</param>
		 * <returns>True if and only if equal.</returns>
		 */
		public bool Equals(Set other)
		=> Name == other.Name
		&& Command == other.Command
		&& Arguments == other.Arguments;
		/// <inheritdoc/>
		public override bool Equals(object obj)
		=> obj is Set value
		&& Equals(value);
		/// <inheritdoc/>
		public override int GetHashCode() => S.HashCode.Combine(Name, Command, Arguments);
		/// <inheritdoc/>
		/// <exception cref='ExternalOperationException'>External command fails to run.</exception>
		public override F.Result<STT.Task<ExitCode>> Operate(IOperation operation) => operation.Set(this);
	}
}