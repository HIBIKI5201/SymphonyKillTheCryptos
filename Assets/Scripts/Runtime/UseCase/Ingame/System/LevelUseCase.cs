using Cryptos.Runtime.Entity.Ingame.System;
using System;
using System.Collections.Generic;
using System.Linq;
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
            LevelUpgradeNode[] allNode = _data.LevelCard;
            int amount = _data.LevelUpgradeAmount;
            LevelUpgradeNode[] candidates = new LevelUpgradeNode[amount];

            int[] indices = Enumerable.Range(0, allNode.Length).ToArray();
            var rng = new Random();

            // F-Y法で部分的にシャッフル
            for (int i = 0; i < amount; i++)
            {
                int j = rng.Next(i, indices.Length);
                (indices[i], indices[j]) = (indices[j], indices[i]);
                candidates[i] = allNode[indices[i]];
            }

            LevelUpgradeNode selectedNode =
                await _onLevelUpSelectNode?.Invoke(candidates);

            return selectedNode;
        }

        private LevelEntity _levelEntity;
        private LevelUpgradeData _data;

        private Func<LevelUpgradeNode[], Task<LevelUpgradeNode>> _onLevelUpSelectNode;
        private Queue<int> _levelUpQueue = new();
    }
}