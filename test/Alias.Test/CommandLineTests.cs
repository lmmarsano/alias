using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using Xunit;
using A = Alias;
using F = Functional;
using Functional;
using CL = CommandLine;
using System.Linq;
using CommandLine;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class CommandLineTests {
		static F.Result<Option.AbstractOption> Parse(Arguments arguments)
		=> new CommandLine(null).Parse(arguments);
		public static TheoryData<string, Arguments> ParseSucceedsData { get; }
		= new TheoryData<string, Arguments>
		  { { @"set name command", new[] { @"set", @"name", @"command" } }
		  , { @"set name command 0 1 2", new[] { @"set", @"name", @"command", @"0", @"1", @"2" } }
		  , { @"set ""spaced name"" command", new[] { @"set", @"spaced name", @"command" } }
		  , { @"set name ""spaced command""", new[] { @"set", @"name", @"spaced command" } }
		  , { @"set name command ""spaced argument""", new[] { @"set", @"name", @"command", @"spaced argument" } }
		  , { @"unset name", new[] { @"unset", @"name" } }
		  , { @"unset ""spaced name""", new[] { @"unset", @"spaced name" } }
		  , { @"reset", new[] { @"reset" } }
		  , { @"restore", new[] { @"restore" } }
		  };
		public static TheoryData<Arguments> ParseFailsData { get; }
		= new TheoryData<Arguments>
		  { new string[] {}
		  , new string?[] {null}
		  , new [] {@""}
		  , new [] {@"-"}
		  , new [] {@"set"}
		  , new [] {@"set", @"name"}
		  , new [] {@"unset"}
		  };
		[Theory]
		[MemberData(nameof(ParseSucceedsData))]
		public void ParseSucceeds(string unparse, Arguments arguments) {
			switch (Parse(arguments)) {
				case F.Ok<Option.AbstractOption> value:
					Assert.Equal(unparse, CL.Parser.Default.FormatCommandLine(value.Value));
					break;
				case var value:
					Assert.IsType<F.Ok<Option.AbstractOption>>(value);
					Assert.True(false);
					break;
			};
		}
		[Theory]
		[MemberData(nameof(ParseFailsData))]
		public void ParseFails(Arguments arguments) {
			var parse = Parse(arguments);
			Assert.IsType<F.Error<Option.AbstractOption>>(parse);
			Assert.IsType<UnparsableOptionException>(((F.Error<Option.AbstractOption>)parse).Value);
		}
		[Fact]
		public void ParseEmptyTest() {
			var parse = Parse(new[] { @"set", @"name", @"", @"" });
			Assert.IsType<F.Error<Option.AbstractOption>>(parse);
		}
	}
}