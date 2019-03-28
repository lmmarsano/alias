using S = System;
using STT = System.Threading.Tasks;
using F = Functional;
using Xunit;

namespace Alias.Test {
	public static class Utility {
		public static T FromOk<T>(this F.Result<T> @this)
		where T: object {
			Assert.IsType<F.Ok<T>>(@this);
			return ((F.Ok<T>)@this).Value;
		}
	}
}