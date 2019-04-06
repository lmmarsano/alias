using STT = System.Threading.Tasks;
using SDC = System.Diagnostics.CodeAnalysis;
using F = Functional;

namespace Alias.Option {
	[ SDC.SuppressMessage("Compiler", "CS0660", Justification = "Need an equality relation.")
	, SDC.SuppressMessage("Compiler", "CS0661", Justification = "Need an equality relation.")
	]
	abstract class AbstractOption {
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
