using Xunit;
using S = System;
using SCG = System.Collections.Generic;
using ST = System.Threading;
using STT = System.Threading.Tasks;
using System.Linq;

namespace Functional.Test {
	public class ExtensionTest {
		public static STT.Task<bool> SuccessfulBoolAsync(bool value)
		=> STT.Task.FromResult(value);
		public static STT.Task<bool> FaultedBoolAsync
		=> STT.Task.FromException<bool>(new S.AggregateException());
		public static STT.Task<bool> CancelledBoolAsync
		=> STT.Task.FromCanceled<bool>(new ST.CancellationToken(true));
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
			var array = new[] { false, true };
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
		public static TheoryData<STT.Task<bool>> ErrorAsyncData
		=> new TheoryData<STT.Task<bool>>
			 { FaultedBoolAsync
			 , CancelledBoolAsync
			 };
		[Theory]
		[MemberData(nameof(ErrorAsyncData))]
		public async STT.Task ToErrorAsync(STT.Task<bool> sut) {
			Assert.IsType<Error<bool>>(await sut.ToResultAsync().ConfigureAwait(false));
			Assert.IsType<Error<bool>>(await sut.ToResultAsync(task => new S.Exception()).ConfigureAwait(false));
		}
		[Fact]
		public async STT.Task ToOkAsync() {
			var sut = SuccessfulBoolAsync(false);
			Assert.IsType<Ok<bool>>(await sut.ToResultAsync().ConfigureAwait(false));
			Assert.IsType<Ok<bool>>(await sut.ToResultAsync(task => new S.Exception()).ConfigureAwait(false));
		}
		[Fact]
		public void NullToLeft() {
			Assert.IsType<Left<S.Exception, Nothing>>(((Nothing?)null).ToEither(() => new S.Exception()));
		}
		[Fact]
		public void ValueToRight() {
			Assert.IsType<Right<S.Exception, Nothing>>(Nothing.Value.ToEither(() => new S.Exception()));
		}
		[Fact]
		public async STT.Task SuccessfulSelectManyAsyncTest()
		=> Assert.True(await SuccessfulBoolAsync(false).SelectManyAsync(x => SuccessfulBoolAsync(true)).ConfigureAwait(false));
		public static TheoryData<STT.Task<bool>, STT.Task<bool>, STT.TaskStatus> ErrorSelectManyAsyncData {
			get {
				var falseAsync = SuccessfulBoolAsync(false);
				return new TheoryData<STT.Task<bool>, STT.Task<bool>, STT.TaskStatus>
				{ {falseAsync, CancelledBoolAsync, STT.TaskStatus.Canceled}
				, {falseAsync, FaultedBoolAsync, STT.TaskStatus.Faulted}
				, {FaultedBoolAsync, falseAsync, STT.TaskStatus.Faulted}
				, {CancelledBoolAsync, falseAsync, STT.TaskStatus.Canceled}
				};
			}
		}
		[Theory]
		[MemberData(nameof(ErrorSelectManyAsyncData))]
		public STT.Task ErrorSelectManyAsyncTest(STT.Task<bool> sut, STT.Task<bool> image, STT.TaskStatus expected)
		=> sut.SelectManyAsync(x => image).ContinueWith(task => Assert.Equal(expected, task.Status));
		[Fact]
		public async STT.Task SuccessfulSelectAsyncTest()
		=> Assert.True(await SuccessfulBoolAsync(false).SelectAsync(x => true).ConfigureAwait(false));
		public static TheoryData<STT.Task<bool>, STT.TaskStatus> ErrorSelectAsyncData
		=> new TheoryData<STT.Task<bool>, STT.TaskStatus>
		   { {FaultedBoolAsync, STT.TaskStatus.Faulted}
		   , {CancelledBoolAsync, STT.TaskStatus.Canceled}
		   };
		[Theory]
		[MemberData(nameof(ErrorSelectAsyncData))]
		public STT.Task ErrorSelectAsyncTest(STT.Task<bool> sut, STT.TaskStatus expected)
		=> sut.SelectAsync(x => false).ContinueWith(task => {
			Assert.Equal(expected, task.Status);
		});
		public static TheoryData<STT.Task<bool>> SuccessfulCatchAsyncData { get; }
		= new TheoryData<STT.Task<bool>>
		  { SuccessfulBoolAsync(false)
		  , CancelledBoolAsync
		  , FaultedBoolAsync
		  };
		[Theory]
		[MemberData(nameof(SuccessfulCatchAsyncData))]
		public async STT.Task SuccessfulCatchAsyncTest(STT.Task<bool> catchValue) {
			Assert.True(await SuccessfulBoolAsync(true).CatchAsync(x => catchValue).ConfigureAwait(false));
		}
		[Theory]
		[MemberData(nameof(ErrorAsyncData))]
		public STT.Task ErrorCatchAsyncTest(STT.Task<bool> sut)
		=> sut.CatchAsync(x => SuccessfulBoolAsync(true)).ContinueWith(task => {
			Assert.True(task.Result);
		});
		[Fact]
		public STT.Task SuccessfulSelectErrorAsyncTest()
		=> SuccessfulBoolAsync(true)
		   .SelectErrorAsync(error => new S.InvalidOperationException(string.Empty, error))
		   .ContinueWith(task => Assert.True(task.Result));
		[Theory]
		[MemberData(nameof(ErrorAsyncData))]
		public STT.Task ErrorSelectErrorAsyncTest(STT.Task<bool> sut)
		=> sut
		   .SelectErrorAsync(error => new S.InvalidOperationException(string.Empty, error))
		   .ContinueWith(task => {
		   	var error = task.Exception.InnerExceptions.First();
		   	Assert.IsType<S.InvalidOperationException>(error);
		   	Assert.IsType<S.AggregateException>(error.InnerException);
		   });
		public static TheoryData<bool, STT.TaskStatus> SuccessfulWhereAsyncData
		=> new TheoryData<bool, STT.TaskStatus>
		   { {true, STT.TaskStatus.RanToCompletion}
		   , {false, STT.TaskStatus.Faulted}
		   };
		[Theory]
		[MemberData(nameof(SuccessfulWhereAsyncData))]
		public STT.Task SuccessfulWhereAsyncTest(bool predicate, STT.TaskStatus expected)
		=> SuccessfulBoolAsync(false)
		   .WhereAsync(x => predicate, value => new S.AggregateException())
		   .ContinueWith(task => Assert.Equal(expected, task.Status));
		public static TheoryData<STT.Task<bool>, STT.TaskStatus> ErrorWhereAsyncData
		=> new TheoryData<STT.Task<bool>, STT.TaskStatus>
		   { {FaultedBoolAsync, STT.TaskStatus.Faulted}
		   , {CancelledBoolAsync, STT.TaskStatus.Canceled}
		   };
		[Theory]
		[MemberData(nameof(ErrorWhereAsyncData))]
		public STT.Task ErrorWhereAsyncTest(STT.Task<bool> sut, STT.TaskStatus expected)
		=> sut.WhereAsync(x => true, value => new S.AggregateException()).ContinueWith(task => {
			Assert.Equal(expected, task.Status);
		});
		public static TheoryData<STT.Task<Nothing>, STT.Task<bool>, STT.TaskStatus> CombineAsyncData {
			get {
				var nothingAsync = STT.Task.FromResult(Nothing.Value);
				var cancelledNothingAsync = STT.Task.FromCanceled<Nothing>(new ST.CancellationToken(true));
				var faultedNothingAsync = STT.Task.FromException<Nothing>(new S.AggregateException());
				var falseAsync = SuccessfulBoolAsync(false);
				return new TheoryData<STT.Task<Nothing>, STT.Task<bool>, STT.TaskStatus>
				       { {nothingAsync, falseAsync, STT.TaskStatus.RanToCompletion}
				       , {nothingAsync, CancelledBoolAsync, STT.TaskStatus.Canceled}
				       , {nothingAsync, FaultedBoolAsync, STT.TaskStatus.Faulted}
				       , {cancelledNothingAsync, falseAsync, STT.TaskStatus.Canceled}
				       , {cancelledNothingAsync, CancelledBoolAsync, STT.TaskStatus.Canceled}
				       , {cancelledNothingAsync, FaultedBoolAsync, STT.TaskStatus.Canceled}
				       , {faultedNothingAsync, falseAsync, STT.TaskStatus.Faulted}
				       , {faultedNothingAsync, CancelledBoolAsync, STT.TaskStatus.Faulted}
				       , {faultedNothingAsync, FaultedBoolAsync, STT.TaskStatus.Faulted}
				       };
			}
		}
		[Theory]
		[MemberData(nameof(CombineAsyncData))]
		public STT.Task CombineAsyncTest(STT.Task<Nothing> sut, STT.Task<bool> next, STT.TaskStatus expected)
		=> sut.CombineAsync(next).ContinueWith(task => Assert.Equal(expected, task.Status));
	}
}
