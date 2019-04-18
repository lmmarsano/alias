using S = System;

namespace Alias.Test.Fixture {
	class FakeTextFile: FakeFile {
		public FakeTextFile(string name, string directoryName, string text): base(name, directoryName) {
			var bytes = new S.Text.UTF8Encoding().GetBytes(text);
			MemoryStream.Write(bytes, 0, bytes.Length);
		}
	}
}
