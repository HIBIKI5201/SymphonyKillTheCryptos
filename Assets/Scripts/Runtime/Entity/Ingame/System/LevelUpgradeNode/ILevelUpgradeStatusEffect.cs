using Cryptos.Runtime.Entity.Ingame.Character;

namespace Cryptos.Runtime.Entity
{
    public abstract class ILevelUpgradeStatusEffect : ILevelUpgradeEffect
    {
        public abstract void ApplyStatusEffect(TentativeCharacterData target);
    }
}
