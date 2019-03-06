using S = System;
using Xunit;

namespace Functional.Test {
	public class FactoryTest {
		[Fact]
		public void TryOkTest() {
			Assert.Equal((Result<bool>)true, Factory.Try(() => true));
			Assert.Equal((Result<Nothing>)Nothing.Value, Factory.Try(() => { return; }));
		}
		[Fact]
		public void TryErrorTest() {
			var exception = new S.Exception();
			Assert.Equal((Result<bool>)exception, Factory.Try<bool>(() => throw exception));
			Assert.Equal((Result<Nothing>)exception, Factory.Try(() => throw exception));
		}
	}
}