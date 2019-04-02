#nullable enable
using S = System;
using SIO = System.IO;
using STT = System.Threading.Tasks;
using System.Linq;
using NJ = Newtonsoft.Json;
using Name = System.String;
using NJL = Newtonsoft.Json.Linq;

namespace Alias.ConfigurationData {
	// using Binding = SCG.IDictionary<Name, CommandEntry>;
	/**
	<summary>
		Application-wide configuration including bindings from alias names to command entries.
	</summary>
	*/
#pragma warning disable CS0659 // overrides Object.Equals(object o) but does not override Object.GetHashCode()
	public class Configuration: S.IEquatable<Configuration> {
		[NJ.JsonProperty("binding")]
#pragma warning restore CS0659
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
			).ToBinding(x => x.Key.Trim(), x => x.Value);
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
		// TODO deprecate
		public static Configuration? Deserialize(SIO.TextReader reader) {
			using var jsonReader = new NJ.JsonTextReader(reader);
			return FromJsonLinq(NJL.JToken.ReadFrom(jsonReader, Converter.JsonLoadSettings));
		}
		/**
		 * <summary>
		 * Asynchronously deserialize a <see cref='Configuration'/> object from <see cref='SIO.TextReader'/>.
		 * </summary>
		 * <param name="reader">Text input stream to deserialize.</param>
		 * <returns>A configuration or <see cref="null"/> for empty configuration.</returns>
		 * <exception cref="UnhandledJsonTokenException">An item with unhandled runtime type derived from <see cref='NJL.JContainer'/> was encountered as a JSON token read from <paramref name="reader"/>.</exception>
		 */
		public static async STT.Task<Configuration?> DeserializeAsync(SIO.TextReader reader) {
			using var jsonReader = new NJ.JsonTextReader(reader);
			return FromJsonLinq(await NJL.JToken.ReadFromAsync(jsonReader, Converter.JsonLoadSettings));
		}
		/**
		 * <summary>
		 * Serialize a <see cref='Configuration'/> object to <see cref='SIO.TextWriter'/>.
		 * </summary>
		 * <param name="writer">Text output stream to serialize to.</param>
		 * <exception cref="SerializerException">Configuration could not be serialized.</exception>
		 */
		// TODO deprecate
		public void Serialize(SIO.TextWriter writer) {
			using var jsonWriter = Converter.ToJsonWriter(writer);
			Converter.JsonSerializer.Serialize(jsonWriter, this);
		}
		/**
		 * <summary>
		 * Asynchronously serialize a <see cref='Configuration'/> object to <see cref='SIO.TextWriter'/>.
		 * </summary>
		 * <param name="writer">Asynchronous text output stream to serialize to.</param>
		 * <exception cref="SerializerException">Configuration could not be serialized.</exception>
		 */
		// FIXME when NewtonSoft provides JsonSerializer.SerializeAsync
		async STT.Task SerializeAsync(SIO.TextWriter writer) {
			using var jsonWriter = Converter.ToJsonWriter(writer);
			await NJL.JToken.FromObject(this).WriteToAsync(jsonWriter);
		}
		/**
		 * <summary>
		 * Equality predicate.
		 * </summary>
		 * <param name="other">A <see cref='Configuration'/>.</param>
		 * <returns>Truth value.</returns>
		 */
		public bool Equals(Configuration other) => Binding.Equals(other.Binding);
		/// <inheritdoc/>
		public override bool Equals(object obj) => (obj as Configuration)?.Equals(this) == true;
	}
}