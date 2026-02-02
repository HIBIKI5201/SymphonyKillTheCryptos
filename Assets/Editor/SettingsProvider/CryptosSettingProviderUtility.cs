using Unity.VisualScripting;
using UnityEditor;

namespace Cryptos.Editor.ProjectSetting
{
    public static class CryptosSettingProviderUtility
    {
        public static string GetSettingPath<T>()
            where T : SettingsProvider
            => $"Cryptos/{typeof(T)}";
    }
}
