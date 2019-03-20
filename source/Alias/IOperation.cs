using AO = Alias.Option;
using F = Functional;
using Functional;

namespace Alias {
	/**
	 * <summary>
	 * Operations associated with each <see cref='AO.AbstractOption'/> implementation.
	 * </summary>
	 */
	interface IOperation {
		F.Result<ExitCode> External(AO.External external);
		F.Result<ExitCode> Set(AO.Set options);
		F.Result<ExitCode> Reset(AO.Reset options);
		F.Result<ExitCode> Unset(AO.Unset options);
		F.Result<ExitCode> Restore(AO.Restore options);
		F.Result<ExitCode> List(AO.List options);
	}
}