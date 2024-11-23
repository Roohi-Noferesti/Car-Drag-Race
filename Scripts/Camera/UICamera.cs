using UnityEngine;

public class UICamera : MonoSingleton<UICamera>
{
    private Camera m_camera;

    public Camera Camera
    {
        get
        {
            if (m_camera == null)
                m_camera = GetComponentInChildren<Camera>();
            return m_camera;
        }
    }

    protected override void Destroy()
    {
        Destroy(gameObject);
    }
}
