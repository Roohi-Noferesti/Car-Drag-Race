using System;
using System.Collections.Generic;
using UnityEngine;

public class CarBaseTextureBaker
{
	private struct LiveryProjectorData
	{
		public Texture2D Texture;

		public Color Tint;

		public Matrix4x4 ViewProjection;
	}

	private static Shader MultiplyAOShader;

	private static Shader LiveryProjectShader;

	private CarVisuals _visuals;

	private GameObject _livery;

	private Action<CarBaseTextureBaker> _callback;

	public CarVisuals CarVisuals
	{
		get
		{
			return this._visuals;
		}
	}

	public GameObject Livery
	{
		get
		{
			return this._livery;
		}
	}

	public CarBaseTextureBaker(CarVisuals visuals)
	{
		this._visuals = visuals;
	}

	static CarBaseTextureBaker()
	{
		CarBaseTextureBaker.MultiplyAOShader = (Resources.Load("Shaders/BakingShaders/CarMultiplyAO") as Shader);
		CarBaseTextureBaker.LiveryProjectShader = (Resources.Load("Shaders/BakingShaders/CarLiveryProject") as Shader);
	}

	public void BakeTexture(GameObject liveryObj, Action<CarBaseTextureBaker> callback)
	{
		if (this._visuals == null)
		{
			return;
		}
		this._livery = liveryObj;
		this._callback = callback;
		CameraPostRender.Instance.AddProcess("base texture for " + this._visuals.gameObject.name, delegate
		{
			if (this._visuals != null)
			{
				this.BakeBaseTexture(liveryObj);
			}
		});
	}

	public void BakeTextureNow(GameObject liveryObj)
	{
		if (this._visuals == null)
		{
			return;
		}
		this._livery = liveryObj;
		this._callback = null;
		this.BakeBaseTexture(liveryObj);
	}

	private void BakeBaseTexture(GameObject liveryObj)
	{
		RenderTexture active = RenderTexture.active;
		this._visuals.BakedTexture.DiscardContents();
		BasePlatform.ActivePlatform.PrepareRenderTexture(this._visuals.BakedTexture);
		RenderTexture.active = this._visuals.BakedTexture;
		Color currentBaseColor = this._visuals.CurrentBaseColor;
		currentBaseColor.a = 1f;
		GL.Clear(false, true, currentBaseColor);
		if (liveryObj != null)
		{
			Material material = new Material(CarBaseTextureBaker.LiveryProjectShader);
			List<CarBaseTextureBaker.LiveryProjectorData> liveryProjectorDatas = this.GetLiveryProjectorDatas(liveryObj);
			foreach (CarBaseTextureBaker.LiveryProjectorData current in liveryProjectorDatas)
			{
				foreach (GameObject current2 in this._visuals.LiveryTargets)
				{
					MeshFilter component = current2.GetComponent<MeshFilter>();
					Matrix4x4 matrix = current.ViewProjection * current2.transform.localToWorldMatrix;
					material.SetTexture("_LiveryTex", current.Texture);
					material.SetColor("_LiveryTint", current.Tint);
					material.SetMatrix("_ModelViewProj", matrix);
					material.SetPass((current.Texture.format != TextureFormat.Alpha8) ? 0 : 1);
					Graphics.DrawMeshNow(component.sharedMesh, Matrix4x4.identity);
				}
			}
		}
		Material mat = new Material(CarBaseTextureBaker.MultiplyAOShader);
		int pass = (this._visuals.AOTexture.format != TextureFormat.Alpha8) ? 0 : 1;
		Graphics.Blit(this._visuals.AOTexture, this._visuals.BakedTexture, mat, pass);
		RenderTexture.active = active;
		if (!this._visuals.HasBeenBakedPreviously)
		{
			foreach (Material current3 in this._visuals.BakeMaterials)
			{
				current3.SetTexture("_MainTex", this._visuals.BakedTexture);
			}
		}
		this._visuals.HasBeenBakedPreviously = true;
		float bodyReflectivityOriginal = this._visuals.BodyReflectivityOriginal;
		float value = Mathf.Lerp(bodyReflectivityOriginal, bodyReflectivityOriginal - 0.4f, this._visuals.CurrentBaseColor.grayscale);
		foreach (Material current4 in this._visuals.BakeMaterials)
		{
			current4.SetFloat("_Reflectivity", value);
		}
		if (this._visuals.DirectionalLight != null && this._visuals.AmbientLight != null)
		{
			this._visuals.UpdateLighting();
		}
		if (this._callback != null)
		{
			this._callback(this);
		}
		this._callback = null;
	}

