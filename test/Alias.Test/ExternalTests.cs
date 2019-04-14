#nullable enable
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using Xunit;
using M = Moq;
using ST = LMMarsano.SumType;
using AT = Alias.Test;
using AC = Alias.ConfigurationData;
using Command = System.String;
using Argument = System.String;

namespace Alias.Option.Test {
	using Arguments = SCG.IEnumerable<Argument>;
	public class ExternalTests {
		static AC.Configuration Configuration { get; }
		= new AC.Configuration
		  ( new AC.BindingDictionary
		    ( new SCG.Dictionary<string, AC.CommandEntry>
		      { { @"alias0", new AC.CommandEntry(string.Empty, null) }
		      , { @"alias1", new AC.CommandEntry(@"command", null) }
		      , { @"alias2", new AC.CommandEntry(@"command", string.Empty) }
		      , { @"alias3", new AC.CommandEntry(@"command", @"arguments") }
		      , { @"alias4", new AC.CommandEntry(@"alias3", @"more arguments") }
		      }
		    )
		  );
		public static TheoryData<Command, Arguments, uint> ParseSucceedsData { get; }
		= new TheoryData<Command, Arguments, uint>
		  { {@"command", Enumerable.Empty<string>(), 1}
		  , {@"command", Enumerable.Empty<string>(), 2}
		  , {@"command", new [] {@"arguments"}, 3}
		  , {@"command", new [] {@"arguments", @"more arguments"}, 4}
		  };
		[ Theory
		, MemberData(nameof(ParseSucceedsData))
		]
		public void ParseSucceeds(Command expectedCommand, Arguments expectedArguments, uint index) {
			var alias = $@"alias{index}";
			var option = AT.Utility.FromOk(AT.Utility.FromJust(External.Parse(Configuration, alias)));
			Assert.Equal(alias, option.Alias);
			Assert.Equal(expectedCommand, option.Command);
			Assert.Equal(expectedArguments, option.Arguments);
		}
		public static TheoryData<uint> ParseNothingData { get; }
		= new TheoryData<uint> { 0, 5 };
		[ Theory
		, MemberData(nameof(ParseNothingData))
		]
		public void ParseNothing(uint index)
		=> Assert.IsType<ST.Nothing<ST.Result<External>>>(External.Parse(Configuration, $@"alias{index}"));
		static AC.Configuration CyclicConfiguration { get; }
		= new AC.Configuration
		  ( new AC.BindingDictionary(Configuration.Binding)
		    {{ @"command", new AC.CommandEntry(@"command", null)}}
		  );
		public static TheoryData<Command> ParseErrorData { get; }
		= new TheoryData<Command> { "command", "alias1", "alias2", "alias3", "alias4" };
		[ Theory
		, MemberData(nameof(ParseErrorData))
		]
		public void ParseError(Command alias)
		=> Assert.IsType<CyclicBindingException>(AT.Utility.FromError(AT.Utility.FromJust(External.Parse(CyclicConfiguration, alias))));
		public static TheoryData<uint> OperateData { get; }
		= new TheoryData<uint> { 1, 2, 3, 4 };
		[ Theory
		, MemberData(nameof(OperateData))
		]
		public async STT.Task OperateTest(uint index) {
			var mock = new M.Mock<IOperation>();
			var maybePossibleOption = External.Parse(Configuration, $@"alias{index}");
			var option = AT.Utility.FromOk(AT.Utility.FromJust(maybePossibleOption));
			mock.Setup(op => op.External(M.It.IsAny<External>()))
			.Returns(Utility.TaskExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)).ConfigureAwait(false));
			mock.Verify(op => op.External(option));
		}
	}
}
