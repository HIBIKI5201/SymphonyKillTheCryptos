using Cryptos.Runtime.Entity.Ingame.System; // ILevelUpgradeEffect のため

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    /// <summary>
    /// レベルアップ選択肢の抽象的な表現。
    /// UseCase層がPresenter層のViewModelに直接依存しないようにするために使用されます。
    /// </summary>
    public class LevelUpgradeOption
    {
        public string NodeName { get; }
        public ILevelUpgradeEffect[] Effects { get; }
        public LevelUpgradeNode OriginalNode { get; } // 必要に応じて元のエンティティへの参照も持つ

        public LevelUpgradeOption(LevelUpgradeNode originalNode)
        {
            NodeName = originalNode.NodeName;
            Effects = originalNode.Effects;
            OriginalNode = originalNode;
        }
    }
}
