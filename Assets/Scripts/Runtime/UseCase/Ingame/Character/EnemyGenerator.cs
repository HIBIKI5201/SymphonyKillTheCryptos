using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Character.Repository;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Character
{
    /// <summary>
    /// 敵のエンティティを生成するファクトリークラス。
    /// IEnemyFactoryインターフェースを実装し、UseCase層のビジネスロジックを
    /// Entity層のEnemyRepositoryに注入します。
    /// </summary>
    public class EnemyGenerator : IEnemyFactory
    {
        /// <summary>
        /// 敵のデータからエンティティを生成する
        /// </summary>
        /// <param name="data">敵のデータ</param>
        /// <returns>生成された敵エンティティ</returns>
        public CharacterEntity Create(CharacterData data)
        {
            if (data == null)
            {
                Debug.LogError("EnemyData is null.");
                return null;
            }

            // ここで敵キャラクターを生成する
            return new CharacterEntity(data);
        }
    }
}

