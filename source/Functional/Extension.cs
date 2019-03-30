using S = System;
using SCG = System.Collections.Generic;
using ST = System.Threading;
using STT = System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;
using System.Linq;

namespace Functional {
	public static class Extension {
		/**
		 * <summary>
		 * <see cref="Just{T}"/> the current value when <paramref name="condition"/>, otherwise <see cref="Nothing{T}"/>.
		 * </summary>
		 * <param name="condition">Condition choosing <see cref="Just{T}"/> or <see cref="Nothing{T}"/>.</param>
		 * <typeparam name="T">Non-nullable.</typeparam>
		 * <returns><see cref="Just{T}"/> current value if condition <see cref="true"/>, otherwise <see cref="Nothing{T}"/>.</returns>
		 */
		public static Maybe<T> When<T>(this T @this, bool condition) where T: object
		=> condition
		 ? (Maybe<T>)@this
		 : Nothing.Value;
		/**
		 * <summary>
		 * <see cref="Just{T}"/> the current value if it satisfies <paramref name="predicate"/>, otherwise <see cref="Nothing{T}"/>.
		 * </summary>
		 * <param name="predicate">Predicate over current value.</param>
		 * <typeparam name="T">Non-nullable.</typeparam>
		 * <returns><see cref="Just{T}"/> current value if predicate maps to <see cref="true"/>, otherwise <see cref="Nothing{T}"/>.</returns>
		 */
		public static Maybe<T> When<T>(this T @this, S.Func<T, bool> predicate) where T: object
		=> @this.When(predicate(@this));
		/**
		 * <summary>
		 * Convert to <see cref="Maybe{T}"/> by mapping nulls to <see cref="Nothing{T}"/>.
		 * </summary>
		 * <typeparam name="T">Non-nullable reference.</typeparam>
		 * <returns><see cref="Nothing{T}"/> if null, otherwise <see cref="Just{T}"/> current value.</returns>
		 */
		public static Maybe<T> ToMaybe<T>(this T? @this) where T: class
		=> @this is T value
		 ? (Maybe<T>)value
		 : Nothing.Value;
		/**
		 * <summary>
		 * Convert to <see cref="Maybe{T}"/> by mapping missing values to <see cref="Nothing{T}"/>.
		 * </summary>
		 * <typeparam name="T">Value type.</typeparam>
		 * <returns><see cref="Just{T}"/> current value is any, otherwise <see cref="Nothing{T}"/>.</returns>
		 */
		public static Maybe<T> ToMaybe<T>(this T? @this) where T: struct
		=> @this is T value
		 ? (Maybe<T>)value
		 : Nothing.Value;
		/**
		 * <summary>
		 * Maybe get the current enumerable’s first element.
		 * </summary>
		 * <typeparam name="T">Non-nullable.</typeparam>
		 * <returns><see cref="Just{T}"/> first element or <see cref="Nothing{T}"/>.</returns>
		 */
		public static Maybe<T> FirstOrNone<T>(this SCG.IEnumerable<T> @this) where T: object
		=> @this.Select(Factory.Maybe).DefaultIfEmpty(Nothing.Value).First();
		/**
		 * <summary>
		 * Maybe get the current enumerable’s first filtered element.
		 * </summary>
		 * <param name="predicate">Predicate for passing filter.</param>
		 * <typeparam name="T">Non-nullable.</typeparam>
		 * <returns><see cref="Just{T}"/> first filtered element or <see cref="Nothing{T}"/>.</returns>
		 */
		public static Maybe<T> FirstOrNone<T>(this SCG.IEnumerable<T> @this, S.Func<T, bool> predicate) where T: object
		=> @this.Where(predicate).FirstOrNone();
		/**
		 * <summary>
		 * Return a sequence of successfully mapped images.
		 * </summary>
		 * <param name="selector">A map that can fail.</param>
		 * <typeparam name="TIn">The current enumerable’s element type.</typeparam>
		 * <typeparam name="TOut">Image type the <paramref name="selector"/> possibly returns.</typeparam>
		 * <returns>An enumerable of images the map successfully returned.</returns>
		 */
		public static SCG.IEnumerable<TOut> SelectMaybe<TIn, TOut>(this SCG.IEnumerable<TIn> @this, S.Func<TIn, Maybe<TOut>> selector) where TOut: object
		=> @this.Select(selector).OfType<Just<TOut>>().Select(x => x.Value);
		/**
		 * <summary>
		 * Maybe return the dictionary’s value for <paramref name="key"/>.
		 * </summary>
		 * <param name="key">Key to lookup in current dictionary.</param>
		 * <typeparam name="TKey">Key type.</typeparam>
		 * <typeparam name="TValue">Value type.</typeparam>
		 * <returns><see cref="Just{T}"/> the lookup value or <see cref="Nothing{T}"/>.</returns>
		 */
		public static Maybe<TValue> TryGetValue<TKey, TValue>(this SCG.IDictionary<TKey, TValue> @this, TKey key) where TValue: object
		=> @this.TryGetValue(key, out var value)
		 ? (Maybe<TValue>)value
		 : Nothing.Value;
		/**
		 * <summary>
		 * Functional resource manager: acquires resource, applies <paramref name="map"/> to it, disposes resource, and returns result.
		 * </summary>
		 * <param name="map">Maps the acquired resource to a result.</param>
		 * <typeparam name="TDisposable">A resource type that implements <see cref="S.IDisposable"/>.</typeparam>
		 * <typeparam name="TResult">Image type of <paramref name="map"/>.</typeparam>
		 * <returns>The result of <paramref name="map"/> applied to the resource.</returns>
		 */
		public static TResult Using<TDisposable, TResult>(this TDisposable @this, S.Func<TDisposable, TResult> map) where TDisposable: S.IDisposable => Disposable.Using(@this, map);
		/**
		 * <summary>
		 * Convert to <see cref="Result{T}"/> by mapping nulls to <see cref="Error{T}"/>.
		 * </summary>
		 * <param name="onNull">Error value map for null.</param>
		 * <typeparam name="T">Non-nullable reference.</typeparam>
		 * <returns><see cref="Error{T}"/> if null, otherwise <see cref="Ok{T}"/> current value.</returns>
		 */
		public static Result<T> ToResult<T>(this T? @this, S.Func<S.Exception> onNull) where T : class
		=> @this is T value
		 ? (Result<T>)value
		 : onNull();
		/**
		 * <summary>
		 * Convert to <see cref="Result{T}"/> by mapping missing values to <see cref="Error{T}"/>.
		 * </summary>
		 * <param name="onNull">Error value map for missing value.</param>
		 * <typeparam name="T">Value type.</typeparam>
		 * <returns><see cref="Ok{T}"/> if current value is any, otherwise <see cref="Error{T}"/>.</returns>
		 */
		public static Result<T> ToResult<T>(this T? @this, S.Func<S.Exception> onNull) where T : struct
		=> @this is T value
		 ? (Result<T>)value
		 : onNull();
		/**
		 * <summary>
		 * Convert a task throwing exceptions to a task returning results.
		 * </summary>
		 * <param name="@this">A task.</param>
		 * <typeparam name="T">A non-nullable type.</typeparam>
		 * <returns>A task returning results.</returns>
		 */
		public static STT.Task<Result<T>> ToResultAsync<T>(this STT.Task<T> @this) where T: object
		=> Factory.ResultAsync(@this);
		/**
		 * <summary>
		 * Convert a task throwing exceptions to a task returning results.
		 * </summary>
		 * <param name="@this">A task.</param>
		 * <param name="onFailure">An map from unsuccessful tasks to results.</param>
		 * <typeparam name="T">A non-nullable type.</typeparam>
		 * <returns>A task returning results.</returns>
		 */
		public static STT.Task<Result<T>> ToResultAsync<T>(this STT.Task<T> @this, S.Func<STT.Task<T>, Result<T>> onFailure) where T: object
		=> Factory.ResultAsync(@this, onFailure);
		/**
		 * <summary>
		 * Evaluate to result.
		 * </summary>
		 * <returns>Result of successful evaluation (<see cref="Nothing"/>) or error.</returns>
		 */
		public static Result<Nothing> Try(this S.Action @this) => Try(@this);
		/**
		 * <summary>
		 * Evaluate to result.
		 * </summary>
		 * <returns>Result of successful evaluation (<see cref="Nothing"/>) or error.</returns>
		 */
		public static Result<Nothing> Try(this S.Action @this, S.Func<S.Exception, S.Exception> errorMap) => Try(@this, errorMap);
		/**
		 * <summary>
		 * Evaluate to result.
		 * </summary>
		 * <typeparam name="T">The function’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(this S.Func<T> @this) where T: object => Try(@this);
		/**
		 * <summary>
		 * Evaluate to result.
		 * </summary>
		 * <param name="errorMap">Error mapper.</param>
		 * <typeparam name="T">The function’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(this S.Func<T> @this, S.Func<S.Exception, S.Exception> errorMap)
		where T: object => Try(@this, errorMap);
		/**
		 * <summary>
		 * Convert to <see cref="Either{TLeft, TRight}"/> by mapping nulls to <see cref="Left{TLeft, TRight}"/>.
		 * </summary>
		 * <param name="onNull">Left value map for null.</param>
		 * <typeparam name="TLeft">Non-nullable type.</typeparam>
		 * <typeparam name="TRight">Non-nullable reference.</typeparam>
		 * <returns><see cref="Left{TLeft, TRight}"/> if null, otherwise <see cref="Right{TLeft, TRight}"/> current value.</returns>
		 */
		public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this TRight? @this, S.Func<TLeft> onNull) where TLeft: object where TRight: class
		=> @this is TRight value
		 ? (Either<TLeft, TRight>)value
		 : onNull();
		/**
		 * <summary>
		 * Convert to <see cref="Either{TLeft, TRight}"/> by mapping missing values to <see cref="Left{TLeft, TRight}"/>.
		 * </summary>
		 * <param name="onNull">Left value map for missing value.</param>
		 * <typeparam name="TLeft">Non-nullable type.</typeparam>
		 * <typeparam name="TRight">Value type.</typeparam>
		 * <returns><see cref="Right{TLeft, TRight}"/> if current value is any, otherwise <see cref="Left{TLeft, TRight}"/>.</returns>
		 */
		public static Either<TLeft, TRight> ToEither<TLeft, TRight>(this TRight? @this, S.Func<TLeft> onNull) where TLeft: object where TRight : struct
		=> @this is TRight value
		 ? (Either<TLeft, TRight>)value
		 : onNull();
		/**
		 * <summary>
		 * Map successful task values to tasks and flatten.
		 * </summary>
		 * <param name="@this">Task yielding a value.</param>
		 * <param name="map">Map from task result to task.</param>
		 * <typeparam name="T">Type of value task yields.</typeparam>
		 * <typeparam name="TResult">Type of values map images yield.</typeparam>
		 * <returns>For successful task, task yielding value map image yields. Otherwise, an unsuccessful task.</returns>
		 */
		public static STT.Task<TResult> SelectManyAsync<T, TResult>(this STT.Task<T> @this, S.Func<T, STT.Task<TResult>> map)
		where T: object
		where TResult: object
		=> @this.SelectAsync(map).Unwrap();
		/**
		 * <summary>
		 * Follow successful task with a next task.
		 * </summary>
		 * <param name="@this">A task.</param>
		 * <param name="next">Task to follow.</param>
		 * <typeparam name="T">Type of values next task yields.</typeparam>
		 * <returns>If current task is successful, the next task follows. Otherwise, the current task remains.</returns>
		 */
		public static STT.Task<T> CombineAsync<T>(this STT.Task @this, STT.Task<T> next)
		where T: object
		=> @this.ContinueWith
		   ( task => {
		     	task.GetAwaiter().GetResult();
		     	return next;
		     }
		   , STT.TaskContinuationOptions.NotOnCanceled
		   ).Unwrap();
		/**
		 * <summary>
		 * Follow successful task with a next task.
		 * </summary>
		 * <param name="@this">A task.</param>
		 * <param name="next">Task to follow.</param>
		 * <returns>If current task is successful, the next task follows. Otherwise, the current task remains.</returns>
		 */
		public static STT.Task CombineAsync(this STT.Task @this, STT.Task next)
		=> @this.ContinueWith
		   ( task => {
		     	task.GetAwaiter().GetResult();
		     	return next;
		     }
		   , STT.TaskContinuationOptions.NotOnCanceled
		   ).Unwrap();
		/**
		 * <summary>
		 * Map successful task values.
		 * </summary>
		 * <param name="@this">Task yielding a value.</param>
		 * <param name="map">Map for successful task value.</param>
		 * <typeparam name="T">Type of value task yields.</typeparam>
		 * <typeparam name="TResult">Map image type.</typeparam>
		 * <returns>For successful task, task yielding map image. Otherwise, an unsuccessful task.</returns>
		 */
		public static STT.Task<TResult> SelectAsync<T, TResult>(this STT.Task<T> @this, S.Func<T, TResult> map)
		where T: object
		where TResult: object
		=> @this.ContinueWith((STT.Task<T> task) => map(task.Result), STT.TaskContinuationOptions.NotOnCanceled);
		/**
		 * <summary>
		 * Map exceptions in faulted or cancelled tasks to tasks and flatten.
		 * </summary>
		 * <param name="@this">A task.</param>
		 * <param name="map">Map from aggregate exceptions to tasks.</param>
		 * <typeparam name="T">Type task yields.</typeparam>
		 * <returns>For successful tasks, the same task. Otherwise, the image for the exception resulting from the unsuccessful task.</returns>
		 */
		public static STT.Task<T> CatchAsync<T>(this STT.Task<T> @this, S.Func<S.AggregateException, STT.Task<T>> map)
		where T: object
		=> @this.ContinueWith
		   ( (STT.Task<T> task)
		     => task.IsCompletedSuccessfully
		      ? task
		      : map
		        ( task.Exception
		       ?? new S.AggregateException
		          ( Enumerable.Append
		            ( Enumerable.Empty<STT.TaskCanceledException>()
		            , new STT.TaskCanceledException(task)
		            )
		          )
		        )
		   )
		   .Unwrap();
		/**
		 * <summary>
		 * Map exceptions in faulted or cancelled tasks to tasks and flatten.
		 * </summary>
		 * <param name="@this">A task.</param>
		 * <param name="map">Map from aggregate exceptions to tasks.</param>
		 * <returns>For successful tasks, the same task. Otherwise, the image for the exception resulting from the unsuccessful task.</returns>
		 */
		public static STT.Task CatchAsync(this STT.Task @this, S.Func<S.AggregateException, STT.Task> map)
		=> @this.ContinueWith
		   ( (STT.Task task)
		     => task.IsCompletedSuccessfully
		      ? task
		      : map
		        ( task.Exception
		       ?? new S.AggregateException
		          ( Enumerable.Append
		            ( Enumerable.Empty<STT.TaskCanceledException>()
		            , new STT.TaskCanceledException(task)
		            )
		          )
		        )
		   )
		   .Unwrap();
		/**
		 * <summary>
		 * Map exceptions in unsuccessful tasks.
		 * </summary>
		 * <param name="@this">Task.</param>
		 * <param name="map">Exception map.</param>
		 * <typeparam name="T">Type of value task yields.</typeparam>
		 * <returns>For successful task, no change. Otherwise, task faulting with mapped exception.</returns>
		 */
		public static STT.Task<T> SelectErrorAsync<T>(this STT.Task<T> @this, S.Func<S.AggregateException, S.Exception> map)
		where T: object
		=> @this.CatchAsync(error => STT.Task.FromException<T>(map(error)));
		/**
		 * <summary>
		 * Map exceptions in unsuccessful tasks.
		 * </summary>
		 * <param name="@this">Task.</param>
		 * <param name="map">Exception map.</param>
		 * <returns>For successful task, no change. Otherwise, task faulting with mapped exception.</returns>
		 */
		public static STT.Task SelectErrorAsync(this STT.Task @this, S.Func<S.AggregateException, S.Exception> map)
		=> @this.CatchAsync(error => STT.Task.FromException(map(error)));
		/**
		 * <summary>
		 * Filter task by yielded value, mapping rejected tasks to faulted tasks.
		 * </summary>
		 * <param name="@this">Task to filter.</param>
		 * <param name="predicate">Function returning true for accepted task values.</param>
		 * <param name="onError">Map from rejected value to aggregate exception.</param>
		 * <typeparam name="T">Type of value task yields.</typeparam>
		 * <returns>Filtered task.</returns>
		 */
		public static STT.Task<T> WhereAsync<T>(this STT.Task<T> @this, S.Func<T, bool> predicate, S.Func<T, S.AggregateException> onError)
		where T: object
		=> @this.SelectManyAsync
		   ( result
		     => predicate(result)
		      ? @this
		      : STT.Task.FromException<T>(onError(result))
		   );
	}
}
