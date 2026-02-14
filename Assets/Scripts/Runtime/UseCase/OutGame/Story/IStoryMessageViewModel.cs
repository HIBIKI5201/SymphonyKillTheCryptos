using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.OutGame.Story
{
    public interface IStoryMessageViewModel
    {
        public ValueTask SetTextAsync(string name, string text, CancellationToken token = default);
    }
}
