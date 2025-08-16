using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Character;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character
{
    /// <summary>
    /// 敵キャラクターの生成と管理を行うリポジトリクラスです。
    /// </summary>
    public class EnemyRepository
    {
        public event Action<CharacterEntity<EnemyData>> OnEnemyCreated;

        public IReadOnlyList<CharacterEntity<EnemyData>> AllEnemies => _enemies;

        public CharacterEntity<EnemyData> CreateEnemy(EnemyData data)
        {
            if (data == null)
            {
                Debug.LogError("EnemyData is null.");
                return null;
            }

            CharacterEntity<EnemyData> enemy = _enemyGenerator.Generate(data);
            if (enemy == null) return null;

            _enemies.Add(enemy);
            enemy.OnDead += () =>
            {
                _enemies.Remove(enemy);
            };
            OnEnemyCreated?.Invoke(enemy);

            EnemyLog(enemy);

            return enemy;
        }

        private List<CharacterEntity<EnemyData>> _enemies = new();
        private EnemyGenerator _enemyGenerator = new();

        /// <summary>
        /// 敵キャラクターの体力ログを設定します。
        /// </summary>
        /// <param name="enemy"></param>
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
