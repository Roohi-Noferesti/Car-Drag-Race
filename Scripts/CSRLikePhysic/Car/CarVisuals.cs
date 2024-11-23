using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

public class CarVisuals : MonoBehaviour
{
	private struct FlareCombinerInfo
	{
		public string searchName;

		public string combinedName;

		public FlareCombinerInfo(string inSearch, string inCombined)
		{
			this.searchName = inSearch;
			this.combinedName = inCombined;
		}
	}

	private struct GroundFlareSetupData
	{
		public Vector3 scale;

		public Vector3 rot;

		public Vector3 pos;

		public GroundFlareSetupData(Vector3 inS, Vector3 inR, Vector3 inP)
		{
			this.scale = inS;
			this.rot = inR;
			this.pos = inP;
		}
	}

	private const string GroundFlareName = "CarLightSetup";

	private const string GroundFlareGeneratedName = "CLSGenerated";

	private const RenderTextureFormat LowQualityTextureFormat = RenderTextureFormat.ARGB32;

	private const int LowQualityTextureScale = 2;

	private const RenderTextureFormat LowQualityBlurFormat = RenderTextureFormat.ARGB32;

	private const int LowQualityBlurScale = 4;

	public List<Color> BaseColors;

	public List<Color> DefaultLiveryTints;

    public string DefaultBodyShaderID = "CarBody_Simple_1";
    public string AdditionalShaderID = "CarBody_Simple_11";
    public string DefaultRingShaderID = "Ring_1";
    public string DefaultHeadlightShaderID = "HeadLight_1";
    public string DefaultStickerID = "Sticker_no";
    public string DefaultSpoilerID = "Spoiler_no";
    public float MinBodyHeight;
    public float MaxBodyHeight;
    public float DefaultBodyHeight;
    public List<string> simpleColorList = new List<string>();
    public List<string> matteColorList = new List<string>();
    public List<string> customColorList = new List<string>();
    public List<string> ringShaderList = new List<string>();
    public List<string> lightShaderList = new List<string>();
    public List<string> stickerShaderList = new List<string>();
    public List<string> spoilerShaderList = new List<string>();

    public CarWheelBlurSettings WheelBlurSettings;

    private EliteVisuals EliteVisuals;

	public bool UseLowQualityRenderTextures;

	private int current_color_index;

	private Texture2D base_ao_texture;

	private RenderTexture base_texture_baked;

	private bool has_been_baked_previously;

	private float body_reflectivity_original;

	private Light ambient_light;

	private Light direction_light;

	private bool is_bake_dirty = true;

    private List<CarLightFlare> headlight_flares = new List<CarLightFlare>();

    private CarBaseTextureBaker _textureBaker;

    //private CarShadowBaker _shadowBaker;

    private Dictionary<string, Texture2D> baseWheelTextures = new Dictionary<string, Texture2D>();

	private Dictionary<string, RenderTexture> blurredWheelTextures = new Dictionary<string, RenderTexture>();

	public readonly HashSet<Material> Materials = new HashSet<Material>();

	public readonly HashSet<Material> BakeMaterials = new HashSet<Material>();

	public readonly HashSet<Material> BodyMaterials = new HashSet<Material>();

	public readonly HashSet<Material> ScrollTexMaterials = new HashSet<Material>();

	public readonly HashSet<Material> ReflectiveMaterials = new HashSet<Material>();

    public readonly HashSet<Material> WheelMaterials = new HashSet<Material>();

    public readonly HashSet<GameObject> TierGameobjects = new HashSet<GameObject>();

	public readonly HashSet<Material> EffectMaterials = new HashSet<Material>();

    public readonly HashSet<Renderer> BlackShadeRenderers = new HashSet<Renderer>();

    public readonly HashSet<Renderer> GlassRenderers = new HashSet<Renderer>();

    public readonly HashSet<Renderer> HeadlightGlassRenderers = new HashSet<Renderer>();

    public readonly HashSet<Renderer> TextureOnlyRenderers = new HashSet<Renderer>();

    public readonly HashSet<Renderer> DiscRenderers = new HashSet<Renderer>();

    public readonly List<Renderer> GlowHeadLightRenderer = new List<Renderer>();

    public readonly List<Projector> LightProjector = new List<Projector>();

    public readonly List<Renderer> BodyRenderers = new List<Renderer>();

    public readonly List<Renderer> AdditionalRenderers = new List<Renderer>();

    public readonly List<Renderer> HeadLightChromeRenderers = new List<Renderer>();

    public readonly List<Projector> ShadowProjectionRenderers = new List<Projector>();




	public static GameObject CarGroundFlaresDefaultGO;

	private static GameObject _CachedShadowBaker;


    [SerializeField] private List<Projector> m_headLightProjector;
    [SerializeField] private List<Renderer> m_ringRenderers;
    [SerializeField] private Renderer m_spoilerRenderer;
    [SerializeField] private List<CarSpoiler> m_spoilers;
    [SerializeField] private bool fixedBodyColor = false;
    [SerializeField] private List<Projector> m_backLightProjector;

    public Material BodyMaterial
    {
        get
        {
            return BodyRenderers.Count > 0 ? 
                (Application.isPlaying?BodyRenderers.First().material:BodyRenderers.First().sharedMaterial) : null;
        }
        set
        {
            if (value == null)
                return;
            //var texture = Lightmap;
            SetMaterial(BodyRenderers, value);
            //Lightmap = texture;

            //Set Also Sticker
            if (m_cachedStickerTexture != null)
            {
                Sticker = m_cachedStickerTexture;
            }
        }
    }



    public Material AdditionalMaterial
    {
        get
        {
            return AdditionalRenderers.Count > 0 ?
                (Application.isPlaying ? AdditionalRenderers.First().material : AdditionalRenderers.First().sharedMaterial) : null;
        }
        set
        {
            if (value == null)
                return;
            SetMaterial(AdditionalRenderers, value);
        }
    }


    public Material GlassMaterial
    {
        get { return GlassRenderers.Count > 0 ? GlassRenderers.First().material : null; }
        set
        {
            if (value == null)
                return;
            //SetMaterial(GlassRenderers, value);
        }
    }

