#nullable enable
using SCG = System.Collections.Generic;
using AC = Alias.ConfigurationData;
using Xunit;

namespace Alias.Test {
	public class BindingTests {
		AC.Binding SampleBinding
		=> new AC.Binding
		   { {string.Empty, new AC.CommandEntry(string.Empty, null)}
		   };
		[Fact]
		public void BindingEquality() {
			Assert.Equal(SampleBinding, SampleBinding);
		}
		[Fact]
		public void BindingInequality() {
			Assert.NotEqual(SampleBinding, new AC.Binding());
		}
	}
}