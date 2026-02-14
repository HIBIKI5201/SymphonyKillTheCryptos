using UnityEngine;

namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public class NovelSettingEntity
    {
        public NovelSettingEntity(
            float textSpeed)
        {
            _textSpeed = textSpeed;
        }

        public float TextSpeed => _textSpeed;

        private readonly float _textSpeed = 30f;
    }
}