    public Material HeadLightMaterial
    {
        set
        {
            SetMaterial(HeadLightChromeRenderers, value);
            if (value != null)
            {
                var color = value.GetColor("_HeadlightColor");
                if (GlowHeadLightRenderer.Count > 0)
                {
                    var mat = Application.isPlaying
                        ? GlowHeadLightRenderer[0].material
                        : GlowHeadLightRenderer[0].sharedMaterial;
                    mat.color = new Color(color.r, color.g, color.b, mat.color.a);
                    foreach (var glow in GlowHeadLightRenderer)
                    {
                        if (Application.isPlaying)
                        {
                            glow.material = mat;
                        }
                        else
                        {
                            glow.sharedMaterial = mat;
                        }
                    }
                }

                foreach (var projector in m_headLightProjector)
                {
                    //projector.material.color = new Color(color.r, color.g, color.b, projector.material.color.a);
                }
            }
        }
    }

    public Material RingMaterial
    {
        set
        {
            SetMaterial(m_ringRenderers, value);
            if (m_spoilerRenderer != null)
                SetMaterial(m_spoilerRenderer, value);
        }
        get
        {
            if (m_ringRenderers.Count > 0)
            {
                return m_ringRenderers[0].material;
            }
            return null;
        }
    }

    public Texture2D Sticker
    {
        set
        {
            m_cachedStickerTexture = value;
            foreach (var bodyRenderer in BodyRenderers)
            {
                var material = Application.isPlaying ? bodyRenderer.material : bodyRenderer.sharedMaterial;
                if (material == null)
                {
                    Debug.LogError(string.Format("Body of car '{0}' has no material", name));
                }
                else
                {
                    if (value == null)
                    {
                        value = ResourceManager.GetSharedAsset<Texture2D>("Sticker_no",
                            ServerItemBase.AssetType.sticker);
                    }
                    BodyMaterial.SetTexture("_Sticker", value);
                    BodyMaterial.SetTextureScale("_Sticker", m_cashedStickerScale);
                }
            }
        }
    }

    public void SetSpoiler(string spoilerID, GameObject spoiler)
    {
        spoilerID = spoilerID.ToLower();
        ClearSpoiler();

        var carSpoiler = m_spoilers.FirstOrDefault(s => s.SpoilerID.ToLower() == spoilerID);
        if (spoiler == null || carSpoiler == null)
        {
            DestroyImmediate(spoiler);
            //Debug.Log("spoiler is null for " + spoilerID);
            return;
        }
        m_activeSpoilerObject = spoiler;
        spoiler.transform.SetParent(BodyNode.transform);
        spoiler.transform.localPosition = carSpoiler.Position;
        spoiler.transform.localScale = carSpoiler.Scale;
        m_spoilerRenderer = m_activeSpoilerObject.GetComponentInChildren<Renderer>();
        SetMaterial(m_spoilerRenderer,RingMaterial);
    }

    public Texture Lightmap
    {
        set
        {
            foreach (var bodyRenderer in BodyRenderers)
            {
                var material = Application.isPlaying ? bodyRenderer.material : bodyRenderer.sharedMaterial;
                material.SetTexture("_LightMap_Garage", value);
            }
        }
        get { return (Texture2D)BodyRenderers[0].material.GetTexture("_LightMap_Garage"); }
    }

    private void SetMaterial(List<Renderer> renderers, Material material)
    {
        foreach (var bodyRenderer in renderers)
        {
            bodyRenderer.material = material;
        }
    }

    private void SetMaterial(Renderer renderer, Material material)
    {
        renderer.material = material;
    }

    public Color CurrentBaseColor
	{
		get
		{
            int currentColorIndex = this.GetCurrentColorIndex();
            if (this.EliteVisuals != null)
            {
                if (currentColorIndex >= 0 && currentColorIndex < this.EliteVisuals.EliteColors.Count)
                {
                    return this.EliteVisuals.EliteColors[currentColorIndex];
                }
            }
            else if (currentColorIndex >= 0 && currentColorIndex < this.BaseColors.Count)
            {
                return this.BaseColors[currentColorIndex];
            }
            return Color.white;
		}
	}

	public List<GameObject> LiveryTargets
	{
		get;
		private set;
	}

	public Light DirectionalLight
	{
		get
		{
			return this.direction_light;
		}
		set
		{
			this.direction_light = value;
			if (this.direction_light != null && this.ambient_light != null)
			{
				this.UpdateLighting();
			}
		}
	}

	public Light AmbientLight
	{
		get
		{
			return this.ambient_light;
		}
		set
		{
			this.ambient_light = value;
			if (this.direction_light != null && this.ambient_light != null)
			{
				this.UpdateLighting();
			}
		}
	}

	public Bounds BodyBounds
	{
		get;
		private set;
	}

	public Texture2D AOTexture
	{
		get
		{
			return this.base_ao_texture;
		}
	}

	public RenderTexture BakedTexture
	{
		get
		{
			return this.base_texture_baked;
		}
	}

	public GameObject CurrentlyAppliedLivery
	{
		get;
		private set;
	}

	public string CurrentlyAppliedLiveryObjName
	{
		get
		{
			return (!(this.CurrentlyAppliedLivery == null)) ? this.CurrentlyAppliedLivery.name : null;
		}
	}

    public CarShaderLod CurrentShaderLod
    {
        get;
        private set;
    }

	public bool HasBeenBakedPreviously
	{
		get
		{
			return this.has_been_baked_previously;
		}
		set
		{
			this.has_been_baked_previously = value;
		}
	}

	public float BodyReflectivityOriginal
	{
		get
		{
			return this.body_reflectivity_original;
		}
	}

    public CarReflectionTechnique ReflectionTechnique
    {
        get
        {
            if (this.CurrentShaderLod == CarShaderLod.RaceHigh)
            {
                return CarReflectionTechnique.Paraboloid;
            }
            if (this.CurrentShaderLod == CarShaderLod.RaceMed || this.CurrentShaderLod == CarShaderLod.RaceLow)
            {
                return CarReflectionTechnique.CubeMapPlusScroll;
            }
            return CarReflectionTechnique.CubeMap;
        }
    }

	public RenderTexture ShadowTexture
	{
		get
		{
		    return null;//this._shadowBaker.ShadowTexture;
		}
	}

	public Dictionary<string, Texture2D> BaseWheelTextures
	{
		get
		{
			return this.baseWheelTextures;
		}
	}

	public Dictionary<string, RenderTexture> BlurredWheelTextures
	{
		get
		{
			return this.blurredWheelTextures;
		}
	}

