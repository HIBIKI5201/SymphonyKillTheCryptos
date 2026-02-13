using CriWare;
using Cryptos.Runtime.Presenter;
using Cryptos.Runtime.Presenter.Ingame.Card;
using Cryptos.Runtime.Presenter.Ingame.Character;
using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.UI.Basis;
using Cryptos.Runtime.UI.Ingame.Card;
using Cryptos.Runtime.UI.Ingame.Character;
using Cryptos.Runtime.UI.Ingame.LevelUp;
using SymphonyFrameWork.Utility;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.Manager
{
    /// <summary>
    ///     インゲームのUIを管理します。
    /// </summary>
    public class IngameUIManager : UIManagerBase, ICardUIManager, IIngameUIManager, IComboUIManager, ILevelUpUIManager
    {
        public event Action OnResultWindowReturnButtonClicked
        {
            add => _resultWindow.OnReturnButtonClicked += value;
            remove => _resultWindow.OnReturnButtonClicked -= value;
        }

        /// <summary>
        ///     レベルアップ時の非同期処理。
        /// </summary>
        /// <param name="nodes">レベルアップ候補のノード。</param>
        /// <returns>選択されたレベルアップノード。</returns>
        public async Task<LevelUpgradeNodeViewModel> LevelUpSelectAsync(Memory<LevelUpgradeNodeViewModel> nodes)
        {
            // ウィンドウを出現させて待機。
            await OpenLevelUpgradeWindow(nodes);
            LevelUpgradeNodeViewModel selectedNodeVM = default;
            await SymphonyTask.WaitUntil(
                () => TryGetSelectedLevelUpgradeNode(out selectedNodeVM));

            Debug.Log($"レベルアップカードを選択しました。{selectedNodeVM.NodeName}");

            CloseLevelUpgradeWindow();

            return selectedNodeVM;
        }

        /// <summary>
        ///     カードをUIに追加する。
        /// </summary>
        /// <param name="instance">追加するカードのビューモデル。</param>
        public void AddCard(CardViewModel instance)
        {
            _hand.HandleAddCard(instance);
        }

        /// <summary>
        ///     カードをUIから削除する。
        /// </summary>
        /// <param name="instance">削除するカードのビューモデル。</param>
        public void RemoveCard(CardViewModel instance)
        {
            _hand.HandleRemoveCard(instance);
        }

        /// <summary>
        ///     カードをデッキからスタックに移動させる。
        /// </summary>
        /// <param name="instance"></param>
        public void MoveCardToStack(CardViewModel instance)
        {
            _hand.MoveCardToStack(instance);
        }

        /// <summary>
        ///     レベルアップウィンドウを開きます。
        /// </summary>
        /// <param name="nodes">表示するノード。</param>
        public async ValueTask OpenLevelUpgradeWindow(Memory<LevelUpgradeNodeViewModel> nodes)
        {
            await _levelUpgrade.OpenWindow(nodes);
        }

        /// <summary>
        ///     レベルアップウィンドウを閉じます。
        /// </summary>
        public void CloseLevelUpgradeWindow()
        {
            _levelUpgrade.CloseWindow();
        }

        /// <summary>
        ///     選択されたレベルアップノードを取得しようとします。
        /// </summary>
        /// <param name="nodeVM">選択されたノードのビューモデル。</param>
        /// <returns>ノードが選択されていればtrue、そうでなければfalse。</returns>
        public bool TryGetSelectedLevelUpgradeNode(out LevelUpgradeNodeViewModel nodeVM)
        {
            UIElementLevelUpgradeNode node = _levelUpgrade.GetSelectedLevelUpgrade();

            bool isNull = node == null;
            nodeVM = isNull ? default : node.NodeViewModel;

            return !isNull;
        }

        public void CardInputChar(char c)
        {
            _typingSoundSource?.Play();
        }

        /// <summary>
        ///     文字入力を処理します。
        /// </summary>
        /// <param name="c">入力された文字。</param>
        public void OnLevelUpgradeInputChar(char c)
        {
            _levelUpgrade.InputChar(c);

            _typingSoundSource?.Play();
        }

        /// <summary>
        ///     ダメージテキストを表示する。
        /// </summary>
        /// <param name="context"></param>
        /// <param name="position"></param>
        public void ShowDamageText(CombatContextViewModel context, Vector3 position)
        {
            _damageTextPool.ShowDamageText(context, position + _damageTextOffset);
        }

        public void CreateHealthBar(HealthBarViewModel healthBarVM)
        {
            UIElementHealthBar healthBar = new();
            healthBar.RegisterTarget(healthBarVM);
            _document.rootVisualElement.Add(healthBar);
        }

        public async void RegisterComboCountHandler(ComboViewModel vm)
        {
            await _comboCounter.InitializeTask;

            vm.OnChangedCounter += _comboCounter.SetCounter;
            vm.OnComboReset += _comboCounter.ResetCounter;
            vm.OnChangedTimer += _comboCounter.SetGuage;

            destroyCancellationToken.Register(() =>
            {
                vm.OnChangedCounter -= _comboCounter.SetCounter;
                vm.OnComboReset -= _comboCounter.ResetCounter;
                vm.OnChangedTimer -= _comboCounter.SetGuage;
            });
        }


        public void OpenResultWindow(ResultViewModel vm)
        {
            _resultWindow.SetResult(vm);
            _resultWindow.Open();
        }

        public void CloseResultWindow()
        {
            _resultWindow.Close();
        }

        protected override async Task InitializeDocumentAsync(UIDocument document, VisualElement root)
        {
            _hand = root.Q<UIElementHand>();
            _levelUpgrade = root.Q<UIElementLevelUpgradeWindow>();
            _comboCounter = root.Q<UIElementComboCounter>();
            _resultWindow = root.Q<UIElementResultWindow>();

            root.Add(_resultWindow);

            await _hand.InitializeTask;
            await _levelUpgrade.InitializeTask;
            await _comboCounter.InitializeTask;
            await _resultWindow.InitializeTask;

            _damageTextPool = new(root);

            _levelUpgrade.CloseWindow(); // 念のためにクローズする。
            _resultWindow.Close(); // 念のためにクローズする。
        }


        [SerializeField]
        private Vector3 _damageTextOffset;

        private CriAtomSource _typingSoundSource;

        private UIElementHand _hand;
        private UIElementLevelUpgradeWindow _levelUpgrade;
        private DamageTextPool _damageTextPool;
        private UIElementComboCounter _comboCounter;
        private UIElementResultWindow _resultWindow;

        private void Awake()
        {
            if (!TryGetComponent(out _typingSoundSource))
            {
                Debug.LogWarning("ui typing sound source is not found");
            }
        }
    }
}
