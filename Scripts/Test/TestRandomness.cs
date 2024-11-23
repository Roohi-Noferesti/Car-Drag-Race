using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRandomness : MonoBehaviour 
{
    public AnimationCurve ProbabilityCurve;

    public int Threshold;

    public int TestRepeat;

    void Awake()
    {
        List<int> values = new List<int>();
        for (int i = 0; i < TestRepeat; i++)
        {
            float randomValue = ProbabilityCurve.Evaluate(Random.value);
            values.Add(Mathf.RoundToInt(randomValue));
        }

        int count = 0;
        foreach (var value in values)
        {
            Debug.Log(value);
            if (Mathf.Abs(value - ProbabilityCurve.keys[0].value) <= Threshold || Mathf.Abs(value - ProbabilityCurve.keys[ProbabilityCurve.length-1].value) <= Threshold)
            {
                count++;
            }
        }

        Debug.Log(count + " number is near edges. Percentage is : " + (count*100 / values.Count)+" %");
    }
}
