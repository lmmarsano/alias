#nullable enable
using Xunit;
using AC = Alias.Configuration;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Test {
	public class JsonPrunerTests {
		[Theory]
		[InlineData("null", "null")]
		[InlineData("{}", "{}")]
		[InlineData("[]", "[]")]
		[InlineData("[{}]", "[]")]
		[InlineData(@"{""name"": null}", "{}")]
		[InlineData(@"{""name"": []}", "{}")]
		[InlineData(@"{""name"": """"}", @"{""name"": """"}")]
		[InlineData(@"{""name"": 0}", @"{""name"": 0}")]
		public void PrunedJsonEquals(string before, string after)
		=> Assert.True(
		   	NJL.JToken.DeepEquals
		   	( AC.JsonPruner.Transform(NJL.JToken.Parse(before))
		   	, NJL.JToken.Parse(after)
		   	)
		   );
	}
}