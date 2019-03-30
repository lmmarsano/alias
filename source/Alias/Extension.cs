using S = System;
using STT = System.Threading.Tasks;
using SCG = System.Collections.Generic;
using F = Functional;
using static Functional.Extension;
using AC = Alias.Configuration;
using System.Linq;

namespace Alias {
	static class Extension {
		public static STT.Task DisplayMessage(this ITerminalException @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> @this is S.Exception error
		 ? Environment.GetErrorStream(maybeEnvironment).WriteAsync(GetErrorMessage(error))
		 : STT.Task.CompletedTask;
		public static STT.Task DisplayMessage(this S.Exception @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> @this switch
		   { S.AggregateException aggregate
		     => STT.Task.WhenAll
		        ( aggregate.Flatten().InnerExceptions
		          .OfType<ITerminalException>()
		          .Select(error => error.DisplayMessage(maybeEnvironment, maybeConfiguration))
		        )
		   , ITerminalException exception => exception.DisplayMessage(maybeEnvironment, maybeConfiguration)
		   , _ => STT.Task.CompletedTask
		   };
		static string GetErrorMessage(S.Exception error) {
			var stringBuilder = new S.Text.StringBuilder();
			var stack = new SCG.Stack<S.Exception>();
			for (stack.Push(error); stack.Any(); error = stack.Pop()) {
				if (error is S.AggregateException { InnerExceptions: var aggregate }) {
					foreach (var item in aggregate) {
						stack.Push(item);
					}
				} else {
					stringBuilder.AppendLine(error.Message);
					if (error.InnerException is S.Exception inner) {
						stack.Push(inner);
					}
				}
			}
			return stringBuilder.ToString();
		}
		/**
		 * <summary>
		 * Map errors/failures of possible tasks.
		 * </summary>
		 * <param name="@this">Possible task.</param>
		 * <param name="errorMap">Map for errors and any task failures.</param>
		 * <typeparam name="T">Type task yields.</typeparam>
		 * <returns>Possible task with mapped errors/failures.</returns>
		 */
		public static F.Result<STT.Task<T>> SelectErrorNested<T>(this F.Result<STT.Task<T>> @this, S.Func<S.Exception, S.Exception> errorMap)
		where T: object
		=> @this.SelectError(errorMap).Select(task => task.SelectErrorAsync(errorMap));
		/**
		 * <summary>
		 * Map errors/failures of possible tasks.
		 * </summary>
		 * <param name="@this">Possible task.</param>
		 * <param name="errorMap">Map for errors and any task failures.</param>
		 * <returns>Possible task with mapped errors/failures.</returns>
		 */
		public static F.Result<STT.Task> SelectErrorNested(this F.Result<STT.Task> @this, S.Func<S.Exception, S.Exception> errorMap)
		=> @this.SelectError(errorMap).Select(task => task.SelectErrorAsync(errorMap));
	}
}