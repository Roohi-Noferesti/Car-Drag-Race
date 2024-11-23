using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine.UI;

public class EventPaneCrewBattle : MonoBehaviour
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

    private RaceEventData eventData;

    public bool isLoaded { get; private set; }

    public Image GetBorder(int zSlotIndex)
    {
        return this.SlotBorders[zSlotIndex];
    }

    public void Initialise(NarrativeSceneCharactersGroup zCharactersGroup, NarrativeSceneDetails zDetails)
    {
        int num = 1;
        int initializedMembers = 0;
        this.LogoController = zCharactersGroup.Logo.CreateLogo(this.LogoParent);
        this.LogoController.SetOptionalStrings(new string[]
        {
        });
        this.MainCharacter.gameObject.SetActive(false);
        this.MainCharacterAnimator = this.MainCharacter.GetComponent<Animator>();
        //Texture2D texture2D = Resources.Load("CharacterCards/Crew/crew_frame") as Texture2D;
        //GameObject original = Resources.Load("CharacterCards/Crew/CrewCross") as GameObject;
        foreach (NarrativeSceneCharacter current in zCharactersGroup.Members)
        {
            //int num2 = num;
            this.SlotCrosses.Add(gameObject);
            gameObject.SetActive(false);
            if (string.IsNullOrEmpty(current.PortraitTextureName))
            {
                initializedMembers++;
                if (initializedMembers > this.CharactersSprites.Count)
                {
                    this.InitialisationFinished(zDetails.IntroText, zCharactersGroup.Leader.Name,0,0);
                }
            }
            else
            {
            }
            num++;
        }
    }

    public void InitialisationFinished(string zIntroText, string zLeaderName, int cashPrize,int goldPrize)
    {
        for (int i = 0; i < this.CharactersSprites.Count; i++)
        {
            this.CharactersSprites[i].gameObject.SetActive(true);
        }
        this.MainCharacter.gameObject.SetActive(true);
        TextMeshProUGUI nameText = this.MainCharacter.transform.Find("NameText").GetComponent<TextMeshProUGUI>();
        nameText.text = LocalizationManager.GetTranslation(zLeaderName);

        var rewardText = this.MainCharacter.transform.Find("rewardText").GetComponent<TextMeshProUGUI>();
        var goldRewardText = this.MainCharacter.transform.Find("goldText").GetComponent<TextMeshProUGUI>();
        goldRewardText.gameObject.SetActive(goldPrize > 0);
        rewardText.text = CurrencyUtils.GetCashString(cashPrize);
        goldRewardText.text = CurrencyUtils.GetGoldStringWithIcon(goldPrize);
    }

    public void Initialise(eCarTier zTier, RaceEventData zRaceEvent)
    {
        foreach (var slotCross in SlotCrosses)
        {
            Destroy(slotCross.gameObject);
        }
        SlotCrosses.Clear();

        eventData = zRaceEvent;
        var eventOrder = zRaceEvent.GetProgressionRaceEventNumber();
        string text = CarTierHelper.TierToString[(int) zTier];
        this.MainCharacter.gameObject.SetActive(false);
        MainCharacterAnimator = this.MainCharacter.GetComponent<Animator>();
        TexturePack.RequestTextureFromBundle("CrewPortraitsTier" + text + ".Boss Card" + (zTier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":""),
            delegate(Texture2D tex) { this.MainCharacter.LoadProtrait(tex, null); });

        GameObject crewCrossObj = Resources.Load("CharacterCards/Crew/CrewCross") as GameObject;
        int crewMember = 1;
        int initializedMembers = 0;
        var crewEventGroup = eventData.GetTierEvent().CrewBattleEvents.RaceEventGroups[0];
        for (int i = 0; i < this.CharactersSprites.Count; i++)
        {
            bool thisIsForthMemberAndIsDefeated = i == 2 && eventOrder > 3;
            bool thisIsForthMemberAndIsNotDefeated = i == 2 && eventOrder == 3;
            var currentCharacter = this.CharactersSprites[i];
            var characterRoot = currentCharacter.transform.parent;
            var nameText = characterRoot.Find("NameText").GetComponent<TextMeshProUGUI>();
            nameText.text = LocalizationManager.GetTranslation(CrewChatter.GetMemberName((int) zTier, crewMember));
            var prizeIndex = eventOrder < 3 ? i : eventOrder;
            var cashPrize = crewEventGroup.RaceEvents[prizeIndex].RaceReward.CashPrize;
            var goldPrize = crewEventGroup.RaceEvents[prizeIndex].RaceReward.GoldPrize;
            var rewardText = characterRoot.Find("rewardText").GetComponent<TextMeshProUGUI>();
            var goldRewardText = characterRoot.Find("goldText").GetComponent<TextMeshProUGUI>();
            goldRewardText.gameObject.SetActive(goldPrize > 0 && thisIsForthMemberAndIsNotDefeated);
            rewardText.text = CurrencyUtils.GetCashString(cashPrize);
            rewardText.gameObject.SetActive(i >= eventOrder || thisIsForthMemberAndIsNotDefeated);
            goldRewardText.text = CurrencyUtils.GetGoldStringWithIcon(goldPrize);
            SlotAnimators.Add(characterRoot.GetComponent<Animator>());
            //nameText.gameObject.SetActive(false);
            var crewCrossInstance = UnityEngine.Object.Instantiate(crewCrossObj) as GameObject;
            crewCrossInstance.transform.SetParent(characterRoot, false);
            crewCrossInstance.transform.localPosition = Vector3.zero;
            this.SlotCrosses.Add(crewCrossInstance);
            crewCrossInstance.SetActive(false);
            //if we beat 2 first member or we beat third member for second time , then cross object should be visible
            if ((i<2 &&  i < eventOrder) || thisIsForthMemberAndIsDefeated)
            {
                this.CharactersSprites[i].color = new Color(0.28F, 0.28F, 0.28F, 1);
                this.SlotCrosses[i].SetActive(true);
            }
            this.CharactersSprites[i].transform.parent.Find("Outline").gameObject.SetActive(i == eventOrder || thisIsForthMemberAndIsNotDefeated);
            var currentCharacterTemp = currentCharacter;

            TexturePack.RequestTextureFromBundle("CrewPortraitsTier" + text + ".Crew "+crewMember + (zTier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":""),
                delegate(Texture2D tex)
                {
                    currentCharacterTemp.texture = tex;
                    initializedMembers++;
                    this.MainCharacter.transform.Find("Outline").gameObject.SetActive(eventOrder >= 4);
                    if (initializedMembers >= this.CharactersSprites.Count)
                    {
                        var bossIndex = eventOrder <= 3 ? 4 : eventOrder;
                        cashPrize = crewEventGroup.RaceEvents[bossIndex].RaceReward.CashPrize;
                        goldPrize = crewEventGroup.RaceEvents[bossIndex].RaceReward.GoldPrize;
                        this.InitialisationFinished("TEXT_ULTIMATE_GOAL_" + (int)zTier,
                            CrewChatter.GetLeaderName((int)zTier), cashPrize, goldPrize);
                    }
                });
            crewMember++;
        }
    }

    public void SetNumCharacterSlotsToCross(int zNumDefeated)
    {
        for (int i = 0; i < zNumDefeated; i++)
        {
            this.CharactersSprites[i].color = new Color(0.28F, 0.28F, 0.28F, 1);
            this.SlotCrosses[i].SetActive(true);
        }
    }

    public float GetTotalWidth()
    {
        return 3.6f + this.spacing*4f + 1.015f + this.spacing + 0.22f;
    }

    public GameObject GetMainCharacterGameObject()
    {
        return this.MainCharacter.gameObject;
    }

    public MainCharacterGraphic GetMainCharacterGraphic()
    {
        return this.MainCharacter;
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

    public void SetActiveSlots(int zSlotIndex, bool zActive)
    {
        if (zSlotIndex >= this.CharactersSprites.Count)
        {
            this.MainCharacter.gameObject.SetActive(zActive);
            this.SlotNames[4].gameObject.SetActive(zActive);
        }
        else
        {
            this.SlotBorders[zSlotIndex].gameObject.SetActive(zActive);
            this.CharactersSprites[zSlotIndex].gameObject.SetActive(zActive);
            this.SlotNames[zSlotIndex].gameObject.SetActive(zActive);
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
        }
        else
        {
            this.CharactersSprites[zMember].color = new Color(1f, 1f, 1f, zAlpha);
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


    public void PlayAnimation(int zMember, string animationName)
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
            return false;
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
            return false;
        }
        if (this.MainCharacterAnimator.IsInTransition(0))
            return true;
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
}
