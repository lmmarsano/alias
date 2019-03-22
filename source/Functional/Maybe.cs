#nullable enable
using System.Collections;
using S = System;
using SCG = System.Collections.Generic;

namespace Functional {
	/**
	 * <summary>
	 * Represents optional values.
	 * </summary>
	 * <typeparam name="T">The optional value’s type.</typeparam>
	 * <remarks>Modeled after <a href='http://hackage.haskell.org/package/base-4.12.0.0/docs/Prelude.html#t:Maybe'>Haskell data type</a>.</remarks>
	 */
#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode() Object.Equals(object o)
	public abstract class Maybe<T>: S.IEquatable<Maybe<T>>, SCG.IEnumerable<T> where T: object {
#pragma warning restore CS0661, CS0660
		public static implicit operator Maybe<T>(T value) => new Just<T>(value);
		public static implicit operator Maybe<T>(Nothing _) => Nothing<T>.Value;
		public static bool operator ==(Maybe<T> a, Maybe<T> b) => object.Equals(a, b);
		public static bool operator !=(Maybe<T> a, Maybe<T> b) => !(a == b);
		/**
		 * <summary>
		 * Project the optional element.
		 * </summary>
		 * <param name="selector">A transform function to apply to each element.</param>
		 * <typeparam name="TResult">Type returned by <paramref name="selector"/>.</typeparam>
		 * <returns>A <see cref="Maybe{TResult}"/> whose optional element is any input element’s transform image.</returns>
		 */
		public abstract Maybe<TResult> Select<TResult>(S.Func<T, TResult> selector) where TResult: object;
		/**
		 * <summary>
		 * Project and flatten the optional element.
		 * </summary>
		 * <param name="selector">A transform function to apply to the optional value.</param>
		 * <typeparam name="TResult">Type of the optional element returned by <paramref name="selector"/>.</typeparam>
		 * <returns>A <see cref="Maybe{TResult}"/> whose optional element is any returned from the transform function.</returns>
		 */
		public abstract Maybe<TResult> SelectMany<TResult>(S.Func<T, Maybe<TResult>> selector) where TResult: object;
		/**
		 * <summary>
		 * Filters a <see cref='Maybe{T}'/> of values based on a predicate.
		 * </summary>
		 * <param name="predicate">A function to test each element for a condition.</param>
		 * <returns>A <see cref='Maybe{T}'/> that contains elements from the input sequence that satisfy the condition.</returns>
		 */
		public abstract Maybe<T> Where(S.Func<T, bool> predicate);
		/**
		 * <summary>
		 * Sequential composition: replace current value, if any, with <paramref name="next"/>.
		 * </summary>
		 * <param name="next">The next value.</param>
		 * <typeparam name="TResult">Next internal type.</typeparam>
		 * <returns>Next value or <see cref='Nothing{TResult}'/>.</returns>
		 */
		public abstract Maybe<TResult> Combine<TResult>(Maybe<TResult> next) where TResult: object;
		/**
		 * <summary>
		 * Extract the optional value or a default alternative.
		 * </summary>
		 * <param name="alternative">Fallback for nothing.</param>
		 * <returns>The optional element or <paramref name="alternative"/> fallback.</returns>
		 */
		public abstract T Reduce(T alternative);
		/**
		 * <summary>
		 * Lazily extract the optional value or a default alternative.
		 * </summary>
		 * <param name="alternative">Function whose image provides a fallback to nothing.</param>
		 * <returns>The optional element or <paramref name="alternative"/>’s image.</returns>
		 */
		public abstract T Reduce(S.Func<T> alternative);
		/**
		 * <summary>
		 * Map the optional value by case.
		 * </summary>
		 * <param name="alternative">Image for nothing.</param>
		 * <param name="map">Map for <see cref='Just{T}'/> values.</param>
		 * <returns>The <paramref name="map"/> image or <paramref name="alternative"/>.</returns>
		 */
		public abstract TResult Reduce<TResult>(TResult alternative, S.Func<T, TResult> map);
		/**
		 * <summary>
		 * Lazily map the optional value by case.
		 * </summary>
		 * <param name="alternative">Function producing image for nothing.</param>
		 * <param name="map">Map for <see cref='Just{T}'/> values.</param>
		 * <returns>The <paramref name="map"/> image or <paramref name="alternative"/> image.</returns>
		 */
		public abstract TResult Reduce<TResult>(S.Func<TResult> alternative, S.Func<T, TResult> map);
		public abstract SCG.IEnumerator<T> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		/**
		 * <summary>
		 * Filter values of <see cref="Maybe{TResult}"/> based on a specified type.
		 * </summary>
		 * <typeparam name="TResult">Type to filter on.</typeparam>
		 * <returns>A <see cref="Maybe{TResult}"/> that contains the values from input of type <typeparamref name="TResult"/>.</returns>
		 */
		public abstract Maybe<TResult> OfType<TResult>() where TResult: object;
		/**
		 * <summary>
		 * Equality relation.
		 * </summary>
		 * <param name="other">Operand.</param>
		 * <returns><see cref="true"/> if and only if operand equals current object.</returns>
		 */
		public abstract bool Equals(Maybe<T> other);
	}
	/**
	 * <summary>
	 * A present value.
	 * </summary>
	 */
	public sealed class Just<T>: Maybe<T>, S.IEquatable<Just<T>> where T: object {
		public static implicit operator Just<T>(T value) => new Just<T>(value);
		public static implicit operator T(Just<T> just) => just.Value;
		public static bool operator ==(Just<T> a, Just<T> b)
		=> object.ReferenceEquals(a, b)
		|| a.Equals(b);
		public static bool operator !=(Just<T> a, Just<T> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The optional value.</value>
		 */
		public T Value { get; }
		public Just(T value) {
			Value = value;
		}
		public override SCG.IEnumerator<T> GetEnumerator() {
			yield return Value;
		}
		/// <inheritdoc/>
		public override Maybe<TResult> Select<TResult>(S.Func<T, TResult> map) => map(Value);
		/// <inheritdoc/>
		public override Maybe<TResult> SelectMany<TResult>(S.Func<T, Maybe<TResult>> map) => map(Value);
		/// <inheritdoc/>
		public override Maybe<T> Where(S.Func<T, bool> predicate)
		=> predicate(Value)
		 ? (Maybe<T>)this
		 : Nothing.Value;
		/// <inheritdoc/>
		public override Maybe<TResult> Combine<TResult>(Maybe<TResult> next) => next;
		public override T Reduce(T alternative) => Value;
		public override T Reduce(S.Func<T> alternative) => Value;
		public override TResult Reduce<TResult>(TResult alternative, S.Func<T, TResult> map) => map(Value);
		public override TResult Reduce<TResult>(S.Func<TResult> alternative, S.Func<T, TResult> map) => map(Value);
		public override Maybe<TResult> OfType<TResult>()
		=> Value is TResult result
		 ? (Maybe<TResult>)result
		 : Nothing.Value;
		// T is non-nullable
		public bool Equals(Just<T> other) => Value!.Equals(other.Value);
		public override bool Equals(Maybe<T> other) => Equals((object)other);
		public override bool Equals(object obj)
		=> obj is Just<T> just
		&& Equals(just);
		// T is non-nullable
		public override int GetHashCode() => Value!.GetHashCode();
		public override string ToString() => $"Just<{typeof(T)}>({Value})";
	}
	/**
	 * <summary>
	 * An absent value.
	 * </summary>
	 */
	public sealed class Nothing<T>: Maybe<T>, S.IEquatable<Nothing<T>> where T: object {
		/**
		 * <summary>
		 * Only instance of <see cref="Nothing{T}"/>.
		 * </summary>
		 * <value><see cref="Nothing{T}"/> instance.</value>
		 */
		public static Nothing<T> Value { get; } = new Nothing<T>();
		Nothing() {}
		public static implicit operator Nothing<T>(Nothing _) => Nothing<T>.Value;
		public static implicit operator Nothing(Nothing<T> _) => Nothing.Value;
		public override SCG.IEnumerator<T> GetEnumerator() {
			yield break;
		}
		/// <inheritdoc/>
		public override Maybe<TResult> Select<TResult>(S.Func<T, TResult> map) => Nothing.Value;
		/// <inheritdoc/>
		public override Maybe<TResult> SelectMany<TResult>(S.Func<T, Maybe<TResult>> map) => Nothing.Value;
		/// <inheritdoc/>
		public override Maybe<T> Where(S.Func<T, bool> predicate) => this;
		/// <inheritdoc/>
		public override Maybe<TResult> Combine<TResult>(Maybe<TResult> next) => Nothing.Value;
		public override T Reduce(T alternative) => alternative;
		public override T Reduce(S.Func<T> alternative) => alternative();
		public override TResult Reduce<TResult>(TResult alternative, S.Func<T, TResult> map) => alternative;
		public override TResult Reduce<TResult>(S.Func<TResult> alternative, S.Func<T, TResult> map) => alternative();
		public override Maybe<TResult> OfType<TResult>() => Nothing.Value;
		public bool Equals(Nothing<T> other) => true;
		public override bool Equals(Maybe<T> other) => Equals((object) other);
		public override bool Equals(object obj) => Nothing.Value.Equals(obj);
		public override int GetHashCode() => 0;
		public override string ToString() => $"Nothing<{typeof(T)}>";
	}
	/**
	 * <summary>
	 * A singleton of a value that implicitly converts to <c cref='Nothing{T}'>generic Nothing</c>.
	 * </summary>
	 */
	public sealed class Nothing: S.IEquatable<Nothing> {
		/**
		 * <summary>
		 * Only instance of <see cref="Nothing"/>.
		 * </summary>
		 * <value><see cref="Nothing"/> instance.</value>
		 */
		public static Nothing Value { get; } = new Nothing();
		Nothing() {}
		/**
		 * <summary>
		 * Always <see cref="true"/>.
		 * </summary>
		 * <param name="other">Operand.</param>
		 * <returns><see cref="true"/></returns>
		 */
		public bool Equals(Nothing other) => true;
		/**
		 * <summary>
		 * <see cref="Nothing"/> equality.
		 * <see cref="Nothing"/> is equal to any instance of <see cref="Nothing"/> or <see cref="Nothing{T}"/>.
		 * </summary>
		 * <param name="obj">Operand.</param>
		 * <returns><see cref="true"/> when <paramref name="obj"/> is <see cref="Nothing"/> or <see cref="Nothing{T}"/>.</returns>
		 */
		public override bool Equals(object obj)
		=> obj is Nothing
		|| obj.GetType() is S.Type { IsGenericType: true } type
		&& type.GetGenericTypeDefinition() == typeof(Nothing<>);
		public override int GetHashCode() => 0;
	}
}