	public Material MainBodyMaterial
	{
		get;
		private set;
	}

	public Material NumberPlateMaterial
	{
		get;
		private set;
	}

    public List<TextMeshPro> NumberPlateTexts
    {
        get;
        private set;
    }

	public Texture2D CurrentlyAppliedNumberPlateTex
	{
		get;
		private set;
	}

	public GameObject BodyNode
	{
		get;
		private set;
	}

	public Collider BodyCollider
	{
		get;
		private set;
	}

	public GameObject BodyMeshNode
	{
		get;
		private set;
	}

	public GameObject GroundFlareNode
	{
		get;
		private set;
	}

	public List<GameObject> InteriorNodes
	{
		get;
		private set;
	}

	public List<GameObject> GroundFlareGOs
	{
		get;
		private set;
	}

	public List<GameObject> HeadlightFlareGOs
	{
		get;
		private set;
	}

    public List<GameObject> Wheels3D
    {
        get;
        private set;
    }

	public List<GameObject> WheelsFront3D
	{
		get;
		private set;
	}

    public List<GameObject> Wheels;
    private GameObject m_activeSpoilerObject;
    private Texture2D m_cachedStickerTexture;
    private Vector2 m_cashedStickerScale = new Vector2(1,1);
    //{
    //    get;
    //    private set;
    //}

	public GameObject Driver
	{
		get;
		private set;
	}

    public List<CarLightFlare> HeadlightFlares
    {
        get
        {
            if (this.headlight_flares.Count == 0)
            {
                this.headlight_flares = (from n in this.HeadlightFlareGOs.FindAll((GameObject n) => n != null)
                                         select n.GetComponent<CarLightFlare>() into n
                                         where n != null
                                         select n).ToList<CarLightFlare>();
            }
            return this.headlight_flares;
        }
        set
        {
            this.headlight_flares = value;
        }
    }

    public GarageCarVisuals GarageVisuals
    {
        get;
        set;
    }

    public ShowroomCarVisuals ShowroomVisuals
    {
        get;
        set;
    }

	public RaceCarVisuals RaceVisuals
	{
		get;
		set;
	}

    public bool HasSpoiler
    {
        get { return m_spoilers.Count(s => !string.IsNullOrEmpty(s.SpoilerID)) > 0; }
    }

    public bool HasFixedBodyColor
    {
        get { return fixedBodyColor; }
    }


    public GameObject SpoilerInstance
    {
        get { return m_activeSpoilerObject; }
    }

    public float BodyHeight
    {
        get { return BodyNode.transform.localPosition.y; }
        set
        {
            if (value == 0)
                value = DefaultBodyHeight;
            value = Mathf.Clamp(value, MinBodyHeight, MaxBodyHeight);
            var bodyPos = BodyNode.transform.localPosition;
            bodyPos.y = value;
            BodyNode.transform.localPosition = bodyPos;
        }
    }

    public bool HaveAdditionalRenderer
    {
        get { return AdditionalRenderers.Count > 0; }
    }


    private void Awake()
	{
        this.Build();
	}

    private void Reset()
    {
        this.InteriorNodes = new List<GameObject>();
        this.GroundFlareGOs = new List<GameObject>();
        this.HeadlightFlareGOs = new List<GameObject>();
        //this.HeadlightFlares = new List<CarLightFlare>();
        this.LiveryTargets = new List<GameObject>();
        this.Wheels3D = new List<GameObject>();
        this.WheelsFront3D = new List<GameObject>();
        //this.Wheels = new List<GameObject>();
        this.baseWheelTextures = new Dictionary<string, Texture2D>();
        this.blurredWheelTextures = new Dictionary<string, RenderTexture>();
        //DefaultBodyShaderID = "CarBody_Simple_1";
        //DefaultRingShaderID = "Ring_1";
        //DefaultHeadlightShaderID = "HeadLight_1";
        //DefaultStickerID = "Sticker_no";
        //DefaultSpoilerID = "Spoiler_no";
        EnableAnimation(false);
    }

    public void Build()
	{
		this.Reset();
		this.CheckGroundFlares();
		this.CacheChildNodes();
		this.CombineGroundFlares();
		if (this.base_ao_texture != null)
		{
            //this._textureBaker = new CarBaseTextureBaker(this);
			int num = this.base_ao_texture.width;
			int num2 = this.base_ao_texture.height;
			RenderTextureFormat format = RenderTextureFormat.ARGB32;
			if (this.UseLowQualityRenderTextures)
			{
				num /= 2;
				num2 /= 2;
				format = RenderTextureFormat.ARGB32;
			}
			this.base_texture_baked = new RenderTexture(num, num2, 0, format);
            //BasePlatform.ActivePlatform.PrepareRenderTexture(this.base_texture_baked);
		}
		this.BakeCarShadows();
		foreach (string current in this.baseWheelTextures.Keys)
		{
			Texture2D texture2D = this.baseWheelTextures[current];
			if (texture2D != null)
			{
				int num3 = texture2D.width;
				int num4 = texture2D.height;
				RenderTextureFormat format2 = RenderTextureFormat.ARGB32;
				if (this.UseLowQualityRenderTextures)
				{
					num3 /= 4;
					num4 /= 4;
					format2 = RenderTextureFormat.ARGB32;
				}
				RenderTexture renderTexture = new RenderTexture(num3, num4, 0, format2);
                //BasePlatform.ActivePlatform.PrepareRenderTexture(renderTexture);
				this.blurredWheelTextures.Add(current, renderTexture);
			}
		}
	}

	[Conditional("CSR_DEVELOPMENT_DEBUG"), Conditional("BUNDLE_BUILDER")]
	private void LogWarningsAndStats()
	{
		if (this.BodyMeshNode == null)
		{
		}
		if (this.BodyCollider == null)
		{
		}
		if (this.NumberPlateMaterial == null)
		{
		}
	}

	private void OnDestroy()
	{
		foreach (string current in this.blurredWheelTextures.Keys)
		{
			RenderTexture renderTexture = this.blurredWheelTextures[current];
			renderTexture.Release();
			Destroy(renderTexture);
		}
		this.blurredWheelTextures.Clear();
		if (this.base_texture_baked != null)
		{
			this.base_texture_baked.Release();
			Destroy(this.base_texture_baked);
		}
		this.ReleaseWheelBlurs();
	}

