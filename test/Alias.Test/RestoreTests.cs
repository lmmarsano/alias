#nullable enable
using STT = System.Threading.Tasks;
using Xunit;
using M = Moq;
using AO = Alias.Option;
using AT = Alias.Test;

namespace Alias.Test {
	public class RestoreTests {
		[Fact]
		public async STT.Task RestoreTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Restore();
			mock.Setup(op => op.Restore(M.It.IsAny<AO.Restore>())).Returns(AT.Fixture.FakeTasks.ExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)));
			mock.Verify(op => op.Restore(option));
		}
	}
}