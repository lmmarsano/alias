using S = System;
using STT = System.Threading.Tasks;
using System.Linq;
using Xunit;
using M = Moq;
using F = Functional;
using ATF = Alias.Test.Fixture;

namespace Alias.Test {
	public class ExtensionTests {
		public static TheoryData<S.Exception, string> DisplayMessageData {
			get {
				var message = @"Error message.";
				var fatal = new ATF.FakeException<ATF.FakeITerminalException>(message).Mock.Object;
				var nonfatal = new ATF.FakeException<ATF.FakeIException>(message).Mock.Object;
				return new TheoryData<S.Exception, string>
				{ {new S.Exception(message), string.Empty}
				, {fatal, message}
				, {nonfatal, string.Empty}
				, {new S.AggregateException(new S.Exception[] {fatal}), message}
				};
			}
		}
		[ Theory
		, MemberData(nameof(DisplayMessageData))
		]
		public async STT.Task DisplayMessageTest(S.Exception exception, string message) {
			using var fakeFileDisposable = new ATF.FakeFile(string.Empty, string.Empty);
			var fakeFile = fakeFileDisposable.Mock.Object;
			using var env = new ATF.FakeEnvironment(fakeFile, Enumerable.Empty<string>(), fakeFile, new M.Mock<IEffect>().Object, string.Empty, string.Empty);
			await exception.DisplayMessage(F.Factory.Maybe(env.Mock.Object), F.Nothing.Value).ConfigureAwait(false);
			Assert.Equal(message, env.StreamError.ToString().Trim());
		}
	}
}
