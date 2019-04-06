using S = System;
using SDC = System.Diagnostics.CodeAnalysis;
using Xunit;
using System.Linq;
using M = Moq;
using ATF = Alias.Test.Fixture;

namespace Alias.Test {
	public class OperationFileSystemTests: S.IDisposable {
		readonly M.Mock<IEffect> _mockEffect;
		[SDC.SuppressMessage("Build", "CA2213", Justification = "Disposed by iterator.")]
		readonly ATF.FakeFile _fakeApp;
		[SDC.SuppressMessage("Build", "CA2213", Justification = "Disposed by iterator.")]
		readonly ATF.FakeTextFile _fakeConf;
		[SDC.SuppressMessage("Build", "CA2213", Justification = "Disposed by iterator.")]
		readonly ATF.FakeEnvironment _fakeEnv;
		readonly M.Mock<IEnvironment> _mockEnv;
		public OperationFileSystemTests() {
			_mockEffect = new M.Mock<IEffect>();
			_fakeApp = new ATF.FakeFile(@"Application Name", @"Application Directory");
			_fakeConf = new ATF.FakeTextFile(string.Empty, string.Empty, string.Empty);
			_fakeEnv = new ATF.FakeEnvironment(_fakeApp.Mock.Object, Enumerable.Empty<string>(), _fakeConf.Mock.Object, _mockEffect.Object, @"Working Directory", string.Empty);
			_mockEnv = _fakeEnv.Mock;
		}
		public void Dispose() {
			Dispose(true);
			S.GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				foreach (var item in new S.IDisposable[] { _fakeApp, _fakeConf, _fakeEnv }) {
					item.Dispose();
				}
			}
		}
		IOperation Operation(IEnvironment environment)
		=> new OperationFileSystem(environment, ATF.Sample.Configuration);
		[Fact]
		public void SetTest() {
			// accepts existing alias file
			// creates missing alias file
			// sets configuration
			var path = S.Reflection.Assembly.GetEntryAssembly().Location;
		}
		// .Setup(effect => effect.CopyFile(M.It.IsAny<SIO.FileInfo>(), M.It.IsAny<Destination>())).Returns(resultNothing);
	}
}
