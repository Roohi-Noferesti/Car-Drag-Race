using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeSceneCharactersContainer : MonoBehaviour
{
	private const float unitConversion = 200f;

    public List<RawImage> CharactersSprites;

	public GameObject LogoParent;

    public GameObject[] LeaderCrossSlotes;

	private INarrativeSceneLogo LogoController;

    private List<Image> SlotBorders = new List<Image>();

	private List<GameObject> SlotCrosses = new List<GameObject>();

    private List<TextMeshProUGUI> SlotNames = new List<TextMeshProUGUI>();

    private List<Animator> SlotAnimators = new List<Animator>();

    private Animator ChatTextAnimator;

    public MainCharacterGraphic MainCharacter;

    public TextMeshProUGUI ChatText;

	public float spacing = 0.2f;

    private Animator MainCharacterAnimator;

    public bool isLoaded
	{
		get;
		private set;
	}

    public string ExitAnimationName { get; set; }

    public Image GetBorder(int zSlotIndex)
	{
		return this.SlotBorders[zSlotIndex];
	}

    //Tier X Initialise
    public void Initialise(NarrativeSceneCharactersGroup zCharactersGroup, NarrativeSceneDetails zDetails, CrewProgressionScreen zScreen, Action zCallback)
	{
		int num = 1;
		int initializedMembers = 0;
		//this.LogoController = zCharactersGroup.Logo.CreateLogo(this.LogoParent);
		//this.LogoController.SetOptionalStrings(new string[]
		//{
  //          LocalizationManager.GetTranslation(zDetails.Title),
  //          LocalizationManager.GetTranslation(zDetails.Subtitle)
  //      });
	    this.MainCharacterAnimator = this.MainCharacter.GetComponent<Animator>();
	    this.MainCharacter.gameObject.SetActive(false);
        TexturePack.RequestTextureFromBundle(zCharactersGroup.Leader.CardTextureName, delegate (Texture2D tex)
        {
            this.MainCharacter.LoadProtrait(tex, zScreen);
            initializedMembers++;
            if (initializedMembers > this.CharactersSprites.Count)
            {
                this.InitialisationFinished(zDetails.IntroText, zCharactersGroup.Leader.Name);
                if (zCallback != null)
                {
                    zCallback();
                }
            }
        });
        //Texture2D texture2D = Resources.Load("CharacterCards/Crew/crew_frame") as Texture2D;
        //GameObject original = Resources.Load("CharacterCards/Crew/CrewCross") as GameObject;
        ChatTextAnimator = ChatText.GetComponent<Animator>();
        foreach (NarrativeSceneCharacter current in zCharactersGroup.Members)
        {
            if (num >= 4)
                return;
            var memberObject = this.CharactersSprites[num - 1].transform.parent;
            //global::Sprite component = memberSprite.transform.parent.gameObject.GetComponent<global::Sprite>();
            //component.renderer.material.SetTexture("_MainTex", texture2D);
            //component.Setup((float)texture2D.width / 200f, (float)texture2D.height / 200f, new Vector2(0f, (float)(texture2D.height - 1)), new Vector2((float)texture2D.width, (float)texture2D.height));
            //this.SlotBorders.Add(component);
            //component.gameObject.SetActive(false);
            TextMeshProUGUI nameText = memberObject.Find("NameText").GetComponent<TextMeshProUGUI>();
		    nameText.text = LocalizationManager.GetTranslation(current.Name);
            this.SlotNames.Insert(num - 1, nameText);
		    nameText.gameObject.SetActive(false);
            //GameObject crossObject = UnityEngine.Object.Instantiate(original) as GameObject;
            GameObject crossObject = memberObject.Find("CrewCross").gameObject;
            SlotAnimators.Add(memberObject.GetComponent<Animator>());
            this.SlotCrosses.Add(crossObject);
		    crossObject.SetActive(false);
			if (string.IsNullOrEmpty(current.PortraitTextureName))
			{
				initializedMembers++;
				if (initializedMembers > this.CharactersSprites.Count)
				{
					this.InitialisationFinished(zDetails.IntroText, zCharactersGroup.Leader.Name);
					if (zCallback != null)
					{
						zCallback();
					}
				}
			}
			else
			{
                TexturePack.RequestTextureFromBundle(current.PortraitTextureName, delegate (Texture2D memberTexture)
                {
                    if (memberTexture != null)
                    {
                        CharactersSprites[num - 1].texture = memberTexture;
                        //memberSprite.renderer.material.SetTexture("_MainTex", memberTexture);
                        //memberSprite.Setup((float)memberTexture.width / 200f, (float)memberTexture.height / 200f, new Vector2(0f, (float)(memberTexture.height - 1)), new Vector2((float)memberTexture.width, (float)memberTexture.height));
                    }
                    initializedMembers++;
                    if (initializedMembers > this.CharactersSprites.Count)
                    {
                        this.InitialisationFinished(zDetails.IntroText, zCharactersGroup.Leader.Name);
                        if (zCallback != null)
                        {
                            zCallback();
                        }
                    }
                });
            }
			num++;
		}
	}


    //Tier 1 - 5 Initialise
    public void Initialise(eCarTier zTier, CrewProgressionScreen zScreen, Action zCallback)
    {
        string text = CarTierHelper.TierToString[(int) zTier];
        MainCharacterAnimator = this.MainCharacter.GetComponent<Animator>();
        this.MainCharacter.gameObject.SetActive(false);

        TexturePack.RequestTextureFromBundle("CrewPortraitsTier" + text + ".Boss Card" + (zTier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":""),
            delegate(Texture2D tex) { this.MainCharacter.LoadProtrait(tex, zScreen); });

        //Texture2D crewFrameTex = Resources.Load("CharacterCards/Crew/crew_frame") as Texture2D;
        GameObject crewCrossObj = Resources.Load("CharacterCards/Crew/CrewCross") as GameObject;
        //NarrativeSceneLogo narrativeSceneLogo = new NarrativeSceneLogo
        //{
        //    Type = "StandardTextureAndText",
        //    Details = new NarrativeSceneLogo.NarrativeSceneLogoDetails
        //    {
        //        StringValues = new string[]
        //        {
        //            "CharacterCards/Crew/Logos/Crew_" + (int)(zTier + 1) + "_Badge"
        //        }
        //    }
        //};
        //this.LogoController = narrativeSceneLogo.CreateLogo(this.LogoParent);
        //this.LogoController.SetOptionalStrings(new string[]
        //{
        //    LocalizationManager.GetTranslation(CrewChatter.GetCrewName((int)zTier)),
        //    LocalizationManager.GetTranslation(CrewChatter.GetTierName((int)zTier))
        //});
        int crewMember = 1;
        int initializedMembers = 0;
        ChatTextAnimator = ChatText.GetComponent<Animator>();
        foreach (var currentCharacter in this.CharactersSprites)
        {
            var characterRoot = currentCharacter.transform.parent;
            //var component = memberSprite.transform.parent.gameObject.GetComponent<global::Sprite>();
            //component.renderer.material.SetTexture("_MainTex", crewFrameTex);
            //component.Setup((float)texture2D.width / 200f, (float)texture2D.height / 200f, new Vector2(0f, (float)(texture2D.height - 1)), new Vector2((float)texture2D.width, (float)texture2D.height));
            //this.SlotBorders.Add(component);
            //component.gameObject.SetActive(false);
            var nameText = characterRoot.Find("NameText").GetComponent<TextMeshProUGUI>();
            nameText.text = LocalizationManager.GetTranslation(CrewChatter.GetMemberName((int) zTier, crewMember));
            //LocalisationManager.AdjustText(nameText, 0.85f);
            this.SlotNames.Insert(crewMember - 1, nameText);
            SlotAnimators.Add(characterRoot.GetComponent<Animator>());
            //nameText.gameObject.SetActive(false);
            var crewCrossInstance = UnityEngine.Object.Instantiate(crewCrossObj) as GameObject;
            crewCrossInstance.transform.SetParent(characterRoot, false);
            crewCrossInstance.transform.localPosition = Vector3.zero;
            this.SlotCrosses.Add(crewCrossInstance);
            crewCrossInstance.SetActive(false);
            var crewMemberPath = string.Concat("CrewPortraitsTier", text, ".Crew ", crewMember) + (zTier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":"");
            var currentCharacterTemp = currentCharacter;

            TexturePack.RequestTextureFromBundle(crewMemberPath, delegate(Texture2D memberTexture)
            {
                currentCharacterTemp.texture = memberTexture;
                //currentCharacter.Setup((float)memberTexture.width / 200f, (float)memberTexture.height / 200f,
                //    new Vector2(0f, (float) (memberTexture.height - 1)),
                //    new Vector2((float) memberTexture.width, (float) memberTexture.height));
                initializedMembers++;
                if (initializedMembers >= this.CharactersSprites.Count)
                {
                    this.InitialisationFinished("TEXT_ULTIMATE_GOAL_" + (int) zTier,
                        CrewChatter.GetLeaderName((int) zTier));
                    zCallback();
                }
            });
            crewMember++;
        }
    }

    public void InitialisationFinished(string zIntroText, string zLeaderName)
	{
		for (int i = 0; i < this.CharactersSprites.Count; i++)
		{
			this.CharactersSprites[i].gameObject.SetActive(true);
            //this.SlotBorders[i].gameObject.SetActive(true);
            //this.SlotNames[i].gameObject.SetActive(true);
		}
		this.MainCharacter.gameObject.SetActive(true);
        TextMeshProUGUI nameText = this.MainCharacter.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        nameText.text = LocalizationManager.GetTranslation(zLeaderName);
        nameText.gameObject.SetActive(true);
        this.SlotNames.Add(nameText);
        this.isLoaded = true;
		this.SetLogoAlpha(0f);
		this.SetDefaultPosition();
        //this.SetIntroText(LocalizationManager.GetTranslationIfNecessary(zIntroText));
		this.SetIntroTextAlpha(0f);
	    gameObject.SetActive(true);
    }

    public void Uninitialise()
	{
        //this.SetLogoAlpha(0f);
        //this.MainCharacter.UnloadProtrait();
        //for (int i = 0; i < this.CharactersSprites.Count; i++)
        //{
        //    global::Sprite sprite = this.SlotBorders[i];
        //    global::Sprite sprite2 = this.CharactersSprites[i];
        //    GameObject obj = this.SlotCrosses[i];
        //    sprite2.renderer.material.SetTexture("_MainTex", null);
        //    sprite.renderer.material.SetTexture("_MainTex", null);
        //    sprite.gameObject.SetActive(false);
        //    sprite2.gameObject.SetActive(false);
        //    UnityEngine.Object.Destroy(obj);
        //}
        //foreach (SpriteText current in this.SlotNames)
        //{
        //    current.gameObject.SetActive(false);
        //}
        //this.SlotBorders.Clear();
        //this.SlotCrosses.Clear();
        //this.SlotNames.Clear();
        //this.LogoParent.SetActive(false);
        //this.isLoaded = false;
	}

	private void SetDefaultPosition()
	{
        //float num = 0f;
        //for (int i = 0; i < this.SlotBorders.Count; i++)
        //{
        //    global::Sprite sprite = this.SlotBorders[i];
        //    global::Sprite sprite2 = this.CharactersSprites[i];
        //    GameObject gameObject = this.SlotCrosses[i];
        //    SpriteText spriteText = this.SlotNames[i];
        //    num += this.spacing * 0.5f;
        //    num += (float)sprite.renderer.material.mainTexture.width * 0.5f * 0.005f;
        //    sprite.transform.localPosition = new Vector3(num, 0f, 0f);
        //    sprite2.transform.localPosition = new Vector3(0f, 0.135f, 0f);
        //    gameObject.transform.localPosition = new Vector3(0f, 0f, 0f);
        //    spriteText.transform.localPosition = new Vector3(0f, -0.38f, -0.05f);
        //    num += (float)sprite.renderer.material.mainTexture.width * 0.5f * 0.005f;
        //    num += this.spacing * 0.5f;
        //}
        //num += this.spacing * 0.5f;
        //num += this.MainCharacter.GetWidth() * 0.5f;
        //this.MainCharacter.transform.localPosition = new Vector3(num, 0f, -0.1f);
        //SpriteText spriteText2 = this.SlotNames[3];
        //spriteText2.transform.localPosition = new Vector3(0f, -0.9f, -0.1f);
        //this.IntroText.gameObject.transform.parent = base.transform.parent.parent;
        //this.LogoParent.gameObject.transform.parent = base.transform.parent.parent;
        //float screenHeight = GUICamera.Instance.ScreenHeight;
        //float screenWidth = GUICamera.Instance.ScreenWidth;
        //this.IntroText.gameObject.transform.localPosition = GUICamera.Instance.transform.position + new Vector3(0f, -screenHeight * 0.5f, 0f);
        //this.IntroText.gameObject.transform.localPosition += new Vector3(0f, 0.4f, 0.5f);
        //this.LogoParent.gameObject.transform.localPosition = GUICamera.Instance.transform.position + new Vector3(-screenWidth * 0.5f, screenHeight * 0.5f, 0f);
        //this.LogoParent.gameObject.transform.localPosition += new Vector3(0.04f, -0.04f, 0.5f);
	}

	public void SetNumCharacterSlotsToCross(int zNumDefeated)
	{
        for (int i = 0; i < zNumDefeated; i++)
        {
            var thisIsForthMemberAndNotDefeated = i == 2 && zNumDefeated == 3;
            if (!thisIsForthMemberAndNotDefeated)
            {
                if (i < CharactersSprites.Count)
                {
                    //this.CharactersSprites[i].renderer.material.SetFloat("_Greyness", 1f);
                    this.CharactersSprites[i].color = new Color(0.28F, 0.28F, 0.28F, 1);
                    this.SlotCrosses[i].SetActive(true);
                }
            }

        }
	}

	public float GetTotalWidth()
	{
		return 3.6f + this.spacing * 4f + 1.015f + this.spacing + 0.22f;
	}

	public GameObject GetMainCharacterGameObject()
	{
		return this.MainCharacter.gameObject;
	}

    public MainCharacterGraphic GetMainCharacterGraphic()
    {
        return this.MainCharacter;
    }

	public void SetIntroScale(float zScale)
	{
        //this.IntroText.transform.localScale = new Vector3(zScale, zScale, zScale);
	}

	public void SetIntroText(string zIntroText)
	{
        //this.IntroText.Text = zIntroText;
	}

	public void SetAlpha(float zAlpha)
	{
		if (!this.isLoaded)
		{
			return;
		}
		for (int i = 0; i < 5; i++)
		{
			this.SetAlpha(i, zAlpha);
		}
	}

	public void SetLogoAlpha(float zAlpha)
	{
		//if (!this.isLoaded)
		//{
		//	return;
		//}
		//if (zAlpha <= 0f)
		//{
		//	if (this.LogoParent.gameObject.activeInHierarchy)
		//	{
		//		this.LogoParent.gameObject.SetActive(false);
		//	}
		//	return;
		//}
		//if (!this.LogoParent.gameObject.activeInHierarchy)
		//{
		//	this.LogoParent.gameObject.SetActive(true);
		//}
  //      this.LogoController.SetPreferredColour(new Color(1f, 1f, 1f, zAlpha));
    }

	public void SetIntroTextAlpha(float zAlpha)
	{
        //if (zAlpha <= 0f)
        //{
        //    if (this.IntroText.gameObject.activeInHierarchy)
        //    {
        //        this.IntroText.gameObject.SetActive(false);
        //    }
        //    return;
        //}
        //if (!this.IntroText.gameObject.activeInHierarchy)
        //{
        //    this.IntroText.gameObject.SetActive(true);
        //}
        //this.IntroText.color = new Color(1f, 1f, 1f, zAlpha);
	}

	public void SetActiveSlots(int zSlotIndex, bool zActive)
	{
		if (zSlotIndex >= this.CharactersSprites.Count)
		{
			this.MainCharacter.gameObject.SetActive(zActive);
			//this.SlotNames[3].gameObject.SetActive(zActive);
		}
		else
		{
			//this.SlotBorders[zSlotIndex].gameObject.SetActive(zActive);
			this.CharactersSprites[zSlotIndex].transform.parent.gameObject.SetActive(zActive);
			//this.SlotNames[zSlotIndex].gameObject.SetActive(zActive);
		}
	}

	public void SetAlpha(int zMember, float zAlpha)
	{
        if (!this.isLoaded)
        {
            return;
        }
        if (zMember >= this.CharactersSprites.Count)
        {
            this.MainCharacter.SetAlpha(zAlpha);
            this.SlotNames[3].color = new Color(1f, 1f, 1f, zAlpha);
        }
        else
        {
            //this.SlotBorders[zMember].SetColor(new Color(1f, 1f, 1f, zAlpha));
            this.CharactersSprites[zMember].color = new Color(1f, 1f, 1f, zAlpha);
            this.SlotNames[zMember].color = new Color(1f, 1f, 1f, zAlpha);
            //global::Sprite[] componentsInChildren = this.SlotCrosses[zMember].GetComponentsInChildren<global::Sprite>();
            //global::Sprite[] array = componentsInChildren;
            //for (int i = 0; i < array.Length; i++)
            //{
            //    global::Sprite sprite = array[i];
            //    sprite.SetColor(new Color(1f, 1f, 1f, zAlpha));
            //}
        }
	}



    public void SetColor(int zMember, float zAlpha)
    {
        if (!this.isLoaded)
        {
            return;
        }
        if (zMember >= this.CharactersSprites.Count)
        {
            this.MainCharacter.SetAlpha(zAlpha);
            this.SlotNames[3].color = new Color(1f, 1f, 1f, zAlpha);
        }
        else
        {
            this.CharactersSprites[zMember].color = new Color(zAlpha, zAlpha, zAlpha, 1);
        }
    }


    public void PlayBossAnimation(string animationName)
    {
        this.MainCharacterAnimator.Play(animationName);
    }

    public void PlayAnimation(string animationName)
    {
        for (int i = 0; i < CharactersSprites.Count; i++)
        {
            PlayAnimation(i, animationName);
        }
    }


    public void PlayAnimation(int zMember,string animationName)
    {
        if (!this.isLoaded)
        {
            return;
        }
        if (zMember >= this.CharactersSprites.Count)
        {
        }
        else
        {
            this.SlotAnimators[zMember].Play(animationName);
        }
    }

    public bool IsPlayingAnimation(string animationName)
    {
        for (int i = 0; i < CharactersSprites.Count; i++)
        {
            if (IsPlayingAnimation(i, animationName))
                return true;
        }
        return false;
    }

    public bool IsPlayingAnimation(int zMember, string animationName)
    {
        if (!this.isLoaded)
        {
            return true;
        }
        if (zMember >= this.CharactersSprites.Count)
        {
            return false;
        }
        else
        {
            var animator = this.SlotAnimators[zMember];
            if (animator.IsInTransition(0))
                return true;
            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            return stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1;
        }
    }


    public bool IsPlayingBossAnimation(string animationName)
    {
        if (!this.isLoaded)
        {
            return true;
        }

        if (this.MainCharacterAnimator.IsInTransition(0))
        {
            return true;
        }
        var stateInfo = this.MainCharacterAnimator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime < 1;
    }

    public void SetChatText(string message)
    {
        ChatText.text = message;
    }

    public void PlayTextAnimation(string animationName)
    {
        ChatTextAnimator.Play(animationName);
    }

    public void SetTextAlpha(float zAlpha)
    {
        var color = this.ChatText.color;
        color.a = zAlpha;
        this.ChatText.color = color;
    }

    //void OnEnable()
    //{
    //    Debug.Log("enable here");
    //}

    //void OnDisable()
    //{
    //    Debug.Log("disable here");
    //}
    public void PositionText(bool isRight)
    {
        ChatTextAnimator.enabled = false;
        if (isRight)
        {
            this.ChatText.alignment = LocalizationManager.IsRight2Left
                ? TextAlignmentOptions.TopRight
                : TextAlignmentOptions.TopLeft;
            ChatText.rectTransform.anchorMin = new Vector2(0, 0.5F);
            ChatText.rectTransform.anchorMax = new Vector2(0, 0.5F);
            ChatText.rectTransform.anchoredPosition = new Vector2(564, 101);
        }
        else
        {
            this.ChatText.alignment = LocalizationManager.IsRight2Left
                ? TextAlignmentOptions.BottomRight
                : TextAlignmentOptions.BottomLeft;
            ChatText.rectTransform.anchorMin = new Vector2(1, 0.5F);
            ChatText.rectTransform.anchorMax = new Vector2(1, 0.5F);
            ChatText.rectTransform.anchoredPosition = new Vector2(-608, -70);
        }
        this.ChatText.enabled = true;
    }
}
