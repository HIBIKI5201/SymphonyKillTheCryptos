using Cryptos.Runtime.UseCase.Ingame.System;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class LevelUpPresenter
    {
        private readonly LevelUseCase _levelUseCase;
        private readonly ILevelUpUIManager _levelUpUIManager;

        public LevelUpPresenter(LevelUseCase levelUseCase, ILevelUpUIManager levelUpUIManager)
        {
            _levelUseCase = levelUseCase;
            _levelUpUIManager = levelUpUIManager;
        }

        public async Task<LevelUpgradeOption> HandleLevelUpSelectAsync(LevelUpgradeOption[] options)
        {
            LevelUpgradeNodeViewModel[] nodeViewModels = 
                options.Select(o => new LevelUpgradeNodeViewModel(
                o.OriginalNode,
                _levelUseCase.GetUpgradeLevel(o.OriginalNode))).ToArray();

            LevelUpgradeNodeViewModel selectedNodeVM = await _levelUpUIManager.LevelUpSelectAsync(nodeViewModels);
            return new LevelUpgradeOption(selectedNodeVM.LevelUpgradeNode);
        }
    }
}
