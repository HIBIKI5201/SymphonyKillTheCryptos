using Cryptos.Runtime;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal.VR;
using UnityEngine;
using UnityEngine.UIElements;

namespace Cryptos.Editor.ProjectSetting
{
    public class WordEditorProvider : SettingsProvider
    {
        public WordEditorProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        {
        }

        [SettingsProvider]
        public static SettingsProvider CreateProvider()
        {
            return new WordEditorProvider(
                CryptosSettingProviderUtility.GetSettingPath<WordEditorProvider>(),
                SettingsScope.Project,
                new string[] { "word", "database" });
        }

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            WordEditorConfig preferences = WordEditorConfig.instance;
            preferences.hideFlags = HideFlags.HideAndDontSave & ~HideFlags.NotEditable;
        }

        public override void OnGUI(string searchContext)
        {
            WordEditorConfig preferences = WordEditorConfig.instance;

            EditorGUI.BeginChangeCheck();

            EditorGUILayout.LabelField("Google Spreadsheet", EditorStyles.boldLabel);

            string url = EditorGUILayout.TextField(
                "URL",
                preferences.URL
            );

            string sheetName = EditorGUILayout.TextField(
                "Sheet Name",
                preferences.SheetName
            );

            if (EditorGUI.EndChangeCheck())
            {
                // 差分があったら保存。
                preferences.URL = url.Trim();
                preferences.SheetName = sheetName.Trim();
                WordEditorConfig.instance.Save();
            }
        }
    }
}
