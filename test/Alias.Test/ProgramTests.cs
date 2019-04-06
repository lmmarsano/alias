#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using Xunit;
using ATF = Alias.Test.Fixture;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class ProgramTests {
		const string _newLine = @"
";
		static string NormalizeLineEnd(string input)
		=> Utility.NormalizeLineEnd(_newLine, input);
		static readonly string _defaultOutput = NormalizeLineEnd(@"testhost 16.0.1
© Microsoft Corporation. All rights reserved.

ERROR(S):
  No verb selected.

  list       List aliases.

  reset      Remove all aliases.

  restore    Recreate file system for configured aliases.

  set        Add or change an alias.

  unset      Remove configured alias.

  help       Display more information on a specific command.

  version    Display version information.

");
		static readonly string _helpOutput = NormalizeLineEnd(@"testhost 16.0.1
© Microsoft Corporation. All rights reserved.

  list       List aliases.

  reset      Remove all aliases.

  restore    Recreate file system for configured aliases.

  set        Add or change an alias.

  unset      Remove configured alias.

  help       Display more information on a specific command.

  version    Display version information.

");
		static readonly string _setUsageOutput = NormalizeLineEnd(@"testhost 16.0.1
© Microsoft Corporation. All rights reserved.
USAGE:
Set mklink.exe as an alias to mklink built into cmd:
  testhost set mklink.exe cmd /c mklink

  --help                Display this help screen.

  --version             Display version information.

  name (pos. 0)         Required. Name for the alias.

  command (pos. 1)      Required. Command the alias invokes.

  arguments (pos. 2)    Arguments alias invokes with command.

");
		const string _configuration = @"{ ""binding"":
  { ""bound"":
    { ""command"": ""name""
    , ""arguments"": ""from configuration""
    }
  }
}";
		public static TheoryData<ExitCode, string, string, string, string, Arguments> EntryData { get; }
		= new TheoryData<ExitCode, string, string, string, string, Arguments>
		  { { ExitCode.Error, string.Empty, _defaultOutput, _configuration, @"alias", Enumerable.Empty<string>() }
		  , { ExitCode.Success, string.Empty, _helpOutput, _configuration, @"alias", new [] {@"help"} }
		  , { ExitCode.Error, string.Empty, NormalizeLineEnd(@"Unable to process file: directory\alias.conf
Unexpected end of content while loading JObject. Path 'binding', line 1, position 12.
"), @"{ ""binding"":", @"alias", Enumerable.Empty<string>() }
		  , { ExitCode.Success, string.Empty, _setUsageOutput, _configuration, @"alias", new [] {@"help", @"set"} }
		  };
		[ Theory
		, MemberData(nameof(EntryData))
		]
		public async STT.Task EntryTest(ExitCode expectedExitCode, string expectedOut, string expectedError, string configuration, string name, Arguments arguments) {
			using var fake = new ATF.FakeTextFile
			(@"alias.conf"
			, @"directory"
			, configuration
			);
			using var fakeFileDisposable = new ATF.FakeFile(name, string.Empty);
			using var environment = new ATF.FakeEnvironment(fakeFileDisposable.Mock.Object, arguments, fake.Mock.Object, new Effect(), S.Environment.CurrentDirectory, string.Empty);
			Assert.Equal(expectedExitCode, await Program.Entry(() => environment.Mock.Object).ConfigureAwait(false));
			Assert.Equal(expectedOut, environment.StreamOut.ToString());
			Assert.Equal(expectedError, environment.StreamError.ToString());
		}
	}
}
