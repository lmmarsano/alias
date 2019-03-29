using S = System;

namespace Alias.Test.Fixture {
	public class FakeException: S.Exception, ITerminalException {
		public FakeException() {}
		public FakeException(string message): base(message) {}
		public FakeException(string message, S.Exception inner): base(message, inner) {}
	}
}