using Cryptos.Runtime.Entity;
using Cryptos.Runtime.Entity.System.SaveData;
using SymphonyFrameWork.System;

namespace Cryptos.Runtime.UseCase
{
    public static class SaveExecuter
    {
        public static void DeckSave()
        {
            PlayerDeckSaveData.Save();
        }

        /// <summary>
        ///     プレイヤーのマスターデータを保存する。
        /// </summary>
        public static void SavePlayerMasterData()
        {
            SaveDataSystem<PlayerMasterSaveData>.Save();
        }
    }
}
