#nullable enable
using AC = Alias.ConfigurationData;
using Xunit;

namespace Alias.Test {
	public class BindingTests {
		static AC.BindingDictionary SampleBinding
		=> new AC.BindingDictionary
		   { {string.Empty, new AC.CommandEntry(string.Empty, null)}
		   };
		[Fact]
		public void BindingEquality() {
			Assert.Equal(SampleBinding, SampleBinding);
		}
		[Fact]
		public void BindingInequality() {
			Assert.NotEqual(SampleBinding, new AC.BindingDictionary());
		}
	}
}
