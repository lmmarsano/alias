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
	[SDC.SuppressMessage("Compile", "CS0659", Justification = "Need equality.")]
	public class BindingDictionary: SCG.Dictionary<Name, CommandEntry>, S.IEquatable<BindingDictionary> {
		public BindingDictionary() {}
		public BindingDictionary(int capacity) : base(capacity) {}
		public bool Equals(BindingDictionary other) => Utility.Equals(this, other);
		public override bool Equals(object other) => (other as BindingDictionary)?.Equals(this) == true;
	}
}
