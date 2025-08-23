using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Cryptos.Editor.ProjectSetting
{
    public static class CryptosEditorProvider
    {
        [SettingsProvider]
        public static SettingsProvider CreateExampleProvider()
        {
            var provider = new SettingsProvider("Cryptos/", SettingsScope.Project)
            {
                // タイトル
                label = "Example",
                // GUI描画
                guiHandler = searchContext => EditorGUILayout.Toggle("Example", true),
                // 検索時のキーワード
                keywords = new HashSet<string>(new[] { "Example" })
            };

            return provider;
        }
    }
}
