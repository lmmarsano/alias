using S = System;
using M = Moq;

namespace Alias.Test.Fixture {
	class FakeIException: S.Exception, IException {
		public FakeIException(string message) : base(message) {}
	}
	class FakeITerminalException: TerminalException {
		public FakeITerminalException(string message) : base(message) {}
	}
}
