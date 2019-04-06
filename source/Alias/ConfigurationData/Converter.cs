#nullable enable
using SIO = System.IO;
using NJ = Newtonsoft.Json;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.ConfigurationData {
	static class Converter {
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
