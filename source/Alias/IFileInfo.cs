using S = System;
using SIO = System.IO;

namespace Alias {
	/**
	 * <summary>
	 * Provides properties and instance methods for the creation, copying, deletion, moving, and opening of files, and aids in the creation of System.IO.FileStream objects.
	 * </summary>
	 */
	interface IFileInfo {
		/**
		 * <summary>
		 * Gets or sets a value that determines if the current file is read only.
		 * </summary>
		 * <value>true if the current file is read only; otherwise, false.</value>
		 * <exception cref='SIO.FileNotFoundException'>The file described by the current <see cref='IFileInfo'/> object could not be found.</exception>
		 * <exception cref='SIO.IOException'>An I/O error occurred while opening the file.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>
		 * <list>
		 * <item><description>This operation is not supported on the current platform.</description></item>
		 * <item><description>The caller does not have the required permission.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.ArgumentException'>The user does not have write permission, but attempted to set this property to false.</exception>
		 */
		bool IsReadOnly { get; set; }
		/**
		 * <summary>
		 * Gets a value indicating whether a file exists.
		 * </summary>
		 * <value>true if the file exists; false if the file does not exist or if the file is a directory.</value>
		 */
		bool Exists { get; }
		/**
		 * <summary>
		 * Gets a string representing the directory&#39;s full path.
		 * </summary>
		 * <value>A string representing the directory&#39;s full path.</value>
		 * <exception cref='S.ArgumentNullException'>null was passed in for the directory name.</exception>
		 * <exception cref='S.IO.PathTooLongException'>The fully qualified path is 260 or more characters.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 */
		string DirectoryName { get; }
		/**
		 * <summary>
		 * Gets an instance of the parent directory.
		 * </summary>
		 * <value>A System.IO.DirectoryInfo object representing the parent directory of this file.</value>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 */
		SIO.DirectoryInfo Directory { get; }
		/**
		 * <summary>
		 * Gets the size, in bytes, of the current file.
		 * </summary>
		 * <value>The size of the current file in bytes.</value>
		 * <exception cref='SIO.IOException'>System.IO.FileSystemInfo.Refresh cannot update the state of the file or directory.</exception>
		 * <exception cref='SIO.FileNotFoundException'>
		 * <list>
		 * <item><description>The file does not exist.</description></item>
		 * <item><description></description>The Length property is called for a directory.</item>
		 * </list>
		 * </exception>
		 */
		long Length { get; }
		/**
		 * <summary>
		 * Gets the name of the file.
		 * </summary>
		 * <value>The name of the file.</value>
		 */
		string Name { get; }
		/**
		 * <summary>
		 * Gets the full path of the directory or file.
		 * </summary>
		 * <value>A string containing the full path.</value>
		 * <exception cref='SIO.PathTooLongException>The fully qualified path and file name is 260 or more characters.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 */
		string FullName { get; }
		/**
		 * <summary>
		 * Copies an existing file to a new file, disallowing the overwriting of an existing file.
		 * </summary>
		 * <param name="destinationFileName">The name of the new file to copy to.</param>
		 * <returns>A new file with a fully qualified path.</returns>
		 * <exception cref='S.ArgumentException'><paramref name="destinationFileName"/> is empty, contains only white spaces, or contains invalid characters.</exception>
		 * <exception cref='SIO.IOException'>An error occurs, or the destination file already exists.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='S.ArgumentNullException'><paramref name="destinationFileName"/> is null.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>A directory path is passed in, or the file is being moved to a different drive.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The directory specified in <paramref name="destinationFileName"/> does not exist.</exception>
		 * <exception cref='SIO.PathTooLongException'>The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
		 * <exception cref='S.NotSupportedException'><paramref name="destinationFileName"/> contains a colon (:) within the string but does not specify the volume.</exception>
		 */
		IFileInfo CopyTo(string destinationFileName);
		/**
		 * <summary>
		 * Copies an existing file to a new file, allowing the overwriting of an existing file.
		 * </summary>
		 * <param name="destinationFileName">The name of the new file to copy to.</param>
		 * <param name="overwrite">true to allow an existing file to be overwritten; otherwise, false.</param>
		 * <returns>A new file, or an <paramref name='overwrite'/> of an existing file if <paramref name='overwrite'/> is true. If the file exists and <paramref name='overwrite'/> is false, an System.IO.IOException is thrown.</returns>
		 * <exception cref='S.ArgumentException'><paramref name="destinationFileName"/> is empty, contains only white spaces, or contains invalid characters.</exception>
		 * <exception cref='SIO.IOException'>An error occurs, or the destination file already exists and <paramref name='overwrite'/> is false.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='S.ArgumentNullException'><paramref name="destinationFileName"/> is null.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>A directory path is passed in, or the file is being moved to a different drive.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The directory specified in <paramref name="destinationFileName"/> does not exist.</exception>
		 * <exception cref='SIO.PathTooLongException'>The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
		 * <exception cref='S.NotSupportedException'><paramref name="destinationFileName"/> contains a colon (:) within the string but does not specify the volume.</exception>
		 */
		IFileInfo CopyTo(string destinationFileName, bool overwrite);
		/**
		 * <summary>
		 * Creates a file.
		 * </summary>
		 * <returns>A new file.</returns>
		 */
		SIO.FileStream Create();
		/**
		 * <summary>
		 * Creates a System.IO.StreamWriter that writes a new text file.
		 * </summary>
		 * <returns>A new StreamWriter.</returns>
		 * <exception cref='S.UnauthorizedAccessException'>The file name is a directory.</exception>
		 * <exception cref='SIO.IOException'>The disk is read-only.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 */
		SIO.StreamWriter CreateText();
		/**
		 * <summary>
		 * Permanently deletes a file.
		 * </summary>
		 * <exception cref='SIO.IOException'>
		 * <list>
		 * <item><description>The target file is open or memory-mapped on a computer running Microsoft Windows NT.</description></item>
		 * <item><description>There is an open handle on the file, and the operating system is Windows XP or earlier. This open handle can result from enumerating directories and files. For more information, see How to: Enumerate Directories and Files.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>The path is a directory.</exception>
		 */
		void Delete();
		/**
		 * <summary>
		 * Moves a specified file to a new location, providing the option to specify a new file name.
		 * </summary>
		 * <param name="destinationFileName">The path to move the file to, which can specify a different file name.</param>
		 * <exception cref='S.ArgumentNullException'><paramref name="destinationFileName"/> is null.</exception>
		 * <exception cref='S.ArgumentException'><paramref name="destinationFileName"/> is empty, contains only white spaces, or contains invalid characters.</exception>
		 * <exception cref='SIO.IOException'>An I/O error occurs, such as the destination file already exists or the destination device is not ready.</exception>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='S.UnauthorizedAccessException'><paramref name="destinationFileName"/> is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='SIO.PathTooLongException'>The specified path, file name, or both exceed the system-defined maximum length. For example, on Windows-based platforms, paths must be less than 248 characters, and file names must be less than 260 characters.</exception>
		 * <exception cref='S.NotSupportedException'><paramref name="destinationFileName"/> contains a colon (:) in the middle of the string.</exception>
		 */
		void MoveTo(string destinationFileName);
		void MoveTo(string destinationFileName, bool overwrite);
		/**
		 * <summary>
		 * Opens a file in the specified mode with read, write, or read/write access.
		 * </summary>
		 * <param name="mode">A System.IO.FileMode constant specifying the mode (for example, Open or Append) in which to open the file.</param>
		 * <param name="access">A System.IO.FileAccess constant specifying whether to open the file with Read, Write, or ReadWrite file access.</param>
		 * <returns>A System.IO.FileStream object opened in the specified mode and access, and unshared.</returns>
		 * <exception cref='S.Security.SecurityException: The caller does not have the required permission.'></exception>
		 * <exception cref='SIO.FileNotFoundException'>The file is not found.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>path is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='SIO.IOException'>The file is already open.</exception>
		 */
		SIO.FileStream Open(SIO.FileMode mode, SIO.FileAccess access);
		/**
		 * <summary>
		 * Opens a file in the specified mode with read, write, or read/write access and the specified sharing option.
		 * </summary>
		 * <param name="mode">A System.IO.FileMode constant specifying the mode (for example, Open or Append) in which to open the file.</param>
		 * <param name="access">A System.IO.FileAccess constant specifying whether to open the file with Read, Write, or ReadWrite file access.</param>
		 * <param name="share">A System.IO.FileShare constant specifying the type of access other FileStream objects have to this file.</param>
		 * <returns>A System.IO.FileStream object opened with the specified mode, access, and sharing options.</returns>
		 * <exception cref='S.Security.SecurityException: The caller does not have the required permission.'></exception>
		 * <exception cref='SIO.FileNotFoundException'>The file is not found.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>path is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='SIO.IOException'>The file is already open.</exception>
		 */
		SIO.FileStream Open(SIO.FileMode mode, SIO.FileAccess access, SIO.FileShare share);
		/**
		 * <summary>
		 * Opens an asynchronous file stream in the specified mode with read, write, or read/write access and the specified sharing option.
		 * </summary>
		 * <param name="mode">A System.IO.FileMode constant specifying the mode (for example, Open or Append) in which to open the file.</param>
		 * <param name="access">A System.IO.FileAccess constant specifying whether to open the file with Read, Write, or ReadWrite file access.</param>
		 * <param name="share">A System.IO.FileShare constant specifying the type of access other FileStream objects have to this file.</param>
		 * <returns>A System.IO.FileStream object opened with the specified mode, access, and sharing options.</returns>
		 * <exception cref='S.Security.SecurityException: The caller does not have the required permission.'></exception>
		 * <exception cref='SIO.FileNotFoundException'>The file is not found.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>path is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='SIO.IOException'>The file is already open.</exception>
		 */
		SIO.Stream OpenAsync(SIO.FileMode mode, SIO.FileAccess access, SIO.FileShare share=SIO.FileShare.Read);
		/**
		 * <summary>
		 * Opens a file in the specified mode.
		 * </summary>
		 * <param name="mode">A System.IO.FileMode constant specifying the mode (for example, Open or Append) in which to open the file.</param>
		 * <returns>A file opened in the specified mode, with read/write access and unshared.</returns>
		 * <exception cref='SIO.FileNotFoundException'>The file is not found.</exception>
		 * <exception cref='UnauthorizedAccessException'>The file is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='SIO.IOException'>The file is already open.</exception>
		 */
		SIO.FileStream Open(SIO.FileMode mode);
		/**
		 * <summary>
		 * Creates a read-only System.IO.FileStream.
		 * </summary>
		 * <returns>A new read-only System.IO.FileStream object.</returns>
		 * <exception cref='S.UnauthorizedAccessException'>path is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 * <exception cref='SIO.IOException'>The file is already open.</exception>
		 */
		SIO.FileStream OpenRead();
		/**
		 * <summary>
		 * Creates a System.IO.StreamReader with UTF8 encoding that reads from an existing text file.
		 * </summary>
		 * <returns>A new StreamReader with UTF8 encoding.</returns>
		 * <exception cref='S.Security.SecurityException'>The caller does not have the required permission.</exception>
		 * <exception cref='SIO.FileNotFoundException'>The file is not found.</exception>
		 * <exception cref='S.UnauthorizedAccessException'>path is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The specified path is invalid, such as being on an unmapped drive.</exception>
		 */
		SIO.StreamReader OpenText();
		/**
		 * <summary>
		 * Creates a write-only System.IO.FileStream.
		 * </summary>
		 * <returns>A write-only unshared System.IO.FileStream object for a new or existing file.</returns>
		 * <exception cref='S.UnauthorizedAccessException'>The path specified when creating an instance of the <see cref='IFileInfo'/> object is read-only or is a directory.</exception>
		 * <exception cref='SIO.DirectoryNotFoundException'>The path specified when creating an instance of the <see cref='IFileInfo'/> object is invalid, such as being on an unmapped drive.</exception>
		 */
		SIO.FileStream OpenWrite();
		/**
		 * <summary>
		 * Replaces the contents of a specified file with the file described by the current <see cref='IFileInfo'/> object, deleting the original file, and creating a backup of the replaced file. Also specifies whether to ignore merge errors.
		 * </summary>
		 * <param name="destinationFileName">The name of a file to replace with the current file.</param>
		 * <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the <paramref name="destinationFileName"/> parameter.</param>
		 * <param name="ignoreMetadataErrors">true to ignore merge errors (such as attributes and ACLs) from the replaced file to the replacement file; otherwise false.</param>
		 * <returns>A <see cref='IFileInfo'/> object that encapsulates information about the file described by the <paramref name="destinationFileName"/> parameter.</returns>
		 * <exception cref='S.ArgumentException'>
		 * <list>
		 * <item><description>The path described by the <paramref name="destinationFileName"/> parameter was not of a legal form.</description></item>
		 * <item><description>The path described by the destBackupFileName parameter was not of a legal form.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.ArgumentNullException'>The <paramref name="destinationFileName"/> parameter is null.</exception>
		 * <exception cref='SIO.FileNotFoundException'>
		 * <list>
		 * <item><description>The file described by the current <see cref='IFileInfo'/> object could not be found.</description></item>
		 * <item><description>The file described by the destinationFileName parameter could not be found.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='SPlatformNotSupportedException'>The current operating system is not Microsoft Windows NT or later.</exception>
		 */
		IFileInfo Replace(string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors);
		/**
		 * <summary>
		 * Replaces the contents of a specified file with the file described by the current <see cref='IFileInfo'/> object, deleting the original file, and creating a backup of the replaced file.
		 * </summary>
		 * <param name="destinationFileName">The name of a file to replace with the current file.</param>
		 * <param name="destinationBackupFileName">The name of a file with which to create a backup of the file described by the <paramref name="destinationFileName"/> parameter.</param>
		 * <returns>A <see cref='IFileInfo'/> object that encapsulates information about the file described by the <paramref name="destinationFileName"/> parameter.</returns>
		 * <exception cref='S.ArgumentException'>
		 * <list>
		 * <item><description>The path described by the <paramref name="destinationFileName"/> parameter was not of a legal form.</description></item>
		 * <item><description>The path described by the destBackupFileName parameter was not of a legal form.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.ArgumentNullException'>The <paramref name="destinationFileName"/> parameter is null.</exception>
		 * <exception cref='SIO.FileNotFoundException'>
		 * <list>
		 * <item><description>The file described by the current <see cref='IFileInfo'/> object could not be found.</description></item>
		 * <item><description>The file described by the destinationFileName parameter could not be found.</description></item>
		 * </list>
		 * </exception>
		 * <exception cref='S.PlatformNotSupportedException'>The current operating system is not Microsoft Windows NT or later.</exception>
		 */
		IFileInfo Replace(string destinationFileName, string destinationBackupFileName);
	}
}
