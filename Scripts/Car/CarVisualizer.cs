using System.Linq;
using UnityEngine;

public class CarVisualizer : MonoBehaviour
{
    [SerializeField] private Renderer[] m_bodyRenderers;
    [SerializeField] private Renderer[] m_headLightRenderers;
    [SerializeField] private Renderer[] m_ringRenderers;
    [SerializeField] private Car_HeadLight[] m_headLights;
    [SerializeField] private CarSpoiler[] m_spoilers;

    public Material BodyMaterial
    {
        set
        {
            //var texture = Lightmap;
            SetMaterial(m_bodyRenderers, value);
            //Lightmap = texture;
        }
    }

    public Material HeadLightMaterial
    {
        set { SetMaterial(m_headLightRenderers, value); }
    }

    public Material RingMaterial
    {
        set { SetMaterial(m_ringRenderers, value); }
    }

    public Texture2D Sticker
    {
        set
        {
            foreach (var bodyRenderer in m_bodyRenderers)
            {
                bodyRenderer.material.SetTexture("_Sticker", value);
            }
        }
    }

    public void SetSpoiler(string spoilerID,GameObject spoiler)
    {
        var carSpoiler = m_spoilers.FirstOrDefault(s => s.SpoilerID == spoilerID);
        spoiler.transform.SetParent(transform);
        spoiler.transform.localPosition = carSpoiler.Position;
    }

    public Transform CameraHeadLight
    {
        set
        {
            if (m_headLights.Length == 0)
            {
                m_headLights = GetComponentsInChildren<Car_HeadLight>(true);
            }
            foreach (var headLight in m_headLights)
            {
                headLight.CameraTransform = value;
            }
        }
    }

    public Texture Lightmap
    {
        set
        {
            foreach (var bodyRenderer in m_bodyRenderers)
            {
                bodyRenderer.material.SetTexture("_LightMap_Garage", value);
            }
        }
        get { return (Texture2D)m_bodyRenderers[0].material.GetTexture("_LightMap_Garage"); }
    }

    private void SetMaterial(Renderer[] renderers, Material material)
    {
        foreach (var bodyRenderer in renderers)
        {
            bodyRenderer.material = material;
        }
    }
}
