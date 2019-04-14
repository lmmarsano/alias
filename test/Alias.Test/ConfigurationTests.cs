#nullable enable
using SIO = System.IO;
using STT = System.Threading.Tasks;
using Xunit;
using ATF = Alias.Test.Fixture;
using AT = Alias.Test;
using AC = Alias.ConfigurationData;
using NJ = Newtonsoft.Json;

namespace Alias.ConfigurationData.Test {
	public class ConfigurationTests {
		const string _newline = @"
";
		public static string NormalizeLineEnd(string input)
		=> AT.Utility.NormalizeLineEnd(_newline, input);
		public static async STT.Task<AC.Configuration?> FromString(string input) {
			using var reader = new SIO.StringReader(input);
			return await AC.Configuration.DeserializeAsync(reader).ConfigureAwait(false);
		}
		[Fact]
		public static void EqualsTest() {
			Assert.Equal(ATF.Sample.Configuration, ATF.Sample.ToConfiguration(ATF.Sample.ConfigurationParameters));
		}
		[ Theory
		, ClassData(typeof(ATF.DeserializesData))
		]
		public async STT.Task Deserializes(AC.Configuration expected, string input)
		=> Assert.Equal(expected, await FromString(input).ConfigureAwait(false));
		[ Theory
		, ClassData(typeof(ATF.InvalidJson))
		]
		public STT.Task DeserializeFail(string input)
		=> FromString(input).ContinueWith(task => {
			Assert.Equal(STT.TaskStatus.Faulted, task.Status);
			Assert.IsType<NJ.JsonReaderException>(task.Exception.InnerException);
		});
		public static TheoryData<string, AC.Configuration> SerializationData { get; }
		= ATF.Sample.SerializationData;
		[ Theory
		, MemberData(nameof(SerializationData))
		]
		public void Serializes(string expected, AC.Configuration sut) {
			using var writer = new SIO.StringWriter();
			sut.Serialize(writer);
			Assert.Equal(expected, writer.ToString());
		}
		// FIXME uncomment when SerializeAsync is possible
		/* [ Theory
		, MemberData(nameof(SerializationData), Skip="Not yet possible")
		]
		public async STT.Task SerializesAsync(string expected, AC.Configuration sut) {
			using var writer = new SIO.StringWriter();
			await sut.SerializeAsync(writer);
			Assert.Equal(expected, writer.ToString());
		} */
	}
}
