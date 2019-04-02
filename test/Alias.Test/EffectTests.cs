using S = System;
using STT = System.Threading.Tasks;
using SIO = System.IO;
using SSP = System.Security.Permissions;
using M = Moq;
using Xunit;
using AT = Alias.Test;
using ATF = Alias.Test.Fixture;
using AC = Alias.ConfigurationData;
using static Functional.Extension;

namespace Alias.Test {
	public class EffectTests {
		readonly IEffect _effect = new Effect();
		[Fact]
		public void EqualsTest() {
			Assert.Equal(ATF.Sample.Configuration.ToMaybe(), ATF.Sample.ToConfiguration(ATF.Sample.ConfigurationParameters).ToMaybe());
		}
		[Theory]
		[ClassData(typeof(ATF.DeserializesData))]
		public async STT.Task TryGetConfigurationSucceeds(AC.Configuration? expected, string input) {
			using var fakeTextFile = new ATF.FakeTextFile("alias.conf", string.Empty, input);
			Assert.Equal(expected.ToMaybe(), await AT.Utility.FromOk(_effect.TryGetConfiguration(fakeTextFile.Mock.Object)));
		}
		[Theory]
		[ClassData(typeof(ATF.InvalidJson))]
		public STT.Task TryGetConfigurationDeserializationError(string input) {
			var fakeTextFile = new ATF.FakeTextFile("alias.conf", string.Empty, input);
			return AT.Utility.FromOk(_effect.TryGetConfiguration(fakeTextFile.Mock.Object))
			.ContinueWith(task => {
				fakeTextFile.Dispose();
				Assert.Equal(STT.TaskStatus.Faulted, task.Status);
				Assert.IsType<DeserialException>(task.Exception.InnerException);
			});
		}
		[Fact]
		public void TryGetConfigurationReadError() {
			using var fakeTextFile = new ATF.FakeTextFile("alias.conf", string.Empty, string.Empty);
			fakeTextFile.Mock.Setup(file => file.OpenAsync(M.It.IsAny<SIO.FileMode>(), M.It.IsAny<SIO.FileAccess>(), M.It.IsAny<SIO.FileShare>()))
			.Throws(new SIO.IOException());
			Assert.IsType<TerminalFileException>(AT.Utility.FromError(_effect.TryGetConfiguration(fakeTextFile.Mock.Object)));
		}
		public static TheoryData<string, AC.Configuration> SerializationData { get; }
		= ATF.Sample.SerializationData;
		[Theory]
		[MemberData(nameof(SerializationData))]
		async STT.Task WriteConfigurationSucceeds(string expected, AC.Configuration conf) {
			using var fakeFile = new ATF.FakeFile(string.Empty, string.Empty);
			await AT.Utility.FromOk(_effect.WriteConfiguration(conf, fakeFile.Mock.Object));
			Assert.Equal(expected, new S.Text.UTF8Encoding().GetString(fakeFile.MemoryStream.ToArray()));
		}
		[Fact]
		void WriteConfigurationWriteError() {
			using var fakeFile = new ATF.FakeFile(string.Empty, string.Empty);
			fakeFile.Mock.Setup(file => file.CreateStream()).Throws(new TerminalFileException(string.Empty, SSP.FileIOPermissionAccess.Write));
			Assert.IsType<TerminalFileException>(AT.Utility.FromError(_effect.WriteConfiguration(ATF.Sample.Configuration, fakeFile.Mock.Object)));
		}
		[Fact]
		void WriteConfigurationSerializationError() {
			using var fakeFile = new ATF.FakeFile(string.Empty, string.Empty);
			var mockConfiguration = new M.Mock<AC.Configuration>(new AC.Binding());
			mockConfiguration.Setup(c => c.Serialize(M.It.IsAny<SIO.TextWriter>())).Throws(new SerializerException(string.Empty));
			Assert.IsType<SerializerException>(AT.Utility.FromError(_effect.WriteConfiguration(mockConfiguration.Object, fakeFile.Mock.Object)));
		}
	}
}