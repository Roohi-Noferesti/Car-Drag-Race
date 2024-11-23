using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RaceEnvironmentSettings : MonoBehaviour
{

	public Transform HumanStartPosition;

	public Transform AiStartPosition;

	public Light CarAmbientLightOpponent;

	public Light CarAmbientLightHuman;

	public Light CarAiLight;

	public Light CarHumanLight;

    public RaceCarLightingCrossFade HumanLightingManager;

    public RaceCarLightingCrossFade AiLightingManager;

	public GameObject Crowds;

    public CarReflectionMapManager.ReflectionTexType ReflectionTextureType;

	public bool ParaboloidReflectionsEnabled;

	public List<MeshFilter> ParaboloidReflectionMeshes;

	public BoxCollider[] EnvFlareColliders;

	public bool OverrideCarsAtRuntime;

	public string OverrideHumanCarWith;

	public string OverrideOpponentCarWith;

    public Vector3 FinalRoadPosition;

    public string SurfaceAudioEventName
    {
        get { return "Surfaces/Tarmac/SurfaceLoopTarmacDry"; }
    } 
    public bool TextureOnlyMatDay;
    public bool EnableGlowHeadlight;
    public bool EnableChrome;

    public RaceRoadTrigger[] RoadTriggers;

    public static RaceEnvironmentSettings Instance
	{
		get;
		private set;
	}

    //private RaceEnvironmentSettings()
    //{
    //    Instance = this;
    //    this.OverrideCarsAtRuntime = false;
    //}

    void Awake()
    {
        Instance = this;
        this.OverrideCarsAtRuntime = false;
    }

	private void OnDestroy()
	{
		Instance = null;
	}

	public bool IsRealtimeReflectionAvailable()
	{
		return this.ParaboloidReflectionMeshes != null && this.ParaboloidReflectionMeshes.Count > 0 && this.ParaboloidReflectionsEnabled;
	}
}