using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;

[AddComponentMenu("BossAlien/GameCenter/AchievementsController")]
public class AchievementsController : MonoBehaviour
{
	private const float TIME_ACH_ON_SCREEN = 2.5f;

	private GameObject AchievementTextObject;

	private TextMeshProUGUI AchievementText;

	private GameObject AchievementHeaderTextObject;

    private TextMeshProUGUI AchievementHeaderText;

	private GameObject AchievementBackground;

	private GameObject ACHPosTM;

	private Animation ACHPosTMAnimation;

	private GameObject ACHPosNode;

	private float waitCounter;

	private bool waitingACH;

	private bool incomingACH;

	private bool outgoingACH;

	private bool displayingACH;

	private Camera UICam;

	private static List<string> ACHMessages = new List<string>();

	public static AchievementsController Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (AchievementsController.Instance != null)
		{
			return;
		}
		AchievementsController.Instance = this;
        //this.UICam = GameObject.FindGameObjectWithTag("GUICamera").GetComponent<Camera>();
        //GameObject original = (GameObject)Resources.Load("Misc/AchievementDialogue");
        //this.ACHPosTM = (GameObject)UnityEngine.Object.Instantiate(original);
        //this.ACHPosTM.transform.parent = base.gameObject.transform;
        //UnityEngine.Object.DontDestroyOnLoad(this.ACHPosTM);
        //this.ACHPosNode = GameObject.Find("AchievementPosNode");
        //this.ACHPosTMAnimation = this.ACHPosNode.GetComponent<Animation>();
        //this.AchievementTextObject = GameObject.Find("AchievementText");
        //this.AchievementText = this.AchievementTextObject.GetComponent<TextMeshProUGUI>();
        ////this.AchievementText.RenderCamera = this.UICam;
        //this.AchievementHeaderTextObject = GameObject.Find("AchievementHeaderText");
        //this.AchievementHeaderText = this.AchievementHeaderTextObject.GetComponent<TextMeshProUGUI>();
        ////this.AchievementHeaderText.RenderCamera = this.UICam;
        //this.AchievementBackground = GameObject.Find("AchievementWindowPane");
        //this.SetAchievementVisibilty(false);
        //this.ResetACH();
	}

	private void Update()
	{
		this.UpdateACHAnimations();
	}

	public void ResetACH()
	{
		this.AnimReset();
	}

	private void SetupACHElements()
	{
	}

	public void SetAchievementText(string text)
	{
		this.AchievementText.text = text;
        this.AchievementHeaderText.text = LocalizationManager.GetTranslation("TEXT_ACHIEVEMENT_UNLOCKED_ANDROID");
	}

	public void IntroduceAchievement(string text)
	{
		if (this.displayingACH)
		{
			AchievementsController.ACHMessages.Add(text);
		}
		else
		{
			this.waitCounter = 0f;
			this.SetAchievementText(text);
			this.SetAchievementVisibilty(true);
			this.AnimReset();
			this.AnimEaseIn();
			this.incomingACH = true;
			this.outgoingACH = false;
			this.waitingACH = false;
			this.displayingACH = true;
		}
	}

	public void SetAchievementVisibilty(bool visible)
	{
		this.AchievementText.gameObject.SetActive(visible);
		this.AchievementHeaderText.gameObject.SetActive(visible);
		this.AchievementBackground.gameObject.SetActive(visible);
	}

	public void DismissAchievement()
	{
		this.AnimEaseOut();
		this.outgoingACH = true;
	}

	private void UpdateACHAnimations()
	{
		if (!this.incomingACH && !this.outgoingACH && !this.waitingACH)
		{
			return;
		}
		if (this.waitingACH)
		{
			this.waitCounter -= Time.deltaTime;
			if (this.waitCounter <= 0f)
			{
				this.waitingACH = false;
				this.DismissAchievement();
			}
		}
		if (this.incomingACH && !this.ACHPosTMAnimation.IsPlaying("ACHPosTM EaseIn"))
		{
			this.waitCounter = 2.5f;
			this.waitingACH = true;
			this.incomingACH = false;
		}
		if (this.outgoingACH && !this.ACHPosTMAnimation.IsPlaying("ACHPosTM EaseOut"))
		{
			this.displayingACH = false;
			this.outgoingACH = false;
			this.SetAchievementVisibilty(false);
			if (AchievementsController.ACHMessages.Count > 0)
			{
				this.IntroduceAchievement(AchievementsController.ACHMessages[0]);
				AchievementsController.ACHMessages.Remove(AchievementsController.ACHMessages[0]);
			}
		}
	}

	private void AnimEaseIn()
	{
		this.ACHPosTMAnimation.Stop();
		this.ACHPosTMAnimation.Play("ACHPosTM EaseIn");
        //string name = this.ACHPosTMAnimation.animation.clip.name;
		AnimationState animationState = this.ACHPosTMAnimation[name];
		animationState.time = 0f;
		animationState.enabled = true;
		animationState.speed = 1f;
	}

	private void AnimEaseOut()
	{
		this.ACHPosTMAnimation.Stop();
		this.ACHPosTMAnimation.Play("ACHPosTM EaseOut");
        //string name = this.ACHPosTMAnimation.animation.clip.name;
		AnimationState animationState = this.ACHPosTMAnimation[name];
		animationState.time = 0f;
		animationState.enabled = true;
		animationState.speed = 1f;
	}

	private void AnimReset()
	{
		this.ACHPosTMAnimation.Stop();
        //string name = this.ACHPosTMAnimation.animation.clip.name;
		AnimationState animationState = this.ACHPosTMAnimation[name];
		animationState.time = 0f;
		animationState.enabled = true;
		animationState.speed = 0f;
		this.ACHPosTMAnimation.Play();
	}
}
