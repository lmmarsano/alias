#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using Xunit;
using M = Moq;
using AT = Alias.Test;
using ATF = Alias.Test.Fixture;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class ProgramTests {
		static readonly string _newLine = @"
";
		static string NormalizeLineEnd(string input)
		=> AT.Utility.NormalizeLineEnd(_newLine, input);
		static readonly string _defaultOutput = NormalizeLineEnd(@"testhost 16.0.1
© Microsoft Corporation. All rights reserved.

  list       List aliases.

  reset      Remove all aliases.

  restore    Recreate file system for configured aliases.

  set        Add or change an alias.

  unset      Remove configured alias.

  help       Display more information on a specific command.

  version    Display version information.

");
		static readonly string _configuration = @"{ ""binding"":
  { ""bound"":
    { ""command"": ""name""
    , ""arguments"": ""from configuration""
    }
  }
}";
		public static TheoryData<ExitCode, string, string, string, string, Arguments> EntryData
		= new TheoryData<ExitCode, string, string, string, string, Arguments>
		  { { ExitCode.Error, string.Empty, _defaultOutput, _configuration, @"alias", Enumerable.Empty<string>() }
			, { ExitCode.Error, string.Empty, _defaultOutput, _configuration, @"alias", new [] {@"help"} }
			, { ExitCode.Error, string.Empty, NormalizeLineEnd(@"Unable to process file: directory\alias.conf
Unexpected end of content while loading JObject. Path 'binding', line 1, position 12.
"), @"{ ""binding"":", @"alias", new [] {@"help"} }
		  };
		[Theory]
		[MemberData(nameof(EntryData))]
		public async STT.Task EntryTest(ExitCode expectedExitCode, string expectedOut, string expectedError, string configuration, string name, Arguments arguments) {
			using var fake = new ATF.FakeTextFile
			( @"alias.conf"
			, @"directory"
			, configuration
			);
			using var fakeFileDisposable = new ATF.FakeFile(name, string.Empty);
			using var environment = new ATF.FakeEnvironment(fakeFileDisposable.Mock.Object, arguments, fake.Mock.Object, new Effect(), S.Environment.CurrentDirectory, string.Empty);
			Assert.Equal(expectedExitCode, await Program.Entry(() => environment.Mock.Object));
			Assert.Equal(expectedOut, environment.StreamOut.ToString());
			Assert.Equal(expectedError, environment.StreamError.ToString());
		}
	}
}