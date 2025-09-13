using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;

namespace Cryptos.Runtime.UseCase.Ingame.System
{
    public abstract class LevelUpgradeStatusEffect : ILevelUpgradeEffect
    {
        public abstract void ApplyStatusEffect(TentativeCharacterData target);
    }
}
