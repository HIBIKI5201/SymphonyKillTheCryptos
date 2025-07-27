using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character
{
    public class EnemyManager : MonoBehaviour
    {
        private List<IHitable> _enemies = new();

        public IReadOnlyList<IHitable> GetAllEnemies()
        {
            return _enemies;
        }

        public CharacterEntity<EnemyData> CreateEnemy(EnemyData data)
        {
            if (data == null)
            {
                Debug.LogError("EnemyData is null.");
                return null;
            }
            CharacterEntity<EnemyData> enemy = _enemyGenerator.Generate(data);
            if (enemy != null)
            {
                _enemies.Add(enemy);
                enemy.OnDead += () =>
                {
                    _enemies.Remove(enemy);
                };
            }
            return enemy;
        }

        private EnemyGenerator _enemyGenerator = new();
    }
}
