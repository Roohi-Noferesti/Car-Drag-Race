using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TesJsonSave : MonoBehaviour
{

    void Start()
    {
        List<JsonClass> list = new List<JsonClass>();
        list.Add(new JsonClass()
        {
            ID = 120,
            Name = "Ali"
        });
        list.Add(new JsonClass()
        {
            ID = 218,
            Name = "Mojtaba"
        });

        var json = JsonConverter.SerializeObject(list);

        var newList = JsonConverter.DeserializeObject<List<JsonClass>>(json);
    }
}


public class JsonClass
{
    public int ID;
    public string Name;
}