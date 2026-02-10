using UnityEngine;
using SymphonyFrameWork.System;
using Cryptos.Runtime.Entity;

namespace Cryptos.Runtime.UseCase
{
    public static class SaveExecuter
    {
        public static void DeckSave()
        {
            SaveDataSystem<PlayerDeckSaveData>.Save();
        }
    }
}
