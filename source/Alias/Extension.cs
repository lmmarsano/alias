using S = System;
using SD = System.Diagnostics;
using STT = System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;
using SCG = System.Collections.Generic;
using ST = LMMarsano.SumType;
using static LMMarsano.SumType.Extension;
using AC = Alias.ConfigurationData;
using System.Linq;
using Name = System.String;

namespace Alias {
	static class Extension {
		public static STT.Task DisplayMessage(this S.Exception @this, ST.Maybe<IEnvironment> maybeEnvironment, ST.Maybe<AC.Configuration> maybeConfiguration)
		=> @this switch
		   { S.AggregateException aggregate
		     => aggregate.Flatten().InnerExceptions
		        .Where(x => x is ITerminalException)
		        .SelectMany(Utility.GetErrorMessage)
		        .Traverse(Environment.GetErrorStream(maybeEnvironment).WriteLineAsync)
		   , ITerminalException exception
		     => Utility.GetErrorMessage(@this)
		        .Traverse(Environment.GetErrorStream(maybeEnvironment).WriteLineAsync)
		   , _ => STT.Task.CompletedTask
		   };
		/**
		 * <summary>
		 * Map errors/failures of possible tasks.
		 * </summary>
		 * <param name="@this">Possible task.</param>
		 * <param name="errorMap">Map for errors and any task failures.</param>
		 * <typeparam name="T">Type task yields.</typeparam>
		 * <returns>Possible task with mapped errors/failures.</returns>
		 */
		public static ST.Result<STT.Task<T>> SelectErrorNested<T>(this ST.Result<STT.Task<T>> @this, S.Func<S.Exception, S.Exception> errorMap)
		where T : object
		=> @this.SelectError(errorMap).Select(task => task.SelectErrorAsync(errorMap));
		/**
		 * <summary>
		 * Map errors/failures of possible tasks.
		 * </summary>
		 * <param name="@this">Possible task.</param>
		 * <param name="errorMap">Map for errors and any task failures.</param>
		 * <returns>Possible task with mapped errors/failures.</returns>
		 */
		public static ST.Result<STT.Task> SelectErrorNested(this ST.Result<STT.Task> @this, S.Func<S.Exception, S.Exception> errorMap)
		=> @this.SelectError(errorMap).Select(task => task.SelectErrorAsync(errorMap));
		/**
		 * <summary>
		 * Map possible tasks to their successful possibilities or alternatives.
		 * </summary>
		 * <param name="@this">Possible task.</param>
		 * <param name="alternative">Map from errors/faults to alternative tasks.</param>
		 * <typeparam name="T">Type task yields.</typeparam>
		 * <returns>The successful task or alternative mapped from error/fault.</returns>
		 */
		public static STT.Task<T> ReduceNested<T>(this ST.Result<STT.Task<T>> @this, S.Func<S.Exception, STT.Task<T>> alternative)
		where T : object
		=> @this.Reduce(alternative).CatchAsync(alternative);
		/**
		 * <summary>
		 * Map possible tasks to their successful possibilities or alternatives.
		 * </summary>
		 * <param name="@this">Possible task.</param>
		 * <param name="alternative">Map from errors/faults to alternative tasks.</param>
		 * <returns>The successful task or alternative mapped from error/fault.</returns>
		 */
		public static STT.Task ReduceNested(this ST.Result<STT.Task> @this, S.Func<S.Exception, STT.Task> alternative)
		=> @this.Reduce(alternative).CatchAsync(alternative);
		/**
		 * <summary>
		 * Run process asynchronously.
		 * </summary>
		 * <param name="@this">Process to run.</param>
		 * <returns>Task of a running process resulting in an exit code.</returns>
		 * <inheritdoc cref='SD.Process.Start' select='exception'/>
		 */
		public static STT.Task<int> RunAsync(this SD.Process @this) {
			var taskCompletionSource = new STT.TaskCompletionSource<int>();
			@this.EnableRaisingEvents = true;
			@this.Exited += (sender, eventArgs) => {
				taskCompletionSource.TrySetResult(@this.ExitCode);
			};
			if (!@this.Start() && @this.HasExited) {
				taskCompletionSource.TrySetResult(@this.ExitCode);
			}
			return taskCompletionSource.Task;
		}
		/**
		 * <summary>
		 * Return started task.
		 * </summary>
		 * <param name="@this">Task.</param>
		 * <typeparam name="T">Task type.</typeparam>
		 * <returns>Started task.</returns>
		 */
		public static T StartAsync<T>(this T @this)
		where T : STT.Task {
			if (@this.Status == STT.TaskStatus.Created) {
				@this.Start();
			}
			return @this;
		}
		/**
		 * <summary>
		 * Convert a sequence to a binding.
		 * </summary>
		 * <param name="@this">A sequence.</param>
		 * <param name="nameSelector">Map from sequence item to name.</param>
		 * <param name="commandEntrySelector">Map from sequence item to command entry.</param>
		 * <typeparam name="T">Sequence item type.</typeparam>
		 * <returns>A binding.</returns>
		 */
		public static AC.BindingDictionary ToBinding<T>(this SCG.IEnumerable<T> @this, S.Func<T, Name> nameSelector, S.Func<T, AC.CommandEntry> commandEntrySelector) {
			var binding = new AC.BindingDictionary();
			foreach (var item in @this) {
				binding.Add(nameSelector(item), commandEntrySelector(item));
			}
			return binding;
		}
		public static STT.Task Traverse<T>(this SCG.IEnumerable<T> @this, S.Func<T, STT.Task> map)
		=> @this.Aggregate
		   ( STT.Task.CompletedTask
		   , (task, item) => task.ContinueWith(_ => map(item)).Unwrap()
		   );
		public static async SCG.IAsyncEnumerable<TResult> Traverse<T, TResult>(this SCG.IEnumerable<T> @this, S.Func<T, STT.Task<TResult>> map) {
			foreach (var item in @this) {
				yield return await map(item).ConfigureAwait(false);
			}
		}
	}
}
