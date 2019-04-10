using STT = System.Threading.Tasks;
using SDC = System.Diagnostics.CodeAnalysis;
using F = Functional;

namespace Alias.Option {
#pragma warning disable CS0660, CS0661 //'AbstractOption' defines operator == or operator != but does not override Object.Equals(object o) Object.GetHashCode() [Alias]
	abstract class AbstractOption {
#pragma warning restore CS0660, CS0661
		public static bool operator ==(AbstractOption a, AbstractOption b) => Equals(a, b);
		public static bool operator !=(AbstractOption a, AbstractOption b) => !(a == b);
		/**
		 * <summary>
		 * Option validation.
		 * </summary>
		 * <value>Result of validation: the options or error.</value>
		 */
		public virtual F.Result<AbstractOption> Validation => F.Factory.Result(this);
		/**
		 * <summary>
		 * Execute command given by options.
		 * </summary>
		 * <param name="operation">Execution operations.</param>
		 * <returns>Result of exit code from command or error.</returns>
		 */
		public abstract F.Result<STT.Task<ExitCode>> Operate(IOperation operation);
	}
}
