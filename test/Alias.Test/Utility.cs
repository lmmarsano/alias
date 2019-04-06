using S = System;
using SIO = System.IO;
using SG = System.Globalization;
using STT = System.Threading.Tasks;
using F = Functional;
using Xunit;

namespace Alias.Test {
	public static class Utility {
		public static string DirectorySeparator { get; }
		= SIO.Path.DirectorySeparatorChar.ToString(SG.CultureInfo.InvariantCulture);
		public static T FromJust<T>(this F.Maybe<T> @this)
		where T : object {
			Assert.IsType<F.Just<T>>(@this);
			return ((F.Just<T>)@this).Value;
		}
		public static T FromOk<T>(this F.Result<T> @this)
		where T : object {
			Assert.IsType<F.Ok<T>>(@this);
			return ((F.Ok<T>)@this).Value;
		}
		public static S.Exception FromError<T>(this F.Result<T> @this)
		where T : object {
			Assert.IsType<F.Error<T>>(@this);
			return ((F.Error<T>)@this).Value;
		}
		public static STT.Task TaskFaulted
		=> STT.Task.FromException(new S.Exception());
		public static STT.Task<T> TaskFault<T>()
		=> STT.Task.FromException<T>(new S.Exception());
		public static string NormalizeLineEnd(string newLine, string input)
		=> newLine == S.Environment.NewLine
		 ? input
		 : input.Replace(newLine, S.Environment.NewLine, S.StringComparison.Ordinal);
	}
}
