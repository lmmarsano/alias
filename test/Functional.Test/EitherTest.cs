using Xunit;

namespace Functional.Test {
	public class EitherTest {
		static Either<bool, int> LeftBoolInt(bool value)
		=> value;
		static Either<bool, int> RightBoolInt(int value)
		=> value;
		[Fact]
		public void ToStringTest() {
			Assert.Equal($"Left<{typeof(bool)}, {typeof(int)}>({true})", LeftBoolInt(true).ToString());
			Assert.Equal($"Right<{typeof(bool)}, {typeof(int)}>({0})", RightBoolInt(0).ToString());
		}
		[Fact]
		public void EqualsTest() {
			Assert.True(LeftBoolInt(true) == LeftBoolInt(true));
			Assert.True(RightBoolInt(0) == RightBoolInt(0));
			Assert.False(LeftBoolInt(true) == RightBoolInt(0));
		}
		[Fact]
		public void InfersLeft() {
			Either<bool, int> either = true;
			Assert.True
			(  either is Left<bool, int> {Value: bool value}
			&& value
			);
		}
		[Fact]
		public void InfersRight() {
			Either<int, bool> either = true;
			Assert.True
			(  either is Right<int, bool> {Value: bool value}
			&& value
			);
		}
		[Fact]
		public void NotInfersRight() {
			Either<bool, int> either = true;
			Assert.IsNotType<Right<bool, int>>(either);
		}
		[Fact]
		public void LeftReduceLeftTest() {
			Assert.True(LeftBoolInt(true).ReduceLeft(false));
		}
		[Fact]
		public void SwapsLeft() {
			Assert.IsType<Right<int, bool>>(LeftBoolInt(true).Swap);
		}
		[Fact]
		public void SwapsRight() {
			Assert.IsType<Left<int, bool>>(RightBoolInt(0).Swap);
		}
		[Fact]
		public void RightReduceLeftTest() {
			Assert.True(RightBoolInt(0).ReduceLeft(true));
		}
		[Fact]
		public void LeftLazyReduceLeftTest() {
			Assert.True(LeftBoolInt(true).ReduceLeft(x => false));
		}
		[Fact]
		public void RightLazyReduceLeftTest() {
			Assert.True(RightBoolInt(0).ReduceLeft(x => true));
		}
		[Fact]
		public void LeftMapLeftTest() {
			Assert.True(LeftBoolInt(false).MapLeft(x => true).ReduceLeft(false));
		}
		[Fact]
		public void RightMapLeftTest() {
			Assert.True(RightBoolInt(0).MapLeft(x => false).ReduceLeft(true));
		}
		[Fact]
		public void LeftEmpty() {
			Assert.Empty(LeftBoolInt(true));
		}
		[Fact]
		public void RightSingle() {
			Assert.Single(RightBoolInt(0));
		}
	}
}