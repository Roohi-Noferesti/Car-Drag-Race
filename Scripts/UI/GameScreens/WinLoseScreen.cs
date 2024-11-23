using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class WinLoseScreen:MonoBehaviour
{
    public static WinLoseScreen Instance;
    
    [SerializeField] private UnityEvent m_onLose;
    [SerializeField] private UnityEvent m_onWine;

    
    
    public void Awake()
    {
        Instance = this;
        var mainCamera = Camera.main;
        var canvas = GetComponent<Canvas>();
        canvas.worldCamera = mainCamera;
        canvas.planeDistance = mainCamera.nearClipPlane + .01f;
        // Color color = this.FlagRenderer.get_material().GetColor("_Tint");
        // this._startColor = color;
        // this._endColor = color;
        // this.useBigFlag = false;
        gameObject.SetActive(false);
    }

    
    public void Show()
    {
        gameObject.SetActive(true);
        if (RaceResultsTracker.You != null)
        {
            if (RaceResultsTracker.You.IsWinner)
            {
                m_onWine.Invoke();
            }
            else
            {
                m_onLose.Invoke();
            }
        }
    }

    // public override void OnActivate(bool zAlreadyOnStack)
    // {
    //     base.OnActivate(zAlreadyOnStack);
    //
    //     Camera.main.backgroundColor = Color.black;
    //     Camera.main.clearFlags = CameraClearFlags.SolidColor;
    // }
}