	public void SetCurrentColor(int newColor)
	{
		if (this.current_color_index == newColor)
		{
			return;
		}
		this.is_bake_dirty = true;
		this.current_color_index = newColor;
	}

	public int GetCurrentColorIndex()
	{
        if (this.EliteVisuals != null)
        {
            if (this.current_color_index >= this.EliteVisuals.EliteColors.Count)
            {
                return this.EliteVisuals.EliteColors.Count - 1;
            }
            return this.current_color_index;
        }
        else
        {
            if (this.current_color_index >= this.BaseColors.Count)
            {
                return this.BaseColors.Count - 1;
            }
            return this.current_color_index;
        }
	}

	public int AddNewColor(Color newColor)
	{
		this.BaseColors.Add(newColor);
		return this.BaseColors.Count - 1;
	}

	public void ForceColor(Color forcedColor)
	{
		this.BaseColors.Clear();
		this.BaseColors.Add(forcedColor);
		this.current_color_index = 0;
	}

    public void SetDefaultShaders()
    {
        if (BodyRenderers.Count > 0)
        {
            DefaultBodyShaderID = BodyRenderers[0].sharedMaterial.name;
        }
        if (m_ringRenderers.Count > 0)
        {
            DefaultRingShaderID = m_ringRenderers[0].sharedMaterial.name;
        }
        if (HeadLightChromeRenderers.Count > 0)
        {
            DefaultHeadlightShaderID = HeadLightChromeRenderers[0].sharedMaterial.name;
        }
        DefaultStickerID = "Sticker_no";
        DefaultSpoilerID = "Spoiler_no";
    }


    public void SetDefaultBodyHeight()
    {
        DefaultBodyHeight = BodyNode.transform.localPosition.y;
        MinBodyHeight = BodyNode.transform.localPosition.y;
        MaxBodyHeight = BodyNode.transform.localPosition.y;
    }

    public void CacheChildNodes()
	{
		Dictionary<Material, List<MeshRenderer>> dictionary = new Dictionary<Material, List<MeshRenderer>>();
		Transform[] componentsInChildren = base.transform.GetComponentsInChildren<Transform>(true);
		Transform[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			Transform transform = array[i];
			GameObject go = transform.gameObject;
			string text = go.name.ToLower();
			if (go.name == "CarLightSetup")
			{
				this.GroundFlareNode = go;
			}
			else if (text.Contains("lightflareground"))
			{
				this.GroundFlareGOs.Add(go);
			}
            else if (text.Contains("glow_headlight"))
			{
			    if (HeadlightFlareGOs == null)
			    {
			        HeadlightFlareGOs = new List<GameObject>();
			    }
				this.HeadlightFlareGOs.Add(go);
                var glow_headlight = go.GetComponent<Renderer>();
                if (!GlowHeadLightRenderer.Contains(glow_headlight) && glow_headlight != null)
                    GlowHeadLightRenderer.Add(glow_headlight);
			}
            else if (text.Contains("player_name"))
            {
                if (this.NumberPlateTexts == null)
                {
                    this.NumberPlateTexts = new List<TextMeshPro>();
                }
                NumberPlateTexts.Add(go.GetComponent<TextMeshPro>());
            }
			else if (text.Contains("collider"))
			{
				this.BodyCollider = go.GetComponent<Collider>();
                //this.BodyCollider.GetComponent<Collider>().enabled = false;
			}
			else if (text.StartsWith("3dwheel"))
			{
				this.Wheels3D.Add(go);
				if (text.Contains("front"))
				{
					this.WheelsFront3D.Add(go);
				}
			}
            //else if()
            else if (text == "f_r" || text == "f_l" || text == "r_l" || text == "r_r")
            {
                if (!Wheels.Contains(go))
                {
                    Wheels.Add(go);
                }
            }
            else if (text == "headlight_chrome")
            {
                var headLightMaterial =  go.GetComponent<Renderer>();
                if (!HeadLightChromeRenderers.Contains(headLightMaterial) && headLightMaterial!=null)
                    HeadLightChromeRenderers.Add(headLightMaterial);
            }
            else if (text == "headlight_projection" || text == "backlight_projection")
            {
                this.LightProjector.Add(go.GetComponent<Projector>());

                if (text == "headlight_projection")
                {
                    var headLight_projection = go.GetComponent<Projector>();
                    if (!m_headLightProjector.Contains(headLight_projection) && headLight_projection != null)
                        m_headLightProjector.Add(headLight_projection);
                }
            }
            else if (text.Contains("ring"))
            {
                var ring = go.GetComponent<Renderer>();
                if (!m_ringRenderers.Contains(ring) && ring!=null)
                    m_ringRenderers.Add(ring);
            }
			else if (text == "body")
			{
				this.BodyNode = go;
			    var ren = go.GetComponent<Renderer>();
			    if (!BodyRenderers.Contains(ren) && ren!=null)
			        BodyRenderers.Add(ren);
			}
            else if (text.Contains("adt"))
            {
                var ren = go.GetComponent<Renderer>();
                if (!AdditionalRenderers.Contains(ren) && ren != null)
                    AdditionalRenderers.Add(ren);
            }
            else if (text.Contains("tire"))
            {
                this.TierGameobjects.Add(go);
            }
            else if (text == "glass")
            {
                var ren = go.GetComponent<Renderer>();
                if (!GlassRenderers.Contains(ren) && ren != null)
                    GlassRenderers.Add(ren);
            }
			else if (this.BodyMeshNode == null && text == "chassis")
			{
				this.BodyMeshNode = go;
				this.BodyBounds = go.GetComponent<Renderer>().bounds;
			}
			else if (this.BodyMeshNode == null && text.Contains("body"))
			{
				MeshFilter component = go.GetComponent<MeshFilter>();
				string a = go.transform.parent.gameObject.name.ToLower();
				if (component != null && a == "body")
				{
					Renderer renderer = go.GetComponent<Renderer>();
					string text2 = component.sharedMesh.name.ToLower();
					if (text2.Contains("body"))
					{
						this.BodyMeshNode = renderer.gameObject;
						this.BodyBounds = renderer.bounds;
					}
				}
			}
            //else if (text.EndsWith("interior"))
            //{
            //    this.InteriorNodes.Add(gameObject);
            //}
			else if (text == "driver")
			{
				this.Driver = go;
			}
            else if (text == "black_shade")
            {
                this.BlackShadeRenderers.Add(go.GetComponent<Renderer>());
            }
            else if (text == "headlight_glass")
            {
                this.HeadlightGlassRenderers.Add(go.GetComponent<Renderer>());
            }
            else if (text == "texture_only")
            {
                this.TextureOnlyRenderers.Add(go.GetComponent<Renderer>());
            }
            else if (text == "disc")
            {
                this.DiscRenderers.Add(go.GetComponent<Renderer>());
            }
            else if (text == "shadow_projection")
            {
                this.ShadowProjectionRenderers.Add(go.GetComponent<Projector>());
            }
			MeshRenderer meshRenderer = go.GetComponent<Renderer>() as MeshRenderer;
			if (meshRenderer != null)
			{
                if (go.CompareTag("LiveryTarget"))
                {
                    this.LiveryTargets.Add(go);
                }
				if (!(meshRenderer.sharedMaterial == null))
				{
					if (!dictionary.ContainsKey(meshRenderer.sharedMaterial))
					{
						dictionary.Add(meshRenderer.sharedMaterial, new List<MeshRenderer>());
					}
					dictionary[meshRenderer.sharedMaterial].Add(meshRenderer);
				}
			}
		}
        if(Application.isPlaying)
		foreach (KeyValuePair<Material, List<MeshRenderer>> current in dictionary)
		{
			Material material = current.Value[0].material;
			foreach (MeshRenderer current2 in current.Value)
			{
				current2.sharedMaterial = material;
			}
			this.CacheMaterial(current.Value[0]);
		}
	}

