#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using System.Linq;
using NJ = Newtonsoft.Json;
using Name = System.String;
using Command = System.String;
using NJL = Newtonsoft.Json.Linq;

namespace Delegator.Configuration {
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
		public static Configuration? FromJsonLinq(NJL.JToken jToken) {
			var pruned = JsonPruner.Transform(jToken);
			return JsonPruner.Filter(pruned)
			? pruned.ToObject<Configuration>(Converter.JsonSerializer)
			: default;
		}
	}
	public class CommandEntry {
		[NJ.JsonProperty("command")]
		/**
		<summary>
			Command to execute.
		</summary>
		<value>A commandline with arguments.</value>
		*/
		public Command Command { get; set; }
		/**
		<summary>
			Initialize a <c cref='CommandEntry'>command entry</c>.
		</summary>
		<param name="command">A commandline with arguments.</param>
		*/
		public CommandEntry(Command command) {
			Command = command.Trim();
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
		public static NJ.JsonSerializer JsonSerializer { get; } = NJ.JsonSerializer.Create(Settings);
		public static T Deserialize<T>(string value)
		=> NJ.JsonConvert.DeserializeObject<T>(value, Settings);
	}
}
// TODO process into non-nullable types (handling empty objects or arrays as null? omit properties with null value?)