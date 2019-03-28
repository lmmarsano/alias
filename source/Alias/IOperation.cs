using STT = System.Threading.Tasks;
using AO = Alias.Option;
using F = Functional;

namespace Alias {
	/**
	 * <summary>
	 * Operations associated with each <see cref='AO.AbstractOption'/> implementation.
	 * </summary>
	 */
	interface IOperation {
		F.Result<STT.Task<ExitCode>> External(AO.External external);
		F.Result<STT.Task<ExitCode>> Set(AO.Set options);
		F.Result<STT.Task<ExitCode>> Reset(AO.Reset options);
		F.Result<STT.Task<ExitCode>> Unset(AO.Unset options);
		F.Result<STT.Task<ExitCode>> Restore(AO.Restore options);
		F.Result<STT.Task<ExitCode>> List(AO.List options);
	}
}