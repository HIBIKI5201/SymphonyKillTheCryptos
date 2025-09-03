using Cryptos.Runtime.Entity.Ingame.System;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    ///     レベル管理を行うユースケース。
    /// </summary>
    public class LevelUseCase
    {
        public LevelUseCase(LevelUpgradeData data,
            Func<LevelUpgradeNode[], Task<LevelUpgradeNode>> onLevelUpSelectNode)
        {
            LevelEntity levelEntity = new LevelEntity(data.LevelRequirePoints);
            levelEntity.OnLevelChanged += newLevel => _levelUpQueue.Enqueue(newLevel);

            _data = data;
            _levelEntity = levelEntity;
            _onLevelUpSelectNode = onLevelUpSelectNode;
        }
        /// <summary> 未処理のレベルアップ </summary>
        public Queue<int> LevelUpQueue => _levelUpQueue;

        /// <summary>
        ///     ウェーブクリア時に経験値を追加します。
        /// </summary>
        /// <param name="waveEntity"></param>
        public void AddLevelProgress(WaveEntity waveEntity)
        {
            _levelEntity.AddLevelProgress(waveEntity.WaveExperiencePoint);
        }

        /// <summary>
        ///     レベルアップのノード選択を待機します。
        /// </summary>
        /// <returns></returns>
        public async Task<LevelUpgradeNode> WaitLevelUpSelectAsync()
        {
            LevelUpgradeNode selectedNode =
                await _onLevelUpSelectNode?.Invoke(_data.LevelCard);

            return selectedNode;
        }

        private LevelEntity _levelEntity;
        private LevelUpgradeData _data;

        private Func<LevelUpgradeNode[], Task<LevelUpgradeNode>> _onLevelUpSelectNode;
        private Queue<int> _levelUpQueue = new();
    }
}