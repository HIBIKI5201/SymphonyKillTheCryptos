using System;

namespace Cryptos.Runtime.Entity.System.SaveData
{
    [Serializable]
    public class PlayerMasterSaveData
    {
        public void SaveDate()
        {
            _lastTime = DateTime.Now;
        }

        public DateTime LastTime => _lastTime;

        private DateTime _lastTime;
    }
}
