using S = System;
using STT = System.Threading.Tasks;
using Xunit;
using AT = Alias.Test;
using ATF = Alias.Test.Fixture;
using AC = Alias.ConfigurationData;

namespace Alias.Test {
	public class EffectTests {
		readonly IEffect _effect = new Effect();
		public static TheoryData<string, AC.Configuration> SerializationData { get; }
		= ATF.Sample.SerializationData;
		[Theory]
		[MemberData(nameof(SerializationData))]
		async STT.Task WriteConfigurationTest(string expected, AC.Configuration conf) {
			using var fakeFile = new ATF.FakeFile(string.Empty, string.Empty);
			await AT.Utility.FromOk(_effect.WriteConfiguration(conf, fakeFile.Mock.Object));
			Assert.Equal(expected, new S.Text.UTF8Encoding().GetString(fakeFile.MemoryStream.ToArray()));
		}
	}
}