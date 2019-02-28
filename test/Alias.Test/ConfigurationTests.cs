#nullable enable
using SIO = System.IO;
using SCG = System.Collections.Generic;
using Xunit;
using AC = Alias.Configuration;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Test {
	public class ConfigurationTests {
		[Theory]
		[InlineData("{}")]
		[InlineData(@"{ ""binding"" : null }")]
		[InlineData(@"{ ""binding"" : {} }")]
		[InlineData(@"{ ""binding"" : { ""name"": null } }")]
		[InlineData(@"{ ""binding"" : { ""name"": {} } }")]
		[InlineData(@"{ ""binding"" : { ""name"": { ""command"": null } } }")]
		public static void DeserializesToNull(string input) {
			Assert.Null(AC.Configuration.Deserialize(new SIO.StringReader(input)));
		}
		[Fact]
		public void NonEmptyBindingDeserializes() {
			var target = AC.Configuration.FromJsonLinq(NJL.JToken.Parse(@"{ ""binding"" : { ""name"": { ""command"": ""value"" } } }"));
			Assert.True
			( target is AC.Configuration {Binding: SCG.IDictionary<string, AC.CommandEntry> {Count: 1} binding}
		 && binding.TryGetValue("name", out var actual)
		 && actual is AC.CommandEntry {Command: "value"}
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