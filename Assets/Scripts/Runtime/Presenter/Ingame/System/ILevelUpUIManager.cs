using Cryptos.Runtime.Presenter.Ingame.System; // LevelUpScreenViewModel が必要になるため
using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public interface ILevelUpUIManager
    {
        Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(LevelUpScreenViewModel vm);
    }
}