	public bool SetGroundFlareState(bool want)
	{
	    //if (this.GroundFlareNode == null)
        //{
        //    return false;
        //}
        //bool activeSelf = this.GroundFlareNode.gameObject.activeSelf;
        //this.GroundFlareNode.gameObject.SetActive(want);
        //return activeSelf;
	    return false;
	}

    public void SetGroundFlareLayer(string layername)
	{
        //int layer = LayerMask.NameToLayer(layername);
        //foreach (GameObject current in this.GroundFlareGOs)
        //{
        //    current.layer = layer;
        //}
	}

	private void CombineGroundFlares()
	{
        //if (this.GroundFlareNode == null)
        //{
        //    return;
        //}
        //List<CarVisuals.FlareCombinerInfo> list = new List<CarVisuals.FlareCombinerInfo>
        //{
        //    new CarVisuals.FlareCombinerInfo("headlightflareground", "CombinedHeadlightFlaresGround"),
        //    new CarVisuals.FlareCombinerInfo("taillightflareground", "CombinedTailLightFlaresGround")
        //};
        //list.ForEach(delegate(CarVisuals.FlareCombinerInfo info)
        //{
        //    List<GameObject> list2 = this.GroundFlareGOs.FindAll((GameObject x) => x.name.ToLower().Contains(info.searchName));
        //    if (list2.Count > 1)
        //    {
        //        this.GroundFlareGOs = this.GroundFlareGOs.Except(list2).ToList<GameObject>();
        //        Material material = list2[0].GetComponent<Renderer>().material;
        //        GameObject gameObject = MeshCombiner.CombineThenDestroy(list2, material, "TransparentFX", this.GroundFlareNode.transform);
        //        gameObject.name = info.combinedName;
        //        this.GroundFlareGOs.Add(gameObject);
        //    }
        //});
	}

	private void CacheMaterial(Renderer renderer)
	{
		Material sharedMaterial = renderer.sharedMaterial;
		string text = sharedMaterial.name.ToLower();
		string text2 = sharedMaterial.shader.name;
		int num = text2.LastIndexOf('/');
		text2 = text2.Substring(num + 1).ToLower();
		if (!text2.StartsWith("car"))
		{
			return;
		}
		if (text2 == "carbody" || text2 == "carbody+")
		{
			this.Materials.Add(sharedMaterial);
			this.ReflectiveMaterials.Add(sharedMaterial);
			this.BodyMaterials.Add(sharedMaterial);
			this.ScrollTexMaterials.Add(sharedMaterial);
			this.BakeMaterials.Add(sharedMaterial);
			Texture2D x = sharedMaterial.GetTexture("_MainTex") as Texture2D;
			if (x != null)
			{
				this.base_ao_texture = x;
				this.MainBodyMaterial = sharedMaterial;
			}
			this.body_reflectivity_original = sharedMaterial.GetFloat("_Reflectivity");
		}
		else if (text2 == "carbodynobake")
		{
			this.Materials.Add(sharedMaterial);
			this.ReflectiveMaterials.Add(sharedMaterial);
			this.BodyMaterials.Add(sharedMaterial);
			this.ScrollTexMaterials.Add(sharedMaterial);
			if (text.Contains("numplate"))
			{
				this.NumberPlateMaterial = sharedMaterial;
			}
		}
		else if (text2 == "carglass" || text2 == "carglasstransparent")
		{
			this.Materials.Add(sharedMaterial);
			this.ReflectiveMaterials.Add(sharedMaterial);
			this.BodyMaterials.Add(sharedMaterial);
			this.ScrollTexMaterials.Add(sharedMaterial);
		}
		else if (text2 == "carwheels")
		{
			this.Materials.Add(sharedMaterial);
			this.ReflectiveMaterials.Add(sharedMaterial);
            this.WheelMaterials.Add(sharedMaterial);
			Texture2D texture2D = sharedMaterial.GetTexture("_MainTex") as Texture2D;
			if (texture2D != null)
			{
				this.baseWheelTextures.Add(sharedMaterial.name, texture2D);
			}
		}
		else if (text2 == "carunlit" || text2 == "carunlitcolor")
		{
			this.Materials.Add(sharedMaterial);
		}
		else if (text2 == "carshadow" || text2 == "cargroundflare")
		{
			this.EffectMaterials.Add(sharedMaterial);
		}
	}

	public void SwichToRaceWheels()
	{
        //this.SwitchWheelsOnOff(this.Wheels, this.Wheels3D);
	}

	public void SwichTo3DWheels()
	{
        //this.SwitchWheelsOnOff(this.Wheels3D, this.Wheels);
	}

