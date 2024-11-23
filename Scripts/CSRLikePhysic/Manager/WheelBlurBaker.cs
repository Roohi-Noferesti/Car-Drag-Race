using System;
using UnityEngine;

public class WheelBlurBaker
{
	private enum LinearBlurShaderPass
	{
		Copy,
		Blur,
		Blit
	}

	private static Shader RadialBlurShader;

	private static Shader LinearBlurShader;

	private RenderTexture RT1;

	private RenderTexture RT2;

	private int RTWidth;

	private int RTHeight;

	static WheelBlurBaker()
	{
		WheelBlurBaker.RadialBlurShader = Shader.Find("CSR/WheelRadialBlur");
		WheelBlurBaker.LinearBlurShader = Shader.Find("CSR/WheelLinearBlur");
	}

	public void InitForRender(int Width, int Height)
	{
		bool flag = Width != this.RTWidth || Height != this.RTHeight;
		if (flag)
		{
			this.RTWidth = Width;
			this.RTHeight = Height;
		}
		if (BaseDevice.ActiveDevice.IsLowMemoryDevice())
		{
			this.RTWidth /= 2;
			this.RTHeight /= 2;
		}
		if (flag || this.RT1 == null)
		{
			this.RT1 = new RenderTexture(this.RTWidth, this.RTHeight, 0, RenderTextureFormat.ARGB32);
			this.RT1.Create();
		}
		if (flag || this.RT2 == null)
		{
			this.RT2 = new RenderTexture(this.RTWidth, this.RTHeight, 0, RenderTextureFormat.ARGB32);
			this.RT2.Create();
		}
		this.RT1.DiscardContents();
		this.RT2.DiscardContents();
	}

	private void FreeRenderTexture(RenderTexture rt)
	{
		if (rt == null)
		{
			return;
		}
		rt.Release();
		UnityEngine.Object.Destroy(rt);
	}

	private void FreeRenderTextures()
	{
		this.FreeRenderTexture(this.RT1);
		this.FreeRenderTexture(this.RT2);
		this.RT1 = null;
		this.RT2 = null;
	}

	public void DoLinearBlur(Texture Source, RenderTexture Destination, LinearBlurSettings BlurSettings)
	{
		Material material = new Material(WheelBlurBaker.LinearBlurShader);
		float num = (float)BlurSettings.BlurPasses;
		material.SetFloat("_BlurLength", BlurSettings.Length);
		material.SetFloat("_RectLeft", BlurSettings.RectPosition.x);
		material.SetFloat("_RectBottom", BlurSettings.RectPosition.y);
		material.SetFloat("_RectWidth", BlurSettings.RectSize.x);
		material.SetFloat("_RectHeight", BlurSettings.RectSize.y);
		float num2 = 0f;
		float num3 = BlurSettings.Length / 16f;
		float num4 = num3 / num;
		RenderTexture.active = this.RT1;
		this.RT1.DiscardContents();
		material.SetTexture("_MainTex", Source);
		Graphics.Blit(Source, this.RT1, material, 0);
		this.RT1.wrapMode = TextureWrapMode.Repeat;
		this.RT2.wrapMode = TextureWrapMode.Repeat;
		int i = 0;
		while (i < BlurSettings.BlurPasses)
		{
			this.RT2.DiscardContents();
			RenderTexture.active = this.RT2;
			material.SetTexture("_MainTex", this.RT1);
			material.SetFloat("_BlurOffset", num2);
			Graphics.Blit(this.RT1, this.RT2, material, 1);
			this.RT2 = this.RT1;
			this.RT1 = RenderTexture.active;
			i++;
			num2 += num4;
		}
		Destination.DiscardContents();
		Graphics.Blit(Source, Destination);
		material.SetTexture("_MainTex", this.RT1);
		material.SetTexture("_Background", Destination);
		RenderTexture.active = Destination;
		Graphics.Blit(this.RT1, Destination, material, 2);
		UnityEngine.Object.Destroy(material);
	}

	public void DoRadialBlur(Texture Source, RenderTexture Destination, RadialBlurSettings BlurSettings)
	{
		Material material = new Material(WheelBlurBaker.RadialBlurShader);
		float num = 0.0174532924f * BlurSettings.Angle / (float)BlurSettings.BlurPasses;
		material.SetFloat("_OriginX", BlurSettings.Position.x);
		material.SetFloat("_OriginY", BlurSettings.Position.y);
		material.SetFloat("_RadiusSquared", BlurSettings.Radius * BlurSettings.Radius);
		material.SetFloat("_Theta", num);
		float f = num / 8f;
		material.SetFloat("_SinGamma", Mathf.Sin(f));
		material.SetFloat("_CosGamma", Mathf.Cos(f));
		float num2 = 0f;
		float num3 = 0.7853982f;
		float num4 = num3 / (float)BlurSettings.BlurPasses;
		int i = 0;
		while (i < BlurSettings.BlurPasses)
		{
			Texture texture = (i != 0) ? this.RT1 : Source;
			this.RT2.DiscardContents();
			RenderTexture.active = this.RT2;
			material.SetTexture("_MainTex", texture);
			float f2 = num2 - 0.5f * num;
			material.SetFloat("_SinPsi", Mathf.Sin(f2));
			material.SetFloat("_CosPsi", Mathf.Cos(f2));
			Graphics.Blit(texture, this.RT2, material);
			this.RT2 = this.RT1;
			this.RT1 = RenderTexture.active;
			i++;
			num2 += num4;
		}
		Graphics.Blit(this.RT1, Destination);
		UnityEngine.Object.Destroy(material);
	}

	public void BakeBlurs(CarVisuals Visuals, CarWheelBlurSettings BlurSettings)
	{
		RenderTexture active = RenderTexture.active;
		foreach (Material current in Visuals.WheelMaterials)
		{
			string name = current.name;
			Texture2D source = Visuals.BaseWheelTextures[name];
			RenderTexture renderTexture = Visuals.BlurredWheelTextures[name];
			renderTexture.DiscardContents();
			this.InitForRender(renderTexture.width, renderTexture.height);
			this.DoRadialBlur(source, renderTexture, BlurSettings.RadialBlur1);
			if (BlurSettings.RadialBlur2.Enabled)
			{
				this.DoRadialBlur(renderTexture, renderTexture, BlurSettings.RadialBlur2);
			}
			current.SetTexture("_BlurTex", renderTexture);
			this.FreeRenderTextures();
		}
		RenderTexture.active = active;
	}
}
