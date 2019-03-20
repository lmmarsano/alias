using S = System;
using CL = CommandLine;
using Name = System.String;
using F = Functional;
using static Functional.Extension;

namespace Alias.Option {
	/**
	 * <summary>
	 * Unset command options.
	 * </summary>
	 */
	[CL.Verb(@"unset", HelpText = @"Remove configured alias.")]
	sealed class Unset: AbstractOption, S.IEquatable<Unset> {
		[CL.Value(0, Required = true, HelpText = @"Alias name.")]
		public Name Name { get; }
		/// <inheritdoc/>
		public override F.Result<AbstractOption> Validation
		=> Utility.ValidateFileName(Name)
		   .SelectError(InvalidOptionException.InvalidAliasName(nameof(Unset), nameof(Name), Name))
			 .Combine(F.Factory.Result<AbstractOption>(this))
			 ;
		/**
		 * <summary>
		 * Construct parsed options for unset verb.
		 * </summary>
		 * <param name="name">Name of alias to unset.</param>
		 */
		public Unset(Name name) {
			Name = name;
		}
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">The other operand.</param>
		 * <returns>True if and only if equal.</returns>
		 */
		public bool Equals(Unset other)
		=> Name == other.Name;
		/// <inheritdoc/>
		public override bool Equals(object obj)
		=> obj is Unset value
		&& Equals(value);
		/// <inheritdoc/>
		public override int GetHashCode() => Name.GetHashCode();
		/// <inheritdoc/>
		public override F.Result<ExitCode> Operate(IOperation operation) => operation.Unset(this);
	}
}