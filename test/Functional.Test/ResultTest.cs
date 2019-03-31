using Xunit;
using S = System;
using STT = System.Threading.Tasks;

namespace Functional.Test {
	public class ResultTest {
		public static TheoryData<string, Result<bool>> ToStringData { get; }
		= new TheoryData<string, Result<bool>>
		  { {$"Error<{typeof(bool)}>({new System.Exception()})", ErrorBool}
		  , {$"Ok<{typeof(bool)}>(True)", OkBool(true)}
		  };
		public static TheoryData<S.Type, Result<bool>> SelectErrorData { get; }
		= new TheoryData<S.Type, Result<bool>>
		  { {typeof(Ok<bool>), OkBool(false)}
			, {typeof(Error<bool>), ErrorBool}
			};
		public static TheoryData<S.Type, Result<bool>, bool> WhereData { get; }
		= new TheoryData<S.Type, Result<bool>, bool>
		  { {typeof(Ok<bool>), OkBool(false), true}
			, {typeof(Error<bool>), OkBool(false), false}
			, {typeof(Error<bool>), ErrorBool, true}
			, {typeof(Error<bool>), ErrorBool, false}
			};
		public static TheoryData<S.Type, Result<bool>> CombineData { get; }
		= new TheoryData<S.Type, Result<bool>>
		  { {typeof(Ok<bool>), Factory.Result(0).Combine(OkBool(true))}
		  , {typeof(Error<bool>), OkBool(true).Combine(ErrorBool)}
		  , {typeof(Error<bool>), ErrorBool.Combine(OkBool(true))}
		  };
		static Result<bool> OkBool(bool value) => value;
		static Result<bool> ErrorBool => new S.Exception();
		static S.Type ErrorType<T>(Result<T> result) where T: object => ((Error<T>)result).Value.GetType();
		[Theory]
		[MemberData(nameof(ToStringData))]
		public void ToStringTest(string expected, Result<bool> sut) => Assert.Equal(expected, sut.ToString());
		[Fact]
		public void EqualsTest() {
			Assert.True(OkBool(true) == OkBool(true));
			Assert.False(ErrorBool == OkBool(true));
		}
		[Fact]
		public void InfersOk() {
			Result<bool> result = true;
			Assert.True
			(  result is Ok<bool> {Value: bool value}
			&& value
			);
		}
		[Fact]
		public void InfersError() {
			Result<bool> result = new S.Exception();
			Assert.IsType<Error<bool>>(result);
		}
		[Fact]
		public void NotInfersOk() {
			Result<bool> result = new S.Exception();
			Assert.IsNotType<Ok<bool>>(result);
		}
		[Fact]
		public void NotInfersError() {
			Result<bool> result = true;
			Assert.IsNotType<Error<bool>>(result);
		}
		[Fact]
		public void OkToTask() => Assert.True(OkBool(true).ToTask.Result);
		[Fact]
		public void ErrorToTask() => Assert.Single<S.Exception>(ErrorBool.ToTask.Exception.InnerExceptions);
		[Fact]
		public void OkReduceTest() {
			Assert.True(OkBool(true).Reduce(false));
		}
		[Fact]
		public void ErrorReduceTest() {
			Assert.True(ErrorBool.Reduce(true));
		}
		[Fact]
		public void OkLazyReduceTest() {
			Assert.True(OkBool(true).Reduce(x => false));
		}
		[Fact]
		public void ErrorLazyReduceTest() {
			Assert.True(ErrorBool.Reduce(x => true));
		}
		[Fact]
		public void OkSelectTest() {
			Assert.True(OkBool(false).Select(x => true).Reduce(false));
		}
		[Fact]
		public void ErrorSelectTest() {
			Assert.True(ErrorBool.Select(x => false).Reduce(true));
		}
		[Theory]
		[MemberData(nameof(SelectErrorData))]
		public void SelectErrorTest(S.Type type, Result<bool> value)
		=> Assert.IsType(type, value.SelectError(x => x));
		[Fact]
		public void SelectErrorMap()
		=> Assert.IsType<S.ArgumentException>(((Error<bool>)ErrorBool.SelectError(x => new S.ArgumentException())).Value);
		[Fact]
		public void OkSelectManyTest() {
			Assert.True(OkBool(false).SelectMany(x => OkBool(true)).Reduce(false));
			Assert.True(OkBool(false).SelectMany(x => ErrorBool).Reduce(true));
		}
		[Fact]
		public void ErrorSelectManyTest() {
			Assert.True(ErrorBool.SelectMany(x => OkBool(false)).Reduce(true));
		}
		[Theory]
		[MemberData(nameof(WhereData))]
		public void WhereTest(S.Type type, Result<bool> value, bool filter)
		=> Assert.IsType(type, value.Where(x => filter, x => new S.Exception()));
		[Theory]
		[MemberData(nameof(CombineData))]
		public void CombineTest(S.Type expected, Result<bool> sut)
		=> Assert.IsType(expected, sut);
		[Fact]
		public void OkCatchTest() {
			Assert.True(OkBool(true).Catch(x => OkBool(false)).Reduce(false));
		}
		[Fact]
		public void ErrorCatchTest() {
			Assert.True(ErrorBool.Catch(x => OkBool(true)).Reduce(false));
		}
		[Fact]
		public void OkOfType() {
			var okTrue = OkBool(true);
			Assert.Equal(okTrue, okTrue.OfType<bool>(x => new S.Exception()));
			Assert.IsType<Error<int>>(okTrue.OfType<int>(x => new S.Exception()));
		}
		[Fact]
		public void ErrorOfType() {
			var error = new S.NotImplementedException();
			var original = ErrorType(ErrorBool);
			Assert.Same(original, ErrorType(ErrorBool.OfType<bool>(x => error)));
			var conversion = ErrorBool.OfType<int>(x => error);
			Assert.IsType<Error<int>>(conversion);
			Assert.Same(original, ErrorType(conversion));
		}
		[Fact]
		public void OkSingle() {
			Assert.Single(OkBool(true));
		}
		[Fact]
		public void ErrorEmpty() {
			Assert.Empty(ErrorBool);
		}
		public static TheoryData<Result<bool>, STT.Task<bool>, bool> TraverseAsyncData {
			get {
				var okFalse = OkBool(false);
				var taskTrue = ExtensionTest.SuccessfulBoolAsync(true);
				return new TheoryData<Result<bool>, STT.Task<bool>, bool>
				       { {okFalse, taskTrue, false}
				       , {okFalse, ExtensionTest.CancelledBoolAsync, true}
				       , {okFalse, ExtensionTest.FaultedBoolAsync, true}
				       , {ErrorBool, taskTrue, true}
				       , {ErrorBool, ExtensionTest.CancelledBoolAsync, true}
				       , {ErrorBool, ExtensionTest.FaultedBoolAsync, true}
				       };
			}
		}
		[Theory]
		[MemberData(nameof(TraverseAsyncData))]
		public STT.Task TraverseAsyncTest(Result<bool> result, STT.Task<bool> task, bool alternative)
		=> result
		   .TraverseAsync(value => task)
		   .ContinueWith(task => Assert.True(task.Result.Reduce(alternative)));
	}
}