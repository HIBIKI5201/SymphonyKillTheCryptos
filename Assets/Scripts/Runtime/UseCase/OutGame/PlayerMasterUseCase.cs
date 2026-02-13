using Cryptos.Runtime.Entity.System.SaveData;
using Cryptos.Runtime.Entity.Outgame.Card;
using Cryptos.Runtime.UseCase; // For SaveExecuter.SavePlayerMasterData()
using System;
using Cryptos.Runtime.Entity;

namespace Cryptos.Runtime.UseCase.OutGame
{
    /// <summary>
    ///     プレイヤーのマスターデータを管理するユースケースである。
    /// </summary>
    public class PlayerMasterUseCase
    {
        private readonly PlayerMasterSaveData _playerMasterSaveData;

        /// <summary>
        ///     PlayerMasterUseCaseの新しいインスタンスを生成する。
        /// </summary>
        /// <param name="playerMasterSaveData">プレイヤーのマスターデータ。</param>
        public PlayerMasterUseCase(PlayerMasterSaveData playerMasterSaveData)
        {
            _playerMasterSaveData = playerMasterSaveData;
        }

        public void Initialize(RoleEntity defaultRole)
        {
            DeckNameValueObject deckName = _playerMasterSaveData.DeckName;
            if (string.IsNullOrEmpty(deckName.Value))
            {
                _playerMasterSaveData.SaveDeckName(defaultRole.Deck);
            }

        }

        /// <summary>
        ///     現在選択されているデッキの名前を取得する。
        /// </summary>
        /// <returns>現在選択されているデッキの名前のValueObject。</returns>
        public DeckNameValueObject GetSelectedDeckName()
        {
            return _playerMasterSaveData.DeckName;
        }

        /// <summary>
        ///     現在選択されているデッキの名前を設定し、保存する。
        /// </summary>
        /// <param name="deckName">設定するデッキの名前のValueObject。</param>
        public void SetSelectedDeckName(DeckNameValueObject deckName)
        {
            _playerMasterSaveData.SaveDeckName(deckName);
            SaveExecuter.SavePlayerMasterData();
        }

        /// <summary>
        ///     ゲームの最終保存時間を更新する。
        /// </summary>
        public void SaveGameDate()
        {
            _playerMasterSaveData.SaveDate();
            SaveExecuter.SavePlayerMasterData();
        }
    }
}
