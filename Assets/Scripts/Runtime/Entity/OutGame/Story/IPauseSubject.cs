using SymphonyFrameWork.Utility;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Entity
{
    public interface IPauseSubject
    {
        public bool IsPaused { get; }
        public async ValueTask WaitResume()
        {
            if (!IsPaused) { return; }
            await SymphonyTask.WaitUntil(() => !IsPaused);
        }
    }
}
