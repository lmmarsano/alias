using S = System;
using STT = System.Threading.Tasks;
using F = Functional;
using static Functional.Extension;
using AC = Alias.Configuration;
using System.Linq;

namespace Alias {
	static class Extension {
		public static STT.Task DisplayMessage(this IException @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> Environment.GetErrorStream(maybeEnvironment).WriteAsync(@this.Message);
		public static STT.Task DisplayMessage(this S.AggregateException @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> STT.Task.WhenAll
		    ( @this.Flatten().InnerExceptions
		      .OfType<ITerminalException>()
		      .Select(error => error.DisplayMessage(maybeEnvironment, maybeConfiguration))
		    );
		public static STT.Task DisplayMessage(this S.Exception @this, F.Maybe<IEnvironment> maybeEnvironment, F.Maybe<AC.Configuration> maybeConfiguration)
		=> STT.Task.CompletedTask;
	}
}