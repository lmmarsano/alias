using S = System;

namespace Functional {
	public static class Factory {
		public static Maybe<T> Maybe<T>(Nothing _) => _;
		public static Maybe<T> Maybe<T>(T value) => value;
		public static Result<T> Result<T>(S.Exception exception) => exception;
		public static Result<T> Result<T>(T value) => value;
		public static Either<TLeft, TRight> Either<TLeft, TRight>(TLeft value) => value;
		public static Either<TLeft, TRight> Either<TLeft, TRight>(TRight value) => value;
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
		public static Result<T> Try<T>(S.Func<T> function) {
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
		public static Result<T> Try<T>(S.Func<T> function, S.Func<S.Exception, S.Exception> errorMap) {
			try {
				return function();
			} catch (S.Exception error) {
				return errorMap(error);
			}
		}
	}
}