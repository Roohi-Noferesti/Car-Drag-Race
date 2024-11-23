using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

[ExecuteInEditMode]
public class ParaboloidReflectionRenderer : MonoBehaviour
{
	public enum ParaboloidFrontVector
	{
		X,
		Z
	}

	private const float NEAR_CLIPPING_VALUE = 0.3f;

	public static Shader DistortionShader;

	public static Shader PerPixelDistortionShader;

	public List<MeshFilter> MeshesToRender = new List<MeshFilter>();

	public Color ClearColor = Color.black;

	public Texture2D PerPixelDistortionTexture;

	public bool EnablePerPixelDistortion;

	[SerializeField]
	private int _rtSize = (!BaseDevice.ActiveDevice.IsLowMemoryDevice()) ? 512 : 256;

	[SerializeField]
	private GameObject _eyeParentObject;

	[SerializeField]
	private ParaboloidReflectionRenderer.ParaboloidFrontVector _frontVector = ParaboloidReflectionRenderer.ParaboloidFrontVector.Z;

	private RenderTexture _frontParaboloidTarget;

	private RenderTexture _rearParaboloidTarget;

	private Material _mat;

	private Material _perPixelDistortionMat;

	private GameObject _rendererOrigin;

	private Camera _frontCamera;

	private Camera _rearCamera;

	public int RenderTextureSize
	{
		get
		{
			return this._rtSize;
		}
		set
		{
			if (value != this._rtSize && base.enabled)
			{
				this.UpdateRenderTextures();
			}
			this._rtSize = value;
		}
	}

	public RenderTexture FrontParaboloid
	{
		get
		{
			return this._frontParaboloidTarget;
		}
	}

	public RenderTexture RearParaboloid
	{
		get
		{
			return this._rearParaboloidTarget;
		}
	}

	public GameObject EyeParentObject
	{
		get
		{
			return this._eyeParentObject;
		}
		set
		{
			this._eyeParentObject = value;
			if (this.RenderOrigin != null)
			{
				this.RenderOrigin.transform.parent = this._eyeParentObject.transform;
				this.RenderOrigin.transform.localPosition = Vector3.zero;
			}
		}
	}

	public GameObject RenderOrigin
	{
		get
		{
			return this._rendererOrigin;
		}
	}

	public ParaboloidReflectionRenderer.ParaboloidFrontVector FrontVector
	{
		get
		{
			return this._frontVector;
		}
		set
		{
			if (base.enabled)
			{
				this.OrientCameras();
			}
			this._frontVector = value;
		}
	}

	public GameObject DebugPreviewObject
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (ParaboloidReflectionRenderer.DistortionShader == null)
		{
			ParaboloidReflectionRenderer.DistortionShader = (Resources.Load("ParaboloidReflections/ParaboloidDistortion") as Shader);
		}
		if (ParaboloidReflectionRenderer.PerPixelDistortionShader == null)
		{
			ParaboloidReflectionRenderer.PerPixelDistortionShader = (Resources.Load("ParaboloidReflections/ParaboloidPerPixelDistortion") as Shader);
		}
		this._mat = new Material(ParaboloidReflectionRenderer.DistortionShader);
		this._perPixelDistortionMat = new Material(ParaboloidReflectionRenderer.PerPixelDistortionShader);
	}

	public void Setup(int renderTextureSize, List<MeshFilter> meshesToRender, GameObject parentObj)
	{
		this.RenderTextureSize = renderTextureSize;
		this.MeshesToRender = meshesToRender;
		this.EyeParentObject = parentObj;
		base.enabled = true;
		if (this._frontCamera == null)
		{
			this.SetupInternal();
		}
	}

	private void OnEnable()
	{
		if (this._frontCamera == null)
		{
			this.SetupInternal();
		}
	}

	private void SetupInternal()
	{
		if (this.MeshesToRender.Count == 0)
		{
			base.enabled = false;
			return;
		}
		this._rendererOrigin = new GameObject("Dual Paraboloid Renderer Eye");
		this._rendererOrigin.hideFlags = HideFlags.DontSave;
		if (this._eyeParentObject != null)
		{
			this._rendererOrigin.transform.parent = this._eyeParentObject.transform;
			this._rendererOrigin.transform.localPosition = Vector3.zero;
		}
		this._frontCamera = new GameObject("Front Paraboloid Camera")
		{
			hideFlags = HideFlags.DontSave,
			transform = 
			{
				parent = this._rendererOrigin.transform
			}
		}.AddComponent<Camera>();
		this._frontCamera.fieldOfView = 90f;
		this._frontCamera.farClipPlane = 1000f;
		this._frontCamera.nearClipPlane = 0.3f;
		this._frontCamera.cullingMask = 0;
		this._frontCamera.clearFlags = CameraClearFlags.Nothing;
		this._frontCamera.enabled = false;
		this._rearCamera = new GameObject("Rear Paraboloid Camera")
		{
			hideFlags = HideFlags.DontSave,
			transform = 
			{
				parent = this._rendererOrigin.transform
			}
		}.AddComponent<Camera>();
		this._rearCamera.fieldOfView = 90f;
		this._rearCamera.farClipPlane = 1000f;
		this._rearCamera.nearClipPlane = 0.3f;
		this._rearCamera.clearFlags = CameraClearFlags.Nothing;
		this._rearCamera.cullingMask = 0;
		this._rearCamera.enabled = false;
		this.OrientCameras();
		this.UpdateRenderTextures();
	}

	private void OrientCameras()
	{
		if (this._frontVector == ParaboloidReflectionRenderer.ParaboloidFrontVector.X)
		{
			this._frontCamera.transform.localPosition = new Vector3(-0.6f, 0f, 0f);
			this._frontCamera.transform.localRotation = Quaternion.Euler(0f, 90f, 0f);
			this._rearCamera.transform.localPosition = new Vector3(0.6f, 0f, 0f);
			this._rearCamera.transform.localRotation = Quaternion.Euler(0f, 270f, 0f);
		}
		else
		{
			this._frontCamera.transform.localPosition = new Vector3(0f, 0f, -0.6f);
			this._frontCamera.transform.localRotation = Quaternion.identity;
			this._rearCamera.transform.localPosition = new Vector3(0f, 0f, 0.6f);
			this._rearCamera.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
		}
	}

	private void UpdateRenderTextures()
	{
		if (this._frontParaboloidTarget != null)
		{
			this._frontParaboloidTarget.Release();
		}
		if (this._rearParaboloidTarget != null)
		{
			this._rearParaboloidTarget.Release();
		}
		this._frontParaboloidTarget = new RenderTexture(this.RenderTextureSize, this.RenderTextureSize, 16, RenderTextureFormat.Default);
		this._rearParaboloidTarget = new RenderTexture(this.RenderTextureSize, this.RenderTextureSize, 16, RenderTextureFormat.Default);
		BasePlatform.ActivePlatform.PrepareRenderTexture(this._frontParaboloidTarget);
		BasePlatform.ActivePlatform.PrepareRenderTexture(this._rearParaboloidTarget);
	}

	[Conditional("UNITY_EDITOR")]
	private void SetupDebugPreview()
	{
		if (this.DebugPreviewObject == null)
		{
			this.DebugPreviewObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
			this.DebugPreviewObject.name = "DebugPreview";
			this.DebugPreviewObject.hideFlags = HideFlags.DontSave;
			this.DebugPreviewObject.transform.parent = this._rendererOrigin.transform;
			this.DebugPreviewObject.transform.localPosition = Vector3.zero;
			this.DebugPreviewObject.layer = LayerMask.NameToLayer("EditorOnly");
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void SetupDebugPreviewMaterial()
	{
		if (this._frontVector == ParaboloidReflectionRenderer.ParaboloidFrontVector.X)
		{
			this.DebugPreviewObject.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("CSR Paraboloid/ParaboloidDebugPreviewX (Normals)"));
		}
		else
		{
			this.DebugPreviewObject.GetComponent<Renderer>().sharedMaterial = new Material(Shader.Find("CSR Paraboloid/ParaboloidDebugPreviewZ (Normals)"));
		}
		Material sharedMaterial = this.DebugPreviewObject.GetComponent<Renderer>().sharedMaterial;
		sharedMaterial.SetTexture("_FrontParaboloid", this.FrontParaboloid);
		sharedMaterial.SetTexture("_RearParaboloid", this.RearParaboloid);
	}

	private void OnDisable()
	{
		if (this.MeshesToRender.Count == 0)
		{
			return;
		}
		if (this._rendererOrigin != null)
		{
			UnityEngine.Object.Destroy(this._rendererOrigin);
		}
		if (this._frontParaboloidTarget != null)
		{
			this._frontParaboloidTarget.Release();
		}
		if (this._rearParaboloidTarget != null)
		{
			this._rearParaboloidTarget.Release();
		}
	}

	private void OnPreRender()
	{
		RenderTexture active = RenderTexture.active;
		this.RenderOneSide(this._frontCamera, this._frontParaboloidTarget);
		this.RenderOneSide(this._rearCamera, this._rearParaboloidTarget);
		RenderTexture.active = active;
	}

	private void RenderOneSide(Camera cam, RenderTexture rt)
	{
		bool flag = this.EnablePerPixelDistortion && this.PerPixelDistortionTexture != null;
		RenderTexture renderTexture = null;
		if (flag)
		{
			renderTexture = RenderTexture.GetTemporary(rt.width, rt.height, rt.depth);
			BasePlatform.ActivePlatform.PrepareRenderTexture(renderTexture);
			RenderTexture.active = renderTexture;
		}
		else
		{
			RenderTexture.active = rt;
		}
		GL.Clear(true, true, this.ClearColor);
		Matrix4x4 matrix4x = cam.projectionMatrix;
		matrix4x = this.CrossPlatformProjectionMatrix(matrix4x);
		foreach (MeshFilter current in this.MeshesToRender)
		{
			if (!(current == null))
			{
				Material sharedMaterial = current.GetComponent<Renderer>().sharedMaterial;
				Matrix4x4 matrix = matrix4x * cam.worldToCameraMatrix * current.transform.localToWorldMatrix;
				Shader.SetGlobalVector("_ProjectionParams", new Vector4(1f, cam.nearClipPlane, cam.farClipPlane, 1f / cam.farClipPlane));
				this._mat.SetTexture("_MainTex", sharedMaterial.mainTexture);
				this._mat.SetMatrix("_ModelViewProj", matrix);
				this._mat.SetPass(this.GetPassNumberForMesh(current));
				Graphics.DrawMeshNow(current.sharedMesh, Vector3.zero, Quaternion.identity);
			}
		}
		if (flag)
		{
			this._perPixelDistortionMat.SetTexture("_OriginalTex", renderTexture);
			this._perPixelDistortionMat.SetTexture("_DeformationTex", this.PerPixelDistortionTexture);
			this._perPixelDistortionMat.SetPass(0);
			Graphics.Blit(null, rt, this._perPixelDistortionMat);
			RenderTexture.ReleaseTemporary(renderTexture);
		}
	}

	private Matrix4x4 CrossPlatformProjectionMatrix(Matrix4x4 projMatrix)
	{
		if (SystemInfo.graphicsDeviceVersion.ToLower().Contains("direct") || SystemInfo.graphicsDeviceVersion.ToLower().Contains("metal"))
		{
			for (int i = 0; i < 4; i++)
			{
				projMatrix[1, i] = -projMatrix[1, i];
			}
			for (int j = 0; j < 4; j++)
			{
				projMatrix[2, j] = projMatrix[2, j] * 0.5f + projMatrix[3, j] * 0.5f;
			}
		}
		return projMatrix;
	}

	private int GetPassNumberForMesh(MeshFilter meshFilter)
	{
		Renderer renderer = meshFilter.GetComponent<Renderer>();
		if (renderer.lightmapIndex > -1)
		{
			return 2;
		}
		string text = renderer.sharedMaterial.shader.name.ToLower();
		if (text.Contains("backdrop"))
		{
			return 1;
		}
		return 0;
	}
}
