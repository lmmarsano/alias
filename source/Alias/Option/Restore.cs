using S = System;
using STT = System.Threading.Tasks;
using CL = CommandLine;
using ST = LMMarsano.SumType;

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
		public override ST.Result<STT.Task<ExitCode>> Operate(IOperation operation) => operation.Restore(this);
	}
}
