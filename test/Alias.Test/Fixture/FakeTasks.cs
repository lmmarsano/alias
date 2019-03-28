using STT = System.Threading.Tasks;

namespace Alias.Test.Fixture {
	public static class FakeTasks {
		public static STT.Task<ExitCode> ExitSuccess { get; } = STT.Task.FromResult(ExitCode.Success);
		public static STT.Task<ExitCode> ExitError { get; } = STT.Task.FromResult(ExitCode.Error);
	}
}