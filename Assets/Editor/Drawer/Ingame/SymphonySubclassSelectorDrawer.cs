#if UNITY_2019_3_OR_NEWER

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(SymphonySubclassSelectorAttribute))]
public class SymphonySubclassSelectorDrawer : PropertyDrawer
{
    /// <summary>
    /// 型情報をキャッシュするための静的な辞書。
    /// Key: 基底クラスの型
    /// Value: (派生型の配列, ポップアップ表示用の型名の配列, 完全修飾名の配列)
    /// </summary>
    private static readonly Dictionary<Type, (Type[] types, string[] names, string[] fullNames)> s_TypeCache = new();

    /// <summary>
    /// プロパティのGUIを描画する。
    /// </summary>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // ManagedReference型でない場合は処理を中断する。
        if (property.propertyType != SerializedPropertyType.ManagedReference)
        {
            return;
        }

        // 型情報を取得し、キャッシュが存在しない場合は作成する。
        var baseType = GetType(property);
        if (baseType == null)
        {
            EditorGUI.PropertyField(position, property, label, true);
            return;
        }

        if (!s_TypeCache.TryGetValue(baseType, out var cachedData))
        {
            cachedData = CreateInheritedTypesCache(baseType, ((SymphonySubclassSelectorAttribute)attribute).IsIncludeMono());
            s_TypeCache.Add(baseType, cachedData);
        }
        var (inheritedTypes, typePopupNameArray, typeFullNameArray) = cachedData;

        // 現在選択されている型のインデックスを取得する。見つからない場合は<null> (インデックス0) にする。
        int currentTypeIndex = Array.IndexOf(typeFullNameArray, property.managedReferenceFullTypename);
        if (currentTypeIndex < 0)
        {
            currentTypeIndex = 0;
        }

        // 型を選択するためのポップアップGUIを描画する。
        int selectedTypeIndex = EditorGUI.Popup(GetPopupPosition(position), currentTypeIndex, typePopupNameArray);

        // 選択が変更された場合、プロパティに新しいインスタンスを割り当てる。
        if (currentTypeIndex != selectedTypeIndex)
        {
            Type selectedType = inheritedTypes[selectedTypeIndex];
            property.managedReferenceValue =
                selectedType == null ? null : Activator.CreateInstance(selectedType);
        }

        // デフォルトのプロパティフィールドを描画する。これにより、選択した型の中身が表示・編集できる。
        EditorGUI.PropertyField(position, property, label, true);
    }

    /// <summary>
    /// プロパティの高さを取得する。
    /// </summary>
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, true);
    }

    /// <summary>
    /// 指定された基底クラスを継承する全ての型情報を収集し、キャッシュデータを作成する。
    /// </summary>
    /// <param name="baseType">基底クラスの型。</param>
    /// <param name="includeMono">MonoBehaviourを継承する型を含めるかどうか。</param>
    /// <returns>派生型情報のキャッシュデータ。</returns>
    private static (Type[], string[], string[]) CreateInheritedTypesCache(Type baseType, bool includeMono)
    {
        Type monoType = typeof(MonoBehaviour);

        // 1. abstractクラスも含む、すべての派生型リストを作成する (カテゴリ判定用)
        var allInheritedTypesWithAbstract = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(s => s.GetTypes())
            .Where(p => baseType.IsAssignableFrom(p) && p.IsClass && (!monoType.IsAssignableFrom(p) || includeMono))
            .ToArray();

        // 2. ユーザーが選択可能な、abstractでない派生型リストを作成する
        var selectableTypesArray = allInheritedTypesWithAbstract
            .Where(t => !t.IsAbstract)
            .ToArray();

        // <null>オプションを先頭に追加する。
        var finalSelectableTypes = selectableTypesArray.Prepend(null).ToArray();

        // 3. ポップアップ表示用の型名配列を作成する
        var typePopupNameArray = finalSelectableTypes.Select(type =>
        {
            if (type == null) return "<null>";

            string displayName;
            var parent = type.BaseType;

            // 親クラスが「中間クラス」(abstract含む)である場合、カテゴリ分けする。
            // 判定には allInheritedTypesWithAbstract を使用する。
            if (parent != null && parent != baseType && allInheritedTypesWithAbstract.Contains(parent))
            {
                displayName = $"{parent.Name}/{type.Name}";
            }
            // 自身が他クラスの「中間クラス」(abstract含む)である場合、自身をカテゴリのルートとして表示する。
            // このロジックは、abstractクラス自身は選択肢にないので、具象クラスが中間クラスになる場合のみ適用される。
            else if (allInheritedTypesWithAbstract.Any(t => t.BaseType == type))
            {
                displayName = $"{type.Name}/{type.Name}";
            }
            // 上記以外の場合は、カテゴリ分けせずに表示する。
            else
            {
                displayName = type.Name;
            }
            
            // ネストクラスの '+' を '/' に置換する。
            if (displayName.Contains('+'))
            {
                displayName = displayName.Replace('+', '/');
            }

            return displayName;
        }).ToArray();

        // 完全修飾名の配列を作成する。
        var typeFullNameArray = finalSelectableTypes.Select(type => type == null ? "" : $"{type.Assembly.GetName().Name} {type.FullName}").ToArray();

        // 最終的に返却するのは、選択可能な型と、それに対応する表示名。
        return (finalSelectableTypes, typePopupNameArray, typeFullNameArray);
    }

    /// <summary>
    /// ポップアップGUIの描画範囲を取得する。
    /// </summary>
    /// <param name="currentPosition">現在のプロパティの描画範囲。</param>
    /// <returns>ポップアップの描画範囲。</returns>
    private Rect GetPopupPosition(Rect currentPosition)
    {
        Rect popupPosition = new Rect(currentPosition);
        popupPosition.width -= EditorGUIUtility.labelWidth;
        popupPosition.x += EditorGUIUtility.labelWidth;
        popupPosition.height = EditorGUIUtility.singleLineHeight;
        return popupPosition;
    }

    // 15. 内部メソッド、抽出メソッド
    /// <summary>
    ///     SerializedPropertyから、そのプロパティの基底となる型を取得する。
    /// </summary>
    /// <param name="property">対象のプロパティ。</param>
    /// <returns>プロパティの基底型。</returns>
    public static Type GetType(SerializedProperty property)
    {
        FieldInfo fieldInfo = GetFieldInfo(property);
        if (fieldInfo == null)
        {
            Debug.LogWarning($"Could not find field for property {property.propertyPath}");
            return null;
        }

        Type fieldType = fieldInfo.FieldType;

        // 配列の場合は、その要素の型を返す。
        if (fieldType.IsArray)
        {
            return fieldType.GetElementType();
        }

        // List<>の場合は、そのジェネリック引数の型を返す。
        if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
        {
            return fieldType.GetGenericArguments()[0];
        }

        return fieldType;
    }

    /// <summary>
    ///     SerializedPropertyのパスを解析し、対応するFieldInfoを取得する。
    /// </summary>
    /// <param name="property">対象のプロパティ。</param>
    /// <returns>対応するFieldInfo。</returns>
    public static FieldInfo GetFieldInfo(SerializedProperty property)
    {
        string propertyPath = property.propertyPath;
        // 配列のパスを解析しやすいように置換する。 e.g. "array.Array.data[0]" -> "array[0]"
        string[] pathElements = propertyPath.Replace(".Array.data[", "[").Split('.');
        Type currentType = property.serializedObject.targetObject.GetType();
        FieldInfo field = null;

        foreach (string element in pathElements)
        {
            if (element.Contains("["))
            {
                string fieldName = element.Substring(0, element.IndexOf("["));

                // 基底クラスを遡ってフィールドを探す。
                field = null;
                for (Type t = currentType; field == null && t != null; t = t.BaseType)
                {
                    field = t.GetField(fieldName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }

                if (field == null) return null;

                // 配列やリストの要素の型に更新する。
                Type fieldType = field.FieldType;
                if (fieldType.IsArray)
                {
                    currentType = fieldType.GetElementType();
                }
                else if (fieldType.IsGenericType && fieldType.GetGenericTypeDefinition() == typeof(System.Collections.Generic.List<>))
                {
                    currentType = fieldType.GetGenericArguments()[0];
                }
                else
                {
                    return null; // 配列でもListでもない場合は解析不能。
                }
            }
            else
            {
                // 基底クラスを遡ってフィールドを探す。
                field = null;
                for (Type t = currentType; field == null && t != null; t = t.BaseType)
                {
                    field = t.GetField(element, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                }
                if (field == null) return null;
                currentType = field.FieldType;
            }
        }
        return field;
    }
}
#endif