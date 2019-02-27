using S = System;
using SCG = System.Collections.Generic;
using NJL = Newtonsoft.Json.Linq;
using System.Linq;

namespace Delegator.Configuration {
	public static class JsonPruner {
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
		 * Produce same type of JSON token as <c cref='jToken'>jToken</c> with children from <c cref='enumerable'>enumerable</c>.
		 * </summary>
		 * <param name="jToken">Template JSON token</param>
		 * <param name="enumerable">Replacement children</param>
		 * <returns>Same type of JSON token as <c cref='jToken'>jToken</c> with children from <c cref='enumerable'>enumerable</c></returns>
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
		   , NJL.JProperty jProperty => new NJL.JProperty(jProperty.Name,    enumerable.ElementAtOrDefault(0))
		   // jProperty = new JProperty(name, value) iff (jProperty.Name = name and    jProperty.ElementAt(0) = value)
		   , NJL.JConstructor jConstructor => new NJL.JConstructor(jConstructor.Name,    enumerable)
		   , NJL.JContainer _ => throw new S.NotImplementedException($"Factory for    {jToken.GetType()} not implemented.")
		   , _ => jToken
		   };
	}
}