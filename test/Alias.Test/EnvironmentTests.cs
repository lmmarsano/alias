#nullable enable
using System.Linq;
using Xunit;

namespace Alias.Test {
	public class EnvironmentTests {
		readonly IEnvironment _fixture;
		public EnvironmentTests() {
			_fixture = new Environment(Enumerable.Empty<string>());
		}
		[Fact]
		public void ApplicationTest() {
			Assert.Empty(_fixture.Arguments);
			/* Assert.Equal(@"dotnet.exe", _fixture.ApplicationName);
			Assert.Equal(@"", _fixture.ApplicationDirectory); */
		}
	}
}
