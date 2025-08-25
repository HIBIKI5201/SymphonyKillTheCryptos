using Cryptos.Runtime.Entity.Ingame.Character;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    [CreateAssetMenu(fileName = nameof(WaveEntity), menuName = "Cryptos/" + nameof(WaveEntity), order = 1)]
    public class WaveEntity : ScriptableObject
    {
        public EnemyData[] Enemies => _enemies;

        [SerializeField]
        private EnemyData[] _enemies;
    }
}
