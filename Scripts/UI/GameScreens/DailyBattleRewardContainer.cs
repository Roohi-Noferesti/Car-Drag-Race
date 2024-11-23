using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DailyBattleRewardContainer : MonoBehaviour
{
	private enum State
	{
		Activate,
		Active,
		Deactivate,
		Inactive
	}

	public TextMeshProUGUI DayText;

	public TextMeshProUGUI DayFadeText;

	public Vector3 DayTextOffset;

	public TextMeshProUGUI PrizeText;

	public Vector3 PrizeTextOffset;

	public Color TextSelectedColor;

	public Color TextUnselectedColor;

	public Transform WindowSelected;

	public Transform TitleBarSelected;

	public Transform WindowUnselected;

	public Transform TitleBarUnselected;

	public Image PrizeIcon;

	public Animation[] FlareAnimations;

	public Vector3 ScaleFactor;

	public float AnimationTime;

	public float FadeInTime;

	public AnimationCurve AnimationScaleCurve;

	public AnimationCurve AlphaCurve;

	public AnimationCurve FadeInCurve;

	private State CurrentState;

	private Vector3 InverseScaleFactor;

	private Vector3 TitleBarScaleFactor;

	private Vector3 TitleBarInvScaleFactor;

	private Vector3 PrizeInvScaleFactor;

	private Vector3 TargetPosition;

	private Vector3 PrevPosition;

	private float StateTimer = 1f;

	private float FadeInTimer = 1f;

	private float FadeInDelay;

	private static GameObject Prefab;

	public bool Selected
	{
		get
		{
			return this.CurrentState == State.Activate || this.CurrentState == State.Active;
		}
		set
		{
			State currentState = (!value) ? State.Deactivate : State.Activate;
			if (value != this.Selected)
			{
				this.StateTimer = 0f;
				this.CurrentState = currentState;
                //this.WindowSelected.gameObject.SetActive(true);
                //this.WindowUnselected.gameObject.SetActive(true);
				if (value)
				{
					Animation[] flareAnimations = this.FlareAnimations;
					for (int i = 0; i < flareAnimations.Length; i++)
					{
						Animation animation = flareAnimations[i];
						animation.Play();
					}
				}
			}
		}
	}

	public static DailyBattleRewardContainer Create(bool initSelected,bool isDaily)
	{
		if (Prefab == null)
		{
            Prefab = (Resources.Load(isDaily ? "Misc/DailyBattleRewardContainer" : "Misc/LadderRewardContainer") as GameObject);
		}
		var gameObject = Instantiate(Prefab);
		var component = gameObject.GetComponent<DailyBattleRewardContainer>();
		if (initSelected)
		{
            component.WindowSelected.gameObject.SetActive(true);
            //component.WindowUnselected.gameObject.SetActive(false);
            //component.PrizeText.color = component.TextSelectedColor;
			component.CurrentState = State.Active;
            var num = 1f / component.ScaleFactor.x;
            component.transform.localScale = new Vector3(num, num, 1f);
		}
		else
		{
            //component.WindowSelected.gameObject.SetActive(false);
            //component.WindowUnselected.gameObject.SetActive(true);
            //component.PrizeText.color = component.TextUnselectedColor;
			component.CurrentState = State.Inactive;
		}
		return component;
	}

	public void AnimateToPosition(Vector3 targetPos)
	{
		this.PrevPosition = this.TargetPosition;
		this.TargetPosition = targetPos;
		this.StateTimer = 0f;
	}

	public void FadeInDelayed(float delay)
	{
		this.FadeInTimer = 0f;
		this.FadeInDelay = delay;
	}

	public void Init(Sprite rewardIcon)
	{
        //this.InverseScaleFactor = new Vector3(1f / this.ScaleFactor.x, 1f / this.ScaleFactor.y, 1f / this.ScaleFactor.z);
        //this.TitleBarScaleFactor = new Vector3(1f, this.ScaleFactor.y, 1f);
        //this.TitleBarInvScaleFactor = new Vector3(1f, this.InverseScaleFactor.y, 1f);
        //this.PrizeInvScaleFactor = new Vector3(this.InverseScaleFactor.x, this.InverseScaleFactor.x, 1f);
        //int frameCount = this.PrizeIcon.animations[0].GetFrameCount();
        //this.PrizeIcon.PlayAnim(0, prizeArtID % frameCount);
	    this.PrizeIcon.sprite = rewardIcon;
		this.TargetPosition = (this.PrevPosition = base.transform.localPosition);
		this.UpdateText();
		this.UpdateFadeIn();
	}

	private void Update()
	{
		if (this.StateTimer < 1f)
		{
			this.StateTimer += Time.deltaTime / this.AnimationTime;
			this.UpdateAnimation();
			this.UpdateText();
		}
		if (this.FadeInTimer < 1f + this.FadeInDelay)
		{
			this.FadeInTimer += Time.deltaTime / this.FadeInTime;
			this.UpdateFadeIn();
		}
	}

	private void UpdateFadeIn()
	{
		float num = FadeInTimer - FadeInDelay;
		SetAlphaRecursive(this.WindowSelected, num);
		SetAlphaRecursive(this.WindowUnselected, num);
        //SetAlphaRecursive(this.PrizeIcon.transform, num);
        PrizeText.color = new Color(this.PrizeText.color.r, this.PrizeText.color.g, this.PrizeText.color.b, num);
        //DayText.color = new Color(this.DayText.color.r, this.DayText.color.g, this.DayText.color.b, num);
	}

	private void UpdateAnimation()
	{
		float t = this.AnimationScaleCurve.Evaluate(this.StateTimer);
		base.transform.localPosition = Vector3.Lerp(this.PrevPosition, this.TargetPosition, t);
		switch (this.CurrentState)
		{
		case State.Activate:
            //this.WindowSelected.localScale = Vector3.Lerp(this.ScaleFactor, Vector3.one, t);
            //this.WindowUnselected.localScale = Vector3.Lerp(Vector3.one, this.InverseScaleFactor, t);
            //this.PrizeIcon.transform.localScale = Vector3.Lerp(Vector3.one, this.PrizeInvScaleFactor, t);
            //this.TitleBarUnselected.transform.localScale = Vector3.Lerp(Vector3.one, this.TitleBarScaleFactor, t);
            //this.TitleBarSelected.transform.localScale = Vector3.Lerp(this.TitleBarInvScaleFactor, Vector3.one, t);
            this.PrizeText.color = Color.Lerp(this.TextUnselectedColor, this.TextSelectedColor, t);
			this.SetAlphaRecursive(this.WindowSelected, this.AlphaCurve.Evaluate(this.StateTimer));
			this.SetAlphaRecursive(this.WindowUnselected, this.AlphaCurve.Evaluate(1f - this.StateTimer));
			if (this.StateTimer >= 1f)
			{
				this.CurrentState = State.Active;
                //this.WindowUnselected.gameObject.SetActive(false);
                //Animation[] flareAnimations = this.FlareAnimations;
                //for (int i = 0; i < flareAnimations.Length; i++)
                //{
                //    Animation animation = flareAnimations[i];
                //    animation.PlayQueued("DailyBattleRewardFlareLoop");
                //}
			}
			break;
		case State.Deactivate:
            //this.WindowSelected.localScale = Vector3.Lerp(Vector3.one, this.ScaleFactor, t);
            //this.WindowUnselected.localScale = Vector3.Lerp(this.InverseScaleFactor, Vector3.one, t);
            //this.PrizeIcon.transform.localScale = Vector3.Lerp(this.PrizeInvScaleFactor, Vector3.one, t);
            //this.TitleBarSelected.transform.localScale = Vector3.Lerp(Vector3.one, this.TitleBarInvScaleFactor, t);
            //this.TitleBarUnselected.transform.localScale = Vector3.Lerp(this.TitleBarScaleFactor, Vector3.one, t);
            this.PrizeText.color = Color.Lerp(this.TextSelectedColor, this.TextUnselectedColor, t);
			this.SetAlphaRecursive(this.WindowSelected, this.AlphaCurve.Evaluate(1f - this.StateTimer));
			this.SetAlphaRecursive(this.WindowUnselected, this.AlphaCurve.Evaluate(this.StateTimer));
			if (this.StateTimer >= 1f)
			{
				this.CurrentState = State.Inactive;
                //this.WindowSelected.gameObject.SetActive(false);
			}
			break;
		}
	}

	private void UpdateText()
	{
        //this.DayText.transform.position = this.GetTitleBarPosition() + this.DayTextOffset;
        //this.DayFadeText.transform.position = this.DayText.transform.position;
        //this.PrizeText.transform.position = this.GetTitleBarPosition() + this.PrizeTextOffset;
	}

	private Vector3 GetTitleBarPosition()
	{
		return (!this.Selected) ? this.TitleBarUnselected.transform.position : this.TitleBarSelected.transform.position;
	}

	private void SetAlphaRecursive(Transform parent, float alpha)
	{
        //if (parent.renderer != null)
        //{
        //    Color color = parent.renderer.material.GetColor("_Tint");
        //    color.a = alpha;
        //    parent.renderer.material.SetColor("_Tint", color);
        //}
        //for (int i = 0; i < parent.childCount; i++)
        //{
        //    Transform child = parent.GetChild(i);
        //    this.SetAlphaRecursive(child, alpha);
        //}
	}
}
