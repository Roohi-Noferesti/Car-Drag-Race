using System;
using UnityEngine;

public class TestCurve : MonoBehaviour
{
    [SerializeField] private AnimationCurve m_curve;
    [SerializeField] private float m_t;

    public float Evaluate(float time)
    {
        var keys = m_curve.keys;
        if (keys == null || keys.Length < 2)
            throw new Exception("No key available");

        int index = -1;
        for (int i = 0; i < keys.Length - 1; i++)
        {
            if (time >= keys[i].time && time <= keys[i + 1].time)
            {
                index = i;
                break;
            }
        }

        var keyFrame0 = keys[index];
        var keyFrame1 = keys[index + 1];

        time = Mathf.InverseLerp(keyFrame0.time, keyFrame1.time, time);

        float dt = (keyFrame1.time - keyFrame0.time);

        float m0 = keyFrame0.outTangent * dt;
        float m1 = keyFrame1.inTangent * dt;

        float t2 = time * time;
        float t3 = t2 * time;

        float a = 2 * t3 - 3 * t2 + 1;
        float b = t3 - 2 * t2 + time;
        float c = t3 - t2;
        float d = -2 * t3 + 3 * t2;

        return a * keyFrame0.value + b * m0 + c * m1 + d * keyFrame1.value;
    }

    void Awake()
    {
        Debug.Log(m_curve.Evaluate(m_t));
        Debug.Log(Evaluate(m_t));
    }

}
