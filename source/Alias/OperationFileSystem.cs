using S = System;
using SIO = System.IO;
using STT = System.Threading.Tasks;
using static System.Threading.Tasks.TaskExtensions;
using System.Linq;
using F = Functional;
using static Functional.Extension;
using AO = Alias.Option;
using AC = Alias.Configuration;

namespace Alias {
	/**
	 * <summary>
	 * Operations associated with each <see cref='Option.AbstractOption'/> implementation.
	 * </summary>
	 */
	class OperationFileSystem: Operation {
		public OperationFileSystem(IEnvironment environment, AC.Configuration configuration): base(environment, configuration) {}
		public override F.Result<STT.Task<ExitCode>> Reset(AO.Reset options) {
			throw new S.NotImplementedException();
			return base.Reset(options);
			// compare files (hash) before deletion
		}

		public override F.Result<STT.Task<ExitCode>> Restore(AO.Restore options) {
			throw new S.NotImplementedException();
			return base.Restore(options);
		}

		public override F.Result<STT.Task<ExitCode>> Set(AO.Set options) {
			throw new S.NotImplementedException();
			return base.Set(options)
			.SelectMany
			 ( serializeTask => {
			   	return Effect.CopyFile
			   	 ( Environment.ApplicationFile
			   	 , SIO.Path.Join(Environment.ApplicationDirectory, options.Name)
			   	 )
			   	.Select
			   	 ( copyTask
			   	   => STT.Task.WhenAll(new [] {copyTask, serializeTask})
			   	      .CombineAsync(serializeTask)
			   	 );
			 });
		}

		public override F.Result<STT.Task<ExitCode>> Unset(AO.Unset options) {
			throw new S.NotImplementedException();
			return base.Unset(options);
		}
	}
}