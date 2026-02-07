using Cryptos.Runtime.Entity.Ingame.Character;
using System;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Ingame.System
{
    [CreateAssetMenu(fileName = nameof(WaveEntity), menuName = "Cryptos/" + nameof(WaveEntity), order = 1)]
    public class WaveEntity : ScriptableObject
    {
        public int WaveExperiencePoint => _waveExperiencePoint;
        public int SkillTreePoint => _skillTreePoint;
        public CharacterData[] Enemies => _enemies;
        public string BGMCueName => _bgmCueName;

        [SerializeField, Min(0)]
        private int _waveExperiencePoint;
        [SerializeField, Min(0)]
        private int _skillTreePoint;

        [SerializeField]
        private CharacterData[] _enemies;

        [SerializeField]
        private string _bgmCueName = "Normal";

    }
}
