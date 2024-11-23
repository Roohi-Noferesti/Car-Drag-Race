using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainCharacterGraphic : MonoBehaviour
{
	public enum eState
	{
		Idle,
		AnimateRight,
		AnimateLeft,
		AnimateRightIn,
		AnimateLeftIn,
		AnimateTextFadeIn
	}

	private const float widthOfCorner = 0.16f;

	private const float heightOfCorner = 0.16f;

	private const float heightOfTop = 0.16f;

	private const float widthOfTop = 0.16f;

	private const float widthPadding = 0.03f;

	private const float heightOfName = 0.28f;

	private const float heightPadding = 0.31f;

    public RawImage LeaderPortrait;

	public Texture2D textureTopLeft;

	public Texture2D textureTop;

	public Texture2D textureLeft;

	public Texture2D textureBlack;

	public Texture2D textureStrikeFrame;

	public Texture2D textureGrey;

	private GameObject simpleSprite;

	public TextMeshProUGUI spriteText;

	private float animationX;

	private GameObject topLeft;

	private GameObject topRight;

	private GameObject bottomLeft;

	private GameObject bottomRight;

	private GameObject top;

	private GameObject left;

	private GameObject right;

	private GameObject bottom;

	private GameObject slideyTop;

	private GameObject slideyBottom;

	private GameObject slideyRight;

	private GameObject slideyTopRight;

	private GameObject slideyBottomRight;

	private GameObject slideyBlack;

	private GameObject nameBlack;

	private GameObject greyLine;

	private GameObject Portrait;

    public List<Image> StrikeFrames = new List<Image>(3);

	private float widthOfPortrait = 1f;

	private float heightOfPortrait = 1f;

	private float heightOfText = 1f;

	private float textOffsetY;

	private float TimeInState;

	private MainCharacterGraphic.eState State;

	private CrewProgressionScreen CrewScreen;

	private bool isRight;

	public void UnloadProtrait()
	{
		UnityEngine.Object.Destroy(this.Portrait);
		UnityEngine.Object.Destroy(this.topLeft);
		UnityEngine.Object.Destroy(this.topLeft);
		UnityEngine.Object.Destroy(this.topRight);
		UnityEngine.Object.Destroy(this.bottomLeft);
		UnityEngine.Object.Destroy(this.bottomRight);
		UnityEngine.Object.Destroy(this.top);
		UnityEngine.Object.Destroy(this.left);
		UnityEngine.Object.Destroy(this.right);
		UnityEngine.Object.Destroy(this.bottom);
		UnityEngine.Object.Destroy(this.slideyTop);
		UnityEngine.Object.Destroy(this.slideyBottom);
		UnityEngine.Object.Destroy(this.slideyRight);
		UnityEngine.Object.Destroy(this.slideyTopRight);
		UnityEngine.Object.Destroy(this.slideyBottomRight);
		UnityEngine.Object.Destroy(this.slideyBlack);
		UnityEngine.Object.Destroy(this.nameBlack);
		UnityEngine.Object.Destroy(this.greyLine);
	}

	public void LoadProtrait(Texture2D zPortrait, CrewProgressionScreen zScreen)
	{
        //this.simpleSprite = (Resources.Load("CharacterCards/Crew/CrewProgressionScreenSprite") as GameObject);
		this.CrewScreen = zScreen;
		this.SetState(MainCharacterGraphic.eState.Idle);
		this.animationX = 0f;
        //this.SetTextAlpha(0f);
		this.isRight = true;
		float num = 200f;
        //GameObject gameObject = new GameObject("Center");
        //gameObject.transform.parent = base.transform;
        //gameObject.transform.localPosition = new Vector3(-(((float)zPortrait.width + 0.03f) * 0.5f) / num, ((float)zPortrait.height + 0.62f) * 0.5f / num, 0f);
        //gameObject.transform.localPosition += new Vector3(0f, 0.16f, 0f);
        //GameObjectHelper.MakeLocalPositionPixelPerfect(gameObject.transform);
        //this.spriteText.transform.parent = gameObject.transform;
		this.Portrait = this.AddSprite(zPortrait, (float)zPortrait.width / num, (float)zPortrait.height / num, "Portrait", gameObject);
        //this.Portrait.transform.localPosition = new Vector3(0.015f, -0.0150000006f, 0f);
        //this.widthOfPortrait = (float)zPortrait.width / num;
        //this.heightOfPortrait = (float)zPortrait.height / num;
        //this.widthOfPortrait -= 0.32f;
        //this.heightOfPortrait -= 0.32f;
        //this.widthOfPortrait += 0.03f;
        //this.heightOfPortrait += 0.31f;
        //this.topLeft = this.AddSprite(this.textureTopLeft, 0.16f, 0.16f, "topLeft", gameObject);
        //this.topRight = this.AddSprite(this.textureTopLeft, -0.16f, 0.16f, "topRight", gameObject);
        //this.bottomLeft = this.AddSprite(this.textureTopLeft, 0.16f, -0.16f, "bottomLeft", gameObject);
        //this.bottomRight = this.AddSprite(this.textureTopLeft, -0.16f, -0.16f, "bottomRight", gameObject);
        //this.top = this.AddSprite(this.textureTop, this.widthOfPortrait, 0.16f, "top", gameObject);
        //this.left = this.AddSprite(this.textureLeft, 0.16f, this.heightOfPortrait, "left", gameObject);
        //this.right = this.AddSprite(this.textureLeft, -0.16f, this.heightOfPortrait, "right", gameObject);
        //this.bottom = this.AddSprite(this.textureTop, this.widthOfPortrait, -0.16f, "bottom", gameObject);
        //this.nameBlack = this.AddSprite(this.textureBlack, this.widthOfPortrait + 0.04f, -0.16f, "nameBlack", gameObject);
        //this.greyLine = this.AddSprite(this.textureGrey, this.widthOfPortrait + 0.32f - 0.02f, 0.02f, "greyLine", gameObject);
        //GameObject gameObject2 = new GameObject("slidey");
        //gameObject2.transform.parent = gameObject.transform;
        //gameObject2.transform.localPosition = new Vector3(0f, 0f, 0.1f);
        //this.slideyTop = this.AddSprite(this.textureTop, 0.16f, 0.16f, "slideyTop", gameObject2);
        //this.slideyBottom = this.AddSprite(this.textureTop, 0.16f, -0.16f, "slideyBottom", gameObject2);
        //this.slideyRight = this.AddSprite(this.textureLeft, -0.16f, 0.16f, "slideyRight", gameObject2);
        //this.slideyTopRight = this.AddSprite(this.textureTopLeft, -0.16f, 0.16f, "slideyTopRight", gameObject2);
        //this.slideyBottomRight = this.AddSprite(this.textureTopLeft, -0.16f, -0.16f, "slideyBottomRight", gameObject2);
        //this.slideyBlack = this.AddSprite(this.textureBlack, 0.16f, -0.16f, "slideyBlack", gameObject2);
        //float num2 = this.GetWidth() * 0.5f;
        for (int i = 0; i < 3 && i<StrikeFrames.Count; i++)
        {
            GameObject gameObject3 = this.StrikeFrames[i].gameObject;//this.AddSprite(this.textureStrikeFrame, 0.32f, 0.32f, "strikeframe", gameObject);
            //Image component = gameObject3.GetComponent<Image>();
            //component.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
            //gameObject3.transform.localPosition = new Vector3(0.4f * (float)(i - 1) + num2, -1.54f, -0.1f);
            gameObject3.SetActive(false);
            gameObject3.transform.GetChild(0).gameObject.SetActive(false);
            //this.StrikeFrames.Add(component);
        }
        //base.transform.parent.position = GameObjectHelper.MakeLocalPositionPixelPerfect(base.transform.parent.position);
        //this.SetTextBorderAlpha(0f);
        //this.SetPositions();
	}

	public GameObject GetPortrait()
	{
		return this.Portrait;
	}

	public GameObject GetStrikeFrame(int zIndex)
	{
		return this.StrikeFrames[zIndex].gameObject;
	}

	public float GetWidth()
	{
		return this.widthOfPortrait + 0.32f;
	}

	public void SetStrikeFramesActive()
	{
        foreach (Image current in this.StrikeFrames)
		{
			current.gameObject.SetActive(true);
		}
	}

	public void SetPortraitAlpha(float zAlpha)
	{
		Color color = new Color(1f, 1f, 1f, zAlpha);
	    this.Portrait.GetComponent<Image>().color = color;
	}

	public void SetAlpha(float zAlpha)
	{
		//Color color = new Color(1f, 1f, 1f, 0f);
		//color.a = Mathf.Clamp(zAlpha, 0f, 1f);
		//this.topLeft.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.topRight.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.bottomLeft.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.bottomRight.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.top.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.left.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.right.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.bottom.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.nameBlack.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.greyLine.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		//this.SetPortraitAlpha(Mathf.Clamp(zAlpha, 0f, 1f));
	}

	public void SetTextBorderAlpha(float zAlpha)
	{
		Color color = new Color(1f, 1f, 1f, zAlpha);
		this.slideyTop.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		this.slideyBottom.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		this.slideyRight.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		this.slideyTopRight.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		this.slideyBottomRight.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
		this.slideyBlack.GetComponent<Renderer>().sharedMaterial.SetColor("_Tint", color);
	}

	public void SetTextAlpha(float zAlpha)
	{
		Color color = new Color(1f, 1f, 1f, zAlpha);
	    this.spriteText.color = color;
	}

	public void ShowText(string zText, float zYOffset, bool zIsRight)
	{
		this.spriteText.text = zText;
		this.textOffsetY = zYOffset;
        //this.heightOfText = (float)this.spriteText.GetDisplayLineCount() * this.spriteText.LineSpan - 0.16f - 0.02f;
        //this.heightOfText = GameObjectHelper.MakeLocalPositionPixelPerfect(this.heightOfText);
		if (zIsRight)
		{
			this.SetState(MainCharacterGraphic.eState.AnimateRight);
		}
		else
		{
			this.SetState(MainCharacterGraphic.eState.AnimateLeft);
		}
	}

	private GameObject AddSprite(Texture2D zTexture, float zWidth, float zHeight, string zName, GameObject zParent)
	{
	    LeaderPortrait.texture = zTexture;
	    return LeaderPortrait.gameObject;
        //GameObject gameObject;
        //if (zName == "Portrait")
        //{
        //    gameObject = (UnityEngine.Object.Instantiate(Resources.Load("CharacterCards/Crew/LeaderSprite")) as GameObject);
        //}
        //else
        //{
        //    gameObject = (UnityEngine.Object.Instantiate(this.simpleSprite) as GameObject);
        //}
        //gameObject.name = zName;
        //gameObject.transform.parent = zParent.transform;
        //Image component = gameObject.GetComponent<Image>();
        //float num = (zWidth >= 0f) ? 1f : -1f;
        //float num2 = (zHeight >= 0f) ? 1f : -1f;
        //component.renderer.material.SetTexture("_MainTex", zTexture);
        //component.Setup(Mathf.Abs(zWidth), Mathf.Abs(zHeight), Vector2.zero, new Vector2((float)zTexture.width * num, (float)zTexture.height * num2));
        //component.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_LEFT);
        //int x = (zWidth >= 0f) ? 0 : (zTexture.width - 1);
        //int y = (zHeight >= 0f) ? (zTexture.height - 1) : 0;
        //component.SetLowerLeftPixel(x, y);
		//return gameObject;
	}

	private void SetPositions()
	{
		this.topLeft.transform.localPosition = new Vector3(0f, 0f, 0.1f);
		this.top.transform.localPosition = this.topLeft.transform.localPosition + new Vector3(0.16f, 0f, 0f);
		this.topRight.transform.localPosition = this.top.transform.localPosition + new Vector3(this.widthOfPortrait, 0f, 0f);
		this.left.transform.localPosition = this.topLeft.transform.localPosition + new Vector3(0f, -0.16f, 0f);
		this.right.transform.localPosition = this.topRight.transform.localPosition + new Vector3(0f, -0.16f, 0f);
		this.bottomLeft.transform.localPosition = this.left.transform.localPosition + new Vector3(0f, -this.heightOfPortrait, 0f);
		this.bottom.transform.localPosition = this.bottomLeft.transform.localPosition + new Vector3(0.16f, 0f, 0f);
		this.bottomRight.transform.localPosition = this.bottom.transform.localPosition + new Vector3(this.widthOfPortrait, 0f, 0f);
		this.nameBlack.transform.localPosition = this.bottom.transform.localPosition + new Vector3(-0.02f, 0.16f, -0.02f);
		this.greyLine.transform.localPosition = this.bottomLeft.transform.localPosition + new Vector3(0f, 0.14f, -0.12f);
		if (this.isRight)
		{
			this.slideyTop.transform.localPosition = this.topRight.transform.localPosition + new Vector3(0.16f, -this.textOffsetY, 0f);
			this.slideyBottom.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(0f, -this.heightOfText - 0.16f, 0f);
			this.slideyRight.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(this.animationX, -0.16f, 0f);
			this.slideyTopRight.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(this.animationX, 0f, 0f);
			this.slideyBottomRight.transform.localPosition = this.slideyBottom.transform.localPosition + new Vector3(this.animationX, 0f, 0f);
			this.slideyBlack.transform.localPosition = this.slideyTop.transform.localPosition;
			this.spriteText.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(0.07f, -0.08f, -0.2f);
			this.slideyTop.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(-0.1f, 0f, 0f);
            //this.slideyTop.GetComponent<global::Sprite>().SetSize(this.animationX + 0.1f, 0.16f);
            //this.slideyBottom.GetComponent<global::Sprite>().SetSize(this.animationX, 0.16f);
            //this.slideyRight.GetComponent<global::Sprite>().SetSize(0.16f, this.heightOfText);
            //this.slideyTopRight.GetComponent<global::Sprite>().SetSize(0.16f, 0.16f);
            //this.slideyBottomRight.GetComponent<global::Sprite>().SetSize(0.16f, 0.16f);
            //this.slideyBlack.GetComponent<global::Sprite>().SetSize(this.animationX, this.heightOfText + 0.16f);
		}
		else
		{
			this.slideyTop.transform.localPosition = this.topLeft.transform.localPosition + new Vector3(0f, -this.textOffsetY, 0f);
			this.slideyBottom.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(0f, -this.heightOfText - 0.16f, 0f);
			this.slideyRight.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(this.animationX, -0.16f, 0f);
			this.slideyTopRight.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(this.animationX, 0f, 0f);
			this.slideyBottomRight.transform.localPosition = this.slideyBottom.transform.localPosition + new Vector3(this.animationX, 0f, 0f);
			this.slideyBlack.transform.localPosition = this.slideyTop.transform.localPosition;
			this.spriteText.transform.localPosition = this.slideyTopRight.transform.localPosition + new Vector3(-0.06f, -0.08f, -0.2f);
			this.slideyTop.transform.localPosition = this.slideyTop.transform.localPosition + new Vector3(0.1f, 0f, 0f);
            //this.slideyTop.GetComponent<global::Sprite>().SetSize(this.animationX - 0.1f, 0.16f);
            //this.slideyBottom.GetComponent<global::Sprite>().SetSize(this.animationX, 0.16f);
            //this.slideyRight.GetComponent<global::Sprite>().SetSize(-0.16f, this.heightOfText);
            //this.slideyTopRight.GetComponent<global::Sprite>().SetSize(-0.16f, 0.16f);
            //this.slideyBottomRight.GetComponent<global::Sprite>().SetSize(-0.16f, 0.16f);
            //this.slideyBlack.GetComponent<global::Sprite>().SetSize(this.animationX, this.heightOfText + 0.16f);
		}
	}

	public void SetState(MainCharacterGraphic.eState zState)
	{
		this.State = zState;
		this.TimeInState = 0f;
		if (this.State == MainCharacterGraphic.eState.AnimateRight)
		{
			this.isRight = true;
		}
		else if (this.State == MainCharacterGraphic.eState.AnimateLeft)
		{
			this.isRight = false;
		}
	}

	public bool IsIdle()
	{
		return this.State == MainCharacterGraphic.eState.Idle;
	}

	private void Update()
	{
		this.TimeInState += Time.deltaTime;
		switch (this.State)
		{
		case MainCharacterGraphic.eState.Idle:
			return;
		case MainCharacterGraphic.eState.AnimateRight:
		{
			float num = this.TimeInState / 0.3f;
			this.SetTextBorderAlpha(1f);
			bool flag = num >= 1f;
			num = Mathf.Clamp(num, 0f, 1f);
			this.animationX = this.CrewScreen.CurveS.Evaluate(num) * 1.4f;
			if (flag)
			{
				this.SetState(MainCharacterGraphic.eState.AnimateTextFadeIn);
			}
			break;
		}
		case MainCharacterGraphic.eState.AnimateLeft:
		{
			float num2 = this.TimeInState / 0.3f;
			this.SetTextBorderAlpha(1f);
			bool flag2 = num2 >= 1f;
			num2 = Mathf.Clamp(num2, 0f, 1f);
			float num3 = this.CrewScreen.CurveS.Evaluate(num2);
			this.animationX = ((1f - num3) * 0f + num3 * 1.4f) * -1f;
			if (flag2)
			{
				this.SetState(MainCharacterGraphic.eState.AnimateTextFadeIn);
			}
			break;
		}
		case MainCharacterGraphic.eState.AnimateRightIn:
		{
			float num4 = this.TimeInState / 0.3f;
			bool flag3 = num4 >= 1f;
			num4 = Mathf.Clamp(num4, 0f, 1f);
			float num5 = this.CrewScreen.CurveS.Evaluate(num4);
			this.animationX = (1f - num5) * 1.4f;
			if (flag3)
			{
				this.SetState(MainCharacterGraphic.eState.Idle);
			}
			break;
		}
		case MainCharacterGraphic.eState.AnimateLeftIn:
		{
			float num6 = this.TimeInState / 0.3f;
			bool flag4 = num6 >= 1f;
			num6 = Mathf.Clamp(num6, 0f, 1f);
			float num7 = this.CrewScreen.CurveS.Evaluate(num6);
			this.animationX = (1f - num7) * 1.4f * -1f;
			if (flag4)
			{
				this.SetState(MainCharacterGraphic.eState.Idle);
			}
			break;
		}
		case MainCharacterGraphic.eState.AnimateTextFadeIn:
		{
			float num8 = this.TimeInState / 0.3f;
			bool flag5 = num8 >= 1f;
			num8 = Mathf.Clamp(num8, 0f, 1f);
			this.SetTextAlpha(this.CrewScreen.CurveS.Evaluate(num8));
			if (flag5)
			{
				this.SetState(MainCharacterGraphic.eState.Idle);
			}
			break;
		}
		}
        //this.SetPositions();
	}
}
