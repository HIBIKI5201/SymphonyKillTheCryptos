using Cryptos.Runtime.Entity.Ingame.Card;
using Cryptos.Runtime.Entity.Ingame.Character;
using Cryptos.Runtime.Entity.Ingame.Word;
using Cryptos.Runtime.Framework;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.UseCase.Ingame.Card;
using SymphonyFrameWork.System;
using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cryptos.Runtime.Presenter.Ingame.Sequence
{
    [Serializable]
    public class IngameStartSequence : IDisposable
    {
        public void Dispose()
        {
            ServiceLocator.DestroyInstance<CardUseCase>();
        }

        [SerializeField, Tooltip("プレイヤーデータ")]
        private SymphonyData _symphonyData;
        [SerializeField, Tooltip("ワードのデータベース")]
        private WordDataBase _wordDataBase;

        [Header("テストコード")]
        [SerializeField]
        private CardData[] _cardDatas;
        [SerializeField, Min(1)]
        private int _cardAmount = 3;

        [Space]
        [SerializeField]
        private EnemyData _enemyData;
        private int _enemyAmount = 3;

        CardUseCase _cardUseCase;
        CharacterEntity<SymphonyData> _symphony;
        EnemyRepository _enemy;

        public async Task StartSequence()
        {
            _symphony = new(_symphonyData);
            _enemy = new();

            _cardUseCase = new(_wordDataBase, new());
            _cardUseCase.GetPlayer += () => _symphony;
            _cardUseCase.GetTargets += () => _enemy.AllEnemies.ToArray();

            ServiceLocator.RegisterInstance(_cardUseCase);

            InputBuffer inputBuffer = await ServiceLocator.GetInstanceAsync<InputBuffer>();
            inputBuffer.OnAlphabetKeyPressed += _cardUseCase.InputCharToDeck;

            await Awaitable.WaitForSecondsAsync(1); //UIの初期化を待つ
            TestCardSpawn();
            TestEnemySpawn();
        }

        private void TestCardSpawn()
        {
            if (_cardDatas == null || _cardDatas.Length == 0)
            {
                Debug.LogWarning("カードデータが設定されていません。");
                return;
            }

            for (int i = 0; i < _cardAmount; i++)
            {
                RandomDraw();
            }

            void RandomDraw()
            {
                Debug.Log("draw");
                var cardData = _cardDatas[UnityEngine.Random.Range(0, _cardDatas.Length)];
                var instance = _cardUseCase.CreateCard(cardData);

                instance.OnComplete += RandomDraw;
            }
        }

        private void TestEnemySpawn()
        {
            for (int i = 0; i < _enemyAmount; i++)
            {
                var enemy = _enemy.CreateEnemy(_enemyData);
                if (enemy == null)
                {
                    Debug.LogWarning("エネミーの生成に失敗しました。");
                    continue;
                }
            }
        }
    }
}
