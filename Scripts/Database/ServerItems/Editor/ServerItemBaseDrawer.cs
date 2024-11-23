using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(ServerItemBase))]
public class ServerItemBaseDrawer : PropertyDrawer
{
    //private bool m_foldOut;
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var itemProp = property.FindPropertyRelative("m_item");
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 2;

        var foldOutRect = new Rect(position.x, position.y, position.x+20, 20);
        property.isExpanded = EditorGUI.Foldout(foldOutRect, property.isExpanded, GUIContent.none);

        var prefixRect = new Rect(foldOutRect.xMax + 5, foldOutRect.y, position.width - foldOutRect.x-50, 20);

        GUI.enabled = false;
        EditorGUI.PropertyField(prefixRect, itemProp.FindPropertyRelative("m_itemID"), GUIContent.none);

        GUI.enabled = true;

        if (property.isExpanded)
        {
            //var y = prefixRect.yMax + 5;
            //var itemRelColProp = itemProp.FindPropertyRelative("m_itemRelations").FindPropertyRelative("m_list");
            //for (int i = 0; i < itemRelColProp.arraySize; i++)
            //{
            //    var relRect = new Rect(prefixRect.x, y, 100, 20);
            //    var itemRelProp = itemRelColProp.GetArrayElementAtIndex(i);
            //    EditorGUI.PropertyField(relRect, itemRelProp.FindPropertyRelative("m_virtualGoodItemID"),
            //        GUIContent.none);
            //    relRect.x = relRect.xMax + 5;
            //    relRect.width = 50;
            //    EditorGUI.LabelField(relRect, "<-->");
            //    relRect.x = relRect.xMax + 5;
            //    relRect.width = 100;
            //    EditorGUI.PropertyField(relRect, itemRelProp.FindPropertyRelative("m_virtualCurrencyItemID"),
            //        GUIContent.none);
            //}

            GUI.enabled = false;
            var typeRect = new Rect(prefixRect.x + 5, prefixRect.yMax + 5, 95, 20);
            var groupRect = new Rect(typeRect.xMax + 5, prefixRect.yMax + 5, 95, 20);

            EditorGUI.PropertyField(typeRect, itemProp.FindPropertyRelative("m_itemType"), GUIContent.none);
            EditorGUI.PropertyField(groupRect, itemProp.FindPropertyRelative("m_group"), GUIContent.none);

            GUI.enabled = true;
            // Calculate rects
            foldOutRect.y = groupRect.yMax + 5;
            foldOutRect.x = prefixRect.x;
            var arraySizerect = new Rect(foldOutRect.xMax + 5, groupRect.yMax + 5, position.width - foldOutRect.x - 50, 20);
            // Draw fields - passs GUIContent.none to each so they are drawn without labels
            var assetprop = property.FindPropertyRelative("m_assets");
            assetprop.isExpanded = EditorGUI.Foldout(foldOutRect, assetprop.isExpanded, GUIContent.none);
            EditorGUI.PropertyField(arraySizerect, assetprop.FindPropertyRelative("Array.size"), new GUIContent("Asset Size"));

            var resourceTypeRect = new Rect(foldOutRect.x, arraySizerect.yMax + 5, 100, 20);
            var buttonRect = new Rect(resourceTypeRect.xMax+5, arraySizerect.yMax + 5, 10, 20);
            var resourcePathRect = new Rect(resourceTypeRect.xMax + 5, arraySizerect.yMax + 5,
                position.width - foldOutRect.x - 110, 20);
            if (assetprop.isExpanded)
            {
                for (int i = 0; i < assetprop.arraySize; i++)
                {
                    var resourceProp = assetprop.GetArrayElementAtIndex(i).FindPropertyRelative("m_resourcePath");
                    if (GUI.Button(buttonRect, "*"))
                    {
                        resourceProp.stringValue = itemProp.FindPropertyRelative("m_itemID").stringValue;
                    }
                    EditorGUI.PropertyField(resourceTypeRect,
                        assetprop.GetArrayElementAtIndex(i).FindPropertyRelative("m_assetType"),GUIContent.none);
                    EditorGUI.PropertyField(resourcePathRect,
                        resourceProp, GUIContent.none);
                    resourceTypeRect.y = resourceTypeRect.yMax + 5;
                    resourcePathRect.y = resourcePathRect.yMax + 5;
                    buttonRect.y = buttonRect.yMax + 5;
                }
            }
            //EditorGUI.PropertyField(resourceRect, property.FindPropertyRelative("m_resourcePath"), GUIContent.none);

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;
        }

        EditorGUI.EndProperty();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        var size = 20;
        if (property.isExpanded)
        {
            size += 70;
            var assetprop = property.FindPropertyRelative("m_assets");
            if (assetprop.isExpanded)
            {
                size += assetprop.arraySize*25;
            }
        }
        return size;
        //return base.GetPropertyHeight(property, label);
    }
}
