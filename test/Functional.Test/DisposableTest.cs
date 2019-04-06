using S = System;
using SDC = System.Diagnostics.CodeAnalysis;
using Xunit;

namespace Functional.Test {
	class DisposalState {
		public bool IsDisposed { get; set; }
	}
	class FakeDisposable: S.IDisposable {
		public DisposalState DisposalState { get; }
		public FakeDisposable() : this(new DisposalState()) {}
		public FakeDisposable(DisposalState disposalState) {
			DisposalState = disposalState;
		}
		public void Dispose() {
			Dispose(true);
			S.GC.SuppressFinalize(this);
		}
		[SDC.SuppressMessage("Build", "CA1801", Justification = "Formal.")]
		public void Dispose(bool disposing)
		=> DisposalState.IsDisposed = true;
	}
	public class DisposableTest {
		[Fact]
		public void GetsDisposable() {
			var disposable = new FakeDisposable();
			Assert.Same(disposable, Disposable.Using(disposable, x => x));
			Assert.Same(disposable, Disposable.UsingMap((FakeDisposable x) => x)(disposable));
		}
		[Fact]
		public void AppliesMap() {
			var disposable = new FakeDisposable();
			Assert.True(Disposable.Using(disposable, x => true));
			Assert.True(Disposable.UsingMap((FakeDisposable x) => true)(disposable));
		}
		[Fact]
		public void Disposes() {
			var disposalState = new DisposalState();
			Disposable.Using(new FakeDisposable(disposalState), x => x);
			Assert.True(disposalState.IsDisposed);
		}
		[Fact]
		public void MapDisposes() {
			var disposalState = new DisposalState();
			Disposable.UsingMap((FakeDisposable x) => x)(new FakeDisposable(disposalState));
			Assert.True(disposalState.IsDisposed);
		}
	}
}
