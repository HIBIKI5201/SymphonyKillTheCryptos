using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public interface IGameInstaller
    {
        public ValueTask GameInitialize();
    }
}
