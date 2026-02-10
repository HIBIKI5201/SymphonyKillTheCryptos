using Cryptos.Runtime.Entity.Outgame.Card;
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

        public void SaveDeckName(DeckNameValueObject value)
        {
            _deckName = value;
        }

        public DateTime LastTime => _lastTime;
        public DeckNameValueObject DeckName => _deckName;

        private DeckNameValueObject _deckName;
        private DateTime _lastTime;
    }
}
