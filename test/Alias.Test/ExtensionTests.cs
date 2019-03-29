#nullable enable
using S = System;
using STT = System.Threading.Tasks;
using SCG = System.Collections.Generic;
using System.Linq;
using Xunit;
using M = Moq;
using F = Functional;
using ATF = Alias.Test.Fixture;

namespace Alias.Test {
	public class ExtensionTests {
		public static TheoryData<S.Exception, string> DisplayMessageData {
			get {
				var message = @"IException message.";
				var exception = new ATF.FakeException(message);
				return new TheoryData<S.Exception, string>
				{ {new S.Exception(message), string.Empty}
				, {exception, message}
				, {new S.AggregateException(new S.Exception[] {exception}), message}
				};
			}
		}
		[Theory]
		[MemberData(nameof(DisplayMessageData))]
		public async STT.Task DisplayMessageTest(S.Exception exception, string message) {
			var fakeFile = new ATF.FakeFile(string.Empty, string.Empty).Mock.Object;
			using var env = new ATF.FakeEnvironment(fakeFile, Enumerable.Empty<string>(), fakeFile, new M.Mock<IEffect>().Object, string.Empty, string.Empty);
			await exception.DisplayMessage(env, F.Nothing.Value);
			Assert.Equal(message, env.StreamError.ToString());
		}
	}
}