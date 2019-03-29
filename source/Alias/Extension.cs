using S = System;
using STT = System.Threading.Tasks;
using F = Functional;
using static Functional.Extension;
using AC = Alias.Configuration;
using System.Linq;

namespace Alias {
	static class Extension {
		public static STT.Task DisplayMessage(this ITerminalException @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> Environment.GetErrorStream(maybeEnvironment).WriteAsync(@this.Message);
		public static STT.Task DisplayMessage(this S.Exception @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> @this switch
		   { S.AggregateException aggregate
		     => STT.Task.WhenAll
		        ( aggregate.Flatten().InnerExceptions
		          .OfType<ITerminalException>()
		          .Select(error => error.DisplayMessage(maybeEnvironment, maybeConfiguration))
		        )
		   , ITerminalException exception => exception.DisplayMessage(maybeEnvironment, maybeConfiguration)
		   , _ => STT.Task.CompletedTask
		   };
	}
}