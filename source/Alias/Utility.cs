using S = System;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using STRE = System.Text.RegularExpressions;
using F = Functional;
using static Functional.Extension;
using System.Linq;

namespace Alias {
	static class Utility {
		/**
		 * <summary>
		 * Task yielding <see cref='ExitCode.Success'/>.
		 * </summary>
		 */
		public static STT.Task<ExitCode> TaskExitSuccess { get; } = STT.Task.FromResult(ExitCode.Success);
		/**
		 * <summary>
		 * Set of filenames to reject.
		 * </summary>
		 */
		static readonly SCG.ISet<string> IllegalFileNames = new SCG.HashSet<string>(2)
		{ @"."
		, @".."
		};
		/**
		 * <summary>
		 * Regular expression matching any number of \ preceding ".
		 * </summary>
		 */
		static readonly STRE.Regex QuoteRegEx = new STRE.Regex(@"(\\*)""");
		/**
		 * <summary>
		 * Regular expression matching repeating \ at the end.
		 * </summary>
		 * <returns></returns>
		 */
		static readonly STRE.Regex EndBackslashRegEx = new STRE.Regex(@"(\\+)$");
		/**
		 * <summary>
		 * Validate path.
		 * </summary>
		 * <param name="path">Path to validate.</param>
		 * <param name="errorMap">Error mapper.</param>
		 * <returns><see cref='Result{string}'/> of validation: full path or error.</returns>
		 * <exception cref='S.ArgumentException'><list type='bullet'>
		 * 	<item><description><paramref name="path"/> is a zero-length string, contains only white space, or contains one or more of the invalid characters defined in <see cref='SIO.Path.GetInvalidPathChars'/>.</description></item>
		 * 	<item><description>The system could not retrieve the absolute path.</description></item>
		 * </list></exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permissions.</exception>
		 * <exception cref='S.ArgumentNullException'><paramref name="path"/> is null.</exception>
		 * <exception cref='S.NotSupportedException'><paramref name="path"/> contains an unsupported specifier.</exception>
		 * <exception cref='SIO.PathTooLongException'>The specified path, file name, or both exceed the system-defined maximum length.</exception>
		 */
		public static F.Result<string> ValidatePath(string path)
		=> F.Factory.Try(() => SIO.Path.GetFullPath(path));
		/**
		 * <summary>
		 * Validate file name.
		 * </summary>
		 * <param name="fileName">File name to validate.</param>
		 * <returns><see cref='Result{string}'/> of validation: file name or error.</returns>
		 * <exception cref='S.ArgumentNullException'><paramref name="fileName"/> is null.</exception>
		 * <exception cref='S.ArgumentException'><paramref name="fileName"/> contains invalid characters or is invalid.</exception>
		 */
		public static F.Result<string> ValidateFileName(string fileName)
		=> fileName
		   .ToResult(() => new S.ArgumentNullException(nameof(fileName), @"Null file name."))
		   .Where
		   ( fileName => !(string.IsNullOrWhiteSpace(fileName) || IllegalFileNames.Contains(fileName))
		   , fileName => new S.ArgumentException(@"Invalid file name.", nameof(fileName))
		   )
		   .Combine(F.Factory.Try(() => SIO.Path.GetFileName(fileName)))
		   .Where
		   ( name => name == fileName
		   , fileName => new S.ArgumentException(@"Invalid file name.", nameof(fileName))
		   );
		/**
		 * <summary>
		 * Quote and escape raw string only if necessary.
		 * </summary>
		 * <param name="value">Raw string.</param>
		 * <returns><paramref name="value"/> quoted to contain any spaces.</returns>
		 */
		// escape \ preceding " https://docs.microsoft.com/en-us/dotnet/api/system.environment.getcommandlineargs?view=netframework-4.7.2#remarks
		public static string SafeQuote(string value) {
			var partial
			= value.Contains('"', S.StringComparison.Ordinal)
			? QuoteRegEx.Replace(value, @"$1$1\""")
			: value;
			return partial.Any(char.IsWhiteSpace)
			     ? $@"""{EndBackslashRegEx.Replace(partial, @"$1$1")}"""
			     : partial;
		}
		/**
		 * <summary>
		 * Equality predicate for dictionaries.
		 * </summary>
		 * <param name="a">A dictionary.</param>
		 * <param name="b">A dictionary.</param>
		 * <typeparam name="TKey">Key type.</typeparam>
		 * <typeparam name="TValue">Value type.</typeparam>
		 * <returns>True if and only if key-value pairs equal.</returns>
		 */
		public static bool Equals<TKey, TValue>(SCG.IDictionary<TKey, TValue> a, SCG.IDictionary<TKey, TValue> b)
		=> ReferenceEquals(a, b)
		|| a.Count.Equals(b.Count)
		&& a.All
		   (element
		    => b.TryGetValue(element.Key, out var value)
		    && Equals(element.Value, value)
		   );
		/**
		 * <summary>
		 * Compile error messages into a sequence.
		 * </summary>
		 * <param name="error">An exception.</param>
		 * <returns>Merged error messages.</returns>
		 */
		public static SCG.IEnumerable<string> GetErrorMessage(S.Exception error) {
			var stack = new SCG.Stack<S.Exception>();
			for (stack.Push(error); stack.Any(); error = stack.Pop()) {
				if (error is S.AggregateException { InnerExceptions: var aggregate }) {
					foreach (var item in aggregate.Reverse()) {
						stack.Push(item);
					}
				} else {
					yield return error.Message;
					if (error.InnerException is S.Exception inner) {
						stack.Push(inner);
					}
				}
			}
		}
	}
}
