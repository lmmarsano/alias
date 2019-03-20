#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using System.Linq;
using Xunit;
using ATF = Alias.Test.Fixture;
using M = Moq;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class ProgramTests {
		public static TheoryData<ExitCode, string, string, string, Arguments> EntryData
		= new TheoryData<ExitCode, string, string, string, Arguments>
		  { { ExitCode.Error
		    , string.Empty
		    , string.Join
				  ( S.Environment.NewLine
					, new []
{ @"testhost 16.0.1"
, @"© Microsoft Corporation. All rights reserved."
, string.Empty
, @"  list       List aliases."
, string.Empty
, @"  reset      Remove all aliases."
, string.Empty
, @"  restore    Recreate file system for configured aliases."
, string.Empty
, @"  set        Add or change an alias."
, string.Empty
, @"  unset      Remove configured alias."
, string.Empty
, @"  help       Display more information on a specific command."
, string.Empty
, @"  version    Display version information."
, string.Empty
, string.Empty
}
          )
		    , @"alias"
		    , new [] {@"help"}
		    }
		  };
		M.Mock<IEffect> MockEffect { get; } = new M.Mock<IEffect>();
		[Theory]
		[MemberData(nameof(EntryData))]
		public void EntryTest(ExitCode expectedExitCode, string expectedOut, string expectedError, string name, Arguments arguments) {
			using var fake = new ATF.FakeConfiguration(
@"{ ""binding"":
  { ""bound"":
    { ""command"": ""name""
    , ""arguments"": ""from configuration""
    }
  }
}");
			using var environment = new ATF.FakeEnvironment(name, arguments, fake.Mock.Object, MockEffect.Object, string.Empty, S.Environment.CurrentDirectory, string.Empty, string.Empty);
			Assert.Equal(expectedExitCode, Program.Entry(() => environment));
			Assert.Equal(expectedOut, environment.StreamOut.ToString());
			Assert.Equal(expectedError, environment.StreamError.ToString());
		}
	}
}