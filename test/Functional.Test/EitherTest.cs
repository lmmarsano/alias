using S = System;
using Xunit;

namespace Functional.Test {
	public class EitherTest {
		public static TheoryData<string, Either<bool, int>> ToStringData { get; }
		= new TheoryData<string, Either<bool, int>>
		  { {$"Left<{typeof(bool)}, {typeof(int)}>({true})", LeftBoolInt(true)}
		  , {$"Right<{typeof(bool)}, {typeof(int)}>({0})", RightBoolInt(0)}
		  };
		public static TheoryData<S.Type, Either<bool, int>, bool> WhereData { get; }
		= new TheoryData<S.Type, Either<bool, int>, bool>
		  { {typeof(Right<bool, int>), RightBoolInt(0), true}
		  , {typeof(Left<bool, int>), RightBoolInt(0), false}
		  , {typeof(Left<bool, int>), LeftBoolInt(false), true}
		  , {typeof(Left<bool, int>), LeftBoolInt(false), false}
		  };
		public static TheoryData<S.Type, Either<bool, int>> CombineData { get; }
		= new TheoryData<S.Type, Either<bool, int>>
		  { {typeof(Right<bool, int>), Factory.Either<bool, Nothing>(Nothing.Value).Combine(RightBoolInt(0))}
		  , {typeof(Left<bool, int>), RightBoolInt(0).Combine(LeftBoolInt(false))}
		  , {typeof(Left<bool, int>), LeftBoolInt(false).Combine(RightBoolInt(0))}
		  };
		static Either<bool, int> LeftBoolInt(bool value) => value;
		static Either<bool, int> RightBoolInt(int value) => value;
		[Theory]
		[MemberData(nameof(ToStringData))]
		public void ToStringTest(string expected, Either<bool, int> sut) => Assert.Equal(expected, sut.ToString());
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
			(either is Left<bool, int> { Value: bool value }
			&& value
			);
		}
		[Fact]
		public void InfersRight() {
			Either<int, bool> either = true;
			Assert.True
			(either is Right<int, bool> { Value: bool value }
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
		public void LeftSelectLeftTest() {
			Assert.True(LeftBoolInt(false).SelectLeft(x => true).ReduceLeft(false));
		}
		[Fact]
		public void RightSelectLeftTest() {
			Assert.True(RightBoolInt(0).SelectLeft(x => false).ReduceLeft(true));
		}
		[Fact]
		public void LeftSelectManyTest() {
			Assert.True(LeftBoolInt(true).SelectMany(x => RightBoolInt(0)).ReduceLeft(false));
		}
		[Fact]
		public void RightSelectManyTest() {
			Assert.Equal(1, RightBoolInt(0).SelectMany(x => RightBoolInt(1)).ReduceRight(0));
		}
		[Theory]
		[MemberData(nameof(WhereData))]
		public void WhereTest(S.Type type, Either<bool, int> value, bool filter)
		=> Assert.IsType(type, value.Where(x => filter, x => false));
		[Theory]
		[MemberData(nameof(CombineData))]
		public void CombineTest(S.Type expected, Either<bool, int> sut) => Assert.IsType(expected, sut);
		[Fact]
		public void LeftCatchTest() {
			Assert.True(LeftBoolInt(false).Catch(x => RightBoolInt(0)).ReduceLeft(true));
		}
		[Fact]
		public void RightCatchTest() {
			Assert.Equal(0, RightBoolInt(0).Catch(x => RightBoolInt(1)).ReduceRight(1));
		}
		[Fact]
		public void LeftEmpty() {
			Assert.Empty(LeftBoolInt(true));
		}
		[Fact]
		public void RightSingle() {
			Assert.Single(RightBoolInt(0));
		}
		[Fact]
		public void RightOfType() {
			var Right0 = RightBoolInt(0);
			Assert.Equal(Right0, Right0.OfType<int>(x => false));
			Assert.Equal((Left<bool, bool>)true, Right0.OfType<bool>(x => true));
		}
		[Fact]
		public void LeftOfType() {
			var leftFalse = LeftBoolInt(false);
			Assert.Equal(leftFalse, leftFalse.OfType<int>(x => true));
			var conversion = leftFalse.OfType<bool>(x => true);
			Assert.IsType<Left<bool, bool>>(conversion);
			Assert.Equal((Left<bool, bool>)false, conversion);
		}
	}
}
