using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using CL = CommandLine;
using CommandLine;
using AO = Alias.Option;
using F = Functional;
using Functional;

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
		public F.Result<AO.AbstractOption> Parse(SCG.IEnumerable<string> arguments)
		=> F.Factory.Try
		   ( ()
		     => /* CL.Parser.Default */new CL.Parser((parserSettings) => parserSettings.HelpWriter = HelpWriter).ParseArguments<Option.List, Option.Reset, Option.Restore, Option.Set, Option.Unset>(arguments)
		   , UnparsableOptionException.UnparsableMap(arguments)
		   )
		   .SelectMany
		    ( result
		      => result switch
		         { CL.Parsed<object> parsed => ((AO.AbstractOption)parsed.Value).Validation
		         , _ => F.Factory.Result<AO.AbstractOption>(UnparsableOptionException.Unparsable(arguments))
		         }
		    );
	}
}