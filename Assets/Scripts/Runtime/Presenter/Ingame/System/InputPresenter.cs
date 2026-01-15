using Cryptos.Runtime.Framework;
using Cryptos.Runtime.UseCase.Ingame.System;
using Cryptos.Runtime.UseCase.Ingame.Card;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    public class InputPresenter : ILevelUpPhaseHandler, IWaveStateReceiver
    {
        private readonly InputBuffer _inputBuffer;
        private readonly CardUseCase _cardUseCase;
        private readonly IIngameUIManager _ingameUIManager;

        public InputPresenter(InputBuffer inputBuffer, CardUseCase cardUseCase, IIngameUIManager ingameUIManager)
        {
            _inputBuffer = inputBuffer;
            _cardUseCase = cardUseCase;
            _ingameUIManager = ingameUIManager;
        }

        public void OnWaveStarted()
        {
            RegisterWaveInputs();
        }

        public void OnWaveCleared()
        {
            UnregisterWaveInputs();
        }

        public void OnLevelUpPhaseStarted()
        {
            // ウェーブ中の入力を一旦停止
            UnregisterWaveInputs();
            // レベルアップ用の入力を開始
            _inputBuffer.OnAlphabetKeyPressed += _ingameUIManager.OnLevelUpgradeInputChar;
        }

        public void OnLevelUpPhaseEnded()
        {
            // レベルアップ用の入力を停止
            _inputBuffer.OnAlphabetKeyPressed -= _ingameUIManager.OnLevelUpgradeInputChar;
            // 次のウェーブが開始されるときに OnWaveStarted が呼ばれて入力が再登録される
        }

        private void RegisterWaveInputs()
        {
            UnregisterWaveInputs(); // 念のため解除してから登録
            _inputBuffer.OnAlphabetKeyPressed += _cardUseCase.InputCharToDeck;
            _inputBuffer.OnAlphabetKeyPressed += _ingameUIManager.CardInputChar;
        }

        private void UnregisterWaveInputs()
        {
            _inputBuffer.OnAlphabetKeyPressed -= _cardUseCase.InputCharToDeck;
            _inputBuffer.OnAlphabetKeyPressed -= _ingameUIManager.CardInputChar;
        }
    }
}
