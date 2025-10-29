using System;
using System.Collections.Generic;
using System.Diagnostics;
using Debug = UnityEngine.Debug;

namespace Cryptos.Runtime.Entity.Ingame.Character.Repository
{
    /// <summary>
    /// 敵キャラクターの生成と管理を行うリポジトリクラスです。
    /// 依存性逆転の原則により、IEnemyFactoryインターフェースを通じて
    /// エンティティの生成を行います。
    /// </summary>
    public class EnemyRepository
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="factory">敵エンティティを生成するファクトリー</param>
        public EnemyRepository(IEnemyFactory factory)
        {
            _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

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

            // ファクトリーを通じてEntityを生成（DI注入）
            CharacterEntity enemy = _factory.Create(data);
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
        private readonly IEnemyFactory _factory;

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

