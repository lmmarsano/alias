using S = System;
using M = Moq;

namespace Alias.Test.Fixture {
	class FakeException<T> where T: S.Exception {
		public M.Mock<T> Mock { get; }
		public FakeException(string message) {
			Mock = new M.Mock<T>(message);
			Mock.Setup(error => error.Message).Returns(message);
		}
	}
	public class FakeIException: S.Exception, IException {
		public FakeIException(string message) : base(message) {}
	}
	public class FakeITerminalException: TerminalException {
		public FakeITerminalException(string message) : base(message) {}
	}
}
