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
		[InlineData(@"{""name"": 0}", @"{""name"": 0}")]
		[InlineData("[{}]", "[]")]
		[InlineData(@"{""name"": null}", "{}")]
		[InlineData(@"{""name"": []}", "{}")]
		public void PrunedJsonEquals(string before, string after)
		=> Assert.True(
		   	NJL.JToken.DeepEquals
		   	( AC.JsonPruner.Transform(NJL.JToken.Parse(before))
		   	, NJL.JToken.Parse(after)
		   	)
		   );
	}
}