using S = System;
using SIO = System.IO;
using M = Moq;

namespace Alias.Test.Fixture {
	class FakeFile {
		public M.Mock<IFileInfo> Mock { get; } = new M.Mock<IFileInfo>();
		public FakeFile(string name, string directoryName) : this(name, directoryName, S.Array.Empty<byte>()) {}
		public FakeFile(string name, string directoryName, byte[] bytes) {
			Mock.Setup(fileInfo => fileInfo.Name).Returns(name);
			Mock.Setup(fileInfo => fileInfo.DirectoryName).Returns(directoryName);
			Mock.Setup(fileInfo => fileInfo.FullName).Returns
			( string.IsNullOrWhiteSpace(directoryName)
			? name
			: SIO.Path.Join(directoryName, name)
			);
			Mock.Setup(fileInfo => fileInfo.Exists).Returns(true);
			Mock.Setup(fileInfo => fileInfo.OpenAsync(M.It.IsAny<SIO.FileMode>(), M.It.IsAny<SIO.FileAccess>(), M.It.IsAny<SIO.FileShare>())).Returns(() => new SIO.MemoryStream(bytes));
		}
	}
}