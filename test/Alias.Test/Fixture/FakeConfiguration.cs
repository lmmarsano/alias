using S = System;
using SIO = System.IO;
using M = Moq;

namespace Alias.Test.Fixture {
	class FakeConfiguration: S.IDisposable {
		readonly SIO.StreamReader StreamReader;
		readonly SIO.MemoryStream MemoryStream;
		public M.Mock<IFileInfo> Mock { get; } = new M.Mock<IFileInfo>();
		public FakeConfiguration(string text) {
			StreamReader = new SIO.StreamReader(MemoryStream = new SIO.MemoryStream(new S.Text.UTF8Encoding().GetBytes(text)));
			Mock.Setup(fileInfo => fileInfo.Exists).Returns(true);
			Mock.Setup(fileInfo => fileInfo.OpenText()).Returns(new SIO.StreamReader(new SIO.MemoryStream(new S.Text.UTF8Encoding().GetBytes(text))));
		}
		private bool allowDisposal = true;
		protected virtual void Dispose(bool disposing) {
			if (allowDisposal) {
				if (disposing) {
					foreach (var item in new S.IDisposable[] {MemoryStream, StreamReader}) {
						item.Dispose();
					}
				}
				allowDisposal = false;
			}
		}
		public void Dispose() => Dispose(true);
	}
}