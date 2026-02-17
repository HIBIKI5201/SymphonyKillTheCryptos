using Cryptos.Runtime.Framework;
using Cryptos.Runtime.InfraStructure.OutGame.Story;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Cryptos.Runtime
{
    public class ScenarioDataGenerator : EditorWindow
    {
        [MenuItem(CryptosPathConstant.TOOL_MENU_ITEM_PATH + nameof(ScenarioDataGenerator))]
        public static void Window()
        {
            ScenarioDataGenerator window = (ScenarioDataGenerator)GetWindow(typeof(ScenarioDataGenerator));
            window.Show();
            window.Initialize();
        }

        private const string DEFALUT_EXPORT_PATH = "Assets/MasterData/Story/" + nameof(ScenarioDataAsset) + "/";
        private const string URL_SAVE_KEY = "ScenarioDataGenerator_URL";

        private string _url;
        private string _sheetName;
        private string _exportPath = DEFALUT_EXPORT_PATH;
        private string _lastCSV;
        private StringBuilder _logBuilder = new();
        private string _log;

        private void OnGUI()
        {
            string url = EditorGUILayout.TextField(_url);
            _sheetName = EditorGUILayout.TextField(_sheetName);
            _exportPath = EditorGUILayout.TextField(_exportPath);

            if (url != _url)
            {
                _url = url;
                EditorPrefs.SetString(URL_SAVE_KEY, _url);
            }

            if (GUILayout.Button("生成"))
            {
                _ = Generate();
            }

            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("log");
            EditorGUILayout.LabelField(_log, new GUIStyle(EditorStyles.label) { wordWrap = true, richText = true });

            EditorGUILayout.Space(20);

            EditorGUILayout.LabelField("preview");
            EditorGUILayout.LabelField(_lastCSV, new GUIStyle(EditorStyles.label) { wordWrap = true });
        }

        private void Initialize()
        {
            _url = EditorPrefs.GetString(URL_SAVE_KEY);
        }

        private async ValueTask Generate()
        {
            _logBuilder.Clear();

            string csv = await GetCSV();
            _lastCSV = csv;

            ScenarioDataAsset data = ScenarioDataConverter.Execute(csv, ref _logBuilder);
            Save(data);

            Repaint();

            _log = _logBuilder.ToString();
        }

        private async ValueTask<string> GetCSV()
        {
            var builder = new StringBuilder(100);
            builder.Append("https://docs.google.com/spreadsheets/d/");
            builder.Append(_url);
            builder.Append("/gviz/tq?tqx=out:csv&sheet=");
            builder.Append(_sheetName);

            UnityWebRequest request = UnityWebRequest.Get(builder.ToString());
            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError(request.error);
                return string.Empty;
            }

            return request.downloadHandler.text;
        }

        private void Save(ScenarioDataAsset asset)
        {
            if (asset == null)
            {
                Debug.LogError("生成された NovelData が null のため、保存できません。");
                return;
            }
            if (string.IsNullOrEmpty(_exportPath) || string.IsNullOrEmpty(_sheetName))
            {
                Debug.LogError("エクスポートパスまたはシート名が指定されていません。");
                return;
            }

            // ディレクトリが存在しない場合は作成
            if (!System.IO.Directory.Exists(_exportPath))
            {
                System.IO.Directory.CreateDirectory(_exportPath);
            }

            // 完全なアセットパスを生成
            string fullPath = System.IO.Path.Combine(_exportPath, $"{_sheetName}.asset");

            // 既存のアセットを削除
            if (AssetDatabase.LoadAssetAtPath<ScenarioDataAsset>(fullPath) != null)
            {
                AssetDatabase.DeleteAsset(fullPath);
            }

            AssetDatabase.CreateAsset(asset, fullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            Selection.activeObject = asset;
        }
    }
}
