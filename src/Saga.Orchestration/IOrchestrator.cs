using System.Threading.Tasks;

namespace Saga.Orchestration
{
    public interface IOrchestrator
    {
        Task Run(Core.Saga saga);
    }
}