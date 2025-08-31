using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.Ingame.Utility
{
    public static class CharacterInitializer
    {
        public static CharacterInitializationData Initialize(SymphonyData symphonyData)
        {
            CharacterEntity<SymphonyData> symphony = new(symphonyData);
            EnemyRepository enemyRepo = new();

            return new(symphony, enemyRepo);
        }

        public readonly struct CharacterInitializationData
        {
            public CharacterInitializationData(
                CharacterEntity<SymphonyData> symphony,
                EnemyRepository enemyRepository)
            {
                _symphony = symphony;
                _enemyRepo = enemyRepository;
            }

            public CharacterEntity<SymphonyData> Symphony => _symphony;
            public EnemyRepository EnemyRepository => _enemyRepo;

            private readonly CharacterEntity<SymphonyData> _symphony;
            private readonly EnemyRepository _enemyRepo;
        }
    }
}
