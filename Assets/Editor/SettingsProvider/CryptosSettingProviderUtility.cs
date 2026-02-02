using UnityEditor;

namespace Cryptos.Editor.ProjectSetting
{
    public static class CryptosSettingProviderUtility
    {
        public const string EDITOR_CONFIG_PAHT = "ProjectSettings/Cryptos/";

        public static string GetSettingPath<T>()
            where T : SettingsProvider
            => $"Cryptos/{typeof(T).Name}";
    }
}
