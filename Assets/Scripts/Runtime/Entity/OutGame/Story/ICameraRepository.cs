using UnityEngine;

namespace Cryptos.Runtime.Entity.Outgame.Story
{
    public interface ICameraRepository
    {
        public void SetActiveCamera(int index);
        public void SetActiveCamera(string name);
    }
}
