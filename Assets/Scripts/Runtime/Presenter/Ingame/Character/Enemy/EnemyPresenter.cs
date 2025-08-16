using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.Character;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Character.Enemy
{
    public class EnemyPresenter : MonoBehaviour
    {
        public void Init(EnemyRepository repository)
        {
            _enemyRepository = repository;
            repository.OnEnemyCreated += HandleEnemyCreated;

            foreach (var enemy in repository.AllEnemies)
            {
                CreateEnemyModel(enemy);
            }
        }

        private EnemyRepository _enemyRepository;

        private void HandleEnemyCreated(CharacterEntity<EnemyData> enemy)
        {
            if (enemy == null) return;
            // ここで敵キャラクターの表示や初期化を行う

            CreateEnemyModel(enemy);
        }

        private void CreateEnemyModel(CharacterEntity<EnemyData> enemy)
        {
            Debug.Log($"Creating enemy model for: {enemy}");
        }
    }
}
