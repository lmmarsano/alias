#nullable enable
using Xunit;
using M = Moq;
using AO = Alias.Option;

namespace Alias.Test {
	public class RestoreTests {
		[Fact]
		public void RestoreTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Restore();
			mock.Setup(op => op.Restore(M.It.IsAny<AO.Restore>())).Returns(ExitCode.Success);
			Assert.Equal(ExitCode.Success, option.Operate(mock.Object));
			mock.Verify(op => op.Restore(option));
		}
	}
}