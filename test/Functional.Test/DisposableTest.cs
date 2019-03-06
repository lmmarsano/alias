using S = System;
using Xunit;

namespace Functional.Test {
	class DisposalState {
		public bool IsDisposed { get; set; }
	}
	class FakeDisposable: S.IDisposable {
		public DisposalState DisposalState { get; }
		public FakeDisposable(): this(new DisposalState()) {}
		public FakeDisposable(DisposalState disposalState) {
			DisposalState = disposalState;
		}
		public void Dispose() {
			Dispose(true);
			S.GC.SuppressFinalize(this);
		}
		public void Dispose(bool disposing)
		=> DisposalState.IsDisposed = true;
	}
	public class DisposableTest {
		[Fact]
		public void GetsDisposable() {
			var disposable = new FakeDisposable();
			Assert.Same(disposable, Disposable.Using(disposable, x => x));
		}
		[Fact]
		public void AppliesMap() {
			Assert.True(Disposable.Using(new FakeDisposable(), x => true));
		}
		[Fact]
		public void Disposes() {
			var disposalState = new DisposalState();
			Disposable.Using(new FakeDisposable(disposalState), x => x);
			Assert.True(disposalState.IsDisposed);
		}
	}
}