#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using Xunit;
using System.Linq;
using M = Moq;
using F = Functional;
using static Functional.Extension;
using AC = Alias.Configuration;
using AO = Alias.Option;
using Directory = System.String;
using Destination = System.String;
using Command = System.String;
using Argument = System.String;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<Argument>;
	public class OperationTests : S.IDisposable {
		static readonly AC.Configuration _configuration
		= new AC.Configuration
			(new SCG.Dictionary<Command, AC.CommandEntry>(4)
				{ { @"alias0", new AC.CommandEntry(@"command", null) }
				, { @"alias1", new AC.CommandEntry(@"command", @"arguments") }
				, { @"alias2", new AC.CommandEntry(@"command", @"arguments with spaces") }
				, { @"spaced alias", new AC.CommandEntry(@"spaced command", @"arguments") }
				}
			);
		readonly M.Mock<IEffect> _mockEffect;
		readonly SIO.StringWriter _streamOut;
		readonly SIO.StringWriter _streamError;
		readonly M.Mock<IEnvironment> _mockEnv;
		public OperationTests() {
			_mockEffect = new M.Mock<IEffect>();
			_streamOut = new SIO.StringWriter();
			_streamError = new SIO.StringWriter();
			_mockEnv = new M.Mock<IEnvironment>();
			_mockEnv.Setup(env => env.ApplicationDirectory).Returns(@"Application Directory");
			_mockEnv.Setup(env => env.ApplicationName).Returns(@"Application Name");
			_mockEnv.Setup(env => env.StreamOut).Returns(_streamOut);
			_mockEnv.Setup(env => env.StreamError).Returns(_streamError);
			_mockEnv.Setup(env => env.WorkingDirectory).Returns(@"Working Directory");
		}
		public void Dispose() {
			Dispose(true);
			S.GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				_streamOut.Dispose();
				_streamError.Dispose();
			}
		}
		IOperation Operation(IEnvironment environment)
		=> new Operation(environment, _configuration);
		[Fact]
		public void ListTest() {
			var environment = _mockEnv.Object;
			Assert.Equal(F.Factory.Result(ExitCode.Success), Operation(environment).List(new AO.List()));
			Assert.Equal
			(string.Join
				(S.Environment.NewLine
				, new[]
					{ @"alias0 → command"
					, @"alias1 → command arguments"
					, @"""spaced alias"" → ""spaced command"" arguments"
					, string.Empty
					}
				)
			, environment.StreamOut.ToString()
			);
		}
		[Fact]
		public void ListFailTest() {
			var mock = new M.Mock<SIO.TextWriter>();
			var inner = new SIO.IOException();
			mock.Setup(stream => stream.WriteLine(M.It.IsAny<string>())).Throws(inner);
			_mockEnv.Setup(env => env.StreamOut).Returns(mock.Object);
			var environment = _mockEnv.Object;
			var option = new AO.List();
			var sut = Operation(environment).List(option);
			Assert.IsType<F.Error<ExitCode>>(sut);
			var error = (ListOperationException)((F.Error<ExitCode>)sut).Value;
			Assert.Equal(option, error.Option);
			Assert.Equal(inner, error.InnerException);
		}
		[Fact]
		public void SetNew() {
			//Given

			//When

			//Then
		}
		[Fact]
		public void SetNewFileExists() {
			//Given

			//When

			//Then
		}
		[Fact]
		public void SetChange() {
			//Given

			//When

			//Then
		}
		public static TheoryData<string, Arguments, string> ExternalArgumentsData { get; }
		= new TheoryData<string, Arguments, string>
			{ {@"main", new [] {@"main"}, @"alias0"}
			, {@"""spaced main""", new [] {@"spaced main"}, @"alias0"}
			, {@"""spaced main"" main", new [] {@"spaced main", @"main"}, @"alias0"}
			, {@"arguments", Enumerable.Empty<Argument>(), @"alias1"}
			, {@"arguments main", new [] {@"main"}, @"alias1"}
			, {@"arguments ""spaced main""", new [] {@"spaced main"}, @"alias1"}
			, {@"arguments ""spaced main"" main", new [] {@"spaced main", @"main"}, @"alias1"}
			, {@"arguments with spaces", Enumerable.Empty<Argument>(), @"alias2"}
			, {@"arguments with spaces main", new [] {@"main"}, @"alias2"}
			, {@"arguments with spaces ""spaced main""", new [] {@"spaced main"}, @"alias2"}
			, {@"arguments with spaces ""spaced main"" main", new [] {@"spaced main", @"main"}, @"alias2"}
			};
		[Theory]
		[MemberData(nameof(ExternalArgumentsData))]
		public void ExternalArgumentsTest(string expected, Arguments arguments, string alias) {
			_mockEffect.Setup(effect => effect.RunCommand(M.It.IsAny<Directory>(), M.It.IsAny<Command>(), M.It.IsAny<string>()))
			.Returns(F.Factory.Result(ExitCode.Success));
			_mockEnv.Setup(env => env.Arguments).Returns(arguments);
			var effect = _mockEffect.Object;
			_mockEnv.Setup(env => env.Effect).Returns(effect);
			var environment = _mockEnv.Object;
			var maybeExternal = AO.External.Parse(_configuration, alias);
			Assert.IsType<F.Just<AO.External>>(maybeExternal);
			maybeExternal.Select
			(external => {
				Assert.Equal(F.Factory.Result(ExitCode.Success), Operation(environment).External(external));
				return F.Nothing.Value;
			});
			M.Mock.Get(effect)
			.Verify
			 (effect
				=> effect.RunCommand
					 (environment.WorkingDirectory
					 , Utility.SafeQuote(_configuration.Binding[alias].Command)
					 , expected
					 )
			 );
		}
		[Fact]
		public void ExternalNoArgumentTest() {
			_mockEffect
			.Setup(effect => effect.RunCommand(M.It.IsAny<Directory>(), M.It.IsAny<Command>()))
			.Returns(F.Factory.Result(ExitCode.Success));
			_mockEnv.Setup(env => env.Arguments).Returns(Enumerable.Empty<Argument>());
			var alias = @"alias0";
			var effect = _mockEffect.Object;
			_mockEnv.Setup(env => env.Effect).Returns(effect);
			var environment = _mockEnv.Object;
			var maybeExternal = AO.External.Parse(_configuration, alias);
			Assert.IsType<F.Just<AO.External>>(maybeExternal);
			maybeExternal.Select
			(external => {
				Assert.Equal(F.Factory.Result(ExitCode.Success), Operation(environment).External(external));
				return F.Nothing.Value;
			});
			M.Mock.Get(effect)
			.Verify
			 (effect
				=> effect.RunCommand
					 (environment.WorkingDirectory
					 , Utility.SafeQuote(_configuration.Binding[alias].Command)
					 )
			 );
		}
		[Fact]
		public void ExternalFailTest() {
			var alias = @"alias0";
			var inner = new SIO.IOException();
			_mockEffect
			.Setup(effect => effect.RunCommand(M.It.IsAny<string>(), M.It.IsAny<string>()))
			.Returns(inner);
			_mockEnv.Setup(env => env.Effect).Returns(_mockEffect.Object);
			var option = ((F.Just<Option.External>)AO.External.Parse(_configuration, alias)).Value;
			var sut = Operation(_mockEnv.Object).External(option);
			Assert.IsType<F.Error<ExitCode>>(sut);
			var error = (ExternalOperationException)((F.Error<ExitCode>)sut).Value;
			Assert.IsType<F.Nothing<string>>(error.Arguments);
			Assert.Equal(option, error.Option);
			Assert.Equal(inner, error.InnerException);
		}
		// .Setup(effect => effect.CopyFile(M.It.IsAny<SIO.FileInfo>(), M.It.IsAny<Destination>())).Returns(resultNothing);
	}
}