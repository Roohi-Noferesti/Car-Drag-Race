using UnityEngine;

public class TestComponent : MonoBehaviour 
{
    void Awake()
    {
        foreach (var VARIABLE in GetComponentsInChildren<RectTransform>(true))
        {
            Debug.Log(VARIABLE.name);
        }
        
    }
}
