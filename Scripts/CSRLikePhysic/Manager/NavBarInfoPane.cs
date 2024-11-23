using System;
using System.Collections.Generic;
using System.Diagnostics;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NavBarInfoPane : MonoBehaviour, IPersistentUI
{
	private enum FlipCase
	{
		NONE,
		XAXIS,
		YAXIS,
		BOTH
	}

	private enum AnimState
	{
		BEFORE,
		APPEAR,
		DISPLAY,
		HIDE,
		FINISHED
	}

	private enum DisplayMode
	{
		SecondCountDown,
		ResidentOfScreen
	}

	private const float AnimDuration = 0.2f;

	private const float YPosOffset = 0.07f;

	public Image TopLeft;

	public Image Top;

	public Image Left;

	public Image Middle;

	public Image Nipple;

	public TextMeshProUGUI Text;

	private Vector3 savedTextLocalPosition;

	public float height;

	public float width;

	private Image TopRight;

	private Image Right;

	private Image BottomLeft;

	private Image Bottom;

	private Image BottomRight;

	public RuntimeButton uiButton;

	private float SecondsToRemain;

	private NavBarInfoPane.DisplayMode TimoutMode = NavBarInfoPane.DisplayMode.ResidentOfScreen;

	private ScreenID ScreenOfResidence;

	private NavBarInfoPane.AnimState CurrentState;

	private Action PressAction;

	public Material WindowMaterial;

	public AnimationCurve AnimCurve;

	private GameObject CombinedWindow;

	private float AnimTimePos;

	private Vector3 FinalPosition;

	private bool IsShowing;

	public bool BeenCreated
	{
		get;
		private set;
	}

	private void OnDestroy()
	{
		this.TopLeft = null;
		this.Top = null;
		this.Left = null;
		this.Middle = null;
		this.Nipple = null;
		this.TopRight = null;
		this.Right = null;
		this.BottomLeft = null;
		this.Bottom = null;
		this.BottomRight = null;
		this.AnimCurve = null;
	}

	public void Start()
	{
		this.BeenCreated = true;
	}

	public void Awake()
	{
		this.CreateParts();
		this.savedTextLocalPosition = this.Text.gameObject.transform.localPosition;
	}

	private void Combine(bool leaveNipple = false)
	{
		List<GameObject> list = new List<GameObject>
		{
			this.TopLeft.gameObject,
			this.Top.gameObject,
			this.Left.gameObject,
			this.Middle.gameObject,
			this.TopRight.gameObject,
			this.Right.gameObject,
			this.BottomLeft.gameObject,
			this.Bottom.gameObject,
			this.BottomRight.gameObject
		};
		if (!leaveNipple)
		{
			list.Add(this.Nipple.gameObject);
		}
        //this.CombinedWindow = MeshCombiner.CombineThenDestroy(list, this.WindowMaterial, "GUI", base.transform);
		this.BeenCreated = true;
	}

	public void SetForThisScreen(string str, bool alreadyLocalised, float XPos, ScreenID screenhome, Action button, bool separateNipple)
	{
		this.SetBase(str, alreadyLocalised, XPos, button, separateNipple);
		this.ScreenOfResidence = screenhome;
		if (ScreenManager.Instance.CurrentScreen == screenhome)
		{
			this.CurrentState = NavBarInfoPane.AnimState.APPEAR;
		}
	}

	public void SetForTime(string str, bool alreadyLocalised, float XPos, float SecondsOnScreen, Action button, bool separateNipple)
	{
		this.SetBase(str, alreadyLocalised, XPos, button, separateNipple);
		this.TimoutMode = NavBarInfoPane.DisplayMode.SecondCountDown;
		this.SecondsToRemain = SecondsOnScreen;
		this.CurrentState = NavBarInfoPane.AnimState.APPEAR;
	}

	public void Create(bool separateNipple = false)
	{
		this.PositionStuffForCurrentSize();
		this.CurrentState = NavBarInfoPane.AnimState.DISPLAY;
		this.Combine(separateNipple);
		this.SetForFade(1f);
	}

	private void SetBase(string str, bool alreadyLocalised, float XPos, Action button, bool separateNipple)
	{
		this.Text.text = ((!alreadyLocalised) ? LocalizationManager.GetTranslation(str) : str);
        //this.width = GameObjectHelper.MakeLocalPositionPixelPerfect(this.Text.TotalWidth + 0.25f);
        //this.height = GameObjectHelper.MakeLocalPositionPixelPerfect((this.Text.BaseHeight + 0.05f) * (float)this.Text.GetDisplayLineCount() + 0.2f);
		if (button != null)
		{
            //this.uiButton.SetSize(this.width, this.height);
			this.PressAction = button;
		}
		this.PositionStuffForCurrentSize();
		Vector3 localPosition = base.gameObject.transform.localPosition;
		localPosition.x = XPos;
		base.gameObject.transform.localPosition = localPosition;
		this.FinalPosition = localPosition;
		this.CurrentState = NavBarInfoPane.AnimState.BEFORE;
		this.Combine(separateNipple);
		this.SetForFade(0f);
	}

	private Image CopyImage(Image spr, NavBarInfoPane.FlipCase flip)
	{
		Image Image = (Image)UnityEngine.Object.Instantiate(spr);
		bool flag = flip == NavBarInfoPane.FlipCase.XAXIS || flip == NavBarInfoPane.FlipCase.BOTH;
		bool flag2 = flip == NavBarInfoPane.FlipCase.YAXIS || flip == NavBarInfoPane.FlipCase.BOTH;
		Image.transform.parent = base.transform;
		if (flag)
		{
            //Image.width = -Image.width;
		}
		if (flag2)
		{
            //Image.height = -Image.height;
		}
		return Image;
	}

	private void CreateParts()
	{
        //this.TopRight = this.CopyImage(this.TopLeft, NavBarInfoPane.FlipCase.XAXIS);
        //this.BottomLeft = this.CopyImage(this.TopLeft, NavBarInfoPane.FlipCase.YAXIS);
        //this.BottomRight = this.CopyImage(this.TopLeft, NavBarInfoPane.FlipCase.BOTH);
        //this.Right = this.CopyImage(this.Left, NavBarInfoPane.FlipCase.XAXIS);
        //this.Bottom = this.CopyImage(this.Top, NavBarInfoPane.FlipCase.YAXIS);
        //this.TopLeft.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT);
        //this.TopRight.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT);
        //this.BottomLeft.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT);
        //this.BottomRight.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_RIGHT);
        //this.Top.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
        //this.Left.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT);
        //this.Right.SetAnchor(SpriteRoot.ANCHOR_METHOD.UPPER_RIGHT);
        //this.Bottom.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
        //this.Middle.SetAnchor(SpriteRoot.ANCHOR_METHOD.MIDDLE_CENTER);
        //this.Nipple.SetAnchor(SpriteRoot.ANCHOR_METHOD.BOTTOM_CENTER);
	}

	private void PositionStuffForCurrentSize()
	{
        //this.Top.SetSize(this.width - this.TopLeft.width + this.TopRight.width, this.Top.height);
        //this.Bottom.SetSize(this.Top.width, -this.Top.height);
        //this.Left.SetSize(this.Left.width, this.height - this.TopLeft.height + this.BottomLeft.height);
        //this.Right.SetSize(-this.Left.width, this.Left.height);
        //this.Middle.SetSize(this.Top.width, this.Left.height);
        //this.TopLeft.transform.localPosition = new Vector3(-(this.width / 2f) + this.TopLeft.width, -this.TopLeft.height, 0f);
        //this.TopRight.transform.localPosition = new Vector3(this.width / 2f - this.TopLeft.width, -this.TopRight.height, 0f);
        //this.BottomLeft.transform.localPosition = new Vector3(-(this.width / 2f) + this.TopLeft.width, -this.height + this.TopLeft.height, 0f);
        //this.BottomRight.transform.localPosition = new Vector3(this.width / 2f - this.TopLeft.width, -this.height + this.TopLeft.height, 0f);
        //this.Left.transform.localPosition = new Vector3(-(this.width / 2f) + this.Left.width, -this.TopLeft.height, 0f);
        //this.Right.transform.localPosition = new Vector3(this.width / 2f - this.Left.width, -this.TopLeft.height, 0f);
        //this.Bottom.transform.localPosition = new Vector3(0f, -this.height + this.Top.height, 0f);
        //this.Top.transform.localPosition = new Vector3(0f, -this.Top.height, 0f);
        //this.Nipple.transform.localPosition = new Vector3(0f, -this.Top.height + 0.01f, -0.01f);
        //this.Middle.transform.localPosition = new Vector3(0f, -this.height / 2f, 0f);
        //this.Text.transform.localPosition = new Vector3(0f, -this.height / 2f - 0.02f, -0.01f);
	}

	private void OnButton()
	{
		if (this.PressAction != null)
		{
			this.PressAction();
		}
	}

	public void Show(bool show)
	{
		this.IsShowing = show;
		base.gameObject.SetActive(show);
	}

	public void OnScreenChanged(ScreenID screen)
	{
		if (this.TimoutMode == NavBarInfoPane.DisplayMode.ResidentOfScreen)
		{
			if (this.CurrentState == NavBarInfoPane.AnimState.BEFORE)
			{
				if (screen == this.ScreenOfResidence)
				{
					this.CurrentState = NavBarInfoPane.AnimState.APPEAR;
				}
			}
			else if (this.CurrentState == NavBarInfoPane.AnimState.DISPLAY && screen != this.ScreenOfResidence)
			{
				this.CurrentState = NavBarInfoPane.AnimState.HIDE;
				this.AnimTimePos = 0f;
			}
		}
	}

	private void Update()
	{
		if (this.CurrentState == NavBarInfoPane.AnimState.DISPLAY)
		{
			if (this.TimoutMode == NavBarInfoPane.DisplayMode.SecondCountDown)
			{
				this.SecondsToRemain -= Time.deltaTime;
				if (this.SecondsToRemain <= 0f)
				{
					this.CurrentState = NavBarInfoPane.AnimState.HIDE;
				}
			}
		}
		else if (this.CurrentState == NavBarInfoPane.AnimState.HIDE)
		{
			this.AnimTimePos += Time.deltaTime;
			float num = Mathf.Min(this.AnimTimePos / 0.2f, 1f);
			if (this.AnimTimePos >= 0.2f)
			{
				num = 1f;
				this.AnimTimePos = 0f;
				this.CurrentState = NavBarInfoPane.AnimState.FINISHED;
				this.Show(false);
			}
			this.SetForFade(1f - num);
		}
		else if (this.CurrentState == NavBarInfoPane.AnimState.APPEAR)
		{
			if (!this.IsShowing)
			{
				this.Show(true);
			}
			this.AnimTimePos += Time.deltaTime;
			float num = Mathf.Min(this.AnimTimePos / 0.2f, 1f);
			if (this.AnimTimePos >= 0.2f)
			{
				num = 1f;
				this.AnimTimePos = 0f;
				this.CurrentState = NavBarInfoPane.AnimState.DISPLAY;
			}
			this.SetForFade(num);
		}
	}

	public void SetForFade(float interp)
	{
		float alphaValue = this.AnimCurve.Evaluate(interp);
		this.ChangeAllTints(alphaValue);
		Vector3 b = new Vector3(0f, (1f - interp) * 0.07f, 0f);
		base.gameObject.transform.localPosition = this.FinalPosition + b;
	}

	public bool HasFinished()
	{
		return this.CurrentState == NavBarInfoPane.AnimState.FINISHED;
	}

	public void ShowNow()
	{
		this.CurrentState = NavBarInfoPane.AnimState.DISPLAY;
		this.Show(true);
	}

	public void KillNow()
	{
		this.CurrentState = NavBarInfoPane.AnimState.FINISHED;
		this.Show(false);
	}

	public void FinishNice()
	{
		this.CurrentState = NavBarInfoPane.AnimState.HIDE;
		this.AnimTimePos = 0f;
	}

	public void MoveNipple(float newX)
	{
		if (this.Nipple == null)
		{
			return;
		}
		Vector3 localPosition = this.Nipple.gameObject.transform.localPosition;
		localPosition.x = newX;
		this.Nipple.gameObject.transform.localPosition = localPosition;
	}

	public float GetNippleXLocalPosition()
	{
		return this.Nipple.gameObject.transform.localPosition.x;
	}

	public void SetText(string newText)
	{
		this.Text.text = newText;
	}

	private void ChangeObjectTint(GameObject gameObject, float alphaValue)
	{
        //Color color = gameObject.renderer.material.GetColor("_Tint");
        //color.a = alphaValue;
        //gameObject.renderer.material.SetColor("_Tint", color);
	}

	public void ChangeAllTints(float alphaValue)
	{
		if (this.CombinedWindow != null)
		{
			this.ChangeObjectTint(this.CombinedWindow, alphaValue);
		}
		if (this.Nipple != null)
		{
			this.ChangeObjectTint(this.Nipple.gameObject, alphaValue);
		}
		this.ChangeObjectTint(this.Text.gameObject, alphaValue);
	}

	private void MoveText(float theFloat)
	{
		Vector3 localPosition = this.savedTextLocalPosition;
		localPosition.y += theFloat;
		this.Text.gameObject.transform.localPosition = localPosition;
	}

	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Vector3 center = base.gameObject.transform.position + new Vector3(0f, -this.height / 2f, 0f);
		Gizmos.DrawWireCube(center, new Vector3(this.width, this.height, 0f));
	}
}
