using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epam.Training._01AdvancedCS.TaskLibrary.Exceptions;
using Epam.Training._01AdvancedCS.TaskLibrary.Types;

namespace Epam.Training._01AdvancedCS.TaskLibrary
{
    public sealed class FileSystemVisitor : IFileSystemVisitor
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemVisitor"/> class.
        /// </summary>
        public FileSystemVisitor(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemVisitor"/> class.
        /// </summary>
        /// <param name="fileSystem">FileSystem</param>
        /// <param name="filter">The filter function.</param>
        /// <remarks>Return <see langword="true"/> to include item in the search process.</remarks>
        public FileSystemVisitor(IFileSystem fileSystem, Func<FileSystemInfoBase, bool> filter)
        {
            _fileSystem = fileSystem;
            Filter = filter;
        }

        /// <summary>
        /// Starts the search.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="token">The token.</param>
        /// <returns></returns>
        /// <exception cref="System.ArgumentNullException">path - Should not be null or empty.</exception>
        /// <exception cref="InvalidDirectoryNameException">Invalid chars in directory detected.</exception>
        /// <exception cref="DirectoryNotFoundException">Directory not found.</exception>
        public Task StartSearch(string path, CancellationTokenSource token = default(CancellationTokenSource))
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path), "Should not be null or empty.");
            var invalidChars = GetInvalidChars(path);
            if (invalidChars.Any())
                throw new InvalidDirectoryNameException("Invalid chars in directory detected.", invalidChars);

            var rootDir = GetRootDir(path);
            return Task.Run(() =>
            {
                Status = FileSystemVisitorStatus.Searching;
                var items = UseSecondMethod
                    ? GetFilteredItemsSecondWay(rootDir).ToList()
                    : GetFilteredItems(rootDir).ToList();
                SearchCompletedEvent?.Invoke(items);
            });
        }

        public void StopSearch()
        {
            Status = FileSystemVisitorStatus.Stopped;
        }

        private DirectoryInfoBase GetRootDir(string path)
        {
            var dir = _fileSystem.DirectoryInfo.FromDirectoryName(path);
            if (!dir.Exists)
                throw new DirectoryNotFoundException("Directory {path} does not exist");

            return dir;
        }

        private IEnumerable<FileSystemInfoBase> GetFilteredItems(DirectoryInfoBase directory)
        {
            var breakSearch = false;
            foreach (var item in directory.GetFileSystemInfos())
            {
                if (Status == FileSystemVisitorStatus.Stopped)
                    break;

                var excludeItem = false;
                var dir = item as DirectoryInfoBase;
                if (dir != null)
                {
                    DirectoryFoundEvent?.Invoke(dir, ref breakSearch, ref excludeItem);
                    if (!breakSearch && !excludeItem)
                        foreach (var i in GetFilteredItems(dir)) //going recursion here
                            yield return i;
                }
                else
                {
                    var file = item as FileInfoBase;
                    if (file != null)
                        FileFoundEvent?.Invoke(file, ref breakSearch, ref excludeItem);
                }
                if (breakSearch)
                    Status = FileSystemVisitorStatus.Stopped;
                if (excludeItem)
                    continue;

                if (Filter != null && !Filter.Invoke(item))
                    continue;

                FilterItemActions(item, ref breakSearch, ref excludeItem);
                if (!excludeItem)
                    yield return item;
                else if (breakSearch)
                    Status = FileSystemVisitorStatus.Stopped;
            }
        }

        private IEnumerable<FileSystemInfoBase> GetFilteredItemsSecondWay(DirectoryInfoBase directory)
        {
            var breakSearch = false;
            foreach (var dir in directory.EnumerateDirectories())
            {
                if (Status == FileSystemVisitorStatus.Stopped)
                    break;

                var excludeItem = false;
                DirectoryFoundEvent?.Invoke(dir, ref breakSearch, ref excludeItem);
                if (!breakSearch && !excludeItem)
                    foreach (var i in GetFilteredItemsSecondWay(dir)) //going recursion here
                        yield return i;
                if (breakSearch)
                    Status = FileSystemVisitorStatus.Stopped;
                if (excludeItem)
                    continue;
                if (Filter != null && !Filter.Invoke(dir))
                    continue;

                yield return dir;
            }

            foreach (var file in GetFilteredFiles(directory))
                yield return file;
        }

        private IEnumerable<FileInfoBase> GetFilteredFiles(DirectoryInfoBase directory)
        {
            var breakSearch = false;
            foreach (var file in directory.EnumerateFiles())
            {
                if (Status == FileSystemVisitorStatus.Stopped)
                    break;

                var excludeItem = false;
                FileFoundEvent?.Invoke(file, ref breakSearch, ref excludeItem);
                if (Filter != null && !Filter.Invoke(file))
                    continue;
                FilterItemActions(file, ref breakSearch, ref excludeItem);
                if (breakSearch)
                    Status = FileSystemVisitorStatus.Stopped;
                if (!excludeItem)
                    yield return file;
            }
        }

        private void FilterItemActions(FileSystemInfoBase item, ref bool breakSearch, ref bool excludeItem)
        {
            var filteredDir = item as DirectoryInfoBase;
            if (filteredDir != null)
                FilteredDirectoryFoundEvent?.Invoke(filteredDir, ref breakSearch, ref excludeItem);
            var filteredFile = item as FileInfoBase;
            if (filteredFile != null)
                FilteredFileFoundEvent?.Invoke(filteredFile, ref breakSearch, ref excludeItem);
        }

        private char[] GetInvalidChars(string str)
        {
            var invChars = Path.GetInvalidPathChars();
            return str.Intersect(invChars).ToArray();
        }

        public event FileFoundEventHandler FileFoundEvent;
        public event DirectoryFoundEventHandler DirectoryFoundEvent;
        public event FileFoundEventHandler FilteredFileFoundEvent;
        public event DirectoryFoundEventHandler FilteredDirectoryFoundEvent;
        public event SearchCompletedDelegate SearchCompletedEvent;

        public FileSystemVisitorStatus Status { get; private set; } = FileSystemVisitorStatus.Stopped;

        /// <summary>
        /// Gets or sets the filter function. 
        /// </summary>
        /// <value>
        /// The filter.
        /// </value>
        /// <remarks>Return <see langword="true"/> to exclude item from the search process.</remarks>
        public Func<FileSystemInfoBase, bool> Filter { get; set; }

        public bool UseSecondMethod { get; set; } = false;

        private readonly IFileSystem _fileSystem;
    }
}
