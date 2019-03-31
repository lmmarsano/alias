using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using M = Moq;
using Path = System.String;

namespace Alias.Test.Fixture {
	using Arguments = SCG.IEnumerable<string>;
	class FakeEnvironment : S.IDisposable {
		bool _allowDisposal = true;
		public M.Mock<IEnvironment> Mock = new M.Mock<IEnvironment>();
		public SIO.TextReader StreamIn { get; }
		public SIO.TextWriter StreamOut { get; } = new SIO.StringWriter();
		public SIO.TextWriter StreamError { get; } = new SIO.StringWriter();
		public FakeEnvironment(string input) {
			StreamIn = new SIO.StringReader(input);
			Mock.Setup(env => env.StreamIn).Returns(StreamIn);
			Mock.Setup(env => env.StreamOut).Returns(StreamOut);
			Mock.Setup(env => env.StreamError).Returns(StreamError);
		}
		public FakeEnvironment(IFileInfo applicationFile, Arguments arguments, IFileInfo configurationFile, IEffect effect, Path workingDirectory, string input): this(input) {
			Mock.Setup(env => env.Arguments).Returns(arguments);
			Mock.Setup(env => env.ApplicationFile).Returns(applicationFile);
			Mock.Setup(env => env.ApplicationName).Returns(applicationFile.Name);
			Mock.Setup(env => env.ConfigurationFile).Returns(configurationFile);
			Mock.Setup(env => env.Effect).Returns(effect);
			Mock.Setup(env => env.ApplicationDirectory).Returns(applicationFile.DirectoryName);
			Mock.Setup(env => env.WorkingDirectory).Returns(workingDirectory);
			Mock.Setup(env => env.ConfigurationFilePath).Returns(configurationFile.FullName);
		}
		protected virtual void Dispose(bool disposing) {
			if (_allowDisposal) {
				if (disposing) {
					foreach (var item in new S.IDisposable[] {StreamIn, StreamOut, StreamError}) {
						item.Dispose();
					}
				}
				_allowDisposal = false;
			}
		}
		public void Dispose() => Dispose(true);
	}
}