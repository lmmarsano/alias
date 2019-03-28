#nullable enable
using STT = System.Threading.Tasks;
using Xunit;
using M = Moq;
using F = Functional;
using AT = Alias.Test;
using AO = Alias.Option;

namespace Alias.Test {
	public class ListTests {
		[Fact]
		public async STT.Task ListTest() {
			var mock = new M.Mock<IOperation>();
			var option = new AO.List();
			mock.Setup(op => op.List(M.It.IsAny<AO.List>())).Returns(Fixture.FakeTasks.ExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)));
			mock.Verify(op => op.List(option));
		}
	}
}