#nullable enable
using S = System;
using SCG = System.Collections.Generic;
using SIO = System.IO;
using STT = System.Threading.Tasks;
using SSP = System.Security.Permissions;
using Xunit;
using System.Linq;
using M = Moq;
using F = Functional;
using AT = Alias.Test;
using ATF = Alias.Test.Fixture;
using A = Alias;
using AC = Alias.ConfigurationData;
using AO = Alias.Option;
using Directory = System.String;
using Command = System.String;
using Argument = System.String;

namespace Alias.Test {
	using Arguments = SCG.IEnumerable<Argument>;
	public class OperationTests : S.IDisposable {
		readonly M.Mock<IEffect> _mockEffect;
		readonly ATF.FakeFile _fakeApp;
		readonly ATF.FakeTextFile _fakeConf;
		readonly ATF.FakeEnvironment _fakeEnv;
		readonly M.Mock<IEnvironment> _mockEnv;
		public OperationTests() {
			_mockEffect = new M.Mock<IEffect>();
			_fakeApp = new ATF.FakeFile(@"Application Name", @"Application Directory");
			_fakeConf = new ATF.FakeTextFile(string.Empty, string.Empty, string.Empty);
			_fakeEnv = new ATF.FakeEnvironment(_fakeApp.Mock.Object, Enumerable.Empty<string>(), _fakeConf.Mock.Object, _mockEffect.Object, @"Working Directory", string.Empty);
			_mockEnv = _fakeEnv.Mock;
		}
		public void Dispose() {
			Dispose(true);
			S.GC.SuppressFinalize(this);
		}
		protected virtual void Dispose(bool disposing) {
			if (disposing) {
				foreach (var item in new S.IDisposable[] {_fakeApp, _fakeConf, _fakeEnv}) {
					item.Dispose();
				}
			}
		}
		IOperation Operation(IEnvironment environment)
		=> new Operation(environment, ATF.Sample.Configuration);
		[Fact]
		public async STT.Task ListTest() {
			var environment = _mockEnv.Object;
			Assert.Equal(ExitCode.Success, await AT.Utility.FromOk(Operation(environment).List(new AO.List())));
			Assert.Equal
			( string.Join
			  ( S.Environment.NewLine
			  , new[]
			    { @"alias0 → command"
			    , @"alias1 → command arguments"
			    , @"alias2 → command arguments with spaces"
			    , @"""spaced alias"" → ""spaced command"" arguments"
			    , string.Empty
			    }
			  )
			, environment.StreamOut.ToString()
			);
		}
		[Fact]
		public STT.Task ListFailTest() {
			var mock = new M.Mock<SIO.TextWriter>();
			var inner = new SIO.IOException();
			mock.Setup(stream => stream.WriteLineAsync(M.It.IsAny<string>())).Throws(inner);
			_mockEnv.Setup(env => env.StreamOut).Returns(mock.Object);
			var environment = _mockEnv.Object;
			var option = new AO.List();
			return AT.Utility.FromOk(Operation(environment).List(option))
			.ContinueWith(task => {
				Assert.Equal(STT.TaskStatus.Faulted, task.Status);
				var error = (ListOperationException)task.Exception.InnerExceptions.Single();
				Assert.Equal(option, error.Option);
				Assert.Equal(inner, ((S.AggregateException)error.InnerException).InnerExceptions.Single());
			});
		}
		[Fact]
		public async STT.Task RestoreTest() {
			Assert.Equal
			( ExitCode.Success
			, await AT.Utility.FromOk(Operation(_mockEnv.Object).Restore(new AO.Restore()))
			);
		}
		[Fact]
		public async STT.Task ResetSuccess() {
			var environment = _mockEnv.Object;
			var configurationFile = environment.ConfigurationFile;
			_mockEffect.Setup(effect => effect.DeleteFile(configurationFile))
			.Returns(A.Utility.TaskExitSuccess);
			Assert.Equal
			( ExitCode.Success
			, await AT.Utility.FromOk(Operation(environment).Reset(new AO.Reset()))
			);
		}
		[Fact]
		public STT.Task ResetFailure() {
			var environment = _mockEnv.Object;
			var configurationFile = environment.ConfigurationFile;
			_mockEffect.Setup(effect => effect.DeleteFile(configurationFile))
			.Returns(STT.Task.FromException(new OperationIOException(configurationFile.FullName, SSP.FileIOPermissionAccess.AllAccess)));
			return AT.Utility.FromOk(Operation(environment).Reset(new AO.Reset()))
			.ContinueWith(task => {
				Assert.Equal(STT.TaskStatus.Faulted, task.Status);
				Assert.IsType<OperationIOException>(task.Exception.InnerExceptions.Single());
			});
		}
		(IEnvironment, IFileInfo) SetupWrite(AC.Configuration configuration, F.Result<STT.Task> writeOutcome) {
			var environment = _mockEnv.Object;
			var configurationFile = environment.ConfigurationFile;
			_mockEffect.Setup(effect => effect.WriteConfiguration(configuration, configurationFile))
			.Returns(writeOutcome);
			return (environment, configurationFile);
		}
		public static TheoryData<AC.Configuration, string, string, Arguments> SetData {
			get {
				var alias = @"alias";
				var alias0 = @"alias0";
				var alias1 = @"alias1";
				var command = @"command";
				var mixedArguments = new [] {@"with spaces", @"nonspace"};
				var emptyArguments = Enumerable.Empty<string>();
				var sameArguments = new [] {@"arguments"};
				var differentCommand = @"different command";
				return new TheoryData<AC.Configuration, string, string, Arguments>
				{ {ATF.Sample.EmptyConfiguration, alias, command, emptyArguments }
				, {ATF.Sample.EmptyConfiguration, alias, command, mixedArguments }
				, {ATF.Sample.Configuration, alias, command, emptyArguments }
				, {ATF.Sample.Configuration, alias, command, mixedArguments }
				, {ATF.Sample.Configuration, alias0, command, emptyArguments }
				, {ATF.Sample.Configuration, alias0, command, mixedArguments }
				, {ATF.Sample.Configuration, alias1, command, emptyArguments }
				, {ATF.Sample.Configuration, alias1, command, sameArguments }
				, {ATF.Sample.Configuration, alias1, command, mixedArguments }
				, {ATF.Sample.EmptyConfiguration, alias, differentCommand, emptyArguments }
				, {ATF.Sample.EmptyConfiguration, alias, differentCommand, mixedArguments }
				, {ATF.Sample.Configuration, alias, differentCommand, emptyArguments }
				, {ATF.Sample.Configuration, alias, differentCommand, mixedArguments }
				, {ATF.Sample.Configuration, alias0, differentCommand, emptyArguments }
				, {ATF.Sample.Configuration, alias0, differentCommand, mixedArguments }
				, {ATF.Sample.Configuration, alias1, differentCommand, emptyArguments }
				, {ATF.Sample.Configuration, alias1, differentCommand, sameArguments }
				, {ATF.Sample.Configuration, alias1, differentCommand, mixedArguments }
				};
			}
		}
		[Theory]
		[MemberData(nameof(SetData))]
		public async STT.Task SetSuccess(AC.Configuration configuration, string name, string command, SCG.IEnumerable<string> arguments) {
			var (environment, configurationFile) = SetupWrite(configuration, A.Utility.TaskExitSuccess);
			var option = new AO.Set(name, command, arguments);
			Assert.Equal
			( ExitCode.Success
			, await AT.Utility.FromOk
			  (new Operation(environment, configuration).Set(option))
			);
			Assert.True(configuration.Binding.ContainsKey(name));
			var entry = configuration.Binding[name];
			Assert.Equal(command, entry.Command);
			Assert.Equal(option.ArgumentString, entry.Arguments);
		}
		[Theory]
		[MemberData(nameof(SetData))]
		public STT.Task SetError(AC.Configuration configuration, string name, string command, SCG.IEnumerable<string> arguments) {
			var (environment, configurationFile) = SetupWrite(configuration, AT.Utility.TaskFault<ExitCode>());
			return AT.Utility.FromOk
			(new Operation(environment, configuration).Set(new AO.Set(name, command, arguments)))
			.ContinueWith(task => Assert.Equal(STT.TaskStatus.Faulted, task.Status));
		}
		[Theory]
		[MemberData(nameof(SetData))]
		public void SetFailure(AC.Configuration configuration, string name, string command, SCG.IEnumerable<string> arguments) {
			var (environment, configurationFile) = SetupWrite(configuration, new S.Exception());
			Assert.IsType<F.Error<STT.Task<ExitCode>>>
			(new Operation(environment, configuration).Set(new AO.Set(name, command, arguments)));
		}
		public static TheoryData<AC.Configuration, string, F.Result<STT.Task>> UnsetErrorData {
			get {
				var absentAlias = @"alias";
				var error = new S.Exception();
				return new TheoryData<AC.Configuration, string, F.Result<STT.Task>>
				{ {ATF.Sample.EmptyConfiguration, absentAlias, A.Utility.TaskExitSuccess}
				, {ATF.Sample.EmptyConfiguration, absentAlias, AT.Utility.TaskFaulted}
				, {ATF.Sample.EmptyConfiguration, absentAlias, error}
				, {ATF.Sample.Configuration, @"alias0", error}
				, {ATF.Sample.Configuration, absentAlias, A.Utility.TaskExitSuccess}
				, {ATF.Sample.Configuration, absentAlias, AT.Utility.TaskFaulted}
				, {ATF.Sample.Configuration, absentAlias, error}
				};
			}
		}
		[Theory]
		[MemberData(nameof(UnsetErrorData))]
		public void UnsetError(AC.Configuration configuration, string name, F.Result<STT.Task> writeOutcome) {
			var (environment, configurationFile) = SetupWrite(configuration, writeOutcome);
			Assert.IsType<F.Error<STT.Task<ExitCode>>>
			(new Operation(environment, configuration).Unset(new AO.Unset(name)));
		}
		[Fact]
		public async STT.Task UnsetSuccess() {
			var configuration = ATF.Sample.Configuration;
			var (environment, configurationFile) = SetupWrite(configuration, A.Utility.TaskExitSuccess);
			Assert.Equal
			( ExitCode.Success
			, await AT.Utility.FromOk(new Operation(environment, configuration).Unset(new AO.Unset(@"alias0")))
			);
		}
		[Fact]
		public STT.Task UnsetFailure() {
			var configuration = ATF.Sample.Configuration;
			var (environment, configurationFile) = SetupWrite(configuration, AT.Utility.TaskFaulted);
			return AT.Utility.FromOk
			(new Operation(environment, configuration).Unset(new AO.Unset(@"alias0")))
			.ContinueWith(task => Assert.Equal(STT.TaskStatus.Faulted, task.Status));
		}
		void SetupRun(Arguments arguments, F.Result<STT.Task<ExitCode>> runOutcome) {
			_mockEffect
			.Setup(effect => effect.RunCommand(M.It.IsAny<Directory>(), M.It.IsAny<Command>(), M.It.IsAny<F.Maybe<string>>()))
			.Returns(runOutcome);
			_mockEnv.Setup(env => env.Arguments).Returns(arguments);
			// _mockEnv.Setup(env => env.Effect).Returns(_mockEffect.Object);
		}
		public static TheoryData<F.Maybe<string>, Arguments, string> ExternalSuccessData {
			get {
				var alias0 = @"alias0";
				var alias1 = @"alias1";
				var alias2 = @"alias2";
				var empty = Enumerable.Empty<Argument>();
				var single = new [] {@"main"};
				var spaced = new [] {@"spaced main"};
				var pair = new [] {@"spaced main", @"main"};
				return new TheoryData<F.Maybe<string>, Arguments, string>
				{ {F.Nothing.Value, empty, alias0}
				, {@"main", single, alias0}
				, {@"""spaced main""", spaced, alias0}
				, {@"""spaced main"" main", pair, alias0}
				, {@"arguments", empty, alias1}
				, {@"arguments main", single, alias1}
				, {@"arguments ""spaced main""", spaced, alias1}
				, {@"arguments ""spaced main"" main", pair, alias1}
				, {@"arguments with spaces", empty, alias2}
				, {@"arguments with spaces main", single, alias2}
				, {@"arguments with spaces ""spaced main""", spaced, alias2}
				, {@"arguments with spaces ""spaced main"" main", pair, alias2}
				};
			}
		}
		[Theory]
		[MemberData(nameof(ExternalSuccessData))]
		public async STT.Task ExternalSuccess(F.Maybe<string> expected, Arguments arguments, string alias) {
			SetupRun(arguments, A.Utility.TaskExitSuccess);
			var environment = _mockEnv.Object;
			Assert.Equal
			( ExitCode.Success
			, await AT.Utility.FromOk
			  (Operation(environment)
			  .External(AT.Utility.FromJust(AO.External.Parse(ATF.Sample.Configuration, alias)))
			  )
			);
			_mockEffect.Verify
			( effect
			  => effect.RunCommand
			     ( environment.WorkingDirectory
			     , Alias.Utility.SafeQuote(ATF.Sample.Configuration.Binding[alias].Command)
			     , expected
			     )
			);
		}
		[Theory]
		[MemberData(nameof(ExternalSuccessData))]
		public void ExternalError(F.Maybe<string> _, Arguments arguments, string alias) {
			SetupRun(arguments, new SIO.IOException());
			Assert.IsType<F.Error<STT.Task<ExitCode>>>
			(Operation(_mockEnv.Object)
			.External(AT.Utility.FromJust(AO.External.Parse(ATF.Sample.Configuration, alias)))
			);
		}
		[Theory]
		[MemberData(nameof(ExternalSuccessData))]
		public STT.Task ExternalFailure(F.Maybe<string> expected, Arguments arguments, string alias) {
			var inner = new SIO.IOException();
			SetupRun(arguments, STT.Task.FromException<ExitCode>(inner));
			var option = AT.Utility.FromJust(AO.External.Parse(ATF.Sample.Configuration, alias));
			return AT.Utility.FromOk(Operation(_mockEnv.Object).External(option))
			.ContinueWith
			(task => {
				Assert.Equal(STT.TaskStatus.Faulted, task.Status);
				var error = (ExternalOperationException)task.Exception.InnerExceptions.Single();
				Assert.Equal(expected, error.Arguments);
				Assert.Equal(option, error.Option);
				Assert.Equal(inner, ((S.AggregateException)error.InnerException).InnerExceptions.Single());
			});
		}
	}
}