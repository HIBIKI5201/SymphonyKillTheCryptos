using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Character.Repository;
using Cryptos.Runtime.Entity.Ingame.System;
using Cryptos.Runtime.Presenter.Ingame.Character.Player;
using Cryptos.Runtime.Presenter.System.Audio;
using Cryptos.Runtime.UseCase.Ingame.System;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.System
{
    /// <summary>
    ///     ウェーブの進行を管理するクラス。
    /// </summary>
    public class WaveSystemPresenter : IInGameLoopHandler
    {
        private readonly WaveUseCase _waveUseCase;
        private readonly WavePathPresenter _wavePath;
        private readonly SymphonyPresenter _symphony;
        private readonly EnemyRepository _enemyRepository;
        private readonly IBGMPlayer _bgmPlayer;
        private readonly IWaveStateReceiver _waveStateReceiver;
        private IWaveHandler _waveHandler;

        private int _enemyCount = 0;

        public WaveSystemPresenter(
            WaveUseCase waveUseCase,
            WavePathPresenter wavePathPresenter,
            SymphonyPresenter player,
            EnemyRepository enemyRepository,
            IBGMPlayer bgmPlayer,
            IWaveStateReceiver waveStateReceiver)
        {
            _waveUseCase = waveUseCase;
            _wavePath = wavePathPresenter;
            _symphony = player;
            _enemyRepository = enemyRepository;
            _bgmPlayer = bgmPlayer;
            _waveStateReceiver = waveStateReceiver;
        }

        public void SetWaveHandler(IWaveHandler waveHandler)
        {
            _waveHandler = waveHandler;
        }

        public async void OnGameStarted()
        {
            await _wavePath.NextWave(_waveUseCase.CurrentWaveIndex); // 最初のウェーブ位置へ移動。

            // 最初のウェーブの敵を生成。
            WaveEntity nextWave = _waveUseCase.CurrentWave;
            CreateWaveEnemies(nextWave);
            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            _waveStateReceiver.OnWaveStarted();
        }

        public async void OnWaveChanged(WaveEntity nextWave)
        {
            _waveStateReceiver.OnWaveCleared();

            _symphony.ResetUsingCard();

            await _wavePath.NextWave(_waveUseCase.CurrentWaveIndex);
            CreateWaveEnemies(nextWave);

            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            _waveStateReceiver.OnWaveStarted();
        }

        public void OnGameEnded()
        {
            // ゲーム終了時の演出などが必要な場合はここに記述
            _waveStateReceiver.OnWaveCleared(); // 念のため入力を止める
        }

        /// <summary>
        ///     敵が倒されたときの処理。
        /// </summary>
        private void HandleEnemyDead()
        {
            _enemyCount--;
            if (_enemyCount <= 0)
            {
                _waveHandler.OnWaveCompleted(); // ウェーブ完了を通知
            }
        }

        private void CreateWaveEnemies(WaveEntity waveEntity)
        {
            CharacterData[] enemyData = waveEntity.Enemies;
            _enemyCount = enemyData.Length;

            foreach (var item in enemyData)
            {
                CharacterEntity enemy = _enemyRepository.CreateEnemy(item);
                enemy.OnDead += HandleEnemyDead;
            }
        }
    }
}
