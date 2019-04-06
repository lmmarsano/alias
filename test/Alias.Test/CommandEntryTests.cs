using Xunit;
using AC = Alias.ConfigurationData;

namespace Alias.Test {
	public class CommandEntryTests {
		public static TheoryData<string, string?, string, string?> CommandEntryEqualityData { get; }
		= new TheoryData<string, string?, string, string?>
		  { {string.Empty, null, string.Empty, null}
		  , {string.Empty, null, string.Empty, string.Empty}
		  , {string.Empty, null, string.Empty, " "}
		  , {string.Empty, string.Empty, string.Empty, null}
		  , {string.Empty, string.Empty, string.Empty, string.Empty}
		  , {string.Empty, string.Empty, string.Empty, " "}
		  , {string.Empty, " ", string.Empty, null}
		  , {string.Empty, " ", string.Empty, string.Empty}
		  , {string.Empty, " ", string.Empty, " "}
		  , {string.Empty, ".", " ", ". "}
		  , {".", ".", ". ", ". "}
		  };
		[ Theory
		, MemberData(nameof(CommandEntryEqualityData))
		]
		public void CommandEntryEquality(string command0, string? arguments0, string command1, string? arguments1)
		=> Assert.Equal(new AC.CommandEntry(command0, arguments0), new AC.CommandEntry(command1, arguments1));
		public static TheoryData<string, string?> CommandEntryInequalityData { get; }
		= new TheoryData<string, string?>
		  { {string.Empty, "."}
		  , {".", null}
		  };
		[ Theory
		, MemberData(nameof(CommandEntryInequalityData))
		]
		public void CommandEntryInequality(string command, string? arguments)
		=> Assert.NotEqual(new AC.CommandEntry(string.Empty, null), new AC.CommandEntry(command, arguments));
	}
}
