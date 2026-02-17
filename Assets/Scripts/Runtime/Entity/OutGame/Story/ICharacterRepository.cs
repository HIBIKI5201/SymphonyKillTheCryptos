using UnityEngine;

namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public interface ICharacterRepository
    {
        public void Show(string name);
        public void Hide(string name);
        public void Move(string name, Vector2 position, float duration);
    }
}
