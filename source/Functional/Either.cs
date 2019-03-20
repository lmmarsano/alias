#nullable enable
using System.Collections;
using S = System;
using SCG = System.Collections.Generic;

namespace Functional {
	/**
	 * <summary>
	 * Represents values with 2 possibilities.
	 * </summary>
	 * <remarks>Modeled after <a href='http://hackage.haskell.org/package/base-4.12.0.0/docs/Prelude.html#t:Either'>Haskell data type</a>.</remarks>
	 */
#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode() Object.Equals(object o)
	public abstract class Either<TLeft, TRight>: SCG.IEnumerable<TRight>, S.IEquatable<Either<TLeft, TRight>>
		where TLeft: object
		where TRight: object {
#pragma warning restore CS0660, CS0661
		public static implicit operator Either<TLeft, TRight>(TLeft left) => new Left<TLeft, TRight>(left);
		public static implicit operator Either<TLeft, TRight>(TRight right) => new Right<TLeft, TRight>(right);
		public static bool operator ==(Either<TLeft, TRight> a, Either<TLeft, TRight> b) => object.Equals(a, b);
		public static bool operator !=(Either<TLeft, TRight> a, Either<TLeft, TRight> b) => !(a == b);
		/**
		 * <summary>
		 * Convert to transposed type.
		 * </summary>
		 * <value>Current value with left and right types transposed.</value>
		 */
		public abstract Either<TRight, TLeft> Swap { get; }
		/**
		 * <summary>
		 * Map the left possibility.
		 * </summary>
		 * <param name="map">The map to apply to the left possibility.</param>
		 * <typeparam name="TLeftResult">Type to map the left possible value.</typeparam>
		 * <returns>A <see cref="Either{TLeftResult, TRight}"/> from projecting the current left element if any.</returns>
		 */
		public abstract Either<TLeftResult, TRight> SelectLeft<TLeftResult>(S.Func<TLeft, TLeftResult> map) where TLeftResult: object;
		/**
		 * <summary>
		 * Map and flatten the left possibility.
		 * </summary>
		 * <param name="map">The map to apply to the left possibility.</param>
		 * <typeparam name="TLeftResult">Type to map the left possible value.</typeparam>
		 * <returns>A <see cref="Either{TLeftResult, TRight}"/> projected from the current left element if any.</returns>
		 */
		public abstract Either<TLeftResult, TRight> Catch<TLeftResult>(S.Func<TLeft, Either<TLeftResult, TRight>> map) where TLeftResult: object;
		/**
		 * <summary>
		 * Map the right possibility.
		 * </summary>
		 * <param name="map">The map to apply to the right possibility.</param>
		 * <typeparam name="TRightResult">Type to map the right possible value.</typeparam>
		 * <returns>A <see cref="Either{TLeft, TRightResult}"/> from projecting the current right element if any.</returns>
		 */
		public abstract Either<TLeft, TRightResult> Select<TRightResult>(S.Func<TRight, TRightResult> map) where TRightResult: object;
		/**
		 * <summary>
		 * Map and flatten the right possibility.
		 * </summary>
		 * <param name="map">The map to apply to the right possibility.</param>
		 * <typeparam name="TRightResult">Type to map the right possible value.</typeparam>
		 * <returns>A <see cref="Either{TLeft, TRightResult}"/> projected from the current right element if any.</returns>
		 */
		public abstract Either<TLeft, TRightResult> SelectMany<TRightResult>(S.Func<TRight, Either<TLeft, TRightResult>> map) where TRightResult: object;
		/**
		 * <summary>
		 * Filters an <see cref='Either{TLeft, TRight}'/> of values based on a predicate.
		 * </summary>
		 * <param name="predicate">A function to test each element for a condition.</param>
		 * <param name="onError">An error map for values that fail <paramref name="predicate"/>.</param>
		 * <returns>An <see cref='Either{TLeft, TRight}'/> that contains elements from the input sequence that satisfy the condition.</returns>
		 */
		public abstract Either<TLeft, TRight> Where(S.Func<TRight, bool> predicate, S.Func<TRight, TLeft> onError);
		/**
		 * <summary>
		 * Sequential composition: replace current value, if any, with <paramref name="next"/>.
		 * </summary>
		 * <param name="next">The next value.</param>
		 * <typeparam name="TRightResult">Next internal type.</typeparam>
		 * <returns>Next value or <see cref='Either{TLeft, TRightResult}'/>.</returns>
		 */
		public abstract Either<TLeft, TRightResult> Combine<TRightResult>(Either<TLeft, TRightResult> next) where TRightResult: object;
		/**
		 * <summary>
		 * Extract the left possible value or a default alternative.
		 * </summary>
		 * <param name="alternative">Alternative for right value.</param>
		 * <returns>Left element or fallback.</returns>
		 */
		public abstract TLeft ReduceLeft(TLeft alternative);
		/**
		 * <summary>
		 * Lazily extract the left possible value or right value alternatives.
		 * </summary>
		 * <param name="alternative">Function that maps right values to alternatives.</param>
		 * <returns>Left element or fallback mapped from right.</returns>
		 */
		public abstract TLeft ReduceLeft(S.Func<TRight, TLeft> alternative);
		/**
		 * <summary>
		 * Extract the right possible value or a default alternative.
		 * </summary>
		 * <param name="alternative">Alternative for right value.</param>
		 * <returns>Right element or fallback.</returns>
		 */
		public abstract TRight ReduceRight(TRight alternative);
		/**
		 * <summary>
		 * Lazily extract the right possible value or left value alternatives.
		 * </summary>
		 * <param name="alternative">Function that maps left values to alternatives.</param>
		 * <returns>Right element or fallback mapped from left.</returns>
		 */
		public abstract TRight ReduceRight(S.Func<TLeft, TRight> alternative);
		/**
		 * <summary>
		 * Filter values of <see cref="Either{TLeft, TResult}"/> based on a specified type.
		 * </summary>
		 * <param name="onError">Maps Right type to Left type when Right is filtered out.</param>
		 * <typeparam name="TResult">Type to filter on.</typeparam>
		 * <returns>A <see cref="Either{TLeft, TResult}"/> that contains the values from input of type <typeparamref name="TResult"/>.</returns>
		 */
		public abstract Either<TLeft, TResult> OfType<TResult>(S.Func<TRight, TLeft> onError) where TResult: object;
		public abstract SCG.IEnumerator<TRight> GetEnumerator();
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
		public abstract bool Equals(Either<TLeft, TRight> other);
	}
	/**
	 * <summary>
	 * The left possibility.
	 * </summary>
	 */
	public sealed class Left<TLeft, TRight>: Either<TLeft, TRight>, S.IEquatable<Left<TLeft, TRight>>
		where TLeft: object
		where TRight: object {
		public static implicit operator Left<TLeft, TRight>(TLeft left) => new Left<TLeft, TRight>(left);
		public static implicit operator TLeft(Left<TLeft, TRight> left) => left.Value;
		public static bool operator ==(Left<TLeft, TRight> a, Left<TLeft, TRight> b)
		=> object.ReferenceEquals(a, b)
		&& a.Equals(b);
		public static bool operator !=(Left<TLeft, TRight> a, Left<TLeft, TRight> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The left possiblity’s value.</value>
		 */
		public TLeft Value { get; }
		public Left(TLeft value) {
			Value = value;
		}
		public override Either<TRight, TLeft> Swap => Value;
		public override Either<TLeftResult, TRight> SelectLeft<TLeftResult>(S.Func<TLeft, TLeftResult> map) => map(Value);
		public override Either<TLeftResult, TRight> Catch<TLeftResult>(S.Func<TLeft, Either<TLeftResult, TRight>> map) => map(Value);
		/// <inheritdoc/>
		public override Either<TLeft, TRightResult> Select<TRightResult>(S.Func<TRight, TRightResult> map) => Value;
		/// <inheritdoc/>
		public override Either<TLeft, TRightResult> SelectMany<TRightResult>(S.Func<TRight, Either<TLeft, TRightResult>> map) => Value;
		/// <inheritdoc/>
		public override Either<TLeft, TRight> Where(S.Func<TRight, bool> predicate, S.Func<TRight, TLeft> onError)
		=> this;
		/// <inheritdoc/>
		public override Either<TLeft, TRightResult> Combine<TRightResult>(Either<TLeft, TRightResult> next) => Value;
		public override TLeft ReduceLeft(TLeft alternative) => Value;
		public override TLeft ReduceLeft(S.Func<TRight, TLeft> alternative) => Value;
		public override TRight ReduceRight(TRight alternative) => alternative;
		public override TRight ReduceRight(S.Func<TLeft, TRight> alternative) => alternative(Value);
		public override Either<TLeft, TResult> OfType<TResult>(S.Func<TRight, TLeft> onError) => Value;
		public bool Equals(Left<TLeft, TRight> other) => Value!.Equals(other.Value);
		public override bool Equals(Either<TLeft, TRight> other) => Equals((object)other);
		public override bool Equals(object obj)
		=> obj is Left<TLeft, TRight> left
		&& Equals(left);
		public override int GetHashCode()
		=> Value is null
		 ? 0
		 : Value.GetHashCode();
		public override SCG.IEnumerator<TRight> GetEnumerator() {
			yield break;
		}
		public override string ToString() => $"Left<{typeof(TLeft)}, {typeof(TRight)}>({Value})";
	}
	/**
	 * <summary>
	 * The right possibility.
	 * </summary>
	 */
	public sealed class Right<TLeft, TRight>: Either<TLeft, TRight>, S.IEquatable<Right<TLeft, TRight>>
		where TLeft: object
		where TRight: object {
		public static implicit operator Right<TLeft, TRight>(TRight right) => new Right<TLeft, TRight>(right);
		public static implicit operator TRight(Right<TLeft, TRight> right) => right.Value;
		public static bool operator ==(Right<TLeft, TRight> a, Right<TLeft, TRight> b)
		=> object.ReferenceEquals(a, b)
		&& a.Equals(b);
		public static bool operator !=(Right<TLeft, TRight> a, Right<TLeft, TRight> b) => !(a == b);
		/**
		 * <summary>
		 * Embedded value.
		 * </summary>
		 * <value>The right possiblity’s value.</value>
		 */
		public TRight Value { get; }
		public Right(TRight value) {
			Value = value;
		}
		public override Either<TRight, TLeft> Swap => Value;
		public override Either<TLeftResult, TRight> SelectLeft<TLeftResult>(S.Func<TLeft, TLeftResult> map) => Value;
		public override Either<TLeftResult, TRight> Catch<TLeftResult>(S.Func<TLeft, Either<TLeftResult, TRight>> map) => Value;
		public override Either<TLeft, TRightResult> Select<TRightResult>(S.Func<TRight, TRightResult> map) => map(Value);
		public override Either<TLeft, TRightResult> SelectMany<TRightResult>(S.Func<TRight, Either<TLeft, TRightResult>> map) => map(Value);
		/// <inheritdoc/>
		public override Either<TLeft, TRight> Where(S.Func<TRight, bool> predicate, S.Func<TRight, TLeft> onError)
		=> predicate(Value)
		 ? this
		 : Factory.Either<TLeft, TRight>(onError(Value));
		/// <inheritdoc/>
		public override Either<TLeft, TRightResult> Combine<TRightResult>(Either<TLeft, TRightResult> next) => next;
		public override TLeft ReduceLeft(TLeft alternative) => alternative;
		public override TLeft ReduceLeft(S.Func<TRight, TLeft> alternative) => alternative(Value);
		public override TRight ReduceRight(TRight alternative) => Value;
		public override TRight ReduceRight(S.Func<TLeft, TRight> alternative) => Value;
		public override Either<TLeft, TResult> OfType<TResult>(S.Func<TRight, TLeft> onError)
		=> Value is TResult right
		 ? (Either<TLeft, TResult>)right
		 : onError(Value);
		public bool Equals(Right<TLeft, TRight> other) => Value!.Equals(other.Value);
		public override bool Equals(Either<TLeft, TRight> other) => Equals((object)other);
		public override bool Equals(object obj)
		=> obj is Right<TLeft, TRight> right
		&& Equals(right);
		public override int GetHashCode() => Value!.GetHashCode();
		public override SCG.IEnumerator<TRight> GetEnumerator() {
			yield return Value;
		}
		public override string ToString() => $"Right<{typeof(TLeft)}, {typeof(TRight)}>({Value})";
	}
}