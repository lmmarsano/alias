using S = System;
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
#pragma warning disable CS0659 // overrides Object.Equals(object o) but does not override Object.GetHashCode()
	public class Binding: SCG.Dictionary<Name, CommandEntry>, S.IEquatable<Binding> {
#pragma warning restore CS0659
		public Binding() {}
		public Binding(int capacity) : base(capacity) {}
		public bool Equals(Binding other) => Utility.Equals(this, other);
		public override bool Equals(object other) => (other as Binding)?.Equals(this) == true;
	}
}