using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using CL = CommandLine;
using CLT = CommandLine.Text;
using Name = System.String;
using ST = LMMarsano.SumType;

namespace Alias.Option {
	/**
	 * <summary>
	 * Unset command options.
	 * </summary>
	 */
	[CL.Verb(@"unset", HelpText = @"Remove configured alias.")]
	sealed class Unset: AbstractOption, S.IEquatable<Unset> {
		[CL.Value(0, MetaName = @"name", Required = true, HelpText = @"Alias name.")]
		public Name Name { get; }
		[CLT.Usage]
		public static SCG.IEnumerable<CLT.Example> Example { get; }
		= new CLT.Example[]
		  { new CLT.Example(@"Remove mklink.exe as an alias", new Unset(@"mklink.exe"))
		  };
		/// <inheritdoc/>
		public override ST.Result<AbstractOption> Validation
		=> Utility.ValidateFileName(Name)
		   .SelectError(InvalidOptionException.InvalidAliasName(nameof(Unset), nameof(Name), Name))
		   .Combine(ST.Factory.Result<AbstractOption>(this))
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
		public override int GetHashCode() => Name.GetHashCode(S.StringComparison.Ordinal);
		/// <inheritdoc/>
		public override ST.Result<STT.Task<ExitCode>> Operate(IOperation operation) => operation.Unset(this);
	}
}
