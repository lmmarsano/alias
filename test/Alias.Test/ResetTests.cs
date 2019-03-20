#nullable enable
using Xunit;
using M = Moq;
using AO = Alias.Option;

namespace Alias.Test {
	public class ResetTests {
		[Fact]
		public void ResetTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Reset();
			mock.Setup(op => op.Reset(M.It.IsAny<AO.Reset>())).Returns(ExitCode.Success);
			Assert.Equal(ExitCode.Success, option.Operate(mock.Object));
			mock.Verify(op => op.Reset(option));
		}
	}
}