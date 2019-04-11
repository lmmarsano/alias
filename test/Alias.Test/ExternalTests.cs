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
		  ( new AC.BindingDictionary(5)
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
		[ Theory
		, MemberData(nameof(ParseAcceptsData))
		]
		public void ParseAcceptsTest(Command expectedCommand, Arguments expectedArguments, uint index) {
			var alias = $@"alias{index}";
			var maybeOption = External.Parse(Configuration, alias);
			Assert.IsType<ST.Just<External>>(maybeOption);
			maybeOption.Select
			(option => {
				Assert.Equal(alias, option.Alias);
				Assert.Equal(expectedCommand, option.Command);
				Assert.Equal(expectedArguments, option.Arguments);
				return ST.Nothing.Value;
			}
			);
		}
		public static TheoryData<uint> ParseRejectsData { get; }
		= new TheoryData<uint> { 0, 5 };
		[ Theory
		, MemberData(nameof(ParseRejectsData))
		]
		public void ParseRejectsTest(uint index)
		=> Assert.IsType<ST.Nothing<External>>(External.Parse(Configuration, $@"alias{index}"));
		public static TheoryData<uint> OperateData { get; }
		= new TheoryData<uint> { 1, 2, 3, 4 };
		[ Theory
		, MemberData(nameof(OperateData))
		]
		public async STT.Task OperateTest(uint index) {
			var mock = new M.Mock<IOperation>();
			var maybeOption = External.Parse(Configuration, $@"alias{index}");
			Assert.IsType<ST.Just<External>>(maybeOption);
			var option = ((ST.Just<External>)maybeOption).Value;
			mock.Setup(op => op.External(M.It.IsAny<External>()))
			.Returns(Utility.TaskExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)).ConfigureAwait(false));
			mock.Verify(op => op.External(option));
		}
	}
}
