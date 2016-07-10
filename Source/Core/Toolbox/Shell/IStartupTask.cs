using System.Threading.Tasks;

namespace XenForms.Core.Toolbox.Shell
{
    public interface IStartupTask
    {
        int ExecutionOrder { get; }
        ExecuteWhen ExecuteWhen { get; }
        Task<bool> ExecuteAsync(object data);
    }
}