	private List<CarBaseTextureBaker.LiveryProjectorData> GetLiveryProjectorDatas(GameObject liveryObj)
	{
		Transform parent = liveryObj.transform.parent;
		liveryObj.transform.parent = this._visuals.transform;
		liveryObj.transform.localPosition = Vector3.zero;
		liveryObj.transform.localRotation = Quaternion.identity;
		Vector3 right = this._visuals.transform.right;
		float w = -Vector3.Dot(right, this._visuals.transform.position);
		Vector4 plane = new Vector4(right.x, right.y, right.z, w);
		Matrix4x4 rhs = CarBaseTextureBaker.CalculateReflectionMatrix(plane);
		Color tint;
		bool overrideTint = this.GetOverrideTint(liveryObj, out tint);
		CarLiveryProjector[] componentsInChildren = liveryObj.GetComponentsInChildren<CarLiveryProjector>();
		List<CarBaseTextureBaker.LiveryProjectorData> list = new List<CarBaseTextureBaker.LiveryProjectorData>();
		CarLiveryProjector[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			CarLiveryProjector carLiveryProjector = array[i];
			if (!(carLiveryProjector.liveryTexture == null))
			{
				CarBaseTextureBaker.LiveryProjectorData liveryProjectorData = default(CarBaseTextureBaker.LiveryProjectorData);
				liveryProjectorData.Texture = carLiveryProjector.liveryTexture;
				liveryProjectorData.Tint = carLiveryProjector.liveryTint;
				if (overrideTint)
				{
					liveryProjectorData.Tint = tint;
				}
				if (liveryProjectorData.Tint.a == 0f)
				{
					liveryProjectorData.Tint.a = 1f;
				}
				Matrix4x4 rhs2 = this.CrossPlatformProjectionMatrix(carLiveryProjector.transform.worldToLocalMatrix);
				Matrix4x4 lhs = Matrix4x4.Ortho(-1f, 1f, -1f, 1f, 0f, -1f);
				liveryProjectorData.ViewProjection = lhs * rhs2;
				list.Add(liveryProjectorData);
				if (carLiveryProjector.enableSymmetryX)
				{
					CarBaseTextureBaker.LiveryProjectorData item = liveryProjectorData;
					item.ViewProjection = lhs * rhs2 * rhs;
					list.Add(item);
				}
			}
		}
		liveryObj.transform.parent = parent;
		return list;
	}

	private bool GetOverrideTint(GameObject liveryObj, out Color color)
	{
		if (liveryObj.name.ToLower().Contains("default"))
		{
			if (this._visuals.DefaultLiveryTints.Count == this._visuals.BaseColors.Count)
			{
				color = this._visuals.DefaultLiveryTints[this._visuals.GetCurrentColorIndex()];
				return true;
			}
			if (this._visuals.DefaultLiveryTints.Count > 0)
			{
			}
			color = Color.white;
			return false;
		}
		else
		{
			EliteVisuals component = liveryObj.GetComponent<EliteVisuals>();
			if (component != null)
			{
				color = component.EliteLiveryTints[this._visuals.GetCurrentColorIndex()];
				return true;
			}
			color = Color.white;
			return false;
		}
	}

	private static Matrix4x4 CalculateReflectionMatrix(Vector4 plane)
	{
		Matrix4x4 identity = Matrix4x4.identity;
		identity.m00 = 1f - 2f * plane[0] * plane[0];
		identity.m01 = -2f * plane[0] * plane[1];
		identity.m02 = -2f * plane[0] * plane[2];
		identity.m03 = -2f * plane[3] * plane[0];
		identity.m10 = -2f * plane[1] * plane[0];
		identity.m11 = 1f - 2f * plane[1] * plane[1];
		identity.m12 = -2f * plane[1] * plane[2];
		identity.m13 = -2f * plane[3] * plane[1];
		identity.m20 = -2f * plane[2] * plane[0];
		identity.m21 = -2f * plane[2] * plane[1];
		identity.m22 = 1f - 2f * plane[2] * plane[2];
		identity.m23 = -2f * plane[3] * plane[2];
		identity.m30 = 0f;
		identity.m31 = 0f;
		identity.m32 = 0f;
		identity.m33 = 1f;
		return identity;
	}

	private Matrix4x4 CrossPlatformProjectionMatrix(Matrix4x4 projMatrix)
	{
		string text = SystemInfo.graphicsDeviceVersion.ToLower();
		if (text.Contains("direct") || text.Contains("metal"))
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
}
