using Xunit;

namespace Functional.Test {
	public class MaybeTest {
		static Maybe<bool> NothingBool => Nothing.Value;
		public Maybe<bool> JustBool(bool value) => value;
		[Fact]
		public void NothingDisplays()
		=> Assert.Equal($"Nothing<{typeof(bool)}>", NothingBool.ToString());
		[Fact]
		public void JustDisplays()
		=> Assert.Equal($"Just<{typeof(bool)}>({true})", JustBool(true).ToString());
		[Fact]
		public void JustEquals()
		=> Assert.True(JustBool(true) == JustBool(true));
		[Fact]
		public void NothingEquals()
		=> Assert.True(NothingBool == NothingBool);
		[Fact]
		public void NothingGenericEquals()
		=> Assert.True(Nothing<bool>.Value.Equals(Nothing<int>.Value));
		[Fact]
		public void JustNotEquals()
		=> Assert.False(JustBool(true) == JustBool(false));
		[Fact]
		public void MaybeNotEquals()
		=> Assert.False(JustBool(true) == NothingBool);
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
		[Fact]
		public void JustReduceTest() {
			Assert.True(JustBool(true).Reduce(false));
		}
		[Fact]
		public void NothingReduceTest() {
			Assert.True(NothingBool.Reduce(true));
		}
		[Fact]
		public void JustLazyReduceTest() {
			Assert.True(JustBool(true).Reduce(() => false));
		}
		[Fact]
		public void NothingLazyReduceTest() {
			Assert.True(NothingBool.Reduce(() => true));
		}
		[Fact]
		public void JustMapTest() {
			Assert.True(JustBool(false).Select(x => true).Reduce(false));
		}
		[Fact]
		public void NothingMapTest() {
			Assert.True(NothingBool.Select(x => false).Reduce(true));
		}
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