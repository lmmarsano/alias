using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using System.Linq;
using CL = CommandLine;
using static CommandLine.ParserExtensions;
using ST = LMMarsano.SumType;
using AO = Alias.Option;

namespace Alias {
	class CommandLine {
		/**
		 * <summary>
		 * The <see cref='SIO.TextWriter'/> used for help method output. null disables help screen.
		 * </summary>
		 */
		public SIO.TextWriter? HelpWriter { get; }
		/**
		 * <summary>
		 * Construct command line parser.
		 * </summary>
		 * <param name="helpWriter"><see cref='SIO.TextWriter'/> for help output.</param>
		 */
		public CommandLine(SIO.TextWriter? helpWriter) {
			HelpWriter = helpWriter;
		}
		/**
		 * <summary>
		 * Parse arguments into a command line.
		 * </summary>
		 * <param name="arguments">Arguments beginning with verb.</param>
		 * <returns>Command line parser result.</returns>
		 * <exception cref="S.ArgumentNullException"><paramref name="arguments"/> has a null item.</exception>
		 * <exception cref='UnparsableOptionException'>Unable to parse <paramref name="arguments"/>.</exception>
		 */
		public ST.Result<AO.AbstractOption> Parse(SCG.IEnumerable<string> arguments)
		=> ST.Factory.Try
		   ( ()
		     => new CL.Parser((with) => with.HelpWriter = HelpWriter)
		        .ParseArguments<AO.List, AO.Reset, AO.Restore, AO.Set, AO.Unset>(arguments)
		   , UnparsableOptionException.UnparsableMap(arguments)
		   )
		   .SelectMany
		    (result
		     => result switch
		        { CL.Parsed<object> { Value: AO.AbstractOption parsed } => parsed.Validation
		        , CL.NotParsed<object> { Errors: var errors }
		          when errors.OfType<CL.NoVerbSelectedError>().Any()
		          => new AO.Exit(ExitCode.Error).Validation
		        , CL.NotParsed<object> { Errors: var errors }
		          when errors.Where
		               (e => e is CL.HelpRequestedError
		                  || e is CL.HelpVerbRequestedError
		                  || e is CL.VersionRequestedError
		               )
		               .Any()
		          => new AO.Exit(ExitCode.Success).Validation
		        , _ => ST.Factory.Result<AO.AbstractOption>(UnparsableOptionException.Unparsable(arguments))
		        }
		    );
	}
}
