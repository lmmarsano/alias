using Xunit;
using S = System;
using SCG = System.Collections.Generic;
using System.Linq;

namespace Functional.Test {
	public class ExtensionTest {
		[Fact]
		public void NothingWhenFalse() {
			Assert.IsType<Nothing<bool>>(true.When(false));
		}
		[Fact]
		public void JustWhenTrue() {
			Assert.IsType<Just<bool>>(true.When(true));
		}
		[Fact]
		public void NothingWhenPredicateFalse() {
			Assert.IsType<Nothing<bool>>(true.When(_ => false));
		}
		[Fact]
		public void JustWhenPredicateTrue() {
			Assert.IsType<Just<bool>>(true.When(_ => true));
		}
		[Fact]
		public void NullToNothing() {
			Assert.IsType<Nothing<Nothing>>(((Nothing?)null).ToMaybe());
		}
		[Fact]
		public void ValueToJust() {
			Assert.IsType<Just<Nothing>>(Nothing.Value.ToMaybe());
		}
		[Fact]
		public void FirstOrNoneTest() {
			Assert.Equal((Maybe<bool>)true, Enumerable.Repeat(true, 1).FirstOrNone());
			Assert.Equal(Nothing<bool>.Value, Enumerable.Empty<bool>().FirstOrNone());
		}
		[Fact]
		public void FirstOrNonePredicateTest() {
			var array = new [] { false, true };
			Assert.Equal((Maybe<bool>)true, array.FirstOrNone(x => x));
			Assert.Equal(Nothing<bool>.Value, array.FirstOrNone(x => false));
		}
		[Fact]
		public void SelectMaybeTest() {
			var array = new[] { false, true };
			Assert.Single(array.SelectMaybe(x => x.When(x)), x => x);
			Assert.Empty(array.SelectMaybe(x => Nothing<bool>.Value));
		}
		[Fact]
		public void DictionaryTest() {
			var dictionary = new SCG.Dictionary<bool, bool> { { true, true } };
			Assert.Equal((Maybe<bool>)true, dictionary.TryGetValue(true));
			Assert.Equal(Nothing<bool>.Value, dictionary.TryGetValue(false));
		}
		[Fact]
		public void UsingGetsDisposable() {
			var disposable = new FakeDisposable();
			Assert.Same(disposable, disposable.Using(x => x));
		}
		[Fact]
		public void UsingAppliesMap() {
			Assert.True(new FakeDisposable().Using(x => true));
		}
		[Fact]
		public void UsingDisposes() {
			var disposalState = new DisposalState();
			new FakeDisposable(disposalState).Using(x => x);
			Assert.True(disposalState.IsDisposed);
		}
		[Fact]
		public void TryOkTest() {
			Assert.Equal((Result<bool>)true, ((S.Func<bool>)(() => true)).Try());
			Assert.Equal((Result<Nothing>)Nothing.Value, ((S.Action)(() => { return; })).Try());
		}
		[Fact]
		public void TryErrorTest() {
			var exception = new S.Exception();
			Assert.Equal((Result<bool>)exception, ((S.Func<bool>)(() => throw exception)).Try());
			Assert.Equal((Result<Nothing>)exception, ((S.Action)(() => throw exception)).Try());
		}
		[Fact]
		public void NullToError() {
			Assert.IsType<Error<Nothing>>(((Nothing?)null).ToResult(() => new S.Exception()));
		}
		[Fact]
		public void ValueToOk() {
			Assert.IsType<Ok<Nothing>>(Nothing.Value.ToResult(() => new S.Exception()));
		}
		[Fact]
		public void NullToLeft() {
			Assert.IsType<Left<S.Exception, Nothing>>(((Nothing?)null).ToEither(() => new S.Exception()));
		}
		[Fact]
		public void ValueToRight() {
			Assert.IsType<Right<S.Exception, Nothing>>(Nothing.Value.ToEither(() => new S.Exception()));
		}
	}
}
