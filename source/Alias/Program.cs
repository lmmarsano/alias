using S = System;
using STT = System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;
using SRC = System.Runtime.CompilerServices;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using SSP = System.Security.Permissions;
using System.Linq;
using F = Functional;
using static Functional.Extension;
using CL = CommandLine;
using AC = Alias.Configuration;
using AO = Alias.Option;

[assembly: SRC.InternalsVisibleTo("DynamicProxyGenAssembly2")
         , SRC.InternalsVisibleTo("Alias.Test")
]
namespace Alias {
	public enum ExitCode
	{ Success
	, Error
	}
	class Program {
		static async STT.Task<int> Main(string[] args) {
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
			 return (int)await Entry(() => new Environment(args));
		}
		public static STT.Task<ExitCode> Entry(S.Func<IEnvironment> getEnvironment)
		=> F.Factory.Try(getEnvironment)
		   .Select(WithEnvironment)
		   .Reduce(ErrorRenderMap(F.Nothing.Value, F.Nothing.Value));
		public static STT.Task<ExitCode> WithEnvironment(IEnvironment environment)
		=> TryGetConfiguration(environment.ConfigurationFile).Select
		   (taskMaybeConfiguration => WithMaybeConfiguration(environment, taskMaybeConfiguration))
		   .Reduce(ErrorRenderMap(F.Factory.Maybe(environment), F.Nothing.Value));
		private static F.Result<STT.Task<F.Maybe<AC.Configuration>>> TryGetConfiguration(IFileInfo file)
		=> F.Factory.Try
		    ( () => file.OpenAsync(SIO.FileMode.Open, SIO.FileAccess.Read)
		    , TerminalFileException.ReadErrorMap(file.FullName)
		    )
		   .SelectMany
		    ( fileStream
		      => F.Factory.Try
		          ( () => new SIO.StreamReader(fileStream)
		          , TerminalFileException.ReadErrorMap(file.FullName)
		          )
		         .Catch(result => {
		          	fileStream.Dispose();
		          	return result;
		          })
		         .Select(DeserializeMap(fileStream))
		    );
		static S.Func<SIO.TextReader, STT.Task<F.Maybe<AC.Configuration>>> DeserializeMap(SIO.Stream fileStream)
		=> async textReader => {
			using (fileStream)
			using (textReader)
			return (await AC.Configuration.DeserializeAsync(textReader)).ToMaybe();
		};
		static STT.Task<ExitCode> WithMaybeConfiguration(IEnvironment environment, STT.Task<F.Maybe<AC.Configuration>> taskMaybeConfiguration)
		=> taskMaybeConfiguration.SelectManyAsync
		    ( maybeConfiguration
		      => (maybeConfiguration switch
		          { F.Just<AC.Configuration> justConfiguration => WithConfiguration(environment, justConfiguration.Value)
		          , _ => WithoutConfiguration(environment)
		          }
		         )
		         .Reduce(ErrorRenderMap(F.Factory.Maybe(environment), maybeConfiguration))
		    );
		static S.Func<S.Exception, STT.Task<ExitCode>> ErrorRenderMap(F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> error
		=> error.DisplayMessage(maybeEnvironment, maybeConfiguration)
		   .ContinueWith(task => ExitCode.Error);
		static F.Result<STT.Task<ExitCode>> WithoutConfiguration(IEnvironment environment)
		=> new CommandLine(environment.StreamError).Parse(environment.Arguments)
		   .SelectMany
		    (option => option.Operate(new Operation(environment, new AC.Configuration(new SCG.Dictionary<string, AC.CommandEntry>()))));
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
		     => maybeExternal switch
		        { F.Just<AO.External> justExternal => F.Factory.Result<AO.AbstractOption>(justExternal.Value)
		        , _ => new CommandLine(environment.StreamError).Parse(environment.Arguments)
		        }
		    )
		   .SelectMany(option => option.Operate(new Operation(environment, configuration)));
	}
}