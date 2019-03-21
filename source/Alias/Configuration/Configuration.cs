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
		 * Deserialize a <see cref='Configuration'/> object from <see cref='SIO.TextReader'/>.
		 * </summary>
		 * <param name="reader">Text input stream to deserialize.</param>
		 * <returns>A configuration or <see cref="null"/> for empty configuration.</returns>
		 * <exception cref="UnhandledJsonTokenException">An item with unhandled runtime type derived from <see cref='NJL.JContainer'/> was encountered as a JSON token read from <paramref name="reader"/>.</exception>
		 */
		public static Configuration? Deserialize(SIO.TextReader reader) {
			using var jsonReader = new NJ.JsonTextReader(reader);
			return FromJsonLinq(NJL.JToken.ReadFrom(jsonReader, Converter.JsonLoadSettings));
		}
		/**
		 * <summary>
		 * Serialize a <see cref='Configuration'/> object to <see cref='SIO.TextWriter'/>.
		 * </summary>
		 * <param name="writer">Text output stream to serialize to.</param>
		 * <exception cref="SerializerException">Configuration could not be serialized.</exception>
		 */
		public void Serialize(SIO.TextWriter writer) {
			using var jsonWriter = Converter.ToJsonWriter(writer);
			Converter.JsonSerializer.Serialize(jsonWriter, this);
		}
	}
	/**
	 * <summary>
	 * Components of a command invocation: command & optional arguments.
	 * </summary>
	 */
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
		=> Command == commandEntry.Command
		&& Arguments == commandEntry.Arguments;
		public override bool Equals(object obj)
		=> obj is CommandEntry commandEntry
		&& Equals(commandEntry);
		public override int GetHashCode() => S.HashCode.Combine(Command, Arguments);
	}
	public static class Converter {
		/**
		 * <summary>
		 * <see cref='NJ.JsonSerializerSettings'/> to skip default value serialization, ignore nulls for (de)serialization, ignore meta data properties, indent serializations, and throw <see cref='SerializerException'/> on errors.
		 * </summary>
		 */
		public static readonly NJ.JsonSerializerSettings Settings = new NJ.JsonSerializerSettings
		{ DefaultValueHandling = NJ.DefaultValueHandling.Ignore
		, Formatting = NJ.Formatting.Indented
		, NullValueHandling = NJ.NullValueHandling.Ignore
		, MetadataPropertyHandling = NJ.MetadataPropertyHandling.Ignore
		, Error = (sender, args) => {
		  	if (sender == args.ErrorContext.OriginalObject) {
		  		throw SerializerException.Failure(args.ErrorContext.Path, args.ErrorContext.Error);
		  	}
		  }
		};
		public static NJ.JsonSerializer JsonSerializer { get; } = NJ.JsonSerializer.Create(Settings);
		public static NJL.JsonLoadSettings JsonLoadSettings { get; }
		= new NJL.JsonLoadSettings()
		  { DuplicatePropertyNameHandling = NJL.DuplicatePropertyNameHandling.Error };
		/**
		 * <summary>
		 * Transform a <see cref='SIO.TextWriter'/> to a tab indenting <see cref='NJ.JsonWriter'/>.
		 * </summary>
		 * <param name="writer">A <see cref='SIO.TextWriter'/>.</param>
		 * <returns>A tab indenting <see cref='NJ.JsonWriter'/>.</returns>
		 */
		public static NJ.JsonWriter ToJsonWriter(SIO.TextWriter writer)
		=> new NJ.JsonTextWriter(writer)
		   { Formatting = NJ.Formatting.Indented
		   , IndentChar = '	'
		   , Indentation = 1
		   };
		public static T Deserialize<T>(string value)
		=> NJ.JsonConvert.DeserializeObject<T>(value, Settings);
	}
}