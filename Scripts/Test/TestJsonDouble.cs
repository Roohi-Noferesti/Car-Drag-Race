using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TestJsonDouble : MonoBehaviour
{
    void Start()
    {
        Number number = new Number();
        number.MyNumber = 10;

        var json = JsonConverter.SerializeObject(number);
        File.WriteAllText(Application.persistentDataPath + "/json.txt", json);


        json = File.ReadAllText(Application.persistentDataPath + "/json.txt");
        var obj = JsonConverter.DeserializeObject<Number>(json);

    }
}


public class Number
{
    public double MyNumber;
}
