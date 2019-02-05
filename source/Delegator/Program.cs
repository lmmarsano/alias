using S = System;

namespace Delegator {
	enum ExitCode
	{ Success = 0
	, Error
	}
	class Program {
		static ExitCode Main(string[] args) {
			return ExitCode.Success;
		}
	}
}
