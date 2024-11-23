using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DataDrivenPortrait : MonoBehaviour
{
	private enum State
	{
		Default,
		FadeIn
	}

	public RawImage Image;

	public TextMeshProUGUI Text;

	public bool AlwaysFade;

	public float FadeInTime = 0.5f;

	public float PixelScale = 0.005f;

	public bool IgnorePixelScale;

	private float Timer;

	private DataDrivenPortrait.State CurrentState;

	//private AutoTranslateEZSpriteText _autoTranslateTextComponent;

	public bool ImageLoaded
	{
		get;
		set;
	}

	private void Start()
	{
		//this._autoTranslateTextComponent = null;
		//if (this.Text != null)
		//{
		//	this._autoTranslateTextComponent = this.Text.gameObject.GetComponent<AutoTranslateEZSpriteText>();
		//}
	}

	public void Init(string texturePath, string label, Action callBack = null)
	{
		if (BuildType.IsAppTuttiBuild && texturePath == "WorldTourBackground.Italia" && !texturePath.Contains("apptutti"))
			texturePath += "_apptutti";
		this.Image.gameObject.SetActive(false);
		bool loadedAsync = false;
		this.ImageLoaded = false;
		TexturePack.RequestTextureFromBundle(texturePath, delegate(Texture2D texture)
		{
		    this.Image.texture = texture;
            //this.Image.SetTexture(texture);
			Vector2 lowerleftPixel = new Vector2(0f, (float)texture.height - 1f);
			Vector2 pixeldimensions = new Vector2((float)texture.width, (float)texture.height);
			if (this.IgnorePixelScale)
			{
				//this.Image.Setup(this.Image.width, this.Image.height, lowerleftPixel, pixeldimensions);
			}
			else
			{
				//this.Image.Setup((float)texture.width * this.PixelScale, (float)texture.height * this.PixelScale, lowerleftPixel, pixeldimensions);
			}
			this.Image.gameObject.SetActive(true);
			this.ImageLoaded = true;
			if (this.Text != null)
			{
                if (label.StartsWith("TEXT"))
                {
                    label = LocalizationManager.GetTranslation(label);
                }
				this.Text.text = label;
				//if (this._autoTranslateTextComponent != null)
				//{
				//	this._autoTranslateTextComponent.Refresh();
				//}
			}
			if (loadedAsync || this.AlwaysFade)
			{
				this.CurrentState = DataDrivenPortrait.State.FadeIn;
				//this.Image.color = (Color.clear);
				if (this.Text != null)
				{
					this.Text.color = (Color.clear);
				}
			}
			else
			{
				this.Image.color = (Color.white);
				if (this.Text != null)
				{
					this.Text.color = (Color.white);
				}
			}
			if (callBack != null)
			{
				callBack();
			}
		});
		loadedAsync = true;
	}

	private void Update()
	{
		if (this.CurrentState == DataDrivenPortrait.State.FadeIn)
		{
			this.Timer += Time.deltaTime;
			Color white = Color.white;
			white.a = Mathf.Clamp01(this.Timer / this.FadeInTime);
			this.Image.color = (white);
			if (this.Text != null)
			{
				this.Text.color = (white);
			}
			if (this.Timer >= this.FadeInTime)
			{
				this.CurrentState = DataDrivenPortrait.State.Default;
				this.Timer = 0f;
			}
		}
	}
}
