using S = System;
using SIO = System.IO;
using STT = System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;
using System.Linq;
using F = Functional;
using static Functional.Extension;
using AO = Alias.Option;
using AC = Alias.Configuration;

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
		public F.Result<STT.Task<ExitCode>> Reset(AO.Reset options) {
			throw new S.NotImplementedException();
			// compare files (hash) before deletion
		}

		public F.Result<STT.Task<ExitCode>> Restore(AO.Restore options) {
			throw new S.NotImplementedException();
		}

		public F.Result<STT.Task<ExitCode>> Set(AO.Set options) {
			throw new S.NotImplementedException();
			Configuration.Binding[options.Name]
			= new AC.CommandEntry(options.Name, options.Arguments.FirstOrDefault());
			return Effect.WriteConfiguration(Configuration, Environment.ConfigurationFile)
			.SelectMany
			 ( serializeTask0 => {
			   	var serializeTask = serializeTask0.CombineAsync(STT.Task.FromResult(ExitCode.Success));
			   	return Effect.CopyFile
			   	 ( Environment.ApplicationFile
			   	 , SIO.Path.Join(Environment.ApplicationDirectory, options.Name)
			   	 )
			   	.Select
			   	 ( copyTask
			   	   => STT.Task.WhenAll(new [] {copyTask, serializeTask})
			   	      .CombineAsync(serializeTask)
			   	 );
			 });
		}

		public F.Result<STT.Task<ExitCode>> Unset(AO.Unset options) {
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
		public F.Result<STT.Task<ExitCode>> List(AO.List options)
		=> ListAsync().SelectErrorAsync(ListOperationException.GetRunFailureMap(options));
		async STT.Task<ExitCode> ListAsync() {
			foreach (var kvp in Configuration.Binding) {
				await Environment.StreamOut.WriteLineAsync($"{Utility.SafeQuote(kvp.Key)} → {kvp.Value.ToString()}");
			}
			return ExitCode.Success;
		}
		/**
		 * <summary>
		 * Run the external command with appended arguments.
		 * </summary>
		 * <param name="options">External command.</param>
		 * <returns>Result of the external command’s exit code or error.</returns>
		 * <exception cref='ExternalOperationException'>External command fails to run.</exception>
		 */
		public F.Result<STT.Task<ExitCode>> External(AO.External options) {
			var arguments = options.Arguments.Concat(Environment.Arguments.Select(Utility.SafeQuote));
			// explicit ProcessStartInfo.WorkingDirectory required https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.workingdirectory#remarks
			if (arguments.Any()) {
				var argumentLine = string.Join(' ', arguments);
				var errorMap = ExternalOperationException.GetRunFailureMap(options, F.Factory.Maybe(argumentLine));
				return Effect.RunCommand(Environment.WorkingDirectory, options.Command, argumentLine)
				       .SelectError(errorMap)
				       .Select(task => task.SelectErrorAsync(errorMap));
			} else {
				var errorMap = ExternalOperationException.GetRunFailureMap(options, F.Nothing.Value);
				return Effect.RunCommand(Environment.WorkingDirectory, options.Command)
				       .SelectError(errorMap)
				       .Select(task => task.SelectErrorAsync(errorMap));
			}
		}
	}
}