using S = System;

namespace Functional {
	public static class Factory {
		public static Maybe<T> Maybe<T>(Nothing _) where T: object => _;
		public static Maybe<T> Maybe<T>(T value) where T: object => value;
		public static Result<T> Result<T>(S.Exception exception) where T: object => exception;
		public static Result<T> Result<T>(T value) where T: object => value;
		public static Either<TLeft, TRight> Either<TLeft, TRight>(TLeft value)
		where TLeft: object
		where TRight: object
		=> value;
		public static Either<TLeft, TRight> Either<TLeft, TRight>(TRight value)
		where TLeft: object
		where TRight: object
		=> value;
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <returns>Result of successful evaluation (<see cref="Nothing"/>) or error.</returns>
		 */
		public static Result<Nothing> Try(S.Action function) {
			try {
				function();
				return Nothing.Value;
			} catch (S.Exception error) {
				return error;
			}
		}
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <param name="errorMap">Error mapper.</param>
		 * <returns>Result of successful evaluation (<see cref="Nothing"/>) or error.</returns>
		 */
		public static Result<Nothing> Try(S.Action function, S.Func<S.Exception, S.Exception> errorMap) {
			try {
				function();
				return Nothing.Value;
			} catch (S.Exception error) {
				return errorMap(error);
			}
		}
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <typeparam name="T"><paramref name="function"/>’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(S.Func<T> function) where T: object {
			try {
				return function();
			} catch (S.Exception error) {
				return error;
			}
		}
		/**
		 * <summary>
		 * Evaluate <paramref name="function"/> to result.
		 * </summary>
		 * <param name="function">Function to evaluate.</param>
		 * <param name="errorMap">Error mapper.</param>
		 * <typeparam name="T"><paramref name="function"/>’s image type.</typeparam>
		 * <returns>Result of successful evaluation or error.</returns>
		 */
		public static Result<T> Try<T>(S.Func<T> function, S.Func<S.Exception, S.Exception> errorMap)
		where T: object {
			try {
				return function();
			} catch (S.Exception error) {
				return errorMap(error);
			}
		}
	}
}