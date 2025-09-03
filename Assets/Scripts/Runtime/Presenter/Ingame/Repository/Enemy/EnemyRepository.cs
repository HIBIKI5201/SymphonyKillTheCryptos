using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.System;
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
        public event Action<CharacterEntity<CharacterData>> OnEnemyCreated;

        public IReadOnlyList<CharacterEntity<CharacterData>> AllEnemies => _enemies;

        public CharacterEntity<CharacterData> CreateEnemy(CharacterData data)
        {
            if (data == null)
            {
                Debug.LogError("EnemyData is null.");
                return null;
            }

            CharacterEntity<CharacterData> enemy = _enemyGenerator.Generate(data);
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

        private List<CharacterEntity<CharacterData>> _enemies = new();
        private EnemyGenerator _enemyGenerator = new();

        /// <summary>
        /// 敵キャラクターの体力ログを設定します。
        /// </summary>
        /// <param name="enemy"></param>
        private void EnemyLog(CharacterEntity<CharacterData> enemy)
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
