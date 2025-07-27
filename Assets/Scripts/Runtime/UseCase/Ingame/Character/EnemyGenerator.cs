using Cryptos.Runtime.Entity.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.UseCase.Ingame.Character
{
    public class EnemyGenerator
    {
        public CharacterEntity<EnemyData> Generate(EnemyData data)
        {
            if (data == null)
            {
                Debug.LogError("EnemyData is null.");
                return null;
            }
            // ここで敵キャラクターを生成する
            return new CharacterEntity<EnemyData>(data);
        }
    }
}