using SCG = System.Collections.Generic;
using System.Linq;
using AC = Alias.ConfigurationData;
using ATF = Alias.Test.Fixture;
using Xunit;

namespace Alias.ConfigurationData.Test {
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
		public static TheoryData<string, SCG.IEnumerable<AC.CommandEntry>> GetResolutionSequenceData{
			get {
				var empty = Enumerable.Empty<AC.CommandEntry>();
				var binding = ATF.Sample.Configuration.Binding;
				return new TheoryData<string, SCG.IEnumerable<AC.CommandEntry>>
				{ {@"absent", empty}
				, {@"alias1", empty.Append(binding[@"alias1"])}
				, {@"chained", new [] {@"chained", @"alias1"}.Select(item => binding[item])}
				};
			}
		}
		[ Theory
		, MemberData(nameof(GetResolutionSequenceData))
		]
		public void GetResolutionSequenceTest(string alias, SCG.IEnumerable<AC.CommandEntry> expected) {
			Assert.True(expected.SequenceEqual(ATF.Sample.Configuration.Binding.GetResolutionSequence(alias)));
		}
	}
}
