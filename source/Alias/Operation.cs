using S = System;
using AO = Alias.Option;
using AC = Alias.Configuration;
using System.Linq;
using F = Functional;
using Functional;

namespace Alias {
	/**
	 * <summary>
	 * Operations associated with each <see cref='Option.AbstractOption'/> implementation.
	 * </summary>
	 */
	class Operation : IOperation {
		public IEffect Effect { get; }
		public IEnvironment Environment { get; }
		public AC.Configuration Configuration { get; }
		public Operation(IEnvironment environment, AC.Configuration configuration) {
			Effect = environment.Effect;
			Environment = environment;
			Configuration = configuration;
		}
		public F.Result<ExitCode> Reset(AO.Reset options) {
			throw new S.NotImplementedException();
		}

		public F.Result<ExitCode> Restore(AO.Restore options) {
			throw new S.NotImplementedException();
		}

		public F.Result<ExitCode> Set(AO.Set options) {
			throw new S.NotImplementedException();
			if (!Configuration.Binding.ContainsKey(options.Name)) {
				// create alias file
			}
			Configuration.Binding[options.Name] = new AC.CommandEntry(options.Name, options.Arguments.FirstOrDefault());
		}

		public F.Result<ExitCode> Unset(AO.Unset options) {
			throw new S.NotImplementedException();
		}
		/**
		 * <summary>
		 * Print alias assignments as specified by options to <see cref='Environment.StreamOut'/>.
		 * </summary>
		 * <param name="options">List specifications.</param>
		 * <returns>Successful result.</returns>
		 * <exception cref='ListOperationException'>List command fails.</exception>
		 */
		public F.Result<ExitCode> List(AO.List options)
		=> F.Factory.Try(() => {
				foreach (var kvp in Configuration.Binding) {
					var command = Utility.SafeQuote(kvp.Value.Command);
					if (!string.IsNullOrWhiteSpace(kvp.Value.Arguments)) {
						command += @" " + kvp.Value.Arguments;
					}
					Environment.StreamOut.WriteLine($"{Utility.SafeQuote(kvp.Key)} → {command}");
				}
			})
			.SelectError(ListOperationException.GetRunFailureMap(options))
			.Combine<ExitCode>(ExitCode.Success);
		/**
		 * <summary>
		 * Run the external command with appended arguments.
		 * </summary>
		 * <param name="options">External command.</param>
		 * <returns>Result of the external command’s exit code or error.</returns>
		 * <exception cref='ExternalOperationException'>External command fails to run.</exception>
		 */
		public F.Result<ExitCode> External(AO.External options) {
			var arguments = options.Arguments.Concat(Environment.Arguments.Select(Utility.SafeQuote));
			// explicit ProcessStartInfo.WorkingDirectory required https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.workingdirectory#remarks
			if (arguments.Any()) {
				var argumentLine = string.Join(' ', arguments);
				return Effect.RunCommand(Environment.WorkingDirectory, options.Command, argumentLine)
				       .SelectError(ExternalOperationException.GetRunFailureMap(options, F.Factory.Maybe(argumentLine)));
			} else {
				return Effect.RunCommand(Environment.WorkingDirectory, options.Command)
				       .SelectError(ExternalOperationException.GetRunFailureMap(options, F.Nothing.Value));
			}
		}
	}
}