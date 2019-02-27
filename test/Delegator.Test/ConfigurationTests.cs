#nullable enable
using SCG = System.Collections.Generic;
using Xunit;
using DC = Delegator.Configuration;
using NJL = Newtonsoft.Json.Linq;

namespace Delegator.Test {
	public class ConfigurationTests {
		[Theory]
		[InlineData("{}")]
		[InlineData(@"{ ""binding"" : null }")]
		[InlineData(@"{ ""binding"" : {} }")]
		[InlineData(@"{ ""binding"" : { ""name"": null } }")]
		[InlineData(@"{ ""binding"" : { ""name"": {} } }")]
		[InlineData(@"{ ""binding"" : { ""name"": { ""command"": null } } }")]
		public static void DeserializesToNull(string input) {
			Assert.Null(DC.Configuration.FromJsonLinq(NJL.JToken.Parse(input)));
		}
		[Fact]
		public void NonEmptyBindingDeserializes() {
			var target = DC.Configuration.FromJsonLinq(NJL.JToken.Parse(@"{ ""binding"" : { ""name"": { ""command"": ""value"" } } }"));
			Assert.True
			( target is DC.Configuration {Binding: SCG.IDictionary<string, DC.CommandEntry> {Count: 1} binding}
		 && binding.TryGetValue("name", out var actual)
		 && actual is DC.CommandEntry {Command: "value"}
			);
			/* Assert.IsType<DC.Configuration>(target);
			var binding = target!.Binding;
			Assert.IsType<SCG.Dictionary<string, DC.CommandEntry>>(binding);
			var expected = new DC.CommandEntry("value");
			Assert.Equal(1, binding!.Count);
			if (binding!.TryGetValue("name", out var actual)) {
				Assert.Equal(expected, actual);
			} else {
				Assert.True(false, binding.ToString());
			} */
		}
	}
}