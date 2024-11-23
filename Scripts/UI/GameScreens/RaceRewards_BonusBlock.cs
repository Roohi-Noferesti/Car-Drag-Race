using System.Collections.Generic;
using Fabric;
using TMPro;
using UnityEngine;

public abstract class RaceRewards_BonusBlock : MonoBehaviour
{
	public GameObject Bonus_Container;

	public GameObject Bonus_1;
	public GameObject Bonus_1_Content;

	public TextMeshProUGUI Bonus_1_Name;

    public TextMeshProUGUI Bonus_1_Prize;

	protected int Bonus_1_PrizeVal;

	public GameObject Bonus_2;
	public GameObject Bonus_2_Content;

    public TextMeshProUGUI Bonus_2_Name;

	public TextMeshProUGUI Bonus_2_Prize;

	protected int Bonus_2_PrizeVal;

	public GameObject Bonus_3;
	public GameObject Bonus_3_Content;

	public TextMeshProUGUI Bonus_3_Name;

	public TextMeshProUGUI Bonus_3_Prize;

	protected int Bonus_3_PrizeVal;

	public GameObject Bonus_4;
	public GameObject Bonus_4_Content;

	public TextMeshProUGUI Bonus_4_Name;

	public TextMeshProUGUI Bonus_4_Prize;

	protected int Bonus_4_PrizeVal;

	public GameObject Bonus_5;
	public GameObject Bonus_5_Content;

	public TextMeshProUGUI Bonus_5_Name;

	public TextMeshProUGUI Bonus_5_Prize;

	protected int Bonus_5_PrizeVal;

	public Transform TotalAddedGlowPos;

	public Transform TotalAddedFlarePos;

	public TextMeshProUGUI TotalAddedText;

    public GameObject Bonus_Gold;

	public TextMeshProUGUI TotalBonusText;

	public GameObject TotalEvoToken;

	public bool SkipAllZeroBonuses;

    [HideInInspector]
	public int TotalXP;
    [HideInInspector]
	public int TotalCash;
    [HideInInspector]
	public int TotalGold;
    [HideInInspector]
	public int TotalRankPoints;

    //public List<SpriteRoot> AnimatingFadingSprites;

	public List<TextMeshProUGUI> AnimatingFadingText;

	public Shader AnimatingFadeReplacementShader;

	public Shader AnimatingFadeReplacementAlpha8Shader;

	protected int _currentTotalValue;

	protected bool playerWon;
    [HideInInspector]
    public int TotalStar;

    public virtual void Setup(RaceResultsTrackerState resultsData)
    {
	    Bonus_5.SetActive(RemoteConfigABTest.CheckRemoteConfigValue());
    }

	public abstract void SetPlayerWon(bool currentEventWon);

	public virtual bool DelayAnimationFinish()
	{
		return false;
	}

	public void WarmupAnimatedItems()
	{
        //foreach (SpriteRoot current in this.AnimatingFadingSprites)
        //{
        //    Shader shader = current.renderer.material.shader;
        //    if (shader.name.ToLower().Contains("vertex"))
        //    {
        //        current.renderer.material.shader = this.AnimatingFadeReplacementShader;
        //    }
        //}
        //foreach (TextMeshProUGUI current2 in this.AnimatingFadingText)
        //{
        //    if (current2.renderer.material.shader.name.ToLower().Contains("alpha-8"))
        //    {
        //        current2.renderer.material.shader = this.AnimatingFadeReplacementAlpha8Shader;
        //    }
        //    else
        //    {
        //        current2.renderer.material.shader = this.AnimatingFadeReplacementShader;
        //    }
        //}
        //AnimationUtils.PlayFirstFrame(this.Bonus_Container.animation);
        //AnimationUtils.PlayFirstFrame(this.Bonus_1.animation);
        //AnimationUtils.PlayFirstFrame(this.Bonus_1_Prize.transform.parent.animation);
        //AnimationUtils.PlayFirstFrame(this.Bonus_2.animation);
        //AnimationUtils.PlayFirstFrame(this.Bonus_2_Prize.transform.parent.animation);
        //AnimationUtils.PlayFirstFrame(this.Bonus_3.animation);
        //AnimationUtils.PlayFirstFrame(this.Bonus_3_Prize.transform.parent.animation);
        //if (this.Bonus_4 != null)
        //{
        //    AnimationUtils.PlayFirstFrame(this.Bonus_4.animation);
        //    AnimationUtils.PlayFirstFrame(this.Bonus_4_Prize.transform.parent.animation);
        //}
        //if (this.Bonus_5 != null)
        //{
        //    AnimationUtils.PlayFirstFrame(this.Bonus_5.animation);
        //    AnimationUtils.PlayFirstFrame(this.Bonus_5_Prize.transform.parent.animation);
        //}
        //AnimationUtils.PlayFirstFrame(this.TotalAddedText.animation);
        //if (this.TotalBonusText != null)
        //{
        //    AnimationUtils.PlayFirstFrame(this.TotalBonusText.animation);
        //}
	}

	public void CooldownAnimatedItems()
	{
        //AnimationUtils.PlayLastFrame(this.Bonus_Container.animation);
        //AnimationUtils.PlayLastFrame(this.Bonus_1.animation);
        //AnimationUtils.PlayAnim(this.Bonus_1_Prize.transform.parent.animation);
        //AnimationUtils.PlayLastFrame(this.Bonus_2.animation);
        //AnimationUtils.PlayAnim(this.Bonus_2_Prize.transform.parent.animation);
        //AnimationUtils.PlayLastFrame(this.Bonus_3.animation);
        //AnimationUtils.PlayAnim(this.Bonus_3_Prize.transform.parent.animation);
        //if (this.Bonus_4 != null)
        //{
        //    AnimationUtils.PlayLastFrame(this.Bonus_4.animation);
        //    AnimationUtils.PlayAnim(this.Bonus_4_Prize.transform.parent.animation);
        //}
        //if (this.Bonus_5 != null)
        //{
        //    AnimationUtils.PlayLastFrame(this.Bonus_5.animation);
        //    AnimationUtils.PlayAnim(this.Bonus_5_Prize.transform.parent.animation);
        //}
        //AnimationUtils.PlayAnim(this.TotalAddedText.animation, "Rewards_Bonus_TotalEnd");
        //if (this.TotalGold > 0)
        //{
        //    AnimationUtils.PlayAnim(this.TotalBonusText.animation, "Rewards_Bonus_TotalGoldEnd");
        //}
        //if (this.TotalEvoToken != null && this.TotalEvoToken.gameObject.activeInHierarchy)
        //{
        //    AnimationUtils.PlayLastFrame(this.TotalEvoToken.animation);
        //}
	}

	protected void PlayAudioStarTally(int val)
	{
		if (val > 0)
		{
			EventManager.Instance.PostEvent("Reward_StarTally", EventAction.PlaySound, null, null);
		}
		else
		{
			EventManager.Instance.PostEvent("Reward_StarNoTally", EventAction.PlaySound, null, null);
		}
	}

	public void Anim_Bonus_Container()
	{
        //AnimationUtils.PlayAnim(this.Bonus_Container.animation, "Rewards_Bonus_In");
	}

	public void Anim_Bonus_1()
	{
		this.Bonus_2_Content.SetActive(false);
		if (!string.IsNullOrEmpty(this.Bonus_3_Prize.text))
		{
			this.Bonus_3_Content.SetActive(false);
		}
		if (this.Bonus_4_Content != null)
		{
			this.Bonus_4_Content.SetActive(false);
		}
		if (this.Bonus_5_Content != null)
		{
			this.Bonus_5_Content.SetActive(false);
		}
        if (this.Bonus_Gold != null)
        {
            this.Bonus_Gold.SetActive(false);
        }
		EventManager.Instance.PostEvent(AudioEvent.Reward_Slide, EventAction.PlaySound, null,Camera.main.gameObject);
        //AnimationUtils.PlayAnim(this.Bonus_1.animation, "Rewards_Bonus_ItemIn");
        this.Bonus_1_Content.SetActive(true);
	    Bonus_1.GetComponent<Animator>().Play("play");
	}

	public void Anim_Bonus_2()
	{
		this.Bonus_2_Content.SetActive(true);
        EventManager.Instance.PostEvent(AudioEvent.Reward_Slide, EventAction.PlaySound, null, Camera.main.gameObject);
        //AnimationUtils.PlayAnim(this.Bonus_2.animation, "Rewards_Bonus_ItemIn");
        Bonus_2.GetComponent<Animator>().Play("play");
	}

	public virtual void Anim_Bonus_3()
	{
		this.Bonus_3_Content.SetActive(true);
        EventManager.Instance.PostEvent(AudioEvent.Reward_Slide, EventAction.PlaySound, null, Camera.main.gameObject);
        //AnimationUtils.PlayAnim(this.Bonus_3.animation, "Rewards_Bonus_ItemIn");
        Bonus_3.GetComponent<Animator>().Play("play");
	}

	public void Anim_Bonus_4()
	{
		if (this.Bonus_4_Content != null)
		{
			this.Bonus_4_Content.SetActive(true);
            EventManager.Instance.PostEvent(AudioEvent.Reward_Slide, EventAction.PlaySound, null, Camera.main.gameObject);
            //AnimationUtils.PlayAnim(this.Bonus_4.animation, "Rewards_Bonus_ItemIn");
            Bonus_4.GetComponent<Animator>().Play("play");
		}
	}

	public void Anim_Bonus_5()
	{
		if (this.Bonus_5_Content != null)
		{
			this.Bonus_5_Content.SetActive(true);
            EventManager.Instance.PostEvent(AudioEvent.Reward_Slide, EventAction.PlaySound, null, null);
            //AnimationUtils.PlayAnim(this.Bonus_5.animation, "Rewards_Bonus_ItemIn");
            Bonus_5.GetComponent<Animator>().Play("play");
		}
	}

	public virtual void Anim_Bonus_6()
	{
	}

	public abstract void Anim_Totals_AddBonus1();

	public abstract void Anim_Totals_AddBonus2();

	public abstract void Anim_Totals_AddBonus3();

	public abstract void Anim_Totals_AddBonus4();

	public abstract void Anim_Totals_AddBonus5();

    public virtual void Anim_Totals_Final()
    {
        if (this.Bonus_Gold != null)
        {
            this.Bonus_Gold.SetActive(true);
            EventManager.Instance.PostEvent(AudioEvent.Reward_Slide, EventAction.PlaySound, null, Camera.main.gameObject);
            //AnimationUtils.PlayAnim(this.Bonus_4.animation, "Rewards_Bonus_ItemIn");
            Bonus_Gold.GetComponent<Animator>().Play("play");
        }
    }

	public void OnDestroy()
	{
		this.TotalAddedText = null;
		this.TotalBonusText = null;
	}

	public void Hide(bool hide)
	{
        //this.TotalAddedText.Hide(hide);
        //if (this.TotalBonusText != null)
        //{
        //    this.TotalBonusText.Hide(hide);
        //}
	}

	public abstract void Finish();
}
