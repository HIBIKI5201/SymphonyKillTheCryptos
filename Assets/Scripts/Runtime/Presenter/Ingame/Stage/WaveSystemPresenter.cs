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
    ///     ウェーブの進行を管理するクラスである。
    /// </summary>
    public class WaveSystemPresenter : IInGameLoopWaveHandler
    {
        private readonly WaveUseCase _waveUseCase;
        private readonly WavePathPresenter _wavePath;
        private readonly SymphonyPresenter _symphony;
        private readonly EnemyRepository _enemyRepository;
        private readonly IBGMPlayer _bgmPlayer;
        private readonly IWaveStateReceiver _waveStateReceiver;
        private IWaveHandler _waveHandler;

        private int _enemyCount;

        /// <summary>
        ///     WaveSystemPresenterの新しいインスタンスを初期化する。
        /// </summary>
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

        /// <summary>
        ///     IWaveHandlerを設定する。
        /// </summary>
        /// <param name="waveHandler">設定するIWaveHandler。</param>
        public void SetWaveHandler(IWaveHandler waveHandler)
        {
            _waveHandler = waveHandler;
        }

        /// <summary>
        ///     ゲーム開始時に最初のウェーブを開始する。
        /// </summary>
        public async void OnGameStarted()
        {
            await _wavePath.NextWave(_waveUseCase.CurrentWaveIndex); // 最初のウェーブ位置へ移動する。

            // 最初のウェーブの敵を生成する。
            WaveEntity nextWave = _waveUseCase.CurrentWave;
            CreateWaveEnemies(nextWave);
            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            _waveStateReceiver.OnWaveStarted();
        }

        /// <summary>
        ///     次のウェーブに移行する。
        /// </summary>
        /// <param name="nextWave">次のWaveエンティティ。</param>
        public async void OnWaveChanged(WaveEntity nextWave)
        {
            _waveStateReceiver.OnWaveCleared();

            _symphony.ResetUsingCard();

            await _wavePath.NextWave(_waveUseCase.CurrentWaveIndex);
            CreateWaveEnemies(nextWave);

            _bgmPlayer.PlayBGM(nextWave.BGMCueName);

            _waveStateReceiver.OnWaveStarted();
        }

        /// <summary>
        ///     ゲーム終了時の処理を実行する。
        /// </summary>
        public void OnGameEnded()
        {
            // ゲーム終了時の演出などが必要な場合はここに記述する。
            _waveStateReceiver.OnWaveCleared(); // 念のため入力を止める。
        }

        /// <summary>
        ///     敵が倒されたときの処理を実行する。
        /// </summary>
        private void HandleEnemyDead()
        {
            _enemyCount--;
            if (_enemyCount <= 0)
            {
                // ウェーブ完了を通知する。
                _waveHandler.OnWaveCompleted();
            }
        }

        /// <summary>
        ///     ウェーブに対応する敵を生成する。
        /// </summary>
        /// <param name="waveEntity">対象のWaveエンティティ。</param>
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
