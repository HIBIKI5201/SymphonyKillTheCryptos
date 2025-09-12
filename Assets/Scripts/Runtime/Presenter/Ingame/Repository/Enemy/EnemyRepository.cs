using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Character;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Cryptos.Runtime.Presenter.Ingame.Character
{
    /// <summary>
    /// 敵キャラクターの生成と管理を行うリポジトリクラスです。
    /// </summary>
    public class EnemyRepository
    {
        /// <summary> 敵が生成された時のイベント </summary>
        public event Action<CharacterEntity> OnEnemyCreated;

        /// <summary> 存在している全ての敵 </summary>
        public IReadOnlyList<CharacterEntity> AllEnemies => _enemies;

        public CharacterEntity CreateEnemy(CharacterData data)
        {
            if (data == null)
            {
                Debug.LogError("EnemyData is null.");
                return null;
            }

            CharacterEntity enemy = _enemyGenerator.Generate(data);
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

        private List<CharacterEntity> _enemies = new();
        private EnemyGenerator _enemyGenerator = new();

        /// <summary>
        /// 敵キャラクターの体力ログを設定します。
        /// </summary>
        /// <param name="enemy"></param>
        [Conditional("UNITY_EDITOR")]
        private void EnemyLog(CharacterEntity enemy)
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
