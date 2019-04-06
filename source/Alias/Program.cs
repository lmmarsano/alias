using S = System;
using STT = System.Threading.Tasks;
using SRC = System.Runtime.CompilerServices;
using F = Functional;
using static Functional.Extension;
using AC = Alias.ConfigurationData;
using AO = Alias.Option;

[assembly: SRC.InternalsVisibleTo("DynamicProxyGenAssembly2")
         , SRC.InternalsVisibleTo("Alias.Test")
]
namespace Alias {
	public enum ExitCode {
		Success
	, Error
	}
	class Program {
		static async STT.Task<int> Main(string[] args)
		=> (int)await Entry(() => new Environment(args)).ConfigureAwait(false);
		/**
		 * <summary>
		 * <para>
		 * Entry point to entire program.
		 * Load an environment.
		 * Read the configuration.
		 * If current name is configured, then run configured command with passed in arguments.
		 * Otherwise, process arguments, and return validity
		 * </para>
		 * <list>
		 * <item>
		 * <term>valid</term>
		 * <description>
		 * <para>dispatch command</para>
		 * <list>
		 * <item><description>set name</description></item>
		 * <item><description>unset name</description></item>
		 * <item><description>reset all</description></item>
		 * <item><description>restore</description></item>
		 * </list>
		 * </description>
		 * </item>
		 * <item>
		 * <term>invalid</term>
		 * <description>display help and exit with error</description>
		 * </item>
		 * </list>
		 * </summary>
		 * <param name="getEnvironment">Map returning environment.</param>
		 * <returns>A task yielding an exit code.</returns>
		 */
		public static STT.Task<ExitCode> Entry(S.Func<IEnvironment> getEnvironment)
		=> F.Factory.Try(getEnvironment)
		   .Select(WithEnvironment)
		   .ReduceNested(ErrorRenderMap(F.Nothing.Value, F.Nothing.Value));
		static STT.Task<ExitCode> WithEnvironment(IEnvironment environment)
		=> environment.Effect.TryGetConfiguration(environment.ConfigurationFile)
		   .Select(WithMaybeConfiguration(environment))
		   .ReduceNested(ErrorRenderMap(F.Factory.Maybe(environment), F.Nothing.Value));
		static S.Func<STT.Task<F.Maybe<AC.Configuration>>, STT.Task<ExitCode>> WithMaybeConfiguration(IEnvironment environment)
		=> taskMaybeConfiguration
		=> taskMaybeConfiguration.SelectManyAsync
		   (maybeConfiguration
		    => (maybeConfiguration is F.Just<AC.Configuration>(var configuration)
		       ? WithConfiguration(environment, configuration)
		       : WithoutConfiguration(environment)
		       )
		       .ReduceNested(ErrorRenderMap(F.Factory.Maybe(environment), maybeConfiguration))
		   );
		static S.Func<S.Exception, STT.Task<ExitCode>> ErrorRenderMap(F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> error
		=> error.DisplayMessage(maybeEnvironment, maybeConfiguration)
		   .ContinueWith(task => ExitCode.Error);
		static F.Result<STT.Task<ExitCode>> WithoutConfiguration(IEnvironment environment)
		=> new CommandLine(environment.StreamError).Parse(environment.Arguments)
		   .SelectMany
		   (option => option.Operate(new Operation(environment, new AC.Configuration(new AC.BindingDictionary()))));
		/**
		 * <summary>
		 * Attempt to lookup program name from configuration and run found associated command.
		 * If name is absent, then attempt to process arguments as an internal command.
		 * </summary>
		 * <param name="environment">Program environment.</param>
		 * <param name="configuration">Program configuration.</param>
		 * <returns>Exit code result: <see cref='Error{ExitCode}'/> case provides any errors.</returns>
		 */
		static F.Result<STT.Task<ExitCode>> WithConfiguration(IEnvironment environment, AC.Configuration configuration)
		=> F.Factory.Try
		   (() => AO.External.Parse(configuration, environment.ApplicationName))
		   .SelectMany
		   (maybeExternal
		    => maybeExternal is F.Just<AO.External>(var external)
		     ? F.Factory.Result<AO.AbstractOption>(external)
		     : new CommandLine(environment.StreamError).Parse(environment.Arguments)
		   )
		   .SelectMany(option => option.Operate(new Operation(environment, configuration)));
	}
}
