using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Character.Repository;
using Cryptos.Runtime.UseCase.Ingame.Character;

namespace Cryptos.Runtime.InfraStructure.Ingame.Utility
{
    /// <summary>
    ///     キャラクターの初期化のユーティリティクラス
    /// </summary>
    public static class CharacterInitializer
    {
        /// <summary>
        ///     キャラクター系の初期化
        /// </summary>
        /// <param name="symphonyData"></param>
        /// <returns></returns>
        public static CharacterInitializationData Initialize(ICharacterData symphonyData)
        {
            SymphonyEntity symphony = new(symphonyData);
            
            // DI: EnemyGeneratorをIEnemyFactoryとして注入
            IEnemyFactory enemyFactory = new EnemyGenerator();
            EnemyRepository enemyRepo = new(enemyFactory);

            return new(symphony, enemyRepo);
        }

        /// <summary>
        ///     キャラクターのリポジトリ
        /// </summary>
        public readonly struct CharacterInitializationData
        {
            public CharacterInitializationData(
                CharacterEntity symphony,
                EnemyRepository enemyRepository)
            {
                _symphony = symphony;
                _enemyRepo = enemyRepository;
            }

            public CharacterEntity Symphony => _symphony;
            public EnemyRepository EnemyRepository => _enemyRepo;

            private readonly CharacterEntity _symphony;
            private readonly EnemyRepository _enemyRepo;
        }
    }
}
