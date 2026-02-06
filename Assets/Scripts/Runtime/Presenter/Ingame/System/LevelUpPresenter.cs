using Cryptos.Runtime.UseCase.Ingame.System;
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
            // LevelUpgradeOption[] を LevelUpgradeNodeViewModel[] に変換
            var nodeViewModels = options.Select(o => new LevelUpgradeNodeViewModel(
                o.OriginalNode,
                _levelUseCase.GetUpgradeLevel(o.OriginalNode))).ToArray();

            // IngameUIManager に LevelUpScreenViewModel を渡す
            var selectedNodeVM = await _levelUpUIManager.LevelUpSelectAsync(screenViewModel);
            // 選択された LevelUpgradeNodeViewModel に含まれる LevelUpgradeNode を元に新しい LevelUpgradeOption を生成して返す
            return new LevelUpgradeOption(selectedNodeVM.LevelUpgradeNode);
        }
    }
}
