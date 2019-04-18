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
		/**
		 * <summary>
		 * Output error message to environment.
		 * </summary>
		 * <param name="this">Error to output.</param>
		 * <param name="maybeEnvironment">Optional environment.</param>
		 * <param name="maybeConfiguration">Optional configuration.</param>
		 * <returns>Task to output error.</returns>
		 */
		public static STT.Task DisplayMessage(this S.Exception @this, ST.Maybe<IEnvironment> maybeEnvironment)
		=> @this switch
		   { S.AggregateException aggregate
		     => aggregate.Flatten().InnerExceptions
		        .OfType<TerminalException>()
		        .SelectMany(error => error.ErrorMessage)
		        .Traverse(Environment.GetErrorStream(maybeEnvironment).WriteLineAsync)
		   , TerminalException exception
		     => exception.ErrorMessage
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
		where T: object
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
		where T: STT.Task {
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
		/**
		 * <summary>
		 * Map a sequence to a task that performs a sequence of tasks in order.
		 * </summary>
		 * <param name="this">A sequence.</param>
		 * <param name="map">Map from sequence elements to tasks.</param>
		 * <typeparam name="T">Sequence element type.</typeparam>
		 * <returns>A task performing the map images sequentially.</returns>
		 */
		public static STT.Task Traverse<T>(this SCG.IEnumerable<T> @this, S.Func<T, STT.Task> map)
		=> @this.Aggregate
		   ( STT.Task.CompletedTask
		   , (task, item) => task.ContinueWith(_ => map(item)).Unwrap()
		   );
		/**
		 * <summary>
		 * Map a sequence to an asynchronous sequence.
		 * </summary>
		 * <param name="this">A sequence.</param>
		 * <param name="map">Map from sequence elements to tasks.</param>
		 * <typeparam name="T">Sequence element type.</typeparam>
		 * <typeparam name="TResult"></typeparam>
		 * <returns>An asynchronous sequence of values yielded by map images.</returns>
		 */
		public static async SCG.IAsyncEnumerable<TResult> Traverse<T, TResult>(this SCG.IEnumerable<T> @this, S.Func<T, STT.Task<TResult>> map) {
			foreach (var item in @this) {
				yield return await map(item).ConfigureAwait(false);
			}
		}
		/**
		 * <summary>
		 * Attempt to fold sequence with cycle detection.
		 * If a cycle is detected, return an element in the cycle, instead.
		 * </summary>
		 * <param name="this">A sequence.</param>
		 * <param name="isCollision">Predicate for detecting cycles: true if and only if the operands collide.</param>
		 * <param name="seed">The initial accumulator value.</param>
		 * <param name="func">An accumulator function to be invoked on each element.</param>
		 * <typeparam name="TSource">Sequence element type.</typeparam>
		 * <typeparam name="TAccumulate">Fold result type.</typeparam>
		 * <returns>Fold result or an element in a cycle.</returns>
		 */
		public static ST.Either<TSource, TAccumulate> AggregateAcyclically<TSource, TAccumulate>(this SCG.IEnumerable<TSource> @this, S.Func<TSource, TSource, bool> isCollision, TAccumulate seed, S.Func<TAccumulate, TSource, TAccumulate> func)
		where TSource: object
		where TAccumulate: object {
			var fast = @this.GetEnumerator();
			var slow = @this.GetEnumerator();
			if (!fast.MoveNext()) {
				return seed;
			}
			slow.MoveNext();
			for (seed = func(seed, fast.Current); fast.MoveNext(); seed = func(seed, fast.Current)) {
				if (isCollision(slow.Current, fast.Current)) {
					return fast.Current;
				}
				seed = func(seed, fast.Current);
				slow.MoveNext();
				if (!fast.MoveNext()) {
					return seed;
				}
				if (isCollision(slow.Current, fast.Current)) {
					return fast.Current;
				}
			}
			return seed;
		}
	}
}
