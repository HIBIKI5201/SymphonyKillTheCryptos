using System;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public interface ILevelUpUIManager
    {
        Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(ReadOnlySpan<LevelUpgradeNodeViewModel> vm);
    }
}
