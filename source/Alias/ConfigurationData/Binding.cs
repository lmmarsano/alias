using S = System;
using SCG = System.Collections.Generic;
using System.Linq;
using Name = System.String;

namespace Alias.ConfigurationData {
	public class Binding: SCG.Dictionary<Name, CommandEntry>, S.IEquatable<Binding> {
		public Binding() {}
		public Binding(int capacity) : base(capacity) {}
		public bool Equals(Binding other) => Utility.Equals(this, other);
	}
}