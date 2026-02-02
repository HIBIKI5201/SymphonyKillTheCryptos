using System.Collections.Generic;
using UnityEditor;

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

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.LabelField("テストだよ");
        }
    }
}
