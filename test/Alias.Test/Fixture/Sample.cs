using SCG = System.Collections.Generic;
using System.Linq;
using Xunit;
using AT = Alias.Test;
using AC = Alias.ConfigurationData;

namespace Alias.Test.Fixture {
	public class Sample {
		const string _newLine = @"
";
		static AC.Configuration ToConfiguration(SCG.IEnumerable<(string Alias, string Command, string? Arguments)> entries)
		=> new AC.Configuration
		   ( entries.ToBinding
		     ( tuple => tuple.Alias
		     , tuple => new AC.CommandEntry(tuple.Command, tuple.Arguments)
		     )
		   );
		public static AC.Configuration Configuration { get; }
		= ToConfiguration
		  ( new []
		    { (@"alias0", @"command", null)
		    , (@"alias1", @"command", @"arguments")
		    , (@"alias2", @"command", @"arguments with spaces")
		    , (@"spaced alias", @"spaced command", @"arguments")
		    }
		  );
		public static AC.Configuration EmptyConfiguration { get; }
		= ToConfiguration(Enumerable.Empty<(string, string, string?)>());
		static string NormalizeLineEnd(string input)
		=> AT.Utility.NormalizeLineEnd(_newLine, input);
		public static TheoryData<string, AC.Configuration> SerializationData { get; }
		= new TheoryData<string, AC.Configuration>
		  { { NormalizeLineEnd
		      ( @"{
	""binding"": {}
}"
		      )
		    , EmptyConfiguration
		    }
		  , { NormalizeLineEnd
		      ( @"{
	""binding"": {
		""alias-arguments"": {
			""command"": ""command"",
			""arguments"": ""arguments""
		},
		""alias-no-arguments"": {
			""command"": ""command""
		}
	}
}"
		      )
		    , ToConfiguration
		      ( new []
		        { ( @"alias-arguments", @"command", @"arguments")
		        , ( @"alias-no-arguments", @"command", null)
		        }
		      )
		    }
		  };
	}
}