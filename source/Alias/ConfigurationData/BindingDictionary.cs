using S = System;
using SDC = System.Diagnostics.CodeAnalysis;
using SCG = System.Collections.Generic;
using static System.Linq.Enumerable;
using Name = System.String;

namespace Alias.ConfigurationData {
	/**
	 * <summary>
	 * Name and command entry associations.
	 * </summary>
	 * <typeparam name="Name">Alias names.</typeparam>
	 * <typeparam name="CommandEntry">Commands with initial arguments.</typeparam>
	 */
#pragma warning disable CS0659 //'BindingDictionary' overrides Object.Equals(object o) but does not override Object.GetHashCode() [Alias]
	public class BindingDictionary: SCG.Dictionary<Name, CommandEntry>, S.IEquatable<BindingDictionary> {
#pragma warning restore CS0659
		public BindingDictionary() {}
		public BindingDictionary(int capacity): base(capacity) {}
		public BindingDictionary(SCG.IDictionary<Name, CommandEntry> dictionary): base
		( dictionary.Where
		  (x => !( string.IsNullOrWhiteSpace(x.Key)
		        || string.IsNullOrWhiteSpace(x.Value.Command)
		         )
		  )
		  .ToBinding(x => x.Key.Trim(), x => x.Value)
		) {}
		public BindingDictionary(BindingDictionary bindingDictionary): base(bindingDictionary) {}
		/**
		 * <summary>
		 * Generate sequence of command entries resolving from <paramref name="name"/> by command lookup.
		 * </summary>
		 * <param name="name">Initial name to lookup.</param>
		 * <returns>Sequence of command entries.</returns>
		 */
		public SCG.IEnumerable<CommandEntry> GetResolutionSequence(Name name) {
			for (; TryGetValue(name, out var commandEntry); name = commandEntry.Command) {
				yield return commandEntry;
			}
		}
		public bool Equals(BindingDictionary other) => Utility.Equals(this, other);
		/// <inheritdoc/>
		public override bool Equals(object other) => (other as BindingDictionary)?.Equals(this) == true;
	}
}
