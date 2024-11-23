using System;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineComponent : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}