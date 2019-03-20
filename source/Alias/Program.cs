using S = System;
using SRC = System.Runtime.CompilerServices;
using SIO = System.IO;
using SCG = System.Collections.Generic;
using SSP = System.Security.Permissions;
using CL = CommandLine;
using AC = Alias.Configuration;
using AO = Alias.Option;
using F = Functional;
using Functional;

[assembly: SRC.InternalsVisibleTo("DynamicProxyGenAssembly2")
         , SRC.InternalsVisibleTo("Alias.Test")
]
namespace Alias {
	public enum ExitCode
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
			 return (int)Entry(() => new Environment(args));
		}
		public static ExitCode Entry(S.Func<IEnvironment> getEnvironment)
		=> F.Factory.Try(getEnvironment)
		   .SelectMany(WithEnvironment)
		   .Reduce(ExitCode.Error);
		public static F.Result<ExitCode> WithEnvironment(IEnvironment environment)
		=> TryGetConfiguration(environment.ConfigurationFile).SelectMany
			 (maybeConfiguration => WithMaybeConfiguration(environment, maybeConfiguration));
		private static F.Result<F.Maybe<AC.Configuration>> TryGetConfiguration(IFileInfo file)
		=> file.When(file.Exists).Select
		    (_
		     => F.Factory.Try
		         ( () => file.OpenText()
		         , TerminalFileException.ReadErrorMap(file.FullName)
		         )
		        .SelectMany
		         ( textReader
		           => F.Factory.Try
		              ( () => textReader.Using(AC.Configuration.Deserialize).ToMaybe()
		              , DeserialException.FailureMap(file)
		              )
		         )
		    )
		   .Reduce(Nothing<AC.Configuration>.Value);
		private static Result<ExitCode> WithMaybeConfiguration(IEnvironment environment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> maybeConfiguration switch
		   { F.Just<AC.Configuration> justConfiguration => WithConfiguration(environment, justConfiguration.Value)
		   , _ => WithoutConfiguration(environment)
		   };
		static Result<ExitCode> WithoutConfiguration(IEnvironment environment)
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
		static Result<ExitCode> WithConfiguration(IEnvironment environment, AC.Configuration configuration)
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