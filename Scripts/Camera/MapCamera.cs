using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MapCamera : MonoSingleton<MapCamera>
{
    [SerializeField] private float m_speed;
    [SerializeField] private float m_lerpSpeed;
    private float m_xInput;
    private float m_yInput;
    private float m_zInput;
    private Vector3 m_targetPosition;

    [SerializeField] private CameraBound m_Max;
    [SerializeField] private CameraBound m_Min;
    [SerializeField] private Vector3 m_activePinCameraOffset;
    private bool m_interactable = true;

    public bool Interactable
    {
        get { return m_interactable; }
        set { m_interactable = value; }
    }


    protected override void Awake()
    {
        base.Awake();
        m_targetPosition = transform.localPosition;
    }

    void Update()
    {
        var screenResolution = (float)720/Screen.height;
        if (Interactable)
        {
            m_xInput = CrossPlatformInputManager.GetAxis("Mouse X");
            m_yInput = CrossPlatformInputManager.GetAxis("Mouse Y");
            m_zInput = CrossPlatformInputManager.GetAxis("Mouse Z");
        }
        //Debug.Log(m_xInput+"    "+ m_yInput+"    "+ m_zInput);
        m_targetPosition.x += -m_xInput * m_speed * screenResolution;
        m_targetPosition.z += -m_yInput * m_speed * screenResolution;
        m_targetPosition.y += -m_zInput * m_speed * screenResolution;

        var t = Mathf.InverseLerp(m_Min.Zoom, m_Max.Zoom, transform.localPosition.y);
        var left = Mathf.Lerp(m_Min.Left, m_Max.Left, t);
        var right = Mathf.Lerp(m_Min.Right, m_Max.Right, t);
        var bottom = Mathf.Lerp(m_Min.Bottom, m_Max.Bottom, t);
        var top = Mathf.Lerp(m_Min.Top, m_Max.Top, t);

        m_targetPosition.x = Mathf.Clamp(m_targetPosition.x, left, right);
        m_targetPosition.z = Mathf.Clamp(m_targetPosition.z, bottom, top);
        m_targetPosition.y = Mathf.Clamp(m_targetPosition.y, m_Min.Zoom, m_Max.Zoom);

        transform.localPosition = Vector3.Lerp(transform.localPosition, m_targetPosition, Time.deltaTime * m_lerpSpeed);
    }

    public static void MoveToPosition(Vector3 position,float delay = 0)
    {
        if (delay < 0.001)
        {
            var localPosition = Instance.transform.parent.InverseTransformPoint(position) +
                                Instance.m_activePinCameraOffset;
            Instance.m_targetPosition = localPosition;
            Instance.m_targetPosition.y = Instance.m_activePinCameraOffset.y ;
        }
        else
        {
            Instance.StartCoroutine(_delayedMovePosition(position,delay));
        }
    }


    public static void MoveToPositionImmediately(Vector3 position,bool worldSpace = false)
    {
        var localPosition = worldSpace?position:Instance.transform.parent.InverseTransformPoint(position) +
                            Instance.m_activePinCameraOffset;
        Instance.m_targetPosition = localPosition;
        Instance.m_targetPosition.y = Instance.m_activePinCameraOffset.y;
        Instance.transform.localPosition = Instance.m_targetPosition;
    }

    private static IEnumerator _delayedMovePosition(Vector3 position,float delay)
    {
        yield return new WaitForSeconds(delay);
        MoveToPosition(position);
    }

}

[Serializable]
public class CameraBound
{
    public float Left;
    public float Right;
    public float Bottom;
    public float Top;
    public float Zoom;
}