	public void SwitchWheelsOnOff(List<GameObject> WheelsOn, List<GameObject> WheelsOff)
	{
		if (WheelsOn[0].activeInHierarchy && !WheelsOff[0].activeInHierarchy)
		{
			return;
		}
		WheelsOn.ForEach(delegate(GameObject wheel)
		{
			wheel.SetActive(true);
		});
		WheelsOff.ForEach(delegate(GameObject wheel)
		{
			wheel.SetActive(false);
		});
	}

    public void ApplyNumberPlateText(string text)
    {
        if (string.IsNullOrEmpty(text) || text.ToLower() == "no_name")
        {
            text = "GT CLUB";
        }
        if (NumberPlateTexts == null)
        {
            GTDebug.LogWarning(GTLogChannel.CarVisuals, String.Format(
                "'{0}' has nor numberplate.make sure this car has object with name 'player_name' in its hierarchy", name));
            return;
        }
        var localizedName = LocalizationManager.FixRTL_IfNeeded(text);
        foreach (var numberPlateText in NumberPlateTexts)
        {
            numberPlateText.text = localizedName.ToUpper();
        }
    }

    public void ApplyNumberPlate(Texture2D tex)
	{
		if (this.NumberPlateMaterial != null)
		{
			this.NumberPlateMaterial.SetTexture("_MainTex", tex);
			this.CurrentlyAppliedNumberPlateTex = tex;
			return;
		}
	}

    public void SetShaderLod(CarShaderLod lod)
    {
        this.CurrentShaderLod = lod;
        foreach (Material current in this.Materials)
        {
            current.shader.maximumLOD = (int)lod;
        }
        bool interiorNodesActive = false;
        int num = (int) lod;//(int)(lod % (CarShaderLod)100);
        if (num != 0)
        {
            interiorNodesActive = true;
        }
        this.InteriorNodes.ForEach(delegate(GameObject node)
        {
            node.SetActive(interiorNodesActive);
        });
    }

	public void SetLayer(string layerName)
	{
        //int layer = LayerMask.NameToLayer(layerName);
        //this.SetLayerRecursive(base.gameObject, layer);
	}

	public void SetLayer(int layerId)
	{
		this.SetLayerRecursive(base.gameObject, layerId);
	}

	private void SetLayerRecursive(GameObject obj, int layer)
	{
		obj.layer = layer;
		foreach (Transform transform in obj.transform)
		{
			this.SetLayerRecursive(transform.gameObject, layer);
		}
	}

	public void MakeBakeDirty()
	{
		this.is_bake_dirty = true;
	}

    public void ApplyLivery(GameObject liveryObj, bool immediately)
    {
        this.ApplyLivery(liveryObj, immediately, null);
    }

    public void ApplyLivery(GameObject liveryObj, bool immediately, Action<CarBaseTextureBaker> callback)
    {
        if (this._textureBaker == null)
        {
            this.CurrentlyAppliedLivery = liveryObj;
            this.is_bake_dirty = false;
            if (callback != null)
            {
                callback(null);
            }
            return;
        }
        if (liveryObj != this.CurrentlyAppliedLivery || liveryObj == null)
        {
            this.is_bake_dirty = true;
        }
        if (liveryObj != null)
        {
            this.EliteVisuals = liveryObj.GetComponent<EliteVisuals>();
        }
        else
        {
            this.EliteVisuals = null;
        }
        if (!this.is_bake_dirty)
        {
            if (callback != null)
            {
                callback(this._textureBaker);
            }
            return;
        }
        this.CurrentlyAppliedLivery = liveryObj;
        this.is_bake_dirty = false;
        if (immediately)
        {
            this._textureBaker.BakeTextureNow(liveryObj);
            if (callback != null)
            {
                callback(this._textureBaker);
            }
        }
        else
        {
            this._textureBaker.BakeTexture(liveryObj, callback);
        }
    }

    public void BakeWheelBlurs()
    {
        WheelBlurBaker wheelBlurBaker = new WheelBlurBaker();
        wheelBlurBaker.BakeBlurs(this, this.WheelBlurSettings);
    }

	public void HideDriver()
	{
		if (this.Driver != null)
		{
			this.Driver.SetActive(false);
		}
	}

	public void ReleaseWheelBlurs()
	{
		foreach (string current in this.blurredWheelTextures.Keys)
		{
			RenderTexture renderTexture = this.blurredWheelTextures[current];
			renderTexture.Release();
			Destroy(renderTexture);
		}
		this.blurredWheelTextures.Clear();
	}

	public void ShowDriver()
	{
        if (this.Driver == null)
        {
            return;
        }
        bool active = this.CurrentShaderLod != CarShaderLod.RaceLow || !this.Driver.activeInHierarchy;
        this.Driver.SetActive(active);
	}

	public void ClearReflectionTextures()
	{
		foreach (Material current in this.ReflectiveMaterials)
		{
			if (!(current == null))
			{
				if (current.HasProperty("_Cube"))
				{
					current.SetTexture("_Cube", null);
				}
				if (current.HasProperty("_FrontParaboloid"))
				{
					current.SetTexture("_FrontParaboloid", null);
				}
				if (current.HasProperty("_RearParaboloid"))
				{
					current.SetTexture("_RearParaboloid", null);
				}
			}
		}
		foreach (Material current2 in this.ScrollTexMaterials)
		{
			if (!(current2 == null))
			{
				if (current2.HasProperty("_ScrollTex"))
				{
					current2.SetTexture("_ScrollTex", null);
				}
			}
		}
	}

	public void UpdateLighting()
	{
        //Vector3 v = -this.DirectionalLight.transform.forward;
        //Color color = this.AmbientLight.color;
        //Color color2 = this.DirectionalLight.color * this.DirectionalLight.intensity;
        //foreach (Material current in this.WheelMaterials)
        //{
        //    current.SetVector("_DirectionalLightDir", v);
        //    current.SetColor("_DirectionalLightColor", color2);
        //    current.SetColor("_AmbientLightColor", color);
        //}
        //float num = this.DirectionalLight.intensity;
        //num = Mathf.Lerp(num + 0.35f, num - 0.1f, this.CurrentBaseColor.grayscale);
        //color2 = this.DirectionalLight.color * num;
        //foreach (Material current2 in this.BodyMaterials)
        //{
        //    current2.SetVector("_DirectionalLightDir", v);
        //    current2.SetColor("_DirectionalLightColor", color2);
        //    current2.SetColor("_AmbientLightColor", color);
        //}
	}

