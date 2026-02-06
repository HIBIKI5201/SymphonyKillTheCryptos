using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Cryptos.Runtime.UI
{
    public abstract class VisualElementBase : VisualElement
    {
        /// <summary>
        ///     初期化のタイプ
        /// </summary>
        [Flags]
        public enum InitializeType
        {
            None = 0,
            Absolute = 1 << 0,
            FullLength = 1 << 1,
            PickModeIgnore = 1 << 2,
            All = Absolute | FullLength | PickModeIgnore
        }

        public VisualElementBase(string path, InitializeType initializeType = InitializeType.All)
        {
            InitializeTask = Initialize(path, initializeType);
        }

        /// <summary>
        ///     初期化処理のタスク
        /// </summary>
        public Task InitializeTask { get; private set; }

        /// <summary>
        ///     初期化処理
        /// </summary>
        /// <param name="address">UXMLのパス</param>
        /// <param name="type">初期化のタイプ</param>
        /// <returns></returns>
        private async Task Initialize(string address, InitializeType type)
        {
            if (string.IsNullOrEmpty(address))
            {
                Debug.LogError($"{name} failed initialize");
                return;
            }

            AsyncOperationHandle<VisualTreeAsset> handle = Addressables.LoadAssetAsync<VisualTreeAsset>(address);
            await handle.Task;
            VisualTreeAsset treeAsset = handle.Result;

            if (treeAsset != null)
            {
                #region 親エレメントの初期化

                treeAsset.CloneTree(this);

                if ((type & InitializeType.PickModeIgnore) != 0)
                {
                    RegisterCallback<KeyDownEvent>(e => e.StopPropagation());
                    pickingMode = PickingMode.Ignore;
                }

                if ((type & InitializeType.Absolute) != 0)
                {
                    style.position = Position.Absolute;
                }

                if ((type & InitializeType.FullLength) != 0)
                {
                    style.height = Length.Percent(100);
                    style.width = Length.Percent(100);
                }

                #endregion

                // UI要素の取得
                await Initialize_S(this);
            }
            else
            {
                Debug.LogError($"Failed to load UXML file \nfrom : {address}");
            }

            handle.Release();
        }

        /// <summary>
        ///     サブクラス固有の初期化処理
        /// </summary>
        /// <param name="container">ロードしたUXMLのコンテナ</param>
        /// <returns></returns>
        protected abstract ValueTask Initialize_S(VisualElement root);
    }
}
