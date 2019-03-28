using S = System;
using STT = System.Threading.Tasks;

namespace Functional {
	/**
	 * <summary>
	 * Functional equivalent of a <c cref="using">using</see> statement.
	 * </summary>
	 */
	public static class Disposable {
		/**
		 * <summary>
		 * Functional resource manager: acquires resource, applies <paramref name="map"/> to it, disposes resource, and returns result.
		 * </summary>
		 * <param name="disposable">Resource to acquire.</param>
		 * <param name="map">Maps the acquired resource to a result.</param>
		 * <typeparam name="TDisposable">A resource type that implements <see cref="S.IDisposable"/>.</typeparam>
		 * <typeparam name="TResult">Image type of <paramref name="map"/>.</typeparam>
		 * <returns>The result of <paramref name="map"/> applied to the resource.</returns>
		 */
		public static TResult Using<TDisposable, TResult>(TDisposable disposable, S.Func<TDisposable, TResult> map) where TDisposable: S.IDisposable {
			using (disposable) {
				return map(disposable);
			}
		}
		/**
		 * <summary>
		 * Functional resource manager map: function that applies <paramref name="map"/> with managed resource.
		 * </summary>
		 * <param name="map">Maps the acquired resource to a result.</param>
		 * <typeparam name="TDisposable">A resource type that implements <see cref="S.IDisposable"/>.</typeparam>
		 * <typeparam name="TResult">Image type of <paramref name="map"/>.</typeparam>
		 * <returns><paramref name="map"/> with resource management.</returns>
		 */
		public static S.Func<TDisposable, TResult> UsingMap<TDisposable, TResult>(S.Func<TDisposable, TResult> map) where TDisposable: S.IDisposable
		=> disposable => {
		   	using (disposable) {
		   		return map(disposable);
		   	}
		   };
	}
}