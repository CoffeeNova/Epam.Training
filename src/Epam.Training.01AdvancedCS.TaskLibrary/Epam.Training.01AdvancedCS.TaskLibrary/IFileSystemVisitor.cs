using System.Threading;
using System.Threading.Tasks;
using Epam.Training._01AdvancedCS.TaskLibrary.Types;

namespace Epam.Training._01AdvancedCS.TaskLibrary
{
    public interface IFileSystemVisitor
    {
        Task StartSearch(string path, CancellationTokenSource token);

        void StopSearch();

        event FileFoundEventHandler FileFoundEvent;

        event DirectoryFoundEventHandler DirectoryFoundEvent;

        event FileFoundEventHandler FilteredFileFoundEvent;

        event DirectoryFoundEventHandler FilteredDirectoryFoundEvent;

        event SearchCompletedDelegate SearchCompletedEvent;
    }
}
