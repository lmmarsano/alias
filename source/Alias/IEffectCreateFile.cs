using SIO = System.IO;
using F = Functional;

namespace Alias {
	interface IEffectCreateFile {
		/**
		 * <summary>
		 * Copy file to destination.
		 * </summary>
		 * <param name="file">File to copy.</param>
		 * <param name="destination">Destination to copy file to.</param>
		 * <returns>Result of nothing or error.</returns>
		 */
		F.Result<F.Nothing> CopyFile(IFileInfo file, string destination);
	}
}