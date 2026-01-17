using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.System
{
    public interface IMasterUIManager
    {
        public Task FadeOut(float duration, CancellationToken token = default);
        public Task FadeIn(float duration, CancellationToken token = default);
    }
}
