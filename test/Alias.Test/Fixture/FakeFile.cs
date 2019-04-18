using S = System;
using SIO = System.IO;
using SDC = System.Diagnostics.CodeAnalysis;
using M = Moq;

namespace Alias.Test.Fixture {
	class FakeFile: S.IDisposable {
		[SDC.SuppressMessage("Build", "CA2213", Justification = "Disposed by iterator.")]
		readonly SIO.MemoryStream _memoryStream = new SIO.MemoryStream();
		public SIO.MemoryStream MemoryStream {
			get {
				_memoryStream.Position = 0;
				return _memoryStream;
			}
		}
		public SIO.StreamReader StreamReader => new SIO.StreamReader(MemoryStream);
		public M.Mock<IFileInfo> Mock { get; } = new M.Mock<IFileInfo>();
		public FakeFile(string name, string directoryName) {
			Mock.Setup(fileInfo => fileInfo.Name).Returns(name);
			Mock.Setup(fileInfo => fileInfo.DirectoryName).Returns(directoryName);
			Mock.Setup(fileInfo => fileInfo.FullName).Returns
			(string.IsNullOrWhiteSpace(directoryName)
			? name
			: SIO.Path.Join(directoryName, name)
			);
			Mock.Setup(fileInfo => fileInfo.Exists).Returns(true);
			Mock.Setup(fileInfo => fileInfo.OpenAsync(M.It.IsAny<SIO.FileMode>(), M.It.IsAny<SIO.FileAccess>(), M.It.IsAny<SIO.FileShare>())).Returns(() => new FakeStream(MemoryStream));
			Mock.Setup(fileInfo => fileInfo.CreateStream()).Returns(() => new FakeStream(MemoryStream));
		}
		public FakeFile(string name, string directoryName, byte[] bytes): this(name, directoryName) {
			MemoryStream.Write(bytes, 0, bytes.Length);
		}
		private bool allowDisposal = true;
		protected virtual void Dispose(bool disposing) {
			if (allowDisposal) {
				if (disposing) {
					foreach (var item in new S.IDisposable[] { _memoryStream }) {
						item.Dispose();
					}
				}
				allowDisposal = false;
			}
		}
		public void Dispose() => Dispose(true);
	}
}
