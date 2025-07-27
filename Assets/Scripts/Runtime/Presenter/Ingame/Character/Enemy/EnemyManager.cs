using Cryptos.Runtime.Entity;
using System.Collections.Generic;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Character.Enemy
{
    public class EnemyManager : MonoBehaviour
    {
        private List<IHitable> _enemies = new();

        public IReadOnlyList<IHitable> GetAllEnemies()
        {
            return _enemies;
        }
    }
}
