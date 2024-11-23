using System;
using DataSerialization;
using UnityEngine;
 
// ---------------
//  String => Int
// ---------------
[Serializable]
public class StringIntDictionary : SerializableDictionary<string, int> {}
 
// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class GameObjectFloatDictionary : SerializableDictionary<GameObject, float> {}


// ---------------
//  GameObject => Float
// ---------------
[Serializable]
public class StringTextureDetailsDictionary : SerializableDictionary<string, TextureDetail> { }


[Serializable]
public class StringVectorDictionary : SerializableDictionary<string, UnityEngine.Vector3> { }

[Serializable]
public class StringMyClassDictionary : SerializableDictionary<string, MyClass> { }


[Serializable]
public class MyClass
{
    public int Number;
}




