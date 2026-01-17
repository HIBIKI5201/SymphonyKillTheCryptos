using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public interface IWaveHandler
    {
        Task OnWaveCompleted();
    }
}
