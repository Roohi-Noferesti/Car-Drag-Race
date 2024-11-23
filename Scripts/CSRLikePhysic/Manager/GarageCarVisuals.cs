using System;
using UnityEngine;

public class GarageCarVisuals : MonoBehaviour
{
	private CarVisuals _carVisuals;

	private Quaternion[] _wheelOriginalRotations;

    //private CarInfo m_carInfo;
	public void Setup(CarVisuals visuals, CarInfo info)
	{
        //m_carInfo = info;
		this.RefreshVisuals(visuals);
		this.RotateWheels(visuals, info);
	}

	public void RefreshVisuals(CarVisuals visuals)
	{
		this._carVisuals = visuals;
		if (this._carVisuals.RaceVisuals != null)
		{
			DestroyImmediate(this._carVisuals.RaceVisuals);
		}
		if (this._carVisuals.ShowroomVisuals != null)
		{
			DestroyImmediate(this._carVisuals.ShowroomVisuals);
		}
		this._carVisuals.GarageVisuals = this;
		this._carVisuals.AmbientLight = GarageCarVisualsSettings.Instance.ambientLight;
		this._carVisuals.DirectionalLight = GarageCarVisualsSettings.Instance.directionalLight;
		this._carVisuals.SetShaderLod(CarQualitySettings.GarageLod);
	    CarReflectionMapManager.ReflectionTexType type = CarReflectionMapManager.ReflectionTexType.WorkshopNight;//!MultiplayerUtils.GarageInMultiplayerMode ? CarReflectionMapManager.ReflectionTexType.WorkshopNight : CarReflectionMapManager.ReflectionTexType.WorkshopDay;
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
            current.material.SetInt("_Day", 0);
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
            if (current.material==null)
                current.material = Resources.Load<Material>("shared_assets/car_shared_shader/Shadow_Projector_Shared");
        }



		base.transform.localPosition = GarageCarVisualsSettings.Instance.carPlacementNode.transform.position;
		base.transform.localRotation = GarageCarVisualsSettings.Instance.carPlacementNode.transform.rotation;
		this._carVisuals.SwichTo3DWheels();
		this._carVisuals.HideDriver();
        //this._carVisuals.ApplyNumberPlate(NumberPlateManager.Instance.PlayerNumPlateTexture);
		this._carVisuals.BakeCarShadows();
        //this._carVisuals.FixMissingReferences();
	}

	public void RotateWheels(CarVisuals visuals, CarInfo info)
	{
		this._wheelOriginalRotations = new Quaternion[2];
		for (int i = 0; i < this._carVisuals.WheelsFront3D.Count; i++)
		{
			this._wheelOriginalRotations[i] = this._carVisuals.WheelsFront3D[i].transform.localRotation;
			Vector3 localEulerAngles = this._carVisuals.WheelsFront3D[i].transform.localEulerAngles;
			localEulerAngles.y = (float)(-(float)info.GarageWheelRotation);
			this._carVisuals.WheelsFront3D[i].transform.localEulerAngles = localEulerAngles;
		}
	}

	private void OnEnable()
	{
		if (this._carVisuals != null)
		{
			this._carVisuals.SwichTo3DWheels();
			this._carVisuals.HideDriver();
		}
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		this._carVisuals.GarageVisuals = null;
		this._carVisuals.ClearReflectionTextures();
		base.transform.localRotation = Quaternion.identity;
		for (int i = 0; i < this._carVisuals.WheelsFront3D.Count; i++)
		{
			if (this._carVisuals.WheelsFront3D[i] != null)
			{
				this._carVisuals.WheelsFront3D[i].transform.localRotation = this._wheelOriginalRotations[i];
			}
		}
	}
}