	public void CheckGroundFlares()
	{
		Transform transform = base.gameObject.transform.Find("CarLightSetup");
		if (transform != null)
		{
			Transform x = transform.Find("CLSGenerated");
			if (x != null)
			{
				return;
			}
			transform.transform.parent = null;
			Destroy(transform.gameObject);
		}
		this.AttachGroundFlares();
	}

	public void AttachGroundFlares()
	{
		//GameObject carGroundFlaresDefaultGO = CarGroundFlaresDefaultGO;
		//GameObject gameObject = Instantiate(carGroundFlaresDefaultGO) as GameObject;
		//gameObject.transform.parent = base.gameObject.transform;
		//gameObject.name = "CarLightSetup";
		//gameObject.transform.localPosition = Vector3.zero;
		//gameObject.transform.localRotation = Quaternion.identity;
		//gameObject.transform.localScale = Vector3.one;
		//gameObject.SetActive(true);
		//GameObject gameObject2 = new GameObject("CLSGenerated");
		//gameObject2.transform.parent = gameObject.transform;
		//gameObject2.transform.localPosition = Vector3.zero;
		//GameObject gameObject3 = base.gameObject.transform.GetComponentsInChildren<MeshCollider>(true)[0].gameObject;
		//Vector3 extents = gameObject3.GetComponent<Collider>().bounds.extents;
		//Vector3 vector = gameObject3.transform.localScale;
		//vector = Vector3.one;
		//float num = extents.x * vector.x;
		//float num2 = extents.z * vector.y;
		//Vector3 inP = new Vector3(0.25f - num, 0.03f, 2.3f + num2);
		//Vector3 inP2 = new Vector3(-0.25f + num, 0.03f, 2.3f + num2);
		//Vector3 inP3 = new Vector3(0.1f - num, 0.03f, -1.3f - num2);
		//Vector3 inP4 = new Vector3(-0.1f + num, 0.03f, -1.3f - num2);
		//Vector3 inS = new Vector3(5f, 2.86f, 1f);
		//Vector3 inS2 = new Vector3(5f, -2.86f, 1f);
		//Vector3 inS3 = new Vector3(2.94f, 2.66f, 1f);
		//Vector3 inR = new Vector3(90f, 90f, 0f);
		//Vector3 inR2 = new Vector3(90f, 285f, 0f);
		//Vector3 inR3 = new Vector3(90f, 255f, 0f);
		//Dictionary<string, GroundFlareSetupData> dictionary = new Dictionary<string, GroundFlareSetupData>
		//{
		//	{
		//		"HeadlightFlareGroundL",
		//		new GroundFlareSetupData(inS, inR, inP)
		//	},
		//	{
		//		"HeadlightFlareGroundR",
		//		new GroundFlareSetupData(inS2, inR, inP2)
		//	},
		//	{
		//		"TaillightFlareGroundL",
		//		new GroundFlareSetupData(inS3, inR2, inP3)
		//	},
		//	{
		//		"TaillightFlareGroundR",
		//		new GroundFlareSetupData(inS3, inR3, inP4)
		//	}
		//};
		//foreach (KeyValuePair<string, GroundFlareSetupData> current in dictionary)
		//{
		//	GroundFlareSetupData value = current.Value;
		//	Transform transform = gameObject.transform.Find(current.Key);
		//	transform.localScale = value.scale;
		//	transform.localPosition = value.pos;
		//	transform.localRotation = Quaternion.Euler(value.rot.x, value.rot.y, value.rot.z);
		//}
	}

	public void BakeCarShadows()
	{
        //if (CarVisuals._CachedShadowBaker == null)
        //{
        //    CarVisuals._CachedShadowBaker = (Resources.Load("CarShadows/CarShadowBaker") as GameObject);
        //}
        //if (this._shadowBaker == null)
        //{
        //    GameObject gameObject = UnityEngine.Object.Instantiate(CarVisuals._CachedShadowBaker) as GameObject;
        //    if (gameObject != null)
        //    {
        //        this._shadowBaker = gameObject.GetComponent<CarShadowBaker>();
        //    }
        //}
        //if (this._shadowBaker != null)
        //{
        //    this._shadowBaker.transform.parent = base.transform;
        //    this._shadowBaker.transform.localPosition = Vector3.zero;
        //    this._shadowBaker.RenderShadowMap(this);
        //}
	}

	private IEnumerator CheckForUncreatedRenderTextures()
	{
	    bool fixedTextures = false;
	    int i = 0;

	    while (i < 16)
	    {
	        if ((BakedTexture != null) && !BakedTexture.IsCreated())
	        {
	            BakedTexture.Create();
	            MakeBakeDirty();
	            ApplyLivery(CurrentlyAppliedLivery, true);
	            fixedTextures = true;
	        }
	        if ((ShadowTexture != null) && !ShadowTexture.IsCreated())
	        {
	            BakeCarShadows();
	            fixedTextures = true;
	        }
	        if (GetComponent<RaceCarVisuals>() != null)
	        {
	            var collection = blurredWheelTextures.Keys.GetEnumerator();
	            try
	            {
	                while (collection.MoveNext())
	                {
	                    var key = collection.Current;
	                    var tex = blurredWheelTextures[key];
	                    if ((tex != null) && !tex.IsCreated())
	                    {
	                        BakeWheelBlurs();
	                        fixedTextures = true;
	                    }
	                }
	            }
	            finally
	            {
	                collection.Dispose();
	            }
	        }


	        if (fixedTextures)
	        {
	            yield break;
	        }
	        else
	        {
	            i++;
	            yield return null;
	        }
	    }
	}

	private void OnApplicationPause(bool paused)
	{
		if (!paused)
		{
            //base.StartCoroutine(this.CheckForUncreatedRenderTextures());
		}
	}

    public string GetDefaultAssetItemID(ServerItemBase.AssetType assetType)
    {
        switch (assetType)
        {
            case ServerItemBase.AssetType.body_shader:
                return DefaultBodyShaderID;
                case ServerItemBase.AssetType.ring_shader:
                return DefaultRingShaderID;
                case ServerItemBase.AssetType.headlight_shader:
                return DefaultHeadlightShaderID;
                case ServerItemBase.AssetType.spoiler:
                return DefaultSpoilerID;
                case ServerItemBase.AssetType.sticker:
                return DefaultStickerID;
        }
        return null;
    }

