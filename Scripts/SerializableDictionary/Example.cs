using System.Collections.Generic;
 
using UnityEngine;
 
[CreateAssetMenu(menuName = "Example Asset")]
public class Example : ScriptableObject {

    [SerializeField]
    private StringMyClassDictionary stringMyClass = StringMyClassDictionary.New<StringMyClassDictionary>();

    //[SerializeField]
    //private StringIntDictionary stringIntegerStore = StringIntDictionary.New<StringIntDictionary>();

    //[SerializeField]
    //private StringVectorDictionary stringVectoregerStore = StringVectorDictionary.New<StringVectorDictionary>();
    //private Dictionary<string, int> stringIntegers
    //{
    //    get { return stringIntegerStore.dictionary; }
    //}

    //[SerializeField]
    //private GameObjectFloatDictionary gameObjectFloatStore = GameObjectFloatDictionary.New<GameObjectFloatDictionary>();
    //private Dictionary<GameObject, float> screenshots {
    //    get { return gameObjectFloatStore.dictionary; }
    //}
}
