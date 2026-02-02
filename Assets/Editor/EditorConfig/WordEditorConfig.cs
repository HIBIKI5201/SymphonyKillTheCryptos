using UnityEditor;
using UnityEngine;

namespace Cryptos.Editor.ProjectSetting
{
    [FilePath(CryptosSettingProviderUtility.EDITOR_CONFIG_PAHT + nameof(WordEditorConfig) + ".asset",
        FilePathAttribute.Location.ProjectFolder)]
    public class WordEditorConfig : ScriptableSingleton<WordEditorConfig>
    {
        public string URL;
        public string SheetName;

        public void Save()
        {
            Save(true);
        }
    }
}
