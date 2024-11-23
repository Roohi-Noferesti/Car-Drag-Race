using System.Collections.Generic;
using UnityEngine;

public class RaceCarVisuals : MonoBehaviour
{
	private CarVisuals _carVisuals;

	//private RaceCar _raceCar;

	private bool _isOpponent;

	//private float _raceScrollTexOffset;

	//private float _raceScrollTexRotation;

	//private float _lastBlurAmount = -1f;

	//private List<Collider> _lightFlareMeshColliders;

    private float m_bodyLightCurrentValue;

    private int m_roadTriggerIndex;

    private float[] m_bodyLightTargetValues = {0, 1};

    RaceRoadTrigger currentRoadTrigger;

    public ParaboloidReflectionRenderer ReflectionRenderer;

	public void Setup(CarVisuals visuals, RaceCar car, bool isHuman)
	{
        this._carVisuals = visuals;
        //this._raceCar = car;
        this._isOpponent = !isHuman;
        if (this._carVisuals.GarageVisuals != null)
        {
            UnityEngine.Object.DestroyImmediate(this._carVisuals.GarageVisuals);
        }
        if (this._carVisuals.ShowroomVisuals != null)
        {
            UnityEngine.Object.DestroyImmediate(this._carVisuals.ShowroomVisuals);
        }
        this._carVisuals.RaceVisuals = this;
        if (isHuman)
        {
            this._carVisuals.AmbientLight = RaceEnvironmentSettings.Instance.CarAmbientLightHuman;
            this._carVisuals.DirectionalLight = RaceEnvironmentSettings.Instance.CarHumanLight;
            RaceCarLightingCrossFade humanLightingManager = RaceEnvironmentSettings.Instance.HumanLightingManager;
            if (humanLightingManager)
            {
                humanLightingManager.TargetCar = this._carVisuals;
            }
            if (visuals.CurrentlyAppliedNumberPlateTex != null)
            {
                this._carVisuals.ApplyNumberPlate(visuals.CurrentlyAppliedNumberPlateTex);
            }
            else
            {
                //this._carVisuals.ApplyNumberPlate(NumberPlateManager.Instance.PlayerNumPlateTexture);
            }
        }
        else
        {
            this._carVisuals.AmbientLight = RaceEnvironmentSettings.Instance.CarAmbientLightOpponent;
            this._carVisuals.DirectionalLight = RaceEnvironmentSettings.Instance.CarAiLight;
            RaceCarLightingCrossFade aiLightingManager = RaceEnvironmentSettings.Instance.AiLightingManager;
            if (aiLightingManager)
            {
                aiLightingManager.TargetCar = this._carVisuals;
            }
        }
        if (isHuman)
        {
            //this._lightFlareMeshColliders = new List<Collider>();
            if (this._carVisuals.BodyCollider != null)
            {
                this._carVisuals.BodyCollider.GetComponent<Collider>().enabled = true;
                //this._lightFlareMeshColliders.Add(this._carVisuals.BodyCollider);
            }
            foreach (CarLightFlare current in this._carVisuals.HeadlightFlares)
            {
                current.gameObject.SetActive(true);
                current.enabled = true;
                current.AddColliders(RaceEnvironmentSettings.Instance.EnvFlareColliders);
            }
        }
        else
        {
            CarVisuals carVisuals = CompetitorManager.Instance.LocalCompetitor.CarVisuals;
            foreach (CarLightFlare current2 in this._carVisuals.HeadlightFlares)
            {
                //current2.gameObject.SetActive(true);
                current2.enabled = true;
                //current2.AddColliders(carVisuals.RaceVisuals._lightFlareMeshColliders.ToArray());
                current2.AddColliders(carVisuals.BodyCollider);
                //current2.AddColliders(RaceEnvironmentSettings.Instance.EnvFlareColliders);
            }
        }
        CarShaderLod carShaderLod = CarQualitySettings.RaceLod;
        if (!RaceEnvironmentSettings.Instance.IsRealtimeReflectionAvailable() && carShaderLod == CarShaderLod.RaceHigh)
        {
            carShaderLod = CarShaderLod.RaceMed;
        }
        this._carVisuals.SetShaderLod(carShaderLod);
        this._carVisuals.SwichTo3DWheels();
        this._carVisuals.ShowDriver();
        this._carVisuals.BakeWheelBlurs();
  //      this._raceScrollTexOffset = 0f;
		//this._raceScrollTexRotation = 0f;
		this.ApplyReflectionSettings();
	}

	public void Reset()
	{
		if (!RaceStateEnter.switchToPreCountdownASAP)
		{
			this._carVisuals.SwichTo3DWheels();
		}
		//this._raceScrollTexOffset = 0f;
		//this._raceScrollTexRotation = 0f;
		this.ApplyReflectionSettings();
        this.ApplyRoadBodyLight(true);
		this.Update();
	}

	private void OnDestroy()
	{
        this._carVisuals.RaceVisuals = null;
        try
        {
            CarReflectionMapManager.ReflectionTexType reflectionTextureType = RaceEnvironmentSettings.Instance.ReflectionTextureType;
            CarReflectionMapManager.UnloadCubemap(reflectionTextureType);
            CarReflectionMapManager.UnloadScrollTex(reflectionTextureType);
        } catch {}
        this._carVisuals.ClearReflectionTextures();
        if (this._isOpponent)
        {
            //NumberPlateManager.Instance.DestroyOpponentNumberPlate();
        }
        this._carVisuals.MakeBakeDirty();
        foreach (CarLightFlare current in this._carVisuals.HeadlightFlares)
        {
            current.ClearColliders();
        }
        if (this._carVisuals.BodyCollider != null)
        {
            this._carVisuals.BodyCollider.GetComponent<Collider>().enabled = false;
        }
        if (this.ReflectionRenderer != null)
        {
            UnityEngine.Object.Destroy(this.ReflectionRenderer);
        }
	}

	private void Update()
	{
        //Debug.Log("Text");
	    //if (PauseGame.isGamePaused)
	    //{
	    //    return;
	    //}
	    //this._carVisuals.UpdateLighting();
	    //if (!RaceController.RaceIsRunning())
	    //{
	    //    return;
	    //}
	    //float speedMPH = this._raceCar.physics.SpeedMPH;
	    //float num = speedMPH * Time.timeScale;
	    //float num2 = this._raceCar.physics.SpeedMS * Time.timeScale;
	    //float num3 = Mathf.Clamp01(num / 30f);
	    //if (this._lastBlurAmount != num3)
	    //{
	    //    foreach (Material current in this._carVisuals.WheelMaterials)
	    //    {
	    //        current.SetFloat("_BlurAmount", num3);
	    //    }
	    //    this._lastBlurAmount = num3;
	    //}
	    //if (this._carVisuals.ReflectionTechnique != CarReflectionTechnique.Paraboloid)
	    //{
	    //    this._raceScrollTexOffset -= num2 * Time.deltaTime * 0.21f;
	    //    this._raceScrollTexRotation += num2 * Time.deltaTime * 4f;
	    //    Quaternion identity = Quaternion.identity;
	    //    identity.eulerAngles = new Vector3(this._raceScrollTexRotation, 0f, 0f);
	    //    Matrix4x4 matrix = Matrix4x4.TRS(Vector3.zero, identity, Vector3.one);
	    //    float value = Mathf.Clamp01(speedMPH / 30f);
	    //    foreach (Material current2 in this._carVisuals.ScrollTexMaterials)
	    //    {
	    //        current2.SetFloat("_ScrollTexOffset", this._raceScrollTexOffset);
	    //        current2.SetFloat("_ScrollTexInfluenceAmount", value);
	    //        current2.SetMatrix("_CubeRotationMatrix", matrix);
	    //    }
	    //}

        ApplyRoadBodyLight(false);

	}

    private void ApplyRoadBodyLight(bool forceInterior)
    {
        if (currentRoadTrigger!=null)
        {
            var targetValue = m_bodyLightTargetValues[(int)currentRoadTrigger.Type];
            m_bodyLightCurrentValue = forceInterior
                ? 0
                : Mathf.Lerp(m_bodyLightCurrentValue, targetValue, (Time.time%1)*.7F);
            var bodyMaterial = _carVisuals.BodyMaterial;
            bodyMaterial.SetFloat("_Inside_OutSide", m_bodyLightCurrentValue);
        }

        if (RaceEnvironmentSettings.Instance!=null && RaceEnvironmentSettings.Instance.RoadTriggers.Length > m_roadTriggerIndex)
        {
            currentRoadTrigger = RaceEnvironmentSettings.Instance.RoadTriggers[m_roadTriggerIndex];
            if (RaceEnvironmentSettings.Instance.RoadTriggers.Length > m_roadTriggerIndex + 1)
            {
                var nextRoadTrigger = RaceEnvironmentSettings.Instance.RoadTriggers[m_roadTriggerIndex + 1];
                if (transform.position.z > nextRoadTrigger.Position)
                {
                    m_roadTriggerIndex++;
                }
            }
        }
    }

