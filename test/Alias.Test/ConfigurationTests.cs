#nullable enable
using S = System;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using Xunit;
using System.Linq;
using AC = Alias.Configuration;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Test {
	public class ConfigurationTests {
		static readonly string _newline = @"
";
		public static string NormalizeLineEnd(string input)
		=> _newline == S.Environment.NewLine
		 ? input
		 : input.Replace(_newline, S.Environment.NewLine);
		public static TheoryData<string> DeserializesToNullData
		= new TheoryData<string>
		  { @"{}"
		  , @"{ ""binding"" : null }"
		  , @"{ ""binding"" : {} }"
		  , @"{ ""binding"" : { ""name"": null } }"
		  , @"{ ""binding"" : { ""name"": {} } }"
		  , @"{ ""binding"" : { ""name"": { ""command"": null } } }"
		  , @"{ ""binding"" : { ""name"": { ""command"": null, ""arguments"": null } } }"
		  };
		[Theory]
		[MemberData(nameof(DeserializesToNullData))]
		public static void DeserializesToNull(string input) {
			Assert.Null(AC.Configuration.Deserialize(new SIO.StringReader(input)));
		}
		[Fact]
		public void NonEmptyBindingDeserializes() {
			var target = AC.Configuration.FromJsonLinq(NJL.JToken.Parse(@"{ ""binding"" : { ""name"": { ""command"": ""value"" } } }"));
			Assert.True
			( target is AC.Configuration { Binding: SCG.IDictionary<string, AC.CommandEntry> { Count: 1 } binding }
			&& binding.TryGetValue("name", out var actual)
			&& actual is AC.CommandEntry { Command: "value", Arguments: null }
			);
		}
		[Fact]
		public void FullBindingDeserializes() {
			var target = AC.Configuration.FromJsonLinq(NJL.JToken.Parse(@"{ ""binding"" : { ""name"": { ""command"": ""command"", ""arguments"": ""arguments"" } } }"));
			Assert.True
			( target is AC.Configuration { Binding: SCG.IDictionary<string, AC.CommandEntry> { Count: 1 } binding }
			&& binding.TryGetValue("name", out var actual)
			&& actual is AC.CommandEntry { Command: "command", Arguments: "arguments" }
			);
		}
		static AC.Configuration ToConfiguration(SCG.IEnumerable<(string, string, string?)> entries)
		=> new AC.Configuration
		   ( entries
		     .Select(tuple => new SCG.KeyValuePair<string, AC.CommandEntry>(tuple.Item1, new AC.CommandEntry(tuple.Item2, tuple.Item3)))
		     .ToDictionary(kv => kv.Key, kv => kv.Value)
		   );
		public static TheoryData<string, AC.Configuration> SerializationData
		=> new TheoryData<string, AC.Configuration>
		   { { NormalizeLineEnd
		       ( @"{
	""binding"": {}
}"
		       )
		     , ToConfiguration(Enumerable.Empty<(string, string, string?)>())
		     }
		   , { NormalizeLineEnd
		       ( @"{
	""binding"": {
		""alias-arguments"": {
			""command"": ""command"",
			""arguments"": ""arguments""
		},
		""alias-no-arguments"": {
			""command"": ""command""
		}
	}
}"
		       )
		     , ToConfiguration
		       ( new []
		         { ( @"alias-arguments", @"command", @"arguments")
		         , ( @"alias-no-arguments", @"command", null)
		         }
		       )
		     }
		   };
		[Theory]
		[MemberData(nameof(SerializationData))]
		public void Serializes(string expected, AC.Configuration sut) {
			var writer = new SIO.StringWriter();
			sut.Serialize(writer);
			Assert.Equal(expected, writer.ToString());
		}
	}
}