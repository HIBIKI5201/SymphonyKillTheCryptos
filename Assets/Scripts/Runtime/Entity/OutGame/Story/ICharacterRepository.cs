using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public interface ICharacterRepository
    {
        public void Show(string name);
        public void Hide(string name);
        public ValueTask Move(string name, Vector2 position, float duration);
        public ValueTask Rotate(string name, float degree, float duration);
    }
}