    public void SetProperty(string carID, ServerItemBase.AssetType assetType, string itemID)
    {
        if (string.IsNullOrEmpty(itemID))
        {
            itemID = GetDefaultAssetItemID(assetType);
        }
        switch (assetType)
        {
            case ServerItemBase.AssetType.body_shader:
                BodyMaterial = ResourceManager.GetSharedAsset<Material>(itemID.GetCurrcetBodyMaterialID(),
                    assetType);
                //var texture = ResourceManager.GetCarAsset<Texture>(carID,
                //    ServerItemBase.AssetType.lightmap_garage);
                //Lightmap = texture;
                //Debug.Log(m_carItemID+"   "+texture.name);
                break;
            case ServerItemBase.AssetType.headlight_shader:
                HeadLightMaterial = ResourceManager.GetSharedAsset<Material>(itemID,
                    assetType);
                break;
            case ServerItemBase.AssetType.ring_shader:
                RingMaterial = ResourceManager.GetSharedAsset<Material>(itemID.GetCurrectRingMaterialID(),
                    assetType);
                break;
            case ServerItemBase.AssetType.sticker:
                if (itemID == DefaultStickerID || string.IsNullOrEmpty(itemID))
                {
                    m_cashedStickerScale = new Vector2(1, 1);
                    Sticker = ResourceManager.GetSharedAsset<Texture2D>(itemID, assetType);
                }
                else
                {
                    int stickerNumber = int.Parse(itemID.ToLower().Replace("sticker_", ""));
                    if (stickerNumber < 40)
                    {
                        m_cashedStickerScale = new Vector2(1, 1);
                        Sticker = ResourceManager.GetCarAsset<Texture2D>(carID, assetType, itemID);
                    }
                    else
                    {
                        m_cashedStickerScale = new Vector2(2, 2);
                        Sticker = ResourceManager.GetSharedAsset<Texture2D>(itemID, assetType);
                    }
                }
                break;
            case ServerItemBase.AssetType.spoiler:
                if (itemID.ToLower() == "spoiler_no")
                {
                    if (m_activeSpoilerObject != null)
                    {
                        Destroy(m_activeSpoilerObject);
                        m_activeSpoilerObject = null;
                    }
                }
                else
                {
                    var spoiler = ResourceManager.GetSharedAsset<GameObject>(itemID, assetType);

                    if (spoiler == null)
                    {
                        GTDebug.LogWarning(GTLogChannel.CarVisuals, string.Format("No spoiler found width ID '{0}'", itemID));
                        return;
                    }
                    var spoilerInstance = Object.Instantiate(spoiler);
                    SetSpoiler(itemID, spoilerInstance);
                }
                break;
        }
    }

    public void SetUpAsCheap()
    {
        if (HeadlightGlassRenderers.Count >0)
        {
            foreach (var headlightGlassRenderer in HeadlightGlassRenderers)
            {
                headlightGlassRenderer.gameObject.SetActive(false);
            }
        }
        else
        {
            GTDebug.LogWarning(GTLogChannel.CarVisuals, string.Format("'{0}' has no healight_glass.", name));
        }

        foreach (var tierGameobject in TierGameobjects)
        {
            var rend = tierGameobject.GetComponent<Renderer>();
            if(rend!=null)
            rend.material =
                Resources.Load<Material>("shared_assets/car_shared_shader/Interior_and_Tire_cheap_Shared");
        }
    }

    public void DestroySpoiler()
    {
        Destroy(m_activeSpoilerObject);
    }

    public void EnableAnimation(bool value)
    {
        var animator = GetComponent<Animator>();
        if (animator != null)
            animator.enabled = value;
    }

    //public void FixMissingReferences()
    //{
    //    //var cubeMap = ResourceManager.GetAsset<Cubemap>("ReflectionMap/Garage_RefLmap_Blur");
    //    foreach (var render in BlackShadeRenderers)
    //    {
    //        render.material =
    //                    ResourceManager.GetSharedAsset<Material>("Black_Shade_Shared_Day",
    //                        ServerItemBase.AssetType.car_shared_shader);//.SetTexture("_RefLmap", cubeMap);
    //    }

    //    var cubeMap = ResourceManager.GetAsset<Cubemap>("ReflectionMap/Garage_RefLmap_Sharp_BW");
    //    foreach (var render in GlassRenderers)
    //    {
    //        render.material.SetTexture("_RefLmap", cubeMap);
    //    }

    //    cubeMap = ResourceManager.GetAsset<Cubemap>("ReflectionMap/Headlight_Glass_RefLmap");
    //    foreach (var render in HeadlightGlassRenderers)
    //    {
    //        render.material.SetTexture("_node_7146", cubeMap);
    //    }

    //    cubeMap = ResourceManager.GetAsset<Cubemap>("ReflectionMap/StreetDay_RefLmap_Sharp");
    //    foreach (var render in TextureOnlyRenderers)
    //    {
    //        render.material.SetTexture("_node_929", cubeMap);
    //    }

    //    var tex = ResourceManager.GetAsset<Texture2D>("CarSharedTexture/Glow_HeadLight");
    //    foreach (var render in GlowHeadLightRenderer)
    //    {
    //        render.material.SetTexture("_node_7386", tex);
    //    }

    //    tex = ResourceManager.GetAsset<Texture2D>("CarSharedTexture/Front_Light_Cookie");
    //    foreach (var render in LightProjector)
    //    {
    //        render.material.SetTexture("_ShadowTex", tex);
    //    }
    //    foreach (var render in m_headLightProjector)
    //    {
    //        render.material.SetTexture("_ShadowTex", tex);
    //    }

    //    var cookie = ResourceManager.GetAsset<Texture2D>("CarSharedTexture/Hyundai_Genesis_Shadow");
    //    var fallOf = ResourceManager.GetAsset<Texture2D>("CarSharedTexture/Falloff");
    //    foreach (var render in m_shadowProjectionRenderers)
    //    {
    //        render.material.SetTexture("_Cookie", cookie);
    //        render.material.SetTexture("_FallOff", fallOf);
    //    }
    //}

    public void CacheStickerScale(Vector2 texScale)
    {
        m_cashedStickerScale = texScale;
    }

    public void ClearSpoiler()
    {
        if (m_activeSpoilerObject != null)
        {
            Destroy(m_activeSpoilerObject);
            m_activeSpoilerObject = null;
        }
    }
}
