#nullable enable
using System.Collections;
using S = System;
using SCG = System.Collections.Generic;

namespace Functional {
	/**
	 * <summary>
	 * Represents a possible value or error.
	 * </summary>
	 */
#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode() Object.Equals(object o)
	public abstract class Result<T>: SCG.IEnumerable<T>, S.IEquatable<Result<T>> where T: object {
#pragma warning restore CS0660, CS0661
		public static implicit operator Result<T>(T value) => new Ok<T>(value);
		public static implicit operator Result<T>(S.Exception exception) => new Error<T>(exception);
		public static bool operator ==(Result<T> a, Result<T> b) => object.Equals(a, b);
		public static bool operator !=(Result<T> a, Result<T> b) => !(a == b);
		/**
		 * <summary>
		 * Project the possible value.
		 * </summary>
		 * <param name="map">The map to apply to the possible value.</param>
		 * <typeparam name="TResult">The map’s image type.</typeparam>
		 * <returns>A <see cref="Result{TResult}"/> whose possible element is any input element’s map projection.</returns>
		 */
		public abstract Result<TResult> Select<TResult>(S.Func<T, TResult> map);
		/**
		 * <summary>
		 * Project and flatten the possible value.
		 * </summary>
		 * <param name="map">The map to apply to the possible value.</param>
		 * <typeparam name="TResult">The map’s possible image type.</typeparam>
		 * <returns>A <see cref="Result{TResult}"/> whose value is any input element’s map projection.</returns>
		 */
		public abstract Result<TResult> SelectMany<TResult>(S.Func<T, Result<TResult>> map);
		/**
		 * <summary>
		 * Extract the possible value or a default alternative.
		 * </summary>
		 * <param name="alternative">Fallback for nothing.</param>
		 * <returns>The possible element or <paramref name="alternative"/> fallback.</returns>
		 */
		public abstract T Reduce(T alternative);
		/**
		 * <summary>
		 * Lazily extract the possible value or error value alternatives.
		 * </summary>
		 * <param name="alternative">Function that maps error values to alternatives.</param>
		 * <returns>The possible element or fallback mapped from error.</returns>
		 */
		public abstract T Reduce(S.Func<S.Exception, T> alternative);
		public abstract Result<TResult> OfType<TResult>() where TResult: object;
		public abstract SCG.IEnumerator<T> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public abstract bool Equals(Result<T> other);
	}
	/**
	 * <summary>
	 * The successful value.
	 * </summary>
	 */
	public sealed class Ok<T>: Result<T>, S.IEquatable<Ok<T>> {
		public static implicit operator Ok<T>(T value) => new Ok<T>(value);
		public static implicit operator T(Ok<T> value) => value.Value;
		public static bool operator ==(Ok<T> a, Ok<T> b)
		=> object.ReferenceEquals(a, b)
		|| a.Equals(b);
		public static bool operator !=(Ok<T> a, Ok<T> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The successful value.</value>
		 */
		public T Value { get; }
		public Ok(T value) {
			Value = value;
		}
		public override Result<TResult> Select<TResult>(S.Func<T, TResult> map) => map(Value);
		public override Result<TResult> SelectMany<TResult>(S.Func<T, Result<TResult>> map) => map(Value);
		public override T Reduce(T alternative) => Value;
		public override T Reduce(S.Func<S.Exception, T> alternative) => Value;
		public bool Equals(Ok<T> other) => Value!.Equals(other.Value);
		public override bool Equals(Result<T> other) => Equals((object)other);
		public override bool Equals(object obj)
		=> obj is Ok<T> ok
		&& Equals(ok);
		public override int GetHashCode() => Value!.GetHashCode();
		public override SCG.IEnumerator<T> GetEnumerator() {
			yield return Value;
		}
		public override Result<TResult> OfType<TResult>()
		=> Value is TResult result
		 ? (Result<TResult>)result
		 : new S.InvalidCastException($"Unable to cast from {typeof(T)} to {typeof(TResult)}");
		public override string ToString() => $"Ok<{typeof(T)}>({Value})";
	}
	/**
	 * <summary>
	 * The error.
	 * </summary>
	 */
	public sealed class Error<T>: Result<T>, S.IEquatable<Error<T>> {
		public static implicit operator Error<T>(S.Exception value) => new Error<T>(value);
		public static implicit operator S.Exception(Error<T> value) => value.Value;
		public static bool operator ==(Error<T> a, Error<T> b)
		=> object.ReferenceEquals(a, b)
		|| a.Equals(b);
		public static bool operator !=(Error<T> a, Error<T> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The error.</value>
		 */
		public S.Exception Value { get; }
		public Error(S.Exception value) {
			Value = value;
		}
		public override Result<TResult> Select<TResult>(S.Func<T, TResult> map) => Value;
		public override Result<TResult> SelectMany<TResult>(S.Func<T, Result<TResult>> map) => Value;
		public override T Reduce(T alternative) => alternative;
		public override T Reduce(S.Func<S.Exception, T> alternative) => alternative(Value);
		public bool Equals(Error<T> other) => Value.Equals(other.Value);
		public override bool Equals(Result<T> other) => Equals((object)other);
		public override bool Equals(object obj)
		=> obj is Error<T> error
		&& Equals(error);
		public override int GetHashCode() => Value.GetHashCode();
		public override SCG.IEnumerator<T> GetEnumerator() {
			yield break;
		}
		public override Result<TResult> OfType<TResult>() => Value;
		public override string ToString() => $"Error<{typeof(T)}>({Value})";
	}
}