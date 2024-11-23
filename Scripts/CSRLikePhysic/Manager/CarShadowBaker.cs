using System;
using UnityEngine;

public class CarShadowBaker : MonoBehaviour
{
	public Camera Cam;

	public Renderer ShadowSurface;

	public Shader ShadowMappingShader;

	public int ResolutionX;

	public int ResolutionY;

	public Material TargetMaterial;

	public Renderer TargetRenderer;

	public Material[] PostProcesses;

	private RenderTexture RT1;

	private RenderTexture RT2;

	public RenderTexture ShadowTexture
	{
		get
		{
			return this.RT1;
		}
	}

	public void InitForRender()
	{
		int num = this.ResolutionX;
		int num2 = this.ResolutionY;
		if (BaseDevice.ActiveDevice.IsLowMemoryDevice())
		{
			num /= 2;
			num2 /= 2;
		}
		if (this.RT1 == null)
		{
			this.RT1 = new RenderTexture(num, num2, 16, RenderTextureFormat.ARGB32);
			this.RT1.Create();
		}
		if (this.RT2 == null)
		{
			this.RT2 = new RenderTexture(num, num2, 16, RenderTextureFormat.ARGB32);
			this.RT2.Create();
		}
		this.TargetMaterial = new Material(this.TargetMaterial);
		this.TargetRenderer.material = this.TargetMaterial;
	}

	public void RenderShadowMap(CarVisuals carVisuals)
	{
		this.InitForRender();
		int layer = carVisuals.gameObject.layer;
		carVisuals.SetLayer("CarShadowBake");
		this.Cam.targetTexture = this.RT1;
		this.Cam.RenderWithShader(this.ShadowMappingShader, null);
		this.Cam.targetTexture = null;
		carVisuals.SetLayer(layer);
		RenderTexture active = RenderTexture.active;
		Material[] postProcesses = this.PostProcesses;
		for (int i = 0; i < postProcesses.Length; i++)
		{
			Material material = postProcesses[i];
			RenderTexture.active = this.RT2;
			material.SetTexture("_MainTex", this.RT1);
			material.SetVector("_ScreenSize", new Vector4((float)this.ResolutionX, (float)this.ResolutionY));
			GL.Clear(true, true, Color.white);
			Graphics.Blit(this.RT1, this.RT2, material);
			this.RT2 = this.RT1;
			this.RT1 = RenderTexture.active;
		}
		this.TargetMaterial.SetTexture("_MainTex", this.RT1);
		this.RT2.Release();
		UnityEngine.Object.Destroy(this.RT2);
		this.RT2 = null;
		RenderTexture.active = active;
	}
}
