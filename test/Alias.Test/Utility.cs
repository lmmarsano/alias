using S = System;
using SIO = System.IO;
using SG = System.Globalization;
using STT = System.Threading.Tasks;
using ST = LMMarsano.SumType;
using Xunit;
using AC = Alias.ConfigurationData;

namespace Alias.Test {
	static class Utility {
		public static string DirectorySeparator { get; }
		= SIO.Path.DirectorySeparatorChar.ToString(SG.CultureInfo.InvariantCulture);
		public static T FromJust<T>(this ST.Maybe<T> @this)
		where T : object {
			Assert.IsType<ST.Just<T>>(@this);
			return ((ST.Just<T>)@this).Value;
		}
		public static T FromOk<T>(this ST.Result<T> @this)
		where T : object {
			Assert.IsType<ST.Ok<T>>(@this);
			return ((ST.Ok<T>)@this).Value;
		}
		public static S.Exception FromError<T>(this ST.Result<T> @this)
		where T : object {
			Assert.IsType<ST.Error<T>>(@this);
			return ((ST.Error<T>)@this).Value;
		}
		public static TRight FromRight<TLeft, TRight>(this ST.Either<TLeft, TRight> @this)
		where TLeft : object
		where TRight : object {
			Assert.IsType<ST.Right<TLeft, TRight>>(@this);
			return ((ST.Right<TLeft, TRight>)@this).Value;
		}
		public static TLeft FromLeft<TLeft, TRight>(this ST.Either<TLeft, TRight> @this)
		where TLeft : object
		where TRight : object {
			Assert.IsType<ST.Left<TLeft, TRight>>(@this);
			return ((ST.Left<TLeft, TRight>)@this).Value;
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
