using S = System;
using SCG = System.Collections.Generic;
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
		public static SCG.IEnumerable<TOut> SelectMaybe<TIn, TOut>(this SCG.IEnumerable<TIn> @this, S.Func<TIn, Maybe<TOut>> selector)
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
		public static Result<T> Try<T>(this S.Func<T> @this) => Try(@this);
		/**
		 * <summary>
		 * Evaluate to result.
		 * </summary>
		 * <param name="errorMap">Error mapper.</param>
		 * <typeparam name="T">The function’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(this S.Func<T> @this, S.Func<S.Exception, S.Exception> errorMap) => Try(@this, errorMap);
	}
}
