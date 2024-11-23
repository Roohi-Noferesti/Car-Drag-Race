using DataSerialization;
using UnityEngine;
using UnityEngine.UI;
 
using UnityEditor;
using Vector3 = UnityEngine.Vector3;

// ---------------
//  String => Int
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringIntDictionary))]
public class StringIntDictionaryDrawer : SerializableDictionaryDrawer<string, int> {
    protected override SerializableKeyValueTemplate<string, int> GetTemplate() {
        return GetGenericTemplate<SerializableStringIntTemplate>();
    }
}
internal class SerializableStringIntTemplate : SerializableKeyValueTemplate<string, int> {}
 
// ---------------
//  GameObject => Float
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(GameObjectFloatDictionary))]
public class GameObjectFloatDictionaryDrawer : SerializableDictionaryDrawer<GameObject, float> {
    protected override SerializableKeyValueTemplate<GameObject, float> GetTemplate() {
        return GetGenericTemplate<SerializableGameObjectFloatTemplate>();
    }
}
internal class SerializableGameObjectFloatTemplate : SerializableKeyValueTemplate<GameObject, float> {}


// ---------------
//  String => Int
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringTextureDetailsDictionary))]
public class StringTextureDetailsDictionaryDrawer : SerializableDictionaryDrawer<string, TextureDetail>
{
    protected override SerializableKeyValueTemplate<string, TextureDetail> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringTextureDetailsTemplate>();
    }
}
internal class SerializableStringTextureDetailsTemplate : SerializableKeyValueTemplate<string, TextureDetail> { }


// ---------------
//  String => Vector3
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringVectorDictionary))]
public class StringVector3DictionaryDrawer : SerializableDictionaryDrawer<string, Vector3>
{
    protected override SerializableKeyValueTemplate<string, Vector3> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringVector3Template>();
    }
}
internal class SerializableStringVector3Template : SerializableKeyValueTemplate<string, Vector3> { }



// ---------------
//  String => MyClass
// ---------------
[UnityEditor.CustomPropertyDrawer(typeof(StringMyClassDictionary))]
public class StringMyClassDictionaryDrawer : SerializableDictionaryDrawer<string, MyClass>
{
    protected override SerializableKeyValueTemplate<string, MyClass> GetTemplate()
    {
        return GetGenericTemplate<SerializableStringMyClassTemplate>();
    }
}
internal class SerializableStringMyClassTemplate : SerializableKeyValueTemplate<string, MyClass> { }
