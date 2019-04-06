using Xunit;
using AC = Alias.ConfigurationData;

namespace Alias.Test.Fixture {
	public class DeserializesData: TheoryData<AC.Configuration?, string> {
		(AC.Configuration? Configuration, string Input)[] _data
		= { (null, @"{}")
		  , (null, @"{ ""binding"" : null }")
		  , (null, @"{ ""binding"" : {} }")
		  , (null, @"{ ""binding"" : { ""name"": null } }")
		  , (null, @"{ ""binding"" : { ""name"": {} } }")
		  , (null, @"{ ""binding"" : { ""name"": { ""command"": null } } }")
		  , (null, @"{ ""binding"" : { ""name"": { ""command"": null, ""arguments"": null } } }")
		  , ( new AC.Configuration(new AC.BindingDictionary(1) {{"name", new AC.CommandEntry("value", null)}})
		    , @"{ ""binding"" : { ""name"": { ""command"": ""value"" } } }"
		    )
		  , ( new AC.Configuration(new AC.BindingDictionary(1) {{"name", new AC.CommandEntry("command",   "arguments")}})
		    , @"{ ""binding"" : { ""name"": { ""command"": ""command"", ""arguments"": ""arguments"" } } }"
		    )
		  };
		public DeserializesData() {
			foreach (var item in _data) {
				Add(item.Configuration, item.Input);
			}
		}
	}
}
