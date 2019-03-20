#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using System.Linq;
using NJ = Newtonsoft.Json;
using Name = System.String;
using Command = System.String;
using Arguments = System.String;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.Configuration {
	using Binding = SCG.IDictionary<Name, CommandEntry>;
	// TODO consider using CommandEntry = System.Object?;
	/**
	<summary>
		Application-wide configuration including bindings from alias names to command entries.
	</summary>
	*/
	public class Configuration {
		[NJ.JsonProperty("binding")]
		/**
		<summary>
			The configuration's bindings between alias names and command entries.
		</summary>
		<value>An association of <c cref='Name'>names</c> with their <c cref='CommandEntry'>command entries</c>.</value>
		*/
		public Binding Binding { get; set; }
		/**
		<summary>
			Initialize an application <c cref='Configuration'>Configuration</c>.
		</summary>
		<param name="binding">An association of <c cref='Name'>names</c> with their <c cref='CommandEntry'>command entries</c>.</param>
		*/
		public Configuration(Binding binding) {
			Binding = binding.Where
			(x => !( string.IsNullOrWhiteSpace(x.Key)
			      || string.IsNullOrWhiteSpace(x.Value.Command)
			       )
			).ToDictionary(x => x.Key.Trim(), x => x.Value);
		}
		/**
		 * <summary>
		 * Convert appropriate <c cref='NJL'>JSON Linq</c> objects to <c cref='Configuration'>Configuration</c> objects.
		 * </summary>
		 * <param name="jToken">JSON Linq object to convert.</param>
		 * <returns>The corresponding <c cref='Configuration'>Configuration</c> or null for empty configuration.</returns>
		 * <exception cref="UnhandledJsonTokenException">An item with unhandled runtime type derived from <see cref='NJL.JContainer'/> was encountered inside <paramref name="jToken"/>.</exception>		 */
		public static Configuration? FromJsonLinq(NJL.JToken jToken) {
			var pruned = JsonPruner.Transform(jToken);
			return JsonPruner.Filter(pruned)
			? pruned.ToObject<Configuration>(Converter.JsonSerializer)
			: default;
		}
		/**
		 * <summary>
		 * Deserialize a <see cref='Configuration'/> object from <c cref='SIO.TextReader'>text input stream</c>.
		 * </summary>
		 * <param name="reader">Text input stream to deserialize.</param>
		 * <returns>A configuration or <see cref="null"/> for empty configuration.</returns>
		 * <exception cref="UnhandledJsonTokenException">An item with unhandled runtime type derived from <see cref='NJL.JContainer'/> was encountered as a JSON token read from <paramref name="reader"/>.</exception>		 */
		public static Configuration? Deserialize(SIO.TextReader reader) {
			using (var jsonReader = new NJ.JsonTextReader(reader)) {
				return FromJsonLinq(NJL.JToken.ReadFrom(jsonReader, Converter.JsonLoadSettings));
			}
		}
	}
	public class CommandEntry {
		/**
		<summary>
			Command to execute.
		</summary>
		<value>A command without arguments.</value>
		*/
		[NJ.JsonProperty("command")]
		public Command Command { get; set; }
		/**
		<summary>
			Command arguments.
		</summary>
		<value>Arguments for a command.</value>
		*/
		[NJ.JsonProperty("arguments")]
		public Arguments? Arguments { get; set; }
		/**
		<summary>
			Initialize a <c cref='CommandEntry'>command entry</c>.
		</summary>
		<param name="command">A commandline with arguments.</param>
		*/
		public CommandEntry(Command command, Arguments? arguments) {
			Command = command.Trim();
			Arguments = string.IsNullOrWhiteSpace(arguments)
			          ? null
			          : arguments.Trim();
		}
		public bool Equals(CommandEntry commandEntry)
		=> Command == commandEntry.Command;
		public override bool Equals(object obj)
		=> obj is CommandEntry commandEntry
		&& Equals(commandEntry);
		public override int GetHashCode()
		=> S.HashCode.Combine(Command);
	}
	public static class Converter {
		public static readonly NJ.JsonSerializerSettings Settings = new NJ.JsonSerializerSettings
		{ DefaultValueHandling = NJ.DefaultValueHandling.Ignore
		, NullValueHandling = NJ.NullValueHandling.Ignore
		, MetadataPropertyHandling = NJ.MetadataPropertyHandling.Ignore
		};
		public static NJ.JsonSerializer JsonSerializer { get; }
		= NJ.JsonSerializer.Create(Settings);
		public static NJL.JsonLoadSettings JsonLoadSettings { get; }
		= new NJL.JsonLoadSettings()
		  { DuplicatePropertyNameHandling = NJL.DuplicatePropertyNameHandling.Error };

		public static T Deserialize<T>(string value)
		=> NJ.JsonConvert.DeserializeObject<T>(value, Settings);
	}
}
// TODO process into non-nullable types (handling empty objects or arrays as null? omit properties with null value?)