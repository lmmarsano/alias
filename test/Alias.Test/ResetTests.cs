#nullable enable
using STT = System.Threading.Tasks;
using Xunit;
using M = Moq;
using A = Alias;
using AO = Alias.Option;

namespace Alias.Test {
	public class ResetTests {
		[Fact]
		public async STT.Task ResetTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Reset();
			mock.Setup(op => op.Reset(M.It.IsAny<AO.Reset>())).Returns(A.Utility.TaskExitSuccess);
			Assert.Equal(ExitCode.Success, await Utility.FromOk(option.Operate(mock.Object)).ConfigureAwait(false));
			mock.Verify(op => op.Reset(option));
		}
	}
}
