using S = System;

namespace Alias {
	enum ExitCode
	{ Success
	, Error
	}
	class Program {
		static int Main(string[] args) {
			/*
			read configuration (from file or default)
			if current name is configured, then run configured command with passed in arguments
			otherwise, process arguments & return validity
			- valid: dispatch command
				- set name
				- unset name
				- reset all
				- restore
			- invalid: display help & exit with error
			 */
			return (int)ExitCode.Success;
		}
	}
}
