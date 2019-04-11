using S = System;
using STT = System.Threading.Tasks;
using CL = CommandLine;
using ST = LMMarsano.SumType;

namespace Alias.Option {
	/**
	 * <summary>
	 * Reset command options.
	 * </summary>
	 */
	[CL.Verb(@"reset", HelpText = @"Remove all aliases.")]
	sealed class Reset: AbstractOption, S.IEquatable<Reset> {
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">The other operand.</param>
		 * <returns>True if and only if equal.</returns>
		 */
		public bool Equals(Reset other) => true;
		/// <inheritdoc/>
		public override bool Equals(object obj)
		=> obj is Reset value
		&& Equals(value);
		/// <inheritdoc/>
		public override int GetHashCode() => 0;
		/// <inheritdoc/>
		public override ST.Result<STT.Task<ExitCode>> Operate(IOperation operation) => operation.Reset(this);
	}
}
