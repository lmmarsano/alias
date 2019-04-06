using S = System;
using SIO = System.IO;

namespace Alias {
	/// <inheritdoc/>
	/// <remarks>Wrapper class around <see cref='SIO.FileInfo'/> facilitates testing.</remarks>
	class FileInfo: IFileInfo {
		readonly SIO.FileInfo _fileInfo;
		/**
		 * <summary>
		 * Initializes a new instance of the <see cref='FileInfo'/> class, which acts as a wrapper for a file path.
		 * </summary>
		 * <param name="fileName">The fully qualified name of the new file, or the relative file name. Do not end the path with the directory separator character.</param>
		 * <exception cref='S.ArgumentNullException'><paramref name='fileName'/> is null.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='S.ArgumentException'>The file name is empty, contains only white spaces, or contains invalid characters.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>Access to <paramref name='fileName'/> is denied.</exception>
		 * <exception cref='SIO.PathTooLongException'>The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
		 * <exception cref='S.NotSupportedException'><paramref name='fileName'/> contains a colon (:) in the middle of the string.</exception>
		 */
		public FileInfo(string fileName) {
			_fileInfo = new SIO.FileInfo(fileName);
		}
		FileInfo(SIO.FileInfo fileInfo) {
			_fileInfo = fileInfo;
		}
		/// <inheritdoc/>
		public bool IsReadOnly {
			get => _fileInfo.IsReadOnly;
			set => _fileInfo.IsReadOnly = value;
		}
		/// <inheritdoc/>
		public bool Exists => _fileInfo.Exists;
		/// <inheritdoc/>
		public string DirectoryName => _fileInfo.DirectoryName;
		/// <inheritdoc/>
		public SIO.DirectoryInfo Directory => _fileInfo.Directory;
		/// <inheritdoc/>
		public long Length => _fileInfo.Length;
		/// <inheritdoc/>
		public string Name => _fileInfo.Name;
		/// <inheritdoc/>
		public string FullName => _fileInfo.FullName;

		/// <inheritdoc/>
		public IFileInfo CopyTo(string destinationFileName)
		=> new FileInfo(_fileInfo.CopyTo(destinationFileName));
		/// <inheritdoc/>
		public IFileInfo CopyTo(string destinationFileName, bool overwrite)
		=> new FileInfo(_fileInfo.CopyTo(destinationFileName, overwrite));
		/// <inheritdoc/>
		public SIO.FileStream Create() => _fileInfo.Create();
		/// <inheritdoc/>
		public SIO.Stream CreateStream() => _fileInfo.Create();
		/// <inheritdoc/>
		public SIO.StreamWriter CreateText() => _fileInfo.CreateText();
		/// <inheritdoc/>
		public void Delete() => _fileInfo.Delete();
		/// <inheritdoc/>
		public void MoveTo(string destinationFileName) => _fileInfo.MoveTo(destinationFileName);
		/// <inheritdoc/>
		public void MoveTo(string destinationFileName, bool overwrite)
		=> _fileInfo.MoveTo(destinationFileName, overwrite);
		/// <inheritdoc/>
		public SIO.FileStream Open(SIO.FileMode mode, SIO.FileAccess access) => _fileInfo.Open(mode, access);
		/// <inheritdoc/>
		public SIO.FileStream Open(SIO.FileMode mode, SIO.FileAccess access, SIO.FileShare share)
		=> _fileInfo.Open(mode, access, share);
		/// <inheritdoc/>
		public SIO.FileStream Open(SIO.FileMode mode) => _fileInfo.Open(mode);
		/// <inheritdoc/>
		public SIO.Stream OpenAsync(SIO.FileMode mode, SIO.FileAccess access, SIO.FileShare share = SIO.FileShare.Read)
		=> Effect.GetFileStream(FullName, mode, access, share);
		/// <inheritdoc/>
		public SIO.FileStream OpenRead() => _fileInfo.OpenRead();
		/// <inheritdoc/>
		public SIO.StreamReader OpenText() => _fileInfo.OpenText();
		/// <inheritdoc/>
		public SIO.FileStream OpenWrite() => _fileInfo.OpenWrite();
		/// <inheritdoc/>
		public IFileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
		=> new FileInfo(_fileInfo.Replace(destinationFileName, destinationBackupFileName, ignoreMetadataErrors));
		/// <inheritdoc/>
		public IFileInfo Replace(string destinationFileName, string destinationBackupFileName)
		=> new FileInfo(_fileInfo.Replace(destinationFileName, destinationBackupFileName));
	}
}
