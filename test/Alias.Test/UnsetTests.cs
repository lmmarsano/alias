#nullable enable
using SIO = System.IO;
using SCG = System.Collections.Generic;
using System.Linq;
using Xunit;
using F = Functional;
using M = Moq;
using AO = Alias.Option;
using Name = System.String;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<string>;
	public class UnsetTests {
		public static TheoryData<Name> InvalidData { get; }
		= new TheoryData<Name>{@"", SIO.Path.DirectorySeparatorChar.ToString()};
		public static TheoryData<Name> ValidData { get; }
		= new TheoryData<Name>{@"alias"};
		[Theory]
		[MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))]
		public void CtorTest(Name name) => Assert.Equal(name, new AO.Unset(name).Name);
		[Theory]
		[MemberData(nameof(InvalidData)), MemberData(nameof(ValidData))]
		public void UnsetTest(Name name) {
			var mock = new M.Mock<IOperation>();
			var option = new AO.Unset(name);
			mock.Setup(op => op.Unset(M.It.IsAny<AO.Unset>())).Returns(ExitCode.Success);
			Assert.Equal(ExitCode.Success, option.Operate(mock.Object));
			mock.Verify(op => op.Unset(option));
		}
		[Theory]
		[MemberData(nameof(InvalidData))]
		public void ValidationFails(Name name)
		=> Assert.IsType<F.Error<AO.AbstractOption>>(new AO.Unset(name).Validation);
		[Theory]
		[MemberData(nameof(ValidData))]
		public void ValidationSucceeds(Name name)
		=> Assert.IsType<F.Ok<AO.AbstractOption>>(new AO.Unset(name).Validation);
	}
}