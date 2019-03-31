#nullable enable
using SIO = System.IO;
using SCG = System.Collections.Generic;
using Xunit;
using AT = Alias.Test;
using ATF = Alias.Test.Fixture;
using AC = Alias.Configuration;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Test {
	public class ConfigurationTests {
		const string _newline = @"
";
		public static string NormalizeLineEnd(string input)
		=> AT.Utility.NormalizeLineEnd(_newline, input);
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
		public static TheoryData<string, AC.Configuration> SerializationData { get; }
		= ATF.Sample.SerializationData;
		[Theory]
		[MemberData(nameof(SerializationData))]
		public void Serializes(string expected, AC.Configuration sut) {
			using var writer = new SIO.StringWriter();
			sut.Serialize(writer);
			Assert.Equal(expected, writer.ToString());
		}
		// FIXME uncomment when SerializeAsync is possible
		/* [Theory]
		[MemberData(nameof(SerializationData), Skip="Not yet possible")]
		public async STT.Task SerializesAsync(string expected, AC.Configuration sut) {
			using var writer = new SIO.StringWriter();
			await sut.SerializeAsync(writer);
			Assert.Equal(expected, writer.ToString());
		} */
	}
}