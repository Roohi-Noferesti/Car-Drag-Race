using UnityEngine;

public class CarSnapshotVisuals : MonoBehaviour
{
	private CarVisuals carVisuals;

	private Quaternion[] wheelOriginalRotations;

	private int originalLayer;

	private Vector3 originalPosition;

	private Quaternion originalRotation;

	private Texture2D originalNumberPlate;

	private bool wasShowingRaceWheels;

    //private Cubemap originalCubemap;

    //private CarShaderLod originalLod;

	private bool wasShowingDriver;

	private bool groundFlarePreviousState;

	private Light originalDirLight;

	private Light originalAmbientLight;

	public void Setup(CarVisuals zCarVisuals, CarInfo zCarInfo)
	{
		this.carVisuals = zCarVisuals;
		this.originalPosition = zCarVisuals.transform.position;
		this.originalRotation = zCarVisuals.transform.rotation;
		zCarVisuals.transform.localPosition = CarSnapshotManager.Instance.PlacementNode.position;
		zCarVisuals.transform.localRotation = CarSnapshotManager.Instance.PlacementNode.rotation;
		this.originalAmbientLight = this.carVisuals.AmbientLight;
		this.originalDirLight = this.carVisuals.DirectionalLight;
		this.carVisuals.AmbientLight = CarSnapshotManager.Instance.AmbientLight;
		this.carVisuals.DirectionalLight = CarSnapshotManager.Instance.DirectionalLight;
        //this.originalLod = this.carVisuals.CurrentShaderLod;
        //this.carVisuals.SetShaderLod(CarQualitySettings.ShowroomLod);

        var type = CarReflectionMapManager.ReflectionTexType.Showroom;
        //Cubemap loadedCubemap = CarReflectionMapManager.GetLoadedCubemap(CarReflectionMapManager.ReflectionTexType.Showroom);
        //foreach (Material current in this.carVisuals.ReflectiveMaterials)
        //{
        //    if (this.originalCubemap == null)
        //    {
        //        this.originalCubemap = (current.GetTexture("_Cube") as Cubemap);
        //    }
        //    current.SetTexture("_Cube", loadedCubemap);
        //}


        Material loadedBlackShadeMaterial = CarReflectionMapManager.GetLoadedBlackShadeMaterial(type);
        foreach (Renderer current in this.carVisuals.BlackShadeRenderers)
        {
            current.material = loadedBlackShadeMaterial;
        }

        Material loadedGlassMaterial = CarReflectionMapManager.GetLoadedGlassMaterial(type);
        foreach (Renderer current in this.carVisuals.GlassRenderers)
        {
            current.material = loadedGlassMaterial;
        }

        Material loadedHeadlightGlassMaterial = CarReflectionMapManager.GetLoadedHeadlightGlassMaterial(type);
        foreach (Renderer current in this.carVisuals.HeadlightGlassRenderers)
        {
            current.material = loadedHeadlightGlassMaterial;
        }

        Cubemap loadedTextureOnlyCubemap = CarReflectionMapManager.GetLoadedTextureOnlyCubemaps(type);
        foreach (Renderer current in this.carVisuals.TextureOnlyRenderers)
        {
            current.material.SetTexture("_node_929", loadedTextureOnlyCubemap);
            current.material.SetInt("_Day", 0);
        }

        foreach (Renderer current in this.carVisuals.DiscRenderers)
        {
            current.material.SetTexture("_node_929", loadedTextureOnlyCubemap);
            current.material.SetInt("_Day", 0);
        }

        Texture2D loadedGlowHeadlightTexture = CarReflectionMapManager.GetLoadedGlowHeadlightTexture(type);
        foreach (Renderer current in this.carVisuals.GlowHeadLightRenderer)
        {
            current.material.SetTexture("_node_7386", loadedGlowHeadlightTexture);
            current.gameObject.SetActive(true);
        }

        Texture2D loadedLightProjectionTexture = CarReflectionMapManager.GetLoadedLightProjectionTexture(type);
        foreach (Projector current in this.carVisuals.LightProjector)
        {
            current.material.SetTexture("_ShadowTex", loadedLightProjectionTexture);
            current.gameObject.SetActive(true);
        }

        foreach (Renderer current in this.carVisuals.HeadLightChromeRenderers)
        {
            current.material.SetInt("_Day", 0);
        }

        foreach (Projector current in this.carVisuals.ShadowProjectionRenderers)
        {
            if (current.material == null)
                current.material = Resources.Load<Material>("shared_assets/car_shared_shader/Shadow_Projector_Shared");
        }


		this.wheelOriginalRotations = new Quaternion[this.carVisuals.WheelsFront3D.Count];
		if (this.carVisuals.WheelsFront3D.Count > 2)
		{
		}
		for (int i = 0; i < this.carVisuals.WheelsFront3D.Count; i++)
		{
			this.wheelOriginalRotations[i] = this.carVisuals.WheelsFront3D[i].transform.localRotation;
			Vector3 localEulerAngles = this.carVisuals.WheelsFront3D[i].transform.localEulerAngles;
			localEulerAngles.y = (float)zCarInfo.GarageWheelRotation;
			this.carVisuals.WheelsFront3D[i].transform.localEulerAngles = localEulerAngles;
		}
		this.wasShowingRaceWheels = this.carVisuals.Wheels[0].activeInHierarchy;
		this.carVisuals.SwichTo3DWheels();
		this.carVisuals.BakeCarShadows();
		this.groundFlarePreviousState = this.carVisuals.SetGroundFlareState(false);
		this.originalLayer = this.carVisuals.gameObject.layer;
		this.carVisuals.SetLayer("CarSnapshot");
		if (this.carVisuals.Driver != null)
		{
			this.wasShowingDriver = this.carVisuals.Driver.activeInHierarchy;
		}
		this.carVisuals.HideDriver();
		this.originalNumberPlate = this.carVisuals.CurrentlyAppliedNumberPlateTex;
	}

	private void OnDestroy()
	{
        //this.carVisuals.ShowroomVisuals = null;
		base.transform.localPosition = this.originalPosition;
		base.transform.localRotation = this.originalRotation;
		this.carVisuals.AmbientLight = this.originalAmbientLight;
		this.carVisuals.DirectionalLight = this.originalDirLight;
        //this.carVisuals.SetShaderLod(this.originalLod);
		for (int i = 0; i < this.carVisuals.WheelsFront3D.Count; i++)
		{
			if (this.carVisuals.WheelsFront3D[i] != null)
			{
				this.carVisuals.WheelsFront3D[i].transform.localRotation = this.wheelOriginalRotations[i];
			}
		}
		this.carVisuals.SetGroundFlareState(this.groundFlarePreviousState);
		this.carVisuals.SetLayer(this.originalLayer);
        //foreach (Material current in this.carVisuals.ReflectiveMaterials)
        //{
        //    current.SetTexture("_Cube", this.originalCubemap);
        //}
		if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend)
		{
            //CarReflectionMapManager.UnloadCubemap(CarReflectionMapManager.ReflectionTexType.Showroom);
		}
		if (this.wasShowingRaceWheels)
		{
			this.carVisuals.SwichToRaceWheels();
		}
		if (this.wasShowingDriver)
		{
			this.carVisuals.ShowDriver();
		}
		this.carVisuals.ApplyNumberPlate(this.originalNumberPlate);
	}
}
