using System.Threading;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public interface IScenarioAction
    {
        public ValueTask ExecuteAsync(IScenarioActionRepository repository,
    IPauseHandler pauseHandler,
    CancellationToken token = default);
    }
}
