using Sirenix.OdinInspector;
using UnityEngine;

public class Cheats : MonoBehaviour
{
#if UNITY_EDITOR

    [Button]
    private void AddXpFake()
    {
        GameDatabase.Instance.XPEvents.AddPlayerXP(500);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.KeypadPlus))
            AddXpFake();
    }
#endif
}

