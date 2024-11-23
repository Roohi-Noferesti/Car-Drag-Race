using UnityEngine;
using UnityEngine.UI;

public class TestToggle : MonoBehaviour 
{
    public void ToggleSelected()
    {
        Debug.Log("here");
    }

    public void ChangeToggle()
    {
        GetComponentInChildren<Toggle>().isOn = !GetComponentInChildren<Toggle>().isOn;
    }
}