	public void ApplyReflectionSettings()
	{
        switch (this._carVisuals.ReflectionTechnique)
        {
            case CarReflectionTechnique.CubeMap:
                this.ApplyReflectionCubeTex();
                break;
            case CarReflectionTechnique.CubeMapPlusScroll:
                this.ApplyReflectionScrollTex();
                this.ApplyReflectionCubeTex();
                break;
            case CarReflectionTechnique.Paraboloid:
                this.ApplyReflectionParaboloidTex();
                break;
        }

	    var type = RaceEnvironmentSettings.Instance.ReflectionTextureType;
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
            current.material.SetInt("_Day", RaceEnvironmentSettings.Instance.TextureOnlyMatDay?1:0);
        }

        foreach (Renderer current in this._carVisuals.DiscRenderers)
        {
            current.material.SetTexture("_node_929", loadedTextureOnlyCubemap);
            current.material.SetInt("_Day", RaceEnvironmentSettings.Instance.TextureOnlyMatDay?1:0);
        }

        Texture2D loadedGlowHeadlightTexture = CarReflectionMapManager.GetLoadedGlowHeadlightTexture(type);
        foreach (Renderer current in this._carVisuals.GlowHeadLightRenderer)
        {
            current.material.SetTexture("_node_7386", loadedGlowHeadlightTexture);
            current.gameObject.SetActive(RaceEnvironmentSettings.Instance.EnableGlowHeadlight);
        }

        Texture2D loadedLightProjectionTexture = CarReflectionMapManager.GetLoadedLightProjectionTexture(type);
        foreach (Projector current in this._carVisuals.LightProjector)
        {
            current.material.SetTexture("_ShadowTex", loadedLightProjectionTexture);
            current.gameObject.SetActive(RaceEnvironmentSettings.Instance.EnableGlowHeadlight);
        }

        foreach (Renderer current in this._carVisuals.HeadLightChromeRenderers)
        {
            current.material.SetInt("_Day", RaceEnvironmentSettings.Instance.EnableChrome ? 1 : 0);
        }

        foreach (Projector current in this._carVisuals.ShadowProjectionRenderers)
        {
            if (current.material == null)
                current.material = Resources.Load<Material>("shared_assets/car_shared_shader/Shadow_Projector_Shared");
        }
	}

	private void ApplyReflectionScrollTex()
	{
        CarReflectionMapManager.ReflectionTexType reflectionTextureType = RaceEnvironmentSettings.Instance.ReflectionTextureType;
        Texture2D loadedScrollTex = CarReflectionMapManager.GetLoadedScrollTex(reflectionTextureType);
        foreach (Material current in this._carVisuals.ScrollTexMaterials)
        {
            current.SetTexture("_ScrollTex", loadedScrollTex);
        }
	}

	private void ApplyReflectionCubeTex()
	{
        if (this.ReflectionRenderer != null)
        {
            this.ReflectionRenderer.enabled = false;
        }
        CarReflectionMapManager.ReflectionTexType reflectionTextureType = RaceEnvironmentSettings.Instance.ReflectionTextureType;
        Cubemap loadedCubemap = CarReflectionMapManager.GetLoadedCubemap(reflectionTextureType);
        foreach (Material current in this._carVisuals.ReflectiveMaterials)
        {
            current.SetTexture("_Cube", loadedCubemap);
        }
	}

	private void ApplyReflectionParaboloidTex()
	{
        if (this._isOpponent)
        {
            RaceCarVisuals raceVisuals = CompetitorManager.Instance.LocalCompetitor.CarVisuals.RaceVisuals;
            this.ReflectionRenderer = raceVisuals.ReflectionRenderer;
        }
        if (this.ReflectionRenderer == null)
        {
            Camera main = Camera.main;
            this.ReflectionRenderer = main.gameObject.AddComponent<ParaboloidReflectionRenderer>();
            this.ReflectionRenderer.FrontVector = ParaboloidReflectionRenderer.ParaboloidFrontVector.X;
            this.ReflectionRenderer.Setup((!BaseDevice.ActiveDevice.IsLowMemoryDevice()) ? 512 : 256, RaceEnvironmentSettings.Instance.ParaboloidReflectionMeshes, this._carVisuals.BodyNode);
            this.ReflectionRenderer.RenderOrigin.transform.Translate(0f, 1f, 0f, this._carVisuals.gameObject.transform);
            this.ReflectionRenderer.RenderOrigin.AddComponent<CarParaboloidEyePositioner>();
        }
        else
        {
            this.ReflectionRenderer.enabled = true;
        }
        foreach (Material current in this._carVisuals.ReflectiveMaterials)
        {
            current.SetTexture("_FrontParaboloid", this.ReflectionRenderer.FrontParaboloid);
            current.SetTexture("_RearParaboloid", this.ReflectionRenderer.RearParaboloid);
        }
	}

    public void EnableVisualAnimation(bool value)
    {
        _carVisuals.enabled = value;
    }
}
