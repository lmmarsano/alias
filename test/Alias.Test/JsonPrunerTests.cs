#nullable enable
using Xunit;
using AC = Alias.ConfigurationData;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Test {
	public class JsonPrunerTests {
		public static TheoryData<string, string> PrunedJsonData { get; }
		= new TheoryData<string, string>
		  { { "null", "null" }
		  , { "{}", "{}" }
		  , { "[]", "[]" }
		  , { "[{}]", "[]" }
		  , { @"{""name"": null}", "{}" }
		  , { @"{""name"": []}", "{}" }
		  , { @"{""name"": """"}", @"{""name"": """"}" }
		  , { @"{""name"": 0}", @"{""name"": 0}" }
		  };
		[ Theory
		, MemberData(nameof(PrunedJsonData))
		]
		public void PrunedJsonEquals(string before, string after)
		=> Assert.True
		   ( NJL.JToken.DeepEquals
		     ( AC.JsonPruner.Transform(NJL.JToken.Parse(before))
		     , NJL.JToken.Parse(after)
		     )
		   );
	}
}
