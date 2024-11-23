using System.IO;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(AnimationCurve))]
public class AnimationCurvePropertyDrawer : PropertyDrawer
{
    private static string m_defaultPath;

    void OnEnable()
    {
        if (string.IsNullOrEmpty(m_defaultPath))
        {
            m_defaultPath = EditorPrefs.GetString("OpenFilePath");
        }
    }
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var buttonSize = 50;
        position.width -= buttonSize;
        //base.OnGUI(position, property, GUIContent.none);
        EditorGUI.PropertyField(position, property, label);
        var buttonRect = position;
        buttonRect.x = position.xMax + 5;
        buttonRect.width = (buttonSize-5);


        //if (GUI.Button(buttonRect, "Save"))
        //{
        //    var path = EditorUtility.SaveFilePanel(
        //                        "Save animation curve as text",
        //                        "",
        //                        "untitled" + ".txt",
        //                        "txt");

        //    if (path.Length != 0)
        //    {
        //        FileStream fs = new FileStream(path, FileMode.Create);
        //        XmlSerializer xml = new XmlSerializer(typeof(AnimationCurve));
        //        xml.Serialize(fs, property.animationCurveValue);
        //        fs.Close();
        //    }
        //}


        if (GUI.Button(buttonRect, "Load"))
        {
            if (string.IsNullOrEmpty(m_defaultPath))
            {
                m_defaultPath = EditorPrefs.GetString("OpenFilePath");
                if (string.IsNullOrEmpty(m_defaultPath))
                    m_defaultPath = Application.persistentDataPath;
            }
            var path = EditorUtility.OpenFilePanel(
                                "load animation curve from json", m_defaultPath, "txt");

            if (path.Length != 0)
            {
                //Save Directory
                var pathDir = Path.GetDirectoryName(path);
                EditorPrefs.SetString("OpenFilePath", pathDir);
                m_defaultPath = pathDir;


                var json = File.ReadAllText(path);
                JsonReader jsonReader = JsonConverter.Reader(json);
                jsonReader.Read();
                JsonList jsonList = new JsonList();
                if (!jsonList.Read(jsonReader))
                {
                    return;
                }
                var curve = property.animationCurveValue;
                curve.keys = new Keyframe[0];
                for (int i = 0; i < jsonList.Count; i++)
                {
                    JsonDict jsonDict = jsonList.GetJsonDict(i);
                    var value = jsonDict.GetFloat("value");
                    var time = jsonDict.GetFloat("time");
                    var inTangent = jsonDict.GetFloat("inTangent");
                    var outTangent = jsonDict.GetFloat("outTangent");
                    int num = jsonDict.GetInt("tangentMode");
                    curve.AddKey(new Keyframe(time, value, inTangent, outTangent));
                    curve.keys[i].tangentMode = num;
                }
                property.animationCurveValue = curve;
                property.serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
