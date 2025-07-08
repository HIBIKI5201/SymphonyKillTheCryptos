using Cryptos.Runtime.Entity.Ingame.Word;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Cryptos.Editor.Ingame
{
    [CustomEditor(typeof(WordDataBase))]
    public class WordDataBaseDrawer : UnityEditor.Editor
    {
        private const string URL_KEY = "word-database-key";
        private const string SHEET_NAME_KEY = "sheet-name-key";
        private const string WORDS_PROP_NAME = "_words";


        private string _url;
        private string _sheetName;

        private void OnEnable()
        {
            _url = EditorPrefs.GetString(URL_KEY, string.Empty);
            _sheetName = EditorPrefs.GetString(SHEET_NAME_KEY, string.Empty);
        }

        public override void OnInspectorGUI()
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

        private async void LoadDataBase()
        {
            //リクエストを待機
            UnityWebRequest request = UnityWebRequest.Get(
                "https://docs.google.com/spreadsheets/d/" + _url
                + "/gviz/tq?tqx=out:csv&sheet=" + _sheetName);

            await request.SendWebRequest();

            //リクエストに失敗したかどうか
            if (request.result == UnityWebRequest.Result.ConnectionError || request.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.Log(request.error);
                return;
            }

            Debug.Log("リクエスト成功");

            StringReader reader = new StringReader(request.downloadHandler.text);

            int index = 0;
            List<WordData> wordDatas = new();

            while (reader.Peek() != -1)
            {
                index++;
                string line = reader.ReadLine();

                string[] words = line
                                .Split(',')                                     // カンマで分割
                                .Select(s => Regex.Replace(s, "[^a-zA-Z]", "")) // 各要素からアルファベット以外を除去
                                .Where(s => !string.IsNullOrEmpty(s))           // 空文字を除外（念のため）
                                .ToArray();

                WordData data = new(words, index);
                wordDatas.Add(data);
            }

            WordDataBase db = (WordDataBase)target;
            // private フィールド _words を取得
            FieldInfo field = typeof(WordDataBase).GetField(WORDS_PROP_NAME, BindingFlags.NonPublic | BindingFlags.Instance);

            if (field != null)
            {
                field.SetValue(db, wordDatas.ToArray());
                EditorUtility.SetDirty(db);
            }
        }
    }
}
