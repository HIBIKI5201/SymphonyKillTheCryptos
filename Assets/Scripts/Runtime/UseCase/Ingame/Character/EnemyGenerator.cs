using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Character
{
    /// <summary>
    ///     敵のエンティティを生成するクラス
    /// </summary>
    public class EnemyGenerator
    {
        /// <summary>
        ///     敵のデータからエンティティを生成する
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public CharacterEntity Generate(CharacterData data)
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