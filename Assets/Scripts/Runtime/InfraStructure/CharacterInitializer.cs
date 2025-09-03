using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.InfraStructure.Ingame.Utility
{
    public static class CharacterInitializer
    {
        public static CharacterInitializationData Initialize(CharacterData symphonyData)
        {
            CharacterEntity<CharacterData> symphony = new(symphonyData);
            EnemyRepository enemyRepo = new();

            return new(symphony, enemyRepo);
        }

        public readonly struct CharacterInitializationData
        {
            public CharacterInitializationData(
                CharacterEntity<CharacterData> symphony,
                EnemyRepository enemyRepository)
            {
                _symphony = symphony;
                _enemyRepo = enemyRepository;
            }

            public CharacterEntity<CharacterData> Symphony => _symphony;
            public EnemyRepository EnemyRepository => _enemyRepo;

            private readonly CharacterEntity<CharacterData> _symphony;
            private readonly EnemyRepository _enemyRepo;
        }
    }
}
