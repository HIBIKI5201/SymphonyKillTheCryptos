using UnityEngine;
using UnityEditor;
using Cryptos.Runtime.Ingame.Entity;

namespace Cryptos.Editor.Ingame
{
    [CustomEditor(typeof(WordDataBase))]
    public class WordDataBaseDrawer : UnityEditor.Editor
    {
        private const string URL_KEY = "word-database-key";
        private const string SHEET_NAME_KEY = "sheet-name-key";
        
        private string _url;
        private string _sheetName;

        private void OnEnable()
        {
            _url = EditorPrefs.GetString(URL_KEY, string.Empty);
            _sheetName = EditorPrefs.GetString(SHEET_NAME_KEY, string.Empty);
        }

        public override void OnInspectorGUI ()
        {
            EditorGUILayout.LabelField("データベースURL");
            _url = EditorGUILayout.TextField("URL", _url);
            _sheetName = EditorGUILayout.TextField("シート名", _sheetName);

            // 値が変更されたら保存
            if (GUI.changed)
            {
                EditorPrefs.SetString(URL_KEY, _url);
                EditorPrefs.SetString(SHEET_NAME_KEY, _sheetName);
            }

            if (GUILayout.Button("データベースを読み込む"))
            {
                LoadDataBase();
            }

            DrawDefaultInspector();
        }

        private void LoadDataBase()
        {

        }
    }
}
