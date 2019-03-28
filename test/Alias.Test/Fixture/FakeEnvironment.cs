using SCG = System.Collections.Generic;
using SIO = System.IO;
using S = System;
using Path = System.String;
using Name = System.String;

namespace Alias.Test.Fixture {
	using Arguments = SCG.IEnumerable<string>;
	class FakeEnvironment : IEnvironment, S.IDisposable {
		public IFileInfo ApplicationFile { get; }
		public string ApplicationDirectory { get; }
		public string ApplicationName { get; }
		public SCG.IEnumerable<string> Arguments { get; }
		public string ConfigurationFilePath { get; }
		public IFileInfo ConfigurationFile { get; }
		public string WorkingDirectory { get; }
		public SIO.TextReader StreamIn { get; }
		public SIO.TextWriter StreamOut { get; } = new SIO.StringWriter();
		public SIO.TextWriter StreamError { get; } = new SIO.StringWriter();
		public IEffect Effect { get; }
		bool allowDisposal = true;
		public FakeEnvironment(IFileInfo applicationFile, Arguments arguments, IFileInfo configurationFile, IEffect effect, Path workingDirectory, string input) {
			Arguments = arguments;
			ApplicationFile = applicationFile;
			ApplicationName = applicationFile.Name;
			ConfigurationFile = configurationFile;
			Effect = effect;
			ApplicationDirectory = applicationFile.DirectoryName;
			WorkingDirectory = workingDirectory;
			ConfigurationFilePath = configurationFile.FullName;
			StreamIn = new SIO.StringReader(input);
		}
		protected virtual void Dispose(bool disposing) {
			if (allowDisposal) {
				if (disposing) {
					foreach (var item in new S.IDisposable[] {StreamIn, StreamOut, StreamError}) {
						item.Dispose();
					}
				}
				allowDisposal = false;
			}
		}
		public void Dispose() => Dispose(true);
	}
}