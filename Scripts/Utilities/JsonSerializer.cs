using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;

public static class JsonSerializer
{
    private const int MAX_DEPTH = 12;
    public static void Serialize(Transform transform, string filename)
    {
        JsonDict jsonDict = new JsonDict();
        SerializeComponent(ref jsonDict, transform);

        if(Application.isPlaying)
            FileUtils.WriteLocalStorage(filename, jsonDict.ToString(), false, true);
        else
        {
            File.WriteAllText(filename, jsonDict.ToString());
        }
    }

    private static void SerializeComponent(ref JsonDict jsonDict, Transform trans)
    {
        if (trans == null)
            return;
        jsonDict.Set("name", trans.name);
        var comps = trans.GetComponents<MonoBehaviour>();
        JsonList jsonList = new JsonList();
        for (int i = 0; i < comps.Length; i++)
        {
            var theObject = comps[i];
            JsonDict value2 = new JsonDict();
            SerializeObject(theObject, ref value2,0);
            jsonList.Add(value2);
        }
        jsonDict.Set("components", jsonList);

        var childs = new Transform[0];
        foreach (Transform t in trans)
        {
            Array.Resize(ref childs, childs.Length+1);
            childs[childs.Length - 1] = t;
        }
        var childsCopy = new Transform[childs.Length];
        Array.Copy(childs, childsCopy, childs.Length);
        JsonList jsonListChild = new JsonList();
        for (int i = 0; i < childsCopy.Length; i++)
        {
            var theObject = childsCopy[i];
            JsonDict value2 = new JsonDict();
            value2.Set("name", theObject.name);
            SerializeComponent(ref value2, theObject);
            jsonListChild.Add(value2);
        }
        jsonDict.Set("childs", jsonListChild);
    }

    private static void SerializeObject(object theObject, ref JsonDict jsondict,int depth)
    {
        if (theObject == null)
            return;
        jsondict.Set("type", theObject.GetType().Name);
        var fields = theObject.GetType().GetFields();//BindingFlags.NonPublic | BindingFlags.Public);
        var fields2 = new List<FieldInfo>();
        foreach (var fieldInfo in fields)
        {
            if (Attribute.IsDefined(fieldInfo, typeof(SerializeField))
                 || ( fieldInfo.IsPublic && !Attribute.IsDefined(fieldInfo, typeof(NonSerializedAttribute))))
            {
                fields2.Add(fieldInfo);
            }
        }
        foreach (var fieldInfo in fields2)
        {
            if (fieldInfo.GetValue(theObject) == null)
                continue;
            if (fieldInfo.FieldType == typeof(int))
            {
                jsondict.Set(fieldInfo.Name, (int)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType.IsEnum)
            {
                jsondict.Set(fieldInfo.Name, fieldInfo.GetValue(theObject).ToString());
            }
            else if (fieldInfo.FieldType == typeof(uint))
            {
                jsondict.Set(fieldInfo.Name, (uint)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(long))
            {
                jsondict.Set(fieldInfo.Name, (long)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(ulong))
            {
                jsondict.Set(fieldInfo.Name, (ulong)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                jsondict.Set(fieldInfo.Name, (float)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(double))
            {
                var floatValue = Convert.ToSingle(fieldInfo.GetValue(theObject));
                jsondict.Set(fieldInfo.Name, floatValue);
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                jsondict.Set(fieldInfo.Name, fieldInfo.GetValue(theObject).ToString());
            }
            else if (fieldInfo.FieldType == typeof(bool))
            {
                jsondict.Set(fieldInfo.Name, (bool)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(List<int>))
            {
                jsondict.Set(fieldInfo.Name, (List<int>)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(int[]))
            {
                jsondict.Set(fieldInfo.Name, (int[])fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(List<float>))
            {
                jsondict.Set(fieldInfo.Name, (List<float>)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(float[]))
            {
                jsondict.Set(fieldInfo.Name, (float[])fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(List<string>))
            {
                jsondict.Set(fieldInfo.Name, (List<string>)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(string[]))
            {
                jsondict.Set(fieldInfo.Name, (string[])fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(List<bool>))
            {
                jsondict.Set(fieldInfo.Name, (List<bool>)fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(bool[]))
            {
                jsondict.Set(fieldInfo.Name, (bool[])fieldInfo.GetValue(theObject));
            }
            else if (fieldInfo.FieldType == typeof(AnimationCurve))
            {
                var value = (AnimationCurve)fieldInfo.GetValue(theObject);
                if (value != null)
                {
                    SerializeAnimationCurve(fieldInfo.Name,value, ref jsondict);
                }
            }
            else if (fieldInfo.FieldType == typeof(AnimationCurve[]))
            {
                var value = (AnimationCurve[])fieldInfo.GetValue(theObject);
                if (value != null)
                {
                    jsondict.SetObjectArray(fieldInfo.Name, value, SerializeAnimationCurve);
                }
            }
            else if (fieldInfo.FieldType.IsArray )//&& depth < MAX_DEPTH)
            {
                var value = fieldInfo.GetValue(theObject);
                if (value != null)
                {
                    var obj = (object[])value;
                    JsonList jsonList = new JsonList();
                    for (int i = 0; i < obj.Length; i++)
                    {
                        var objAtArray = obj[i];
                        JsonDict value2 = new JsonDict();
                        SerializeObject(objAtArray, ref value2,depth);
                        jsonList.Add(value2);
                    }
                    jsondict.Set(fieldInfo.Name, jsonList);
                }
            }
            else if (fieldInfo.GetValue(theObject) is IList && fieldInfo.FieldType.IsGenericType)//&& depth < MAX_DEPTH)
            {
                var value = fieldInfo.GetValue(theObject);
                if (value != null)
                {
                    var values = value as IList;
                    List<object> objs = new List<object>();
                    foreach (var value1 in values)
                    {
                        objs.Add(value1);
                    }
                    JsonList jsonList = new JsonList();
                    foreach (var current in objs)
                    {
                        JsonDict value2 = new JsonDict();
                        SerializeObject(current, ref value2,depth);
                        jsonList.Add(value2);
                    }
                    jsondict.Set(fieldInfo.Name, jsonList);
                }
            }
            else if (fieldInfo.FieldType == typeof(Object))
            {
                var obj = (Object)fieldInfo.GetValue(theObject);
                if (obj != null)
                    jsondict.Set(fieldInfo.Name, obj.name);
            }
            else if (Attribute.IsDefined(fieldInfo, typeof(SerializeField))
                && depth < MAX_DEPTH)
            {

                var value = fieldInfo.GetValue(theObject);
                if (value != null)
                {
                    depth++;
                    JsonDict value2 = new JsonDict();
                    SerializeObject(value, ref value2, depth);
                    jsondict.Set(fieldInfo.Name , value2);
                }
            }
            else
            {
                var value = fieldInfo.GetValue(theObject);
                if (value != null)
                    jsondict.Set(fieldInfo.Name, value.ToString());
            }
        }
    }

    private static void SerializeAnimationCurve(AnimationCurve theobject, ref JsonDict jsondict)
    {
        SerializeAnimationCurve("curves", theobject, ref jsondict);
    }

    private static void SerializeAnimationCurve(string key ,AnimationCurve theobject, ref JsonDict jsondict)
    {
        if (theobject != null)
        {
            jsondict.SetObjectArray(key, theobject.keys, SerializeAnimationKeys);
        }
    }

    private static void SerializeAnimationKeys(Keyframe key, ref JsonDict jsonDict)
    {
        jsonDict.Set("time", key.time);
        jsonDict.Set("value", key.value);
        jsonDict.Set("inTangent", key.inTangent);
        jsonDict.Set("outTangent", key.outTangent);
        jsonDict.Set("tangentMode", key.tangentMode);
    }





    #region Deserialization

    public static void DeserializeComponent(JsonDict jsonDict, Transform trans)
    {
        List<int> componentVisited = new List<int>();
        jsonDict.GetString("name");
        var comps = trans.GetComponents<MonoBehaviour>();

        JsonList jsonList;
        if (jsonDict.TryGetValue("components", out jsonList))
        {
            for (int i = 0; i < jsonList.Count; i++)
            {
                JsonDict tempJsonDict;
                jsonList.TryGetValue(i, out tempJsonDict);
                for (int j = 0; j < comps.Length; j++)
                {
                    if (comps[j].GetType().Name == tempJsonDict.GetString("type")
                        && !componentVisited.Contains(j))
                    {
                        var obj = (object)comps[j];
                        DeserializeObject(tempJsonDict, ref obj, 0);
                        componentVisited.Add(j);
                        break;
                    }
                }
            }
        }

        var childs = new Transform[0];
        foreach (Transform t in trans)
        {
            Array.Resize(ref childs, childs.Length + 1);
            childs[childs.Length - 1] = t;
        }
        JsonList jsonListChilds;
        if (jsonDict.TryGetValue("childs", out jsonListChilds))
        {
            for (int i = 0; i < jsonListChilds.Count; i++)
            {
                JsonDict tempJsonDict;
                jsonListChilds.TryGetValue(i, out tempJsonDict);
                foreach (var child in childs)
                {
                    if (child.name == tempJsonDict.GetString("name"))
                    {
                        DeserializeComponent(tempJsonDict, child);
                        break;
                    }
                }
            }
        }
    }

    private static void DeserializeObject(JsonDict jsondict, ref object theObject,int depth)
    {
        if (theObject == null)
        {
            return;
        }
        var fields = theObject.GetType().GetFields();
        var fields2 = new List<FieldInfo>();
        foreach (var fieldInfo in fields)
        {
            if ((Attribute.IsDefined(fieldInfo, typeof(SerializeField))
                 || (fieldInfo.IsPublic && !Attribute.IsDefined(fieldInfo, typeof(NonSerializedAttribute))))
                && !fieldInfo.IsStatic)
            {
                fields2.Add(fieldInfo);
            }
        }
        foreach (var fieldInfo in fields2)
        {
            if (!jsondict.ContainsKey(fieldInfo.Name))
                continue;
            if (fieldInfo.FieldType == typeof(int))
            {
                var value = jsondict.GetInt(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType.IsEnum)
            {
                var stringValue = jsondict.GetString(fieldInfo.Name);
                var value = Enum.Parse(fieldInfo.FieldType, stringValue);
                //Debug.Log(theObject + "." + fieldInfo.Name + ":" + stringValue + " --> " + value);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(uint))
            {
                var value = jsondict.GetInt(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(long))
            {
                var value = jsondict.GetLong(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(ulong))
            {
                var value = jsondict.GetUlong(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(float))
            {
                var value = jsondict.GetFloat(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(double))
            {
                var value = jsondict.GetFloat(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(string))
            {
                var value = jsondict.GetString(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(bool))
            {
                var value = jsondict.GetBool(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(List<int>))
            {
                var value = jsondict.GetIntList(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(int[]))
            {
                var value = jsondict.GetIntArray(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(List<float>))
            {
                var value = jsondict.GetFloatList(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(float[]))
            {
                var value = jsondict.GetFloatArray(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(List<string>))
            {
                var value = jsondict.GetStringList(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(string[]))
            {
                var value = jsondict.GetStringArray(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(List<bool>))
            {
                var value = jsondict.GetBoolList(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(bool[]))
            {
                var value = jsondict.GetBoolArray(fieldInfo.Name);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(AnimationCurve))
            {
                var value = DeserializeAnimationCurve(fieldInfo.Name, ref jsondict);
                fieldInfo.SetValue(theObject, value);
            }
            else if (fieldInfo.FieldType == typeof(AnimationCurve[]))
            {
                JsonList jsonList;
                if (jsondict.TryGetValue(fieldInfo.Name, out jsonList))
                {
                    var values = new AnimationCurve[jsonList.Count];
                    for (int i = 0; i < jsonList.Count; i++)
                    {
                        var t = Activator.CreateInstance<AnimationCurve>();
                        JsonDict jsonDict;
                        jsonList.TryGetValue(i, out jsonDict);
                        DeserializeAnimationCurve(jsonDict, ref t);
                        values[i] = t;
                    }
                    fieldInfo.SetValue(theObject, values);
                }
            }
            else if (fieldInfo.FieldType.IsArray
                     && depth < 4)
            {
                JsonList jsonList;


                if (jsondict.TryGetValue(fieldInfo.Name, out jsonList))
                {
                    if (fieldInfo.FieldType.GetElementType().IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        var values = (MonoBehaviour[])fieldInfo.GetValue(theObject);
                        for (int i = 0; i < jsonList.Count; i++)
                        {
                            JsonDict jsonDict;
                            jsonList.TryGetValue(i, out jsonDict);
                            var temp = (object)values[i];
                            DeserializeObject(jsonDict, ref temp,depth);
                            values.SetValue(temp, i);
                        }
                    }
                    else
                    {
                        var values = Array.CreateInstance(fieldInfo.FieldType.GetElementType(), jsonList.Count);
                        for (int i = 0; i < jsonList.Count; i++)
                        {
                            object t = Activator.CreateInstance(fieldInfo.FieldType.GetElementType());
                            JsonDict jsonDict;
                            jsonList.TryGetValue(i, out jsonDict);
                            DeserializeObject(jsonDict, ref t,depth);
                            values.SetValue(t, i);
                        }
                        fieldInfo.SetValue(theObject, values);
                    }
                }
            }
            else if (fieldInfo.GetValue(theObject) is IList && fieldInfo.FieldType.IsGenericType
                     && depth < 4)
            {
                var value = fieldInfo.GetValue(theObject);
                if (value != null)
                {
                    JsonList jsonList;
                    if (jsondict.TryGetValue(fieldInfo.Name, out jsonList))
                    {
                        var values = value as IList;
                        for (int i = 0; i < jsonList.Count; i++)
                        {
                            var obj = Activator.CreateInstance(fieldInfo.FieldType);
                            JsonDict jsonDict;
                            jsonList.TryGetValue(i, out jsonDict);
                            if (jsonDict != null)
                            {
                                DeserializeObject(jsonDict, ref obj,depth);
                            }
                        }
                        fieldInfo.SetValue(theObject, values);
                    }
                }
            }
            else if (fieldInfo.FieldType == typeof(Object))
            {
                //var obj = (Object) fieldInfo.GetValue(theObject);
                //if (obj != null)
                //    jsondict.Set(fieldInfo.Name, obj.name);
            }
            else if (Attribute.IsDefined(fieldInfo, typeof(SerializeField))
                     && depth < 4)
            {
                depth++;
                object obj = null;
                if (jsondict.TryGetValue(fieldInfo.Name, out obj) && obj != null)
                {
                    object value;
                    if (fieldInfo.FieldType.IsSubclassOf(typeof(MonoBehaviour)))
                    {
                        value = fieldInfo.GetValue(theObject);
                    }
                    else
                    {
                        value = Activator.CreateInstance(fieldInfo.FieldType);
                    }
                    //Debug.Log(theObject + "_" + fieldInfo.Name);
                    if (obj is JsonDict)
                        DeserializeObject((JsonDict)obj, ref value,depth);
                    fieldInfo.SetValue(theObject, value);
                }
            }
        }
    }

    private static void DeserializeAnimationCurve(JsonDict jsondict, ref AnimationCurve theobject)
    {
        var animationCurve = DeserializeAnimationCurve("curves", ref jsondict);
        theobject.keys = animationCurve.keys;
    }

    private static AnimationCurve DeserializeAnimationCurve(string key, ref JsonDict jsondict)
    {
        var keys = jsondict.GetObjectArray<Keyframe>(key, DeserializeAnimationKeys);
        return new AnimationCurve(keys);
    }

    private static void DeserializeAnimationKeys(JsonDict jsonDict, ref Keyframe key)
    {
        key.time = jsonDict.GetFloat("time");
        key.value = jsonDict.GetFloat("value");
        key.inTangent = jsonDict.GetFloat("inTangent");
        key.outTangent = jsonDict.GetFloat("outTangent");
        key.tangentMode = jsonDict.GetInt("tangentMode");
    }
    #endregion
}
