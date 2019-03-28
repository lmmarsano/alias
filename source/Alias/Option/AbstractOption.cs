using STT = System.Threading.Tasks;
using F = Functional;

namespace Alias.Option {
#pragma warning disable CS0660, CS0661 // Type defines operator == or operator != but does not override Object.GetHashCode() Object.Equals(object o)
	abstract class AbstractOption {
#pragma warning restore CS0660, CS0661
		public static bool operator ==(AbstractOption a, AbstractOption b) => object.Equals(a, b);
		public static bool operator !=(AbstractOption a, AbstractOption b) => !(a == b);
		/**
		 * <summary>
		 * Option validation.
		 * </summary>
		 * <value>Result of validation: the options or error.</value>
		 */
		public virtual F.Result<AbstractOption> Validation => F.Factory.Result<AbstractOption>(this);
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