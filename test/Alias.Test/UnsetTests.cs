using STT = System.Threading.Tasks;
using Xunit;
using M = Moq;
using ST = LMMarsano.SumType;
using A = Alias;
using AT = Alias.Test;
using AO = Alias.Option;
using Name = System.String;

namespace Alias.Option.Test {
	public class UnsetTests {
		public static TheoryData<Name> InvalidData { get; }
		= new TheoryData<Name> { @"", AT.Utility.DirectorySeparator };
		public static TheoryData<Name> ValidData { get; }
		= new TheoryData<Name> { @"alias" };
		[ Theory
		, MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))
		]
		public void CtorTest(Name name) => Assert.Equal(name, new AO.Unset(name).Name);
		[ Theory
		, MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))
		]
		public async STT.Task UnsetTest(Name name) {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Unset(name);
			mock.Setup(op => op.Unset(M.It.IsAny<AO.Unset>())).Returns(A.Utility.TaskExitSuccess);
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(option.Operate(mock.Object)).ConfigureAwait(false));
			mock.Verify(op => op.Unset(option));
		}
		[ Theory
		, MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))
		]
		public void ValidationSucceeds(Name name)
		=> Assert.IsType<ST.Ok<AO.AbstractOption>>(new AO.Unset(name).Validation);
	}
}
