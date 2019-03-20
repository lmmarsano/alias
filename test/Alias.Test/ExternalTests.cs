#nullable enable
using SCG = System.Collections.Generic;
using AO = Alias.Option;
using AC = Alias.Configuration;
using Xunit;
using M = Moq;
using F = Functional;
using System.Linq;
using Command = System.String;
using Argument = System.String;

namespace Alias.Option.Test {
	using Arguments = SCG.IEnumerable<Argument>;
	public class ExternalTests {
		static AC.Configuration Configuration { get; }
		= new AC.Configuration
		  ( new SCG.Dictionary<Command, AC.CommandEntry>(5)
			  { { @"alias0", new AC.CommandEntry(string.Empty, null) }
				, { @"alias1", new AC.CommandEntry(@"command", null) }
				, { @"alias2", new AC.CommandEntry(@"command", string.Empty) }
				, { @"alias3", new AC.CommandEntry(@"command", @"arguments") }
				, { @"alias4", new AC.CommandEntry(@"alias3", @"more arguments") }
				}
			);
		public static TheoryData<Command, Arguments, uint> ParseAcceptsData { get; }
		= new TheoryData<Command, Arguments, uint>
		  { {@"command", Enumerable.Empty<string>(), 1}
			, {@"command", Enumerable.Empty<string>(), 2}
			, {@"command", new [] {@"arguments"}, 3}
			, {@"alias3", new [] {@"more arguments"}, 4}
			};
		[Theory]
		[MemberData(nameof(ParseAcceptsData))]
		public void ParseAcceptsTest(Command expectedCommand, Arguments expectedArguments, uint index) {
			var alias = $@"alias{index}";
			var maybeOption = AO.External.Parse(Configuration, alias);
			Assert.IsType<F.Just<AO.External>>(maybeOption);
			maybeOption.Select
			( option => {
			  	Assert.Equal(alias, option.Alias);
			  	Assert.Equal(expectedCommand, option.Command);
			  	Assert.Equal(expectedArguments, option.Arguments);
			  	return F.Nothing.Value;
			  }
			);
		}
		public static TheoryData<uint> ParseRejectsData { get; }
		= new TheoryData<uint>{0, 5};
		[Theory]
		[MemberData(nameof(ParseRejectsData))]
		public void ParseRejectsTest(uint index)
		=> Assert.IsType<F.Nothing<AO.External>>(AO.External.Parse(Configuration, $@"alias{index}"));
		public static TheoryData<uint> OperateData { get; }
		= new TheoryData<uint>{1, 2, 3, 4};
		[Theory]
		[MemberData(nameof(OperateData))]
		public void OperateTest(uint index) {
			var mock = new M.Mock<IOperation>();
			var maybeOption = AO.External.Parse(Configuration, $@"alias{index}");
			Assert.IsType<F.Just<AO.External>>(maybeOption);
			maybeOption.Select
			( option => {
					mock.Setup(op => op.External(M.It.IsAny<AO.External>())).Returns(ExitCode.Success);
			    Assert.Equal(ExitCode.Success, option.Operate(mock.Object));
			    mock.Verify(op => op.External(option));
					return F.Nothing.Value;
				}
			);
		}
	}
}