using Xunit;
using S = System;

namespace Functional.Test {
	public class ResultTest {
		static Result<bool> OkBool(bool value)
		=> value;
		static Result<bool> ErrorBool
		=> new S.Exception();
		[Fact]
		public void ToStringTest() {
			Assert.Equal($"Error<{typeof(bool)}>({new System.Exception()})", ErrorBool.ToString());
			Assert.Equal($"Ok<{typeof(bool)}>(True)", OkBool(true).ToString());
		}
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
		public void OkMapTest() {
			Assert.True(OkBool(false).Select(x => true).Reduce(false));
		}
		[Fact]
		public void ErrorMapTest() {
			Assert.True(ErrorBool.Select(x => false).Reduce(true));
		}
		[Fact]
		public void OkSingle() {
			Assert.Single(OkBool(true));
		}
		[Fact]
		public void ErrorEmpty() {
			Assert.Empty(ErrorBool);
		}
	}
}
