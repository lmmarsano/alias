using S = System;
using STT = System.Threading.Tasks;
using SRC = System.Runtime.CompilerServices;
using ST = LMMarsano.SumType;
using static LMMarsano.SumType.Extension;
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
		=> ST.Factory.Try(getEnvironment)
		   .Select(WithEnvironment)
		   .ReduceNested(ErrorRenderMap(ST.Nothing.Value));
		static STT.Task<ExitCode> WithEnvironment(IEnvironment environment)
		=> environment.Effect.TryGetConfiguration(environment.ConfigurationFile)
		   .Select(WithMaybeConfiguration(environment))
		   .ReduceNested(ErrorRenderMap(ST.Factory.Maybe(environment)));
		static S.Func<STT.Task<ST.Maybe<AC.Configuration>>, STT.Task<ExitCode>> WithMaybeConfiguration(IEnvironment environment)
		=> taskMaybeConfiguration
		=> taskMaybeConfiguration.SelectManyAsync
		   (maybeConfiguration
		    => (maybeConfiguration is ST.Just<AC.Configuration>(var configuration)
		       ? WithConfiguration(environment, configuration)
		       : WithoutConfiguration(environment)
		       )
		       .ReduceNested(ErrorRenderMap(ST.Factory.Maybe(environment)))
		   );
		static S.Func<S.Exception, STT.Task<ExitCode>> ErrorRenderMap(ST.Maybe<IEnvironment> maybeEnvironment)
		=> error
		=> error.DisplayMessage(maybeEnvironment).ContinueWith(task => ExitCode.Error);
		static ST.Result<STT.Task<ExitCode>> WithoutConfiguration(IEnvironment environment)
		=> new CommandLine(environment.StreamError).Parse(environment.Arguments)
		   .SelectMany
		   (option => option.Operate(new Operation(environment, new AC.Configuration(new AC.BindingDictionary()))));
		/**
		 * <summary>
		 * Attempt to process arguments as external command, then as an internal command.
		 * </summary>
		 * <param name="environment">Program environment.</param>
		 * <param name="configuration">Program configuration.</param>
		 * <returns>Possible exit code, otherwise errors.</returns>
		 */
		static ST.Result<STT.Task<ExitCode>> WithConfiguration(IEnvironment environment, AC.Configuration configuration)
		=> AO.External.Parse(configuration, environment.ApplicationName)
		   .Reduce
		   ( () => new CommandLine(environment.StreamError).Parse(environment.Arguments)
		   , result => result.OfType<AO.AbstractOption>(value => new S.InvalidCastException($"Unable to cast {nameof(AO.External)} to {nameof(AO.AbstractOption)}."))
		   )
		   .SelectMany(option => option.Operate(new Operation(environment, configuration)));

	}
}
