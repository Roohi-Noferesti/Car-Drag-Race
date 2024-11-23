using System;
using UnityEngine;

public class ShowroomCarVisuals : MonoBehaviour
{
	private CarVisuals _carVisuals;

	private Quaternion[] _wheelOriginalRotations;

	public void Setup(CarVisuals zCarVisuals, CarInfo zCarInfo)
	{
		this._carVisuals = zCarVisuals;
		if (this._carVisuals.GarageVisuals != null)
		{
			DestroyImmediate(this._carVisuals.GarageVisuals);
		}
		if (this._carVisuals.RaceVisuals != null)
		{
			DestroyImmediate(this._carVisuals.RaceVisuals);
		}
		this._carVisuals.ShowroomVisuals = this;
		this._carVisuals.AmbientLight = ShowroomCarVisualsSettings.Instance.ambientLight;
		this._carVisuals.DirectionalLight = ShowroomCarVisualsSettings.Instance.directionalLight;
        //this._carVisuals.SetShaderLod(CarQualitySettings.ShowroomLod);

	    var type = CarReflectionMapManager.ReflectionTexType.Showroom;
        //Cubemap loadedCubemap = CarReflectionMapManager.GetLoadedCubemap(type);
        //foreach (Material current in this._carVisuals.ReflectiveMaterials)
        //{
        //    current.SetTexture("_Cube", loadedCubemap);
        //}

        Material loadedBlackShadeMaterial = CarReflectionMapManager.GetLoadedBlackShadeMaterial(type);
        foreach (Renderer current in this._carVisuals.BlackShadeRenderers)
        {
            current.material = loadedBlackShadeMaterial;
        }

        Material loadedGlassMaterial = CarReflectionMapManager.GetLoadedGlassMaterial(type);
        foreach (Renderer current in this._carVisuals.GlassRenderers)
        {
            current.material = loadedGlassMaterial;
        }

        Material loadedHeadlightGlassMaterial = CarReflectionMapManager.GetLoadedHeadlightGlassMaterial(type);
        foreach (Renderer current in this._carVisuals.HeadlightGlassRenderers)
        {
            current.material = loadedHeadlightGlassMaterial;
        }

        Cubemap loadedTextureOnlyCubemap = CarReflectionMapManager.GetLoadedTextureOnlyCubemaps(type);
        foreach (Renderer current in this._carVisuals.TextureOnlyRenderers)
        {
            current.material.SetTexture("_node_929", loadedTextureOnlyCubemap);
            current.material.SetInt("_Day",0);
        }

        foreach (Renderer current in this._carVisuals.DiscRenderers)
        {
            current.material.SetTexture("_node_929", loadedTextureOnlyCubemap);
            current.material.SetInt("_Day", 0);
        }

        Texture2D loadedGlowHeadlightTexture = CarReflectionMapManager.GetLoadedGlowHeadlightTexture(type);
        foreach (Renderer current in this._carVisuals.GlowHeadLightRenderer)
        {
            current.material.SetTexture("_node_7386", loadedGlowHeadlightTexture);
            current.gameObject.SetActive(true);
        }

        Texture2D loadedLightProjectionTexture = CarReflectionMapManager.GetLoadedLightProjectionTexture(type);
        foreach (Projector current in this._carVisuals.LightProjector)
        {
            current.material.SetTexture("_ShadowTex", loadedLightProjectionTexture);
            current.gameObject.SetActive(true);
        }

        foreach (Renderer current in this._carVisuals.HeadLightChromeRenderers)
        {
            current.material.SetInt("_Day", 0);
        }

        foreach (Projector current in this._carVisuals.ShadowProjectionRenderers)
        {
            if (current.material == null)
                current.material = Resources.Load<Material>("shared_assets/car_shared_shader/Shadow_Projector_Shared");
        }



		base.transform.Rotate(Vector3.up, 30f);
		this._wheelOriginalRotations = new Quaternion[2];
        //for (int i = 0; i < this._carVisuals.WheelsFront3D.Count; i++)
        //{
        //    this._wheelOriginalRotations[i] = this._carVisuals.WheelsFront3D[i].transform.localRotation;
        //    Vector3 localEulerAngles = this._carVisuals.WheelsFront3D[i].transform.localEulerAngles;
        //    localEulerAngles.y = (float)zCarInfo.GarageWheelRotation;
        //    this._carVisuals.WheelsFront3D[i].transform.localEulerAngles = localEulerAngles;
        //}
		this._carVisuals.SwichTo3DWheels();
		this._carVisuals.SetGroundFlareState(false);
        if (ShowroomCarVisualsSettings.Instance.flareColliders.Count > 0)
        {
            foreach (CarLightFlare current2 in this._carVisuals.HeadlightFlares)
            {
                current2.AddColliders(ShowroomCarVisualsSettings.Instance.flareColliders.ToArray());
            }
        }
        this._carVisuals.SetLayer("Car");
        this._carVisuals.SetGroundFlareLayer("Default");
		this._carVisuals.HideDriver();
        //this._carVisuals.ApplyNumberPlate(NumberPlateManager.Instance.DefaultNumberPlateTexture);
		this._carVisuals.BakeCarShadows();
	}

	private void OnDestroy()
	{
		this._carVisuals.ShowroomVisuals = null;
		this._carVisuals.ClearReflectionTextures();
		base.transform.localRotation = Quaternion.identity;
		for (int i = 0; i < this._carVisuals.WheelsFront3D.Count; i++)
		{
			if (this._carVisuals.WheelsFront3D[i] != null)
			{
				this._carVisuals.WheelsFront3D[i].transform.localRotation = this._wheelOriginalRotations[i];
			}
		}
		this._carVisuals.SetGroundFlareState(true);
		foreach (CarLightFlare current in this._carVisuals.HeadlightFlares)
		{
			current.ClearColliders();
		}
	}
}
