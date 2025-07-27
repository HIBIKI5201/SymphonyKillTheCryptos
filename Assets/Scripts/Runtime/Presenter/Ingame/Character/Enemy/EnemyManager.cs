using Cryptos.Runtime.Entity;
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

        private void Awake()
        {
            
        }

        private EnemyGenerator _enemyGenerator = new();
    }
}
