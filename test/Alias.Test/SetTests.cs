#nullable enable
using SIO = System.IO;
using SCG = System.Collections.Generic;
using STT = System.Threading.Tasks;
using System.Linq;
using Xunit;
using M = Moq;
using F = Functional;
using static Functional.Extension;
using AT = Alias.Test;
using AO = Alias.Option;
using Name = System.String;
using Command = System.String;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class SetTests {
		static Arguments Empty { get; } = Enumerable.Empty<string>();
		public static TheoryData<Name, Command, Arguments> InvalidData { get; }
		= new TheoryData<Name, Command, Arguments>
		  { {@"", @"", Empty}
			, {@"alias", @"", Empty}
			, {SIO.Path.DirectorySeparatorChar.ToString(), @"command", Empty}
			, {@"alias", "\0", Empty}
			};
		public static TheoryData<Name, Command, Arguments> ValidData { get; }
		= new TheoryData<Name, Command, Arguments>
		  { {@"alias", @"command", Empty}
			, {@"alias", @"command", new [] {@"argument"}}
			, {@"alias", @"command", Enumerable.Repeat(@"argument", 2)}
			};
		[Theory]
		[MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))]
		public void CtorTest(Name name, Command command, Arguments arguments) {
			var option = new AO.Set(name, command, arguments);
			Assert.Equal(name, option.Name);
			Assert.Equal(command, option.Command);
			Assert.Equal(arguments, option.Arguments);
		}
		[Theory]
		[MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))]
		public async STT.Task SetTest(Name name, Command command, Arguments arguments) {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Set(name, command, arguments);
			mock.Setup(op => op.Set(M.It.IsAny<AO.Set>())).Returns(AT.Fixture.FakeTasks.ExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)));
			mock.Verify(op => op.Set(option));
		}
		[Theory]
		[MemberData(nameof(InvalidData))]
		public void ValidationFails(Name name, Command command, Arguments arguments)
		=> Assert.IsType<F.Error<AO.AbstractOption>>(new AO.Set(name, command, arguments).Validation);
		[Theory]
		[MemberData(nameof(ValidData))]
		public void ValidationSucceeds(Name name, Command command, Arguments arguments)
		=> Assert.IsType<F.Ok<AO.AbstractOption>>(new AO.Set(name, command, arguments).Validation);
	}
}