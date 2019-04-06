using STT = System.Threading.Tasks;
using F = Functional;

namespace Alias.Option {
	/**
	 * <summary>
	 * Options whose operations result in task yielding an exit code.
	 * </summary>
	 */
	class Exit: AbstractOption {
		/**
		 * <summary>
		 * Exit code operations with options yields.
		 * </summary>
		 */
		public ExitCode ExitCode { get; }
		/**
		 * <summary>
		 * Create options whose operations yield a supplied exit code.
		 * </summary>
		 * <param name="exitCode">Exit code for operations to yield.</param>
		 */
		public Exit(ExitCode exitCode) {
			ExitCode = exitCode;
		}
		/// <inheritdoc/>
		public override F.Result<STT.Task<ExitCode>> Operate(IOperation _)
		=> F.Factory.Result(STT.Task.FromResult(ExitCode));
	}
}
