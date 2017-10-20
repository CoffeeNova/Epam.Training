using System.Collections.Generic;
using System.IO.Abstractions;

namespace Epam.Training._01AdvancedCS.TaskLibrary.Types
{
    public delegate void FileFoundEventHandler(FileInfoBase file, ref bool breakSearch, ref bool excludeFile);
    public delegate void DirectoryFoundEventHandler(DirectoryInfoBase directory, ref bool breakSearch, ref bool excludeDirectory);
    public delegate void SearchCompletedDelegate(IEnumerable<FileSystemInfoBase> result);
}
