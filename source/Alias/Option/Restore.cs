using S = System;
using CL = CommandLine;
using F = Functional;
using static Functional.Extension;

namespace Alias.Option {
	/**
	 * <summary>
	 * Restore command options.
	 * </summary>
	 */
	[CL.Verb(@"restore", HelpText = @"Recreate file system for configured aliases.")]
	sealed class Restore: AbstractOption, S.IEquatable<Restore> {
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">The other operand.</param>
		 * <returns>True if and only if equal.</returns>
		 */
		public bool Equals(Restore other) => true;
		/// <inheritdoc/>
		public override bool Equals(object obj)
		=> obj is Restore value
		&& Equals(value);
		/// <inheritdoc/>
		public override int GetHashCode() => 0;
		/// <inheritdoc/>
		public override F.Result<ExitCode> Operate(IOperation operation) => operation.Restore(this);
	}
}
