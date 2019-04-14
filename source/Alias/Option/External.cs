using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using AC = Alias.ConfigurationData;
using ST = LMMarsano.SumType;
using static LMMarsano.SumType.Extension;
using Command = System.String;

namespace Alias.Option {
	using Arguments = SCG.IEnumerable<string>;
	/**
	 * <summary>
	 * External command options.
	 * </summary>
	 */
	sealed class External: AbstractOption, S.IEquatable<External> {
		/**
		 * <summary>
		 * An alias.
		 * </summary>
		 * <value>An alias to an external command to execute.</value>
		 */
		public Command Alias { get; }
		/**
		 * <summary>
		 * A command name.
		 * </summary>
		 * <value>An unquoted name of an external command to execute.</value>
		 */
		public Command Command { get; }
		/**
		 * <summary>
		 * A sequence of argument lines.
		 * </summary>
		 * <value>An initial sequence of space-separated arguments to invoke with command.</value>
		 * <remarks>Each sequence item is a list of space-separated arguments. Arguments containing spaces must include explicit quotes.</remarks>
		 */
		public Arguments Arguments { get; set; }
		External(Command alias, Command command, Arguments arguments) {
			Alias = alias;
			Command = command;
			Arguments = arguments;
		}
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">The other operand.</param>
		 * <returns>True if and only if equal.</returns>
		 */
		public bool Equals(External other)
		=> Command == other.Command
		&& Arguments.SequenceEqual(other.Arguments);
		/// <inheritdoc/>
		public override bool Equals(object obj)
		=> obj is External value
		&& Equals(value);
		/// <inheritdoc/>
		public override int GetHashCode() => S.HashCode.Combine(Command, Arguments);
		/**
		 * <summary>
		 * Construct external options from command resolution of <paramref name="alias"/> in <paramref name="configuration"/>.
		 * </summary>
		 * <param name="configuration">Application configuration.</param>
		 * <param name="alias">Alias to lookup.</param>
		 * <returns>Nothing if alias is unconfigured. Otherwise, optional possibility of external options if alias successfully resolves. Otherwise, <see cref='CyclicBindingException'/> as optional error.</returns>
		 * <exception cref='CyclicBindingException'>Resolution fails due to a resolution cycle.</exception>
		 */
		public static ST.Maybe<ST.Result<External>> Parse(AC.Configuration configuration, Command alias) {
			var commandSequence = configuration.Binding.GetResolutionSequence(alias);
			if (commandSequence.Any()) {
				var first = commandSequence.First();
				return commandSequence.Skip(1)
				.AggregateAcyclically<AC.CommandEntry, (Command Command, Arguments Arguments)>
				( object.ReferenceEquals
				, (first.Command, first.Arguments.ToMaybe())
				, (acc, entry)
				  => ( entry.Command
				     , entry.Arguments is null
				     ? acc.Arguments
				     : acc.Arguments.Prepend(entry.Arguments)
				     )
				)
				.Select(pair => ST.Factory.Result(new External(alias, pair.Command, pair.Arguments)))
				.ReduceRight(commandEntry => CyclicBindingException.CommandError(configuration, commandEntry));
			} else {
				return ST.Nothing.Value;
			}
		}
		/// <inheritdoc/>
		public override ST.Result<STT.Task<ExitCode>> Operate(IOperation operation) => operation.External(this);
	}
}
