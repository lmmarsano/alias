using S = System;
using Xunit;

namespace Functional.Test {
	public class MaybeTest {
		static Maybe<bool> NothingBool => Nothing.Value;
		static Maybe<bool> JustBool(bool value) => value;
		public static TheoryData<string, Maybe<bool>> ToStringData { get; }
		= new TheoryData<string, Maybe<bool>>
		  { {$"Nothing<{typeof(bool)}>", NothingBool}
		  , {$"Just<{typeof(bool)}>({true})", JustBool(true)}
		  };
		[Theory]
		[MemberData(nameof(ToStringData))]
		public void NothingDisplays(string expected, Maybe<bool> sut) => Assert.Equal(expected, sut.ToString());
		public static TheoryData<Maybe<bool>, Maybe<bool>> EqualsData { get; }
		= new TheoryData<Maybe<bool>, Maybe<bool>>
		  { {JustBool(false), JustBool(false)}
		  , {NothingBool, NothingBool}
		  };
		[Theory]
		[MemberData(nameof(EqualsData))]
		public void EqualsTest(Maybe<bool> left, Maybe<bool> right) => Assert.True(left == right);
		[Fact]
		public void NothingGenericEquals() => Assert.True(Nothing<bool>.Value.Equals(Nothing<int>.Value));
		public static TheoryData<Maybe<bool>, Maybe<bool>> NotEqualsData { get; }
		= new TheoryData<Maybe<bool>, Maybe<bool>>
		  { {JustBool(false), JustBool(true)}
		  , {JustBool(false), NothingBool}
		  };
		[Theory]
		[MemberData(nameof(NotEqualsData))]
		public void NotEqualsTest(Maybe<bool> left, Maybe<bool> right) => Assert.False(left == right);
		[Fact]
		public void InfersSome() {
			Maybe<bool> maybe = true;
			Assert.True
			(  maybe is Just<bool> {Value: bool value}
			&& value
			);
		}
		[Fact]
		public void InfersNone() {
			Maybe<bool> maybe = Nothing.Value;
			Assert.IsType<Nothing<bool>>(maybe);
		}
		public static TheoryData<Maybe<bool>, bool> ReduceData
		= new TheoryData<Maybe<bool>, bool>
		  { {JustBool(true), false}
		  , {NothingBool, true}
		  };
		[Theory]
		[MemberData(nameof(ReduceData))]
		public void ReduceTest(Maybe<bool> maybe, bool alternative) {
			Assert.True(maybe.Reduce(alternative));
			Assert.True(maybe.Reduce(() => alternative));
			Assert.True(maybe.Reduce(alternative, _ => !alternative));
			Assert.True(maybe.Reduce(() => alternative, _ => !alternative));
		}
		public static TheoryData<Maybe<bool>, bool> SelectData
		= new TheoryData<Maybe<bool>, bool>
		  { {JustBool(false), true}
		  , {NothingBool, false}
		  };
		[Theory]
		[MemberData(nameof(SelectData))]
		public void SelectTest(Maybe<bool> maybe, bool select) {
			Assert.True(maybe.Select(x => select).Reduce(!select));
		}
		public static TheoryData<S.Type, Maybe<bool>, bool> WhereData { get; }
		= new TheoryData<S.Type, Maybe<bool>, bool>
		  { {typeof(Just<bool>), JustBool(false), true}
		  , {typeof(Nothing<bool>), JustBool(false), false}
		  , {typeof(Nothing<bool>), NothingBool, true}
		  , {typeof(Nothing<bool>), NothingBool, false}
		  };
		[Theory]
		[MemberData(nameof(WhereData))]
		public void WhereTest(S.Type type, Maybe<bool> value, bool filter)
		=> Assert.IsType(type, value.Where(x => filter));
		public static TheoryData<S.Type, Maybe<bool>> CombineData { get; }
		= new TheoryData<S.Type, Maybe<bool>>
		  { {typeof(Just<bool>), Factory.Maybe(0).Combine(JustBool(false))}
		  , {typeof(Nothing<bool>), JustBool(false).Combine(NothingBool)}
		  , {typeof(Nothing<bool>), NothingBool.Combine(JustBool(false))}
		  };
		[Theory]
		[MemberData(nameof(CombineData))]
		public void CombineTest(S.Type expected, Maybe<bool> sut) => Assert.IsType(expected, sut);
		[Fact]
		public void JustSingle() {
			Assert.Single(JustBool(true));
		}
		[Fact]
		public void NothingEmpty() {
			Assert.Empty(NothingBool);
		}
	}
}