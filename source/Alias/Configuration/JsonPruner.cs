using S = System;
using SCG = System.Collections.Generic;
using NJL = Newtonsoft.Json.Linq;
using System.Linq;

namespace Alias.Configuration {
	/**
	 * <summary>
	 * Static methods for pruning and filtering out null, undefined, and empty collection tokens, and for rebuilding JSON Linq tokens from enumerated children.
	 * For use with <c cref='NJL'>Json.NET JSON to Linq classes</c>.
	 * </summary>
	 */
	public static class JsonPruner {
		/**
		 * <summary>
		 * Prune JSON Linq structure by recursively removing null, undefined, and empty arrays/objects.
		 * </summary>
		 * <param name="item">JSON Linq token to prune.</param>
		 * <returns>Pruned JSON Linq token.</returns>
		 * <exception cref="UnhandledJsonTokenException">An item with unhandled runtime type derived from <see cref='NJL.JContainer'/> was encountered inside <paramref name="jToken"/>.</exception>
		 */
		public static S.Func<NJL.JToken, NJL.JToken> Transform { get; }
		= (new Pruner<NJL.JToken>(Factory, Filter)).Transform;
		/**
		 * <summary>
		 * Predicate for non-(empty, null, or undefined) <c cref='NJL.JToken'>JSON tokens</c>.
		 * </summary>
		 * <param name="jToken">JSON token to test</param>
		 * <returns>True when token non-(empty, null, or undefined).</returns>
		 */
		public static bool Filter(NJL.JToken jToken)
		=> !( jToken is NJL.JContainer jContainer
			 && ( !jContainer.Any()
			   || jToken is NJL.JProperty jProperty
			   && !Filter(jProperty.Value)
			    )
			 || jToken.Type == NJL.JTokenType.Null
			 || jToken.Type == NJL.JTokenType.Undefined
			  );
		/**
		 * <summary>
		 * Produce same type of JSON token as <paramref name='jToken'/> with children from <paramref name='enumerable'/>.
		 * </summary>
		 * <param name="jToken">Template JSON token</param>
		 * <param name="enumerable">Replacement children</param>
		 * <returns>Same type of JSON token as <paramref name='jToken'/> with children from <paramref name='enumerable'/>.</returns>
		 * <exception cref="UnhandledJsonTokenException"><paramref name='jToken'/>’s runtime type is an unhandled type derived from <c cref='NJL.JContainer'>NJL.JContainer</c>.</exception>
		 */
		public static NJL.JToken Factory(NJL.JToken jToken, SCG.IEnumerable<NJL.JToken> enumerable)
			/* switch chosen over
			 * - (dynamic) with extension methods
			 * - dictionary from token type to function
			 * this approach favors compile-time checks, others carry runtime penalties
			 */
		=> jToken switch
		   { NJL.JArray _ => new NJL.JArray(enumerable)
		   , NJL.JObject _ => new NJL.JObject(enumerable)
		   , NJL.JProperty {Name: var name} => new NJL.JProperty(name, enumerable.ElementAtOrDefault(0))
		   // jProperty = new JProperty(name, value) iff (jProperty.Name = name and jProperty.ElementAt(0) = value)
		   , NJL.JConstructor {Name: var name} => new NJL.JConstructor(name, enumerable)
		   , NJL.JContainer _ => throw UnhandledJsonTokenException.GetUnknown(jToken)
		   , _ => jToken
		   };
	}
}