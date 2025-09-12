using Cryptos.Runtime.Entity.Ingame.Character;

namespace Cryptos.Runtime.Entity
{
    public abstract class LevelUpgradeStatusEffect : ILevelUpgradeEffect
    {
        public abstract void ApplyStatusEffect(TentativeCharacterData target);
    }
}
