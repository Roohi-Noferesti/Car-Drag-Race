using System;
using TMPro;
using UnityEngine;

public class NumberPlateManager : MonoBehaviour
{
	public delegate void NumberPlateRenderedDelegate(Texture2D texture);

	private const int _pixelWidth = 128;

	private const int _pixelHeight = 64;

	private static NumberPlateManager _instance;

	public Material TextMaterial;

	public TextAsset TextFont;

	public Shader BackgroundShader;

	public Texture2D BorderTexture;

	public Shader BorderShader;

	public Texture2D DefaultNumberPlateTexture;

	private Camera _camera;

	private Material _bgMaterial;

	private Material _borderMaterial;

	private TextMeshPro _spriteText;

	private string _lastRenderedPlayerNumPlateStr = string.Empty;

	public static NumberPlateManager Instance
	{
		get
		{
			return NumberPlateManager._instance;
		}
	}

	public Texture2D PlayerNumPlateTexture
	{
		get;
		private set;
	}

	public Texture2D OpponentNumPlateTexture
	{
		get;
		private set;
	}

	private void Awake()
	{
		NumberPlateManager._instance = this;
		this._camera = base.transform.Find("NumberPlate Camera").GetComponent<Camera>();
		GameObject gameObject = new GameObject("DEFAULT");
		gameObject.layer = LayerMask.NameToLayer("NumberPlate");
		gameObject.transform.parent = base.transform;
		this._spriteText = gameObject.AddComponent<TextMeshPro>();
		this._spriteText.text = gameObject.name;
        //this._spriteText.pixelPerfect = false;
		this._spriteText.characterSpacing = 0.66f;
        //this._spriteText.Anchor = SpriteText.Anchor_Pos.Middle_Center;
        //this._spriteText.SetAlignment(SpriteText.Alignment_Type.Center);
        //this._spriteText.SetFont(this.TextFont, this.TextMaterial);
        //this._spriteText.SetColor(Color.white);
		this._bgMaterial = new Material(this.BackgroundShader);
		this._borderMaterial = new Material(this.BorderShader);
		this._borderMaterial.SetTexture("_MainTex", this.BorderTexture);
	}

	public void RenderPlayerNumberPlate()
	{
		this.RenderPlayerNumberPlate(delegate(Texture2D texture)
		{
		});
	}

	public void RenderPlayerNumberPlate(NumberPlateManager.NumberPlateRenderedDelegate callback)
	{
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		NumberPlate playerNumPlate = activeProfile.GetCurrentCar().NumberPlate;
		if (playerNumPlate == null)
		{
			playerNumPlate = new NumberPlate();
			activeProfile.GetCurrentCar().NumberPlate = playerNumPlate;
		}
		if (playerNumPlate.Text.ToLower() == this._lastRenderedPlayerNumPlateStr.ToLower() && this.PlayerNumPlateTexture != null)
		{
			if (callback != null)
			{
				callback(this.PlayerNumPlateTexture);
			}
			return;
		}
		this._lastRenderedPlayerNumPlateStr = playerNumPlate.Text;
		CameraPostRender.Instance.AddProcess("player number plate " + playerNumPlate.Text, delegate
		{
			if (this.PlayerNumPlateTexture != null)
			{
				UnityEngine.Object.Destroy(this.PlayerNumPlateTexture);
			}
			this.PlayerNumPlateTexture = this.RenderImmediate(playerNumPlate);
			if (callback != null)
			{
				callback(this.PlayerNumPlateTexture);
			}
		});
	}

	public void RenderOpponentNumberPlate(NumberPlate enemyPlate, NumberPlateManager.NumberPlateRenderedDelegate callback)
	{
		CameraPostRender.Instance.AddProcess("opponent number plate " + enemyPlate.Text, delegate
		{
			this.DestroyOpponentNumberPlate();
			this.OpponentNumPlateTexture = this.RenderImmediate(enemyPlate);
			if (callback != null)
			{
				callback(this.OpponentNumPlateTexture);
			}
		});
	}

	public void DestroyOpponentNumberPlate()
	{
		if (this.OpponentNumPlateTexture != null)
		{
			UnityEngine.Object.Destroy(this.OpponentNumPlateTexture);
			this.OpponentNumPlateTexture = null;
		}
	}

	public void RenderOnPostSceneRender(NumberPlate plate, NumberPlateManager.NumberPlateRenderedDelegate callback)
	{
		CameraPostRender.Instance.AddProcess("render number plate: " + plate.Text, delegate
		{
			NumberPlateManager.Instance.RenderImmediate(plate, delegate(Texture2D newPlateTexture)
			{
				if (callback != null)
				{
					callback(newPlateTexture);
				}
			});
		});
	}

	public void RenderImmediate(NumberPlate plate, NumberPlateManager.NumberPlateRenderedDelegate callback)
	{
		Texture2D texture = this.RenderImmediate(plate);
		if (callback != null)
		{
			callback(texture);
		}
	}

	public Texture2D RenderImmediate(NumberPlate plate)
	{
		RenderTexture active = RenderTexture.active;
		RenderTexture renderTexture = new RenderTexture(128, 64, 0, RenderTextureFormat.ARGB32);
		BasePlatform.ActivePlatform.PrepareRenderTexture(renderTexture);
		this._bgMaterial.SetColor("_Color", plate.BackgroundColor);
		Graphics.Blit(null, renderTexture, this._bgMaterial);
		this._borderMaterial.SetColor("_Tint", plate.BorderColor);
		Graphics.Blit(this.BorderTexture, renderTexture, this._borderMaterial);
		this._camera.targetTexture = renderTexture;
		this._spriteText.renderer.enabled = true;
		this._spriteText.gameObject.name = plate.Text;
		this._spriteText.text = plate.Text;
        //this._spriteText.SetCamera(this._camera);
        //this._spriteText.SetColor(plate.TextColor);
        //this._spriteText.UpdateMesh();
		this._camera.Render();
		Texture2D texture2D = new Texture2D(renderTexture.width, renderTexture.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)renderTexture.width, (float)renderTexture.height), 0, 0);
		texture2D.Apply(false, true);
		RenderTexture.active = active;
		this._camera.targetTexture = null;
		UnityEngine.Object.Destroy(renderTexture);
		this._spriteText.renderer.enabled = false;
		return texture2D;
	}
}
