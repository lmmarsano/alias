#nullable enable
using SIO = System.IO;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using Xunit;
using AT = Alias.Test;
using ATF = Alias.Test.Fixture;
using AC = Alias.ConfigurationData;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Test {
	public class ConfigurationTests {
		const string _newline = @"
";
		public static string NormalizeLineEnd(string input)
		=> AT.Utility.NormalizeLineEnd(_newline, input);
		public static async STT.Task<AC.Configuration?> FromString(string input) {
			using var reader = new SIO.StringReader(input);
			return await AC.Configuration.DeserializeAsync(reader);
		}
		public static TheoryData<AC.Configuration?, string> DeserializesData
		= new TheoryData<AC.Configuration?, string>
		  { {null, @"{}"}
		  , {null, @"{ ""binding"" : null }"}
		  , {null, @"{ ""binding"" : {} }"}
		  , {null, @"{ ""binding"" : { ""name"": null } }"}
		  , {null, @"{ ""binding"" : { ""name"": {} } }"}
		  , {null, @"{ ""binding"" : { ""name"": { ""command"": null } } }"}
		  , {null, @"{ ""binding"" : { ""name"": { ""command"": null, ""arguments"": null } } }"}
		  , { new AC.Configuration(new AC.Binding(1) {{"name", new AC.CommandEntry("value", null)}})
		    , @"{ ""binding"" : { ""name"": { ""command"": ""value"" } } }"
		    }
		  , { new AC.Configuration(new AC.Binding(1) {{"name", new AC.CommandEntry("command", "arguments")}})
		    , @"{ ""binding"" : { ""name"": { ""command"": ""command"", ""arguments"": ""arguments"" } } }"
		    }
		  };
		[Theory]
		[MemberData(nameof(DeserializesData))]
		public static async STT.Task Deserializes(AC.Configuration expected, string input)
		=> Assert.Equal(expected, await FromString(input));
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