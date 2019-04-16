using SCG = System.Collections.Generic;
using System.Linq;
using Xunit;
using ST = LMMarsano.SumType;
using CommandLine;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class CommandLineTests {
		static ST.Result<Option.AbstractOption> Parse(Arguments arguments)
		=> new CommandLine(null).Parse(arguments);
		public static TheoryData<string, Arguments> ParseSucceedsData { get; }
		= new TheoryData<string, Arguments>
		  { { @"set name command", new[] { @"set", @"name", @"command" } }
		  , { @"set name command 0 1 2", new[] { @"set", @"name", @"command", @"0", @"1", @"2" } }
		  , { @"set ""spaced name"" command", new[] { @"set", @"spaced name", @"command" } }
		  , { @"set name ""spaced command""", new[] { @"set", @"name", @"spaced command" } }
		  , { @"set name command ""spaced argument""", new[] { @"set", @"name", @"command", @"spaced argument" } }
		  , { @"set name command -argument", new[] { @"set", @"name", @"command", @"--", @"-argument" } }
		  , { @"set name command -- -argument", new[] { @"set", @"name", @"command", @"--", @"--", @"-argument" } }
		  , { @"unset name", new[] { @"unset", @"name" } }
		  , { @"unset ""spaced name""", new[] { @"unset", @"spaced name" } }
		  , { @"reset", new[] { @"reset" } }
		  , { @"list", new[] { @"list" } }
		  };
		[ Theory
		, MemberData(nameof(ParseSucceedsData))
		]
		public void ParseSucceeds(string unparse, Arguments arguments) {
			Assert.Equal(unparse, Parser.Default.FormatCommandLine(Utility.FromOk(Parse(arguments))));
		}
		public static TheoryData<Arguments> ParsePassesData { get; }
		= new TheoryData<Arguments>
		  { Enumerable.Empty<string>()
		  , new string[] { @"help" }
		  , new string[] { @"--help" }
		  , new string[] { @"version" }
		  , new string[] { @"--version" }
		  };
		[ Theory
		, MemberData(nameof(ParsePassesData))
		]
		public void ParsePasses(Arguments arguments) {
			var parse = Parse(arguments);
			Assert.IsType<ST.Ok<Option.AbstractOption>>(parse);
		}
		public static TheoryData<Arguments> ParseFailsData { get; }
		= new TheoryData<Arguments>
		  { new string?[] {null}
		  , new [] {@""}
		  , new [] {@"-"}
		  , new [] {@"set"}
		  , new [] {@"set", @"name"}
		  , new [] {@"set", @"name", @"-argument"}
		  , new [] {@"unset"}
		  };
		[ Theory
		, MemberData(nameof(ParseFailsData))
		]
		public void ParseFails(Arguments arguments) {
			var parse = Parse(arguments);
			Assert.IsType<ST.Error<Option.AbstractOption>>(parse);
			Assert.IsType<UnparsableOptionException>(Utility.FromError(parse));
		}
		[Fact]
		public void ParseEmptyTest() {
			var parse = Parse(new[] { @"set", @"name", @"", @"" });
			Assert.IsType<ST.Error<Option.AbstractOption>>(parse);
		}
	}
}
