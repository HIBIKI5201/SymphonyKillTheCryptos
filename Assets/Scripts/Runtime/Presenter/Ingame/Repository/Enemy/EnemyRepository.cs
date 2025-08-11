using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Character;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character
{
    public class EnemyRepository
    {
        public IReadOnlyList<ICharacter> AllEnemies => _enemies;

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
                EnemyLog(enemy);
            }
            return enemy;
        }

        private List<ICharacter> _enemies = new();
        private EnemyGenerator _enemyGenerator = new();

        private void EnemyLog(CharacterEntity<EnemyData> enemy)
        {
            enemy.OnHealthChanged += (currentHealth, maxHealth) =>
            {
                Debug.Log($"Enemy health changed: {currentHealth}/{maxHealth}");
            };

            enemy.OnDead += () =>
            {
                Debug.Log("Enemy has died.");
            };
        }
    }
}
