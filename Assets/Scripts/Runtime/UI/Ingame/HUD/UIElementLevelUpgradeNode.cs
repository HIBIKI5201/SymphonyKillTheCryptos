using Cryptos.Runtime.Presenter.Ingame.System;
using Cryptos.Runtime.Presenter.Ingame.Word;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UIElements;

namespace Cryptos.Runtime.UI.Ingame.LevelUp
{
    /// <summary>
    /// レベルアップノードのUI要素。
    /// </summary>
    [UxmlElement]
    public partial class UIElementLevelUpgradeNode : VisualElementBase
    {
        /// <summary>
        /// コンストラクタ。
        /// </summary>
        public UIElementLevelUpgradeNode() : base(LEVEL_UPGRADE_NODE_NAME, 0)
        {
        }

        /// <summary>
        /// このノードが選択されたかどうか。
        /// </summary>
        public bool IsSelected => _isSelected;
        /// <summary>
        /// このノードのビューモデル。
        /// </summary>
        public LevelUpgradeNodeViewModel NodeViewModel => _nodeViewModel;

        /// <summary>
        /// 文字入力を処理します。
        /// </summary>
        /// <param name="c">入力された文字。</param>
        public void OnInputChar(char c)
        {
            _wordEntity.InputChar(c);
        }

        /// <summary>
        /// このノードにデータを設定します。
        /// </summary>
        /// <param name="vm">設定するデータを含むビューモデル。</param>
        public void SetData(LevelUpgradeNodeViewModel vm)
        {
            _iconElement.style.backgroundImage = new StyleBackground(vm.Texture);
            _nameLabel.text = vm.NodeName;
            _descriptionLabel.text = vm.Description;

            _nodeViewModel = vm;

            //アルファベット以外を無くす。
            string word = Regex.Replace(vm.NodeName, "[^a-zA-Z]", "");
            _wordEntity = WordGenerator.GetWordEntity(word);

            _wordEntity.OnComplete += () => _isSelected = true;

            HandleWordUpdated(_wordEntity.Word, 0);
            _wordEntity.OnWordUpdated += HandleWordUpdated;

            RankElementActivate(vm.MaxRank);

        }

        protected override async ValueTask Initialize_S(VisualElement root)
        {
            _wordLabel = root.Q<Label>(WORD_ELEMENT_NAME);
            _iconElement = root.Q<VisualElement>(ICON_ELEMENT_NAME);
            _nameLabel = root.Q<Label>(NAME_ELEMENT_NAME);
            _descriptionLabel = root.Q<Label>(EXPLANATION_ELEMENT_NAME);
            _rankRoot = root.Q<VisualElement>(RANK_ROOT_NAME);

            AsyncOperationHandle<VisualTreeAsset> rankElement
                = Addressables.LoadAssetAsync<VisualTreeAsset>(LEVEL_UPGRADE_NODE_RANK_ELEMENT_NAME);
            await rankElement.Task;
            _rankeElement = rankElement.Result;
        }

        private const string LEVEL_UPGRADE_NODE_NAME = "LevelUpgradeNode";
        private const string LEVEL_UPGRADE_NODE_RANK_ELEMENT_NAME = "LevelUpgradeNodeRankElement";

        private const string WORD_ELEMENT_NAME = "word";
        private const string ICON_ELEMENT_NAME = "icon";
        private const string NAME_ELEMENT_NAME = "name";
        private const string EXPLANATION_ELEMENT_NAME = "explanation";
        private const string RANK_ROOT_NAME = "rank-root";

        private VisualTreeAsset _rankeElement;

        private Label _wordLabel;
        private VisualElement _iconElement;
        private Label _nameLabel;
        private Label _descriptionLabel;
        private VisualElement _rankRoot;
        private VisualElement _rankElement;

        private LevelUpgradeNodeViewModel _nodeViewModel;
        private WordEntityViewModel _wordEntity;
        private bool _isSelected;

        /// <summary>
        /// ワードの表示を更新します。
        /// </summary>
        /// <param name="word">表示するワード。</param>
        /// <param name="index">入力済みの文字数。</param>
        private void HandleWordUpdated(string word, int index)
        {
            _wordLabel.text = $"<b><color=green>{word[..index]}</color></b>{word[index..]}";
        }

        private void RankElementActivate(int count)
        {
            for (int i = 0; i < count; i++)
            {
                VisualElement e = _rankeElement.Instantiate();
                _rankRoot.Add(e);

                if (i < _nodeViewModel.CurrentRank)
                {
                    e.style.backgroundColor = Color.blue;
                }
            }
        }
    }
}
