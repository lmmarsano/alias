using S = System;
using CL = CommandLine;
using F = Functional;
using static Functional.Extension;

namespace Alias.Option {
	/**
	 * <summary>
	 * List command options.
	 * </summary>
	 */
	[CL.Verb(@"list", HelpText = @"List aliases.")]
	sealed class List: AbstractOption, S.IEquatable<List> {
		/**
		 * <summary>
		 * Construct parsed options for list verb.
		 * </summary>
		 */
		public List() {}
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">The other operand.</param>
		 * <returns>True if and only if equal.</returns>
		 */
		public bool Equals(List other) => true;
		/// <inheritdoc/>
		public override bool Equals(object obj)
		=> obj is List value
		&& Equals(value);
		/// <inheritdoc/>
		public override int GetHashCode() => 0;
		/// <inheritdoc/>
		public override F.Result<ExitCode> Operate(IOperation operation) => operation.List(this);
	}
}
