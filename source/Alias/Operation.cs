using STT = System.Threading.Tasks;
using System.Linq;
using F = Functional;
using static Functional.Extension;
using AO = Alias.Option;
using AC = Alias.ConfigurationData;

namespace Alias {
	/**
	 * <summary>
	 * Operations associated with each <see cref='Option.AbstractOption'/> implementation.
	 * </summary>
	 */
	class Operation: IOperation {
		public IEffect Effect { get; }
		public IEnvironment Environment { get; }
		public AC.Configuration Configuration { get; }
		public Operation(IEnvironment environment, AC.Configuration configuration) {
			Effect = environment.Effect;
			Environment = environment;
			Configuration = configuration;
		}
		/**
		 * <summary>
		 * Write configuration to file.
		 * </summary>
		 * <returns>Possible task yielding exit code.</returns>
		 * <inheritdoc cref='IEffect.WriteConfiguration' select='exception'/>
		 */
		F.Result<STT.Task<ExitCode>> WriteConfiguration()
		=> Effect.WriteConfiguration(Configuration, Environment.ConfigurationFile)
		   .Select(task => task.CombineAsync(Utility.TaskExitSuccess));
		/**
		 * <summary>
		 * Delete configuration.
		 * </summary>
		 * <param name="options">Reset options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 * <exception cref='OperationIOException'>Unable to access file for deletion.</exception>
		 */
		public virtual F.Result<STT.Task<ExitCode>> Reset(AO.Reset options)
		=> Effect.DeleteFile(Environment.ConfigurationFile)
		   .Select(task => task.CombineAsync(Utility.TaskExitSuccess));
		/**
		 * <summary>
		 * No operation. Derived classes may extend functionality.
		 * </summary>
		 * <param name="options">Restore options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 */
		public virtual F.Result<STT.Task<ExitCode>> Restore(AO.Restore options) => Utility.TaskExitSuccess;
		/**
		 * <summary>
		 * Set an alias and save configuration.
		 * </summary>
		 * <param name="options">Set options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 * <exception cref='SerializerException'>Configuration could not be serialized or written to file.</exception>
		 */
		public virtual F.Result<STT.Task<ExitCode>> Set(AO.Set options) {
			Configuration.Binding[options.Name]
			= new AC.CommandEntry
			  ( options.Command
			  , options.ArgumentString
			  );
			return WriteConfiguration();
		}
		/**
		 * <summary>
		 * Remove an alias and save configuration.
		 * </summary>
		 * <param name="options">Unset options.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 * <exception cref='UnsetOperationException'>Given alias doesn’t exist.</exception>
		 * <inheritdoc cref='WriteConfiguration' select='exception'/>
		 */
		public virtual F.Result<STT.Task<ExitCode>> Unset(AO.Unset options)
		=> Configuration.Binding.Remove(options.Name)
		 ? WriteConfiguration()
		 : UnsetOperationException.AliasUndefined(options);
		/**
		 * <summary>
		 * Print alias assignments as specified by options to <see cref='Environment.StreamOut'/>.
		 * </summary>
		 * <param name="options">List specifications.</param>
		 * <returns>Possible task yielding exit code.</returns>
		 * <exception cref='ListOperationException'>List command fails.</exception>
		 */
		public virtual F.Result<STT.Task<ExitCode>> List(AO.List options)
		=> ListAsync().SelectErrorAsync(ListOperationException.OutputFailureMap(options));
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
		 * <returns>Possible task yielding external command’s exit code.</returns>
		 * <exception cref='ExternalOperationException'>External command fails to run.</exception>
		 */
		public virtual F.Result<STT.Task<ExitCode>> External(AO.External options) {
			var arguments = options.Arguments.Concat(Environment.Arguments.Select(Utility.SafeQuote));
			// explicit ProcessStartInfo.WorkingDirectory required https://docs.microsoft.com/en-us/dotnet/api/system.diagnostics.processstartinfo.workingdirectory#remarks
			var maybeArgumentLine
			= arguments.Any()
			? F.Factory.Maybe(string.Join(' ', arguments))
			: F.Nothing.Value;
			return Effect.RunCommand(Environment.WorkingDirectory, options.Command, maybeArgumentLine)
			       .SelectErrorNested(ExternalOperationException.GetRunFailureMap(options, maybeArgumentLine));
		}
	}
}