using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using SDC = System.Diagnostics.CodeAnalysis;
using System.Linq;
using CL = CommandLine;
using CLT = CommandLine.Text;
using ST = LMMarsano.SumType;
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
		[CL.Value(0, MetaName = @"name", Required = true, HelpText = @"Name for the alias.")]
		public Name Name { get; }
		[CL.Value(1, MetaName = @"command", Required = true, HelpText = @"Command the alias invokes.")]
		public Command Command { get; }
		[CL.Value(2, MetaName = @"arguments", HelpText = @"Arguments alias invokes with command.")]
		public Arguments Arguments { get; }
		/**
		 * <summary>
		 * Usage examples.
		 * </summary>
		 */
		[CLT.Usage]
		public static SCG.IEnumerable<CLT.Example> Example { get; }
		= new CLT.Example[]
		  { new CLT.Example(@"Set mklink.exe as an alias to mklink built into cmd", new Set(@"mklink.exe", @"cmd", new [] {@"/c", @"mklink"}))
		  };
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
		[SDC.SuppressMessage("Usage", "CA2208:Instantiate argument exceptions correctly", Justification = "Refers to constructor.")]
		public override ST.Result<AbstractOption> Validation
		=> Utility.ValidateFileName(Name)
		   .SelectError(InvalidOptionException.InvalidAliasName(nameof(Set), nameof(Name), Name))
		   .Combine
		   (Utility.ValidatePath(Command)
		    .SelectError(InvalidOptionException.InvalidAliasCommand(nameof(Set), nameof(Command), Command))
		   )
		   .Combine(ST.Factory.Result<AbstractOption>(this));
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
		public override ST.Result<STT.Task<ExitCode>> Operate(IOperation operation) => operation.Set(this);
	}
}
