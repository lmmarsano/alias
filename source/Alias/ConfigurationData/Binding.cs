using S = System;
using SDC = System.Diagnostics.CodeAnalysis;
using SCG = System.Collections.Generic;
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
		public BindingDictionary(int capacity) : base(capacity) {}
		public bool Equals(BindingDictionary other) => Utility.Equals(this, other);
		public override bool Equals(object other) => (other as BindingDictionary)?.Equals(this) == true;
	}
}
