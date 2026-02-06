using Cryptos.Runtime.Entity.Ingame.Character;
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
        /// <summary>
        ///     コンストラクタ。
        /// </summary>
        public LevelUseCase(LevelUpgradeData data, TentativeCharacterData characterData)
        {
            LevelEntity levelEntity = new(data.LevelRequirePoints);
            levelEntity.OnLevelChanged += newLevel => _levelUpQueue.Enqueue(newLevel);

            _data = data;
            _characterData = characterData;
            _levelEntity = levelEntity;
        }

        /// <summary>
        ///     未処理のレベルアップ。
        /// </summary>
        public Queue<int> LevelUpQueue => _levelUpQueue;

        /// <summary>
        ///     ウェーブクリア時に経験値を追加します。
        /// </summary>
        public void AddLevelProgress(WaveEntity waveEntity)
        {
            _levelEntity.AddLevelProgress(waveEntity.WaveExperiencePoint);
        }

        /// <summary>
        ///     レベルアップ処理（ノード選択と効果適用）を非同期で行います。
        /// </summary>
        public async Task HandleLevelUpAsync(Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> onLevelUpSelectNode)
        {
            var selectedNode = await WaitLevelUpSelectAsync(onLevelUpSelectNode);

            if (selectedNode == null) return;

            // 選択されたノードを記録します。
            _acquiredUpgradeEntity.Add(selectedNode);

            // 選択されたノードの効果を適用します。
            foreach (var effect in selectedNode.Effects)
            {
                if (effect is LevelUpgradeStatusEffect statusEffect)
                {
                    statusEffect.ApplyStatusEffect(_characterData);
                }
            }
        }

        public int GetUpgradeLevel(LevelUpgradeNode node)
        {
            return _acquiredUpgradeEntity.GetCount(node);
        }

        private readonly LevelEntity _levelEntity;
        private readonly LevelUpgradeData _data;
        private readonly TentativeCharacterData _characterData;
        private readonly AcquiredUpgradeEntity _acquiredUpgradeEntity = new();
        private readonly Queue<int> _levelUpQueue = new();

        /// <summary>
        ///     レベルアップのノード選択を待機します。
        /// </summary>
        private async Task<LevelUpgradeNode> WaitLevelUpSelectAsync(
            Func<LevelUpgradeOption[], Task<LevelUpgradeOption>> onLevelUpSelectNode)
        {
            // 取得回数が上限に達していないノードを候補とする。
            LevelUpgradeNode[] allNode = _data.LevelCard
                .Where(node => _acquiredUpgradeEntity.GetCount(node) < node.MaxStack)
                .ToArray();
            
            int amount = _data.LevelUpgradeAmount;

            //シャッフル用のインデックス配列を作る
            int[] indices = Enumerable.Range(0, allNode.Length).ToArray();
            var rng = new Random();

            var candidates = new LevelUpgradeNode[amount];
            var candidateOptions = new LevelUpgradeOption[amount];

            // F-Y法で部分的にシャッフル。
            for (int i = 0; i < amount; i++)
            {
                int j = rng.Next(i, indices.Length);
                (indices[i], indices[j]) = (indices[j], indices[i]);
                candidates[i] = allNode[indices[i]];
                candidateOptions[i] = new LevelUpgradeOption(allNode[indices[i]]);
            }

            //候補を渡して選択されるのを待機
            LevelUpgradeOption selectedOption =
                await onLevelUpSelectNode?.Invoke(candidateOptions);

            return selectedOption.OriginalNode;
        }
    }
}