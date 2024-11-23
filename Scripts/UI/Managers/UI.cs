using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI : MonoBehaviour {

    private void Start()
    {
        if (UICamera.Instance != null)
        {
            GetComponentInChildren<Canvas>().worldCamera = UICamera.Instance.Camera;
        }
    }
}
