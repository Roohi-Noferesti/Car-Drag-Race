using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestTwovalueSlider : MonoBehaviour
{
    public TwoValueSlider TwoValueSlider;

    public float Value2;

    void Update()
    {
        TwoValueSlider.value2 = Value2;
    }
}
