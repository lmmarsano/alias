#nullable enable
using Xunit;
using M = Moq;
using AO = Alias.Option;

namespace Alias.Test {
	public class ListTests {
		[Fact]
		public void ListTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.List();
			mock.Setup(op => op.List(M.It.IsAny<AO.List>())).Returns(ExitCode.Success);
			Assert.Equal(ExitCode.Success, option.Operate(mock.Object));
			mock.Verify(op => op.List(option));
		}
	}
}