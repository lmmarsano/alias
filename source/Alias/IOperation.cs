using STT = System.Threading.Tasks;
using AO = Alias.Option;
using ST = LMMarsano.SumType;

namespace Alias {
	/**
	 * <summary>
	 * Operations associated with each <see cref='AO.AbstractOption'/> implementation.
	 * </summary>
	 */
	interface IOperation {
		/**
		 * <summary>
		 * Run the external command with appended arguments.
		 * </summary>
		 * <param name="options">External command.</param>
		 * <returns>Possible task yielding external command’s exit code.</returns>
		 */
		ST.Result<STT.Task<ExitCode>> External(AO.External external);
		/**
		 * <summary>
		 * Set an alias and save configuration.
		 * </summary>
		 * <param name="options">Set options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 */
		ST.Result<STT.Task<ExitCode>> Set(AO.Set options);
		/**
		 * <summary>
		 * Delete configuration.
		 * </summary>
		 * <param name="options">Reset options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 */
		ST.Result<STT.Task<ExitCode>> Reset(AO.Reset options);
		/**
		 * <summary>
		 * Remove an alias and save configuration.
		 * </summary>
		 * <param name="options">Unset options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 */
		ST.Result<STT.Task<ExitCode>> Unset(AO.Unset options);
		/**
		 * <summary>
		 * No operation. Derived classes may extend functionality.
		 * </summary>
		 * <param name="options">Restore options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 */
		ST.Result<STT.Task<ExitCode>> Restore(AO.Restore options);
		/**
		 * <summary>
		 * Print alias assignments as specified by options to <see cref='Environment.StreamOut'/>.
		 * </summary>
		 * <param name="options">List specifications.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 */
		ST.Result<STT.Task<ExitCode>> List(AO.List options);
	}
}
