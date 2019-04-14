using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using Xunit;
using M = Moq;
using ST = LMMarsano.SumType;
using ATF = Alias.Test.Fixture;

namespace Alias.Test {
	public class ExtensionTests {
		public static TheoryData<S.Exception, string> DisplayMessageData {
			get {
				var message = @"Error message.";
				var fatal = new ATF.FakeITerminalException(message);
				var nonfatal = new ATF.FakeIException(message);
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
			await exception.DisplayMessage(ST.Factory.Maybe(env.Mock.Object)).ConfigureAwait(false);
			Assert.Equal(message, env.StreamError.ToString().Trim());
		}
		public static TheoryData<int> TraverseData { get; }
		= new TheoryData<int> { 0, 1, 2 };
		[ Theory
		, MemberData(nameof(TraverseData))
		]
		public void AggregateAcyclicallyAccepts(int n) {
			var sequence = Enumerable.Range(0, n);
			Assert.True
			( sequence.SequenceEqual
			  ( Utility.FromRight
			    ( sequence.AggregateAcyclically
			      ( (m, n) => m == n
			      , Enumerable.Empty<int>()
			      , (sequence, n) => sequence.Append(n)
			      )
			    )
			  )
			);
		}
		[ Theory
		, MemberData(nameof(TraverseData))
		]
		public void AggregateAcyclicallyRejects(int n) {
			var sequence = Enumerable.Range(0, n + 1);
			SCG.IEnumerable<int> cyclic() {
				while (true) {
					foreach (var item in sequence) {
						yield return item;
					}
				}
			}
			Assert.IsType<int>
			( Utility.FromLeft
			  ( cyclic()
			    .AggregateAcyclically
			    ( (m, n) => m == n
			    , Enumerable.Empty<int>()
			    , (sequence, n) => sequence.Append(n)
			    )
			  )
			);
		}
	}
}
