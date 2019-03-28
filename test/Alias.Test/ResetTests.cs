#nullable enable
using STT = System.Threading.Tasks;
using Xunit;
using M = Moq;
using AO = Alias.Option;
using AT = Alias.Test;

namespace Alias.Test {
	public class ResetTests {
		[Fact]
		public async STT.Task ResetTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Reset();
			mock.Setup(op => op.Reset(M.It.IsAny<AO.Reset>())).Returns(Fixture.FakeTasks.ExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)));
			mock.Verify(op => op.Reset(option));
		}
	}
}