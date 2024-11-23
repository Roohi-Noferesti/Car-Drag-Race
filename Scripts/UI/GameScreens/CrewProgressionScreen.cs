using DataSerialization;
using Metrics;
using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CrewProgressionScreen : ZHUDScreen
{
	public float spacingBetweenEachTier = 0.5f;

    public RawImage BackgroundTexture;

	public AnimationCurve CurveLinear;

	public AnimationCurve CurveS;

	public AnimationCurve CurveEaseOut;

	public RuntimeTextButton RaceButton;

    public RuntimeTextButton NextButton;

    public RuntimeTextButton NextSpeechButton;

	public TextMeshProUGUI NewsItem;

	public Action OnNextSpeechButtonPressed;

	private List<List<BaseCrewState>> States = new List<List<BaseCrewState>>();

	private int currentState;

	public float offsetX;

    public List<NarrativeSceneCharactersContainer> charactersSlots = new List<NarrativeSceneCharactersContainer>();

    public bool ShouldTriggerHighStakesChallenge;

	public eCarTier TierOfHighStakesTrigger = eCarTier.MAX_CAR_TIERS;

	private bool isLoaded;

	private List<int> characterSlotsToLoad = new List<int>();

    public static int Crew = -1;

    public static string BackgroundImageText;

    private float _timeStarted;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.CrewProgression;
		}
	}

	public float WidthOfEachCrew
	{
		get;
		private set;
	}

    public override void OnCreated(bool zAlreadyOnStack)
    {
        if (!string.IsNullOrEmpty(BackgroundImageText))
        {
	        if (BuildType.IsAppTuttiBuild && BackgroundImageText.ToLower().Contains("italia-logo") && !BackgroundImageText.Contains("apptutti"))
		        BackgroundImageText += "_apptutti";
            BackgroundTexture.texture = ResourceManager.GetAsset<Texture2D>("textures/"+ BackgroundImageText);
        }
        else if (Crew != -1)
            BackgroundTexture.texture = ResourceManager.GetAsset<Texture2D>("textures/BG_Urban_Crew" + (Crew + 1));

        Transform container = base.transform.Find("Container");
        GameObject original = Resources.Load("CharacterCards/Crew/NarrativeSceneCharactersContainer") as GameObject;
        float num = -0.22f;
        int numberOfCrews = 6;
        for (int i = 0; i < numberOfCrews; i++)
        {
            var narrativeSceneCharactersContainerInstance = Instantiate(original);
            var narrativeSceneCharactersContainer = narrativeSceneCharactersContainerInstance.GetComponent<NarrativeSceneCharactersContainer>();
            narrativeSceneCharactersContainerInstance.transform.SetParent(container, false);
            num += narrativeSceneCharactersContainer.GetTotalWidth() + this.spacingBetweenEachTier;
            this.charactersSlots.Add(narrativeSceneCharactersContainer);
            narrativeSceneCharactersContainerInstance.SetActive(false);
        }
        this.WidthOfEachCrew = this.charactersSlots[0].GetTotalWidth() + this.spacingBetweenEachTier;
        this.RaceButton.CurrentState = BaseRuntimeControl.State.Hidden;
        this.NextButton.CurrentState = BaseRuntimeControl.State.Hidden;
        this.NextSpeechButton.CurrentState = BaseRuntimeControl.State.Hidden;
        MenuAudio.Instance.FadeMusicToCrewProgression();
        this.NewsItem.gameObject.SetActive(false);
	}

	public void InitialiseAllCharactersUntilCrew(int zCrew)
	{
		for (int i = 0; i < 6; i++)
		{
			this.LoadCharactersForCrew(i, null);
		}
	}

	public override void OnDeactivate()
	{
		MenuAudio.Instance.FadeMusicFromCrewProgression();
        CleanDownManager.Instance.OnCrewProgressionDeactivate();
	}

	protected override void OnDestroy()
	{
        this.charactersSlots.Clear();
		foreach (List<BaseCrewState> current in this.States)
		{
			current.Clear();
		}
		this.States.Clear();
		this.States = null;
        base.OnDestroy();
	}

	public Transform GetContainer()
	{
		return base.transform.Find("Container");
	}

	private bool OnSetup()
	{
		foreach (int current in this.characterSlotsToLoad)
		{
			if (!this.charactersSlots[current].isLoaded)
			{
				return false;
			}
		}
		this.currentState = 0;
		if (this.States.Count > 0)
		{
			foreach (BaseCrewState current2 in this.States[0])
			{
				current2.OnEnter();
			}
		}
		this.UpdateBasedOnOffset();
		this.isLoaded = true;
		this.characterSlotsToLoad.Clear();
        _timeStarted = Time.time;
        return true;
	}

	public void LoadCharactersForWorldTour(NarrativeSceneStateConfiguration zScene)
	{
		this.isLoaded = false;
		for (int j = 0; j < zScene.CharactersDetails.CharacterGroups.Count; j++)
		{
			this.characterSlotsToLoad.Add(j);
		}
		zScene.CharactersDetails.CharacterGroups.ForEachWithIndex(delegate(NarrativeSceneCharactersGroup group, int crew)
		{
			this.charactersSlots[crew].Initialise(group, zScene.SceneDetails, this, delegate
			{
				if (group.HideMembers)
				{
					for (int i = 0; i < 3; i++)
					{
						this.charactersSlots[crew].SetActiveSlots(i, false);
					}
				}
				this.charactersSlots[crew].SetNumCharacterSlotsToCross(group.CrewDefeatedCount);
				if (group.SetStrikeFramesActive)
				{
                    this.charactersSlots[crew].GetMainCharacterGraphic().SetStrikeFramesActive();
                }
			    charactersSlots[crew].gameObject.SetActive(true);
                this.OnSetup();
			});
		});
	}

	public void LoadCharactersForCrew(int zCrew, Action zCallback = null)
	{
		this.isLoaded = false;
		this.characterSlotsToLoad.Add(zCrew);
		this.charactersSlots[zCrew].Initialise((eCarTier)zCrew, this, delegate
		{
			if (zCallback != null)
			{
				zCallback();
			}
			this.OnSetup();
		});
	}

	public void UnloadCharactersForCrew(int zCrew)
	{
		this.charactersSlots[zCrew].Uninitialise();
		CleanDownManager.Instance.OnCrewProgressionUnloadCrew();
	}

	public void SetupIntroduction(int zCrew)
	{
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStatePlayAnimation(this, zCrew, 0, "Crew_Intro"));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateDelay(this, 1));
	    for (int i = 0; i < 2; i++)
	    {
	        this.States.Add(list = new List<BaseCrewState>());
            list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, CrewChatter.GetCrewIntroPreRace(zCrew, i+1), true));
	        this.States.Add(list = new List<BaseCrewState>());
	        list.Add(new CrewStateBossTalkFadeOut(this, zCrew));
	        list.Add(new CrewStateDelay(this, 1));
	    }

	    this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateFadeOutMembers(this, zCrew, 0, 0.6f, 0.08f));
        list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, CrewChatter.GetCrewPreRace(zCrew, 1), true));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateMemberTalk(this,Crew, 0, CrewChatter.GetCrewMemberPreRace(zCrew, 1)));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateTextAlignment(this, Crew, "right"));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateShowRaceButton(this));
        this.LoadCharactersForCrew(zCrew, null);
	}

	public void SetupRaceAgainstCrewMember(int zCrew, int zMember)
	{
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateIntroductionFadeIn(this, zCrew,0));
        //list.Add(new CrewStateTransitionWithinCrew(this, zCrew, zMember));
        //list.Add(new CrewStateLogoFadeIn(this, zCrew));
        this.States.Add(list = new List<BaseCrewState>());
        int fadeOutMemberIndex = zMember == 3 ? 2 : zMember;
        list.Add(new CrewStateFadeOutMembers(this, zCrew, fadeOutMemberIndex, 0.6f, 0.08f));
        list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, CrewChatter.GetCrewPreRace(zCrew, zMember + 1), true));
        //list.Add(new CrewStateBossAndMember(this, zCrew, zMember));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateMemberTalk(this,Crew, zMember, CrewChatter.GetCrewMemberPreRace(zCrew, zMember + 1)));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateTextAlignment(this, Crew, "right"));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateShowRaceButton(this));
        this.LoadCharactersForCrew(zCrew, delegate
		{
			this.charactersSlots[zCrew].SetNumCharacterSlotsToCross(zMember);
		});
	}

	public void SetupForNarrativeScene(string sceneID)
	{
		NarrativeScene scene = null;
		if (TierXManager.Instance.GetNarrativeScene(sceneID, out scene))
		{
			this.SetupForNarrativeScene(scene);
		}
	}

	public void SetupForNarrativeScene(NarrativeScene scene)
	{
		IGameState gs = new GameStateFacade();
		NarrativeSceneStateConfiguration narrativeSceneStateConfiguration = new NarrativeSceneStateConfiguration();
		narrativeSceneStateConfiguration.ParentScreen = this;
		narrativeSceneStateConfiguration.SceneDetails = scene.SceneDetails;
		narrativeSceneStateConfiguration.CharactersDetails = scene.CharactersDetails;
		if (!string.IsNullOrEmpty(scene.SceneDetails.Background))
		{
			//BackgroundManager.BackgroundType zType = (BackgroundManager.BackgroundType)((int)Enum.Parse(typeof(BackgroundManager.BackgroundType), scene.SceneDetails.Background));
            //ScreenManager.Instance.SetupBackground(zType);
		}
		if (scene.Lines.Count > 0)
		{
			for (NarrativeSceneLine narrativeSceneLine = scene.Lines[0]; narrativeSceneLine != null; narrativeSceneLine = scene.GetSceneLineFromResponse(narrativeSceneLine.GetResponse(gs)))
			{
				List<BaseCrewState> list;
				foreach (NarrativeStateDataGroup current in narrativeSceneLine.PreStates)
				{
					this.States.Add(list = new List<BaseCrewState>());
					foreach (NarrativeStateData current2 in current.States)
					{
						narrativeSceneStateConfiguration.StateDetails = current2.StateDetails;
						list.Add(BaseCrewState.MakeStateFromConfiguration(current2.EnumType, narrativeSceneStateConfiguration));
					}
				}
				this.States.Add(list = new List<BaseCrewState>());
				narrativeSceneStateConfiguration.StateDetails = narrativeSceneLine.StateData.StateDetails;
				list.Add(BaseCrewState.MakeStateFromConfiguration(narrativeSceneLine.StateData.EnumType, narrativeSceneStateConfiguration));
				foreach (NarrativeStateDataGroup current3 in narrativeSceneLine.PostStates)
				{
					this.States.Add(list = new List<BaseCrewState>());
					foreach (NarrativeStateData current4 in current3.States)
					{
						narrativeSceneStateConfiguration.StateDetails = current4.StateDetails;
						list.Add(BaseCrewState.MakeStateFromConfiguration(current4.EnumType, narrativeSceneStateConfiguration));
					}
				}
			}
		}
		this.LoadCharactersForWorldTour(narrativeSceneStateConfiguration);
	}

	public void SetupCrewMemberDefeated(int zCrew, int zMember)
	{
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateIntroductionFadeIn(this, zCrew,0));
        //list.Add(new CrewStateTransitionWithinCrew(this, zCrew, zMember));
        //list.Add(new CrewStateLogoFadeIn(this, zCrew));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateFadeOutMembers(this, zCrew, zMember, 0.6f, 0.08f));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateMarkMemberAsDefeated(this, zCrew, zMember));
        //this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateBossAndMember(this, zCrew, zMember));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, CrewChatter.GetCrewWonRace(zCrew, zMember + 1), false));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateShowNextButton(this));
		this.LoadCharactersForCrew(zCrew, delegate
		{
			this.charactersSlots[zCrew].SetNumCharacterSlotsToCross(zMember);
		});
	}

    public void SetupRaceAgainstLeader(int zCrew, int zBossRaceIndex)
    {
        List<BaseCrewState> list;
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateIntroductionFadeIn(this, zCrew, 0));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStatePlayAnimation(this, zCrew, 0, "Crew_Go"));
        //list.Add(new CrewStateTransitionWithinCrew(this, zCrew, 3));
        //list.Add(new CrewStateLogoFadeIn(this, zCrew));
        //this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateFadeOutMembers(this, zCrew, -1, 0.6f, 0.08f));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateLeaderStrikeActive(this,zCrew));
        for (int i = 0; i < zBossRaceIndex; i++)
        {
            list.Add(new CrewStateLeaderStrike(this, zCrew, i, true));
        }
        //list.Add(new CrewStateDelay(this, 0.5F));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, CrewChatter.GetBossPreRace(zCrew, zBossRaceIndex + 1),
            false, "right"));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateShowRaceButton(this));
        this.LoadCharactersForCrew(zCrew, delegate
        {
            this.charactersSlots[zCrew].SetNumCharacterSlotsToCross(4);
        });
    }

    public void SetupLeaderDefeatedStrike(int zCrew, int zTimes)
	{
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
		for (int i = 0; i < zTimes; i++)
		{
			list.Add(new CrewStateLeaderStrike(this, zCrew, i, true));
		}
        list.Add(new CrewStatePlayAnimation(this, zCrew, -1, "Crew_Enter2"));
        //list.Add(new CrewStateIntroductionFadeIn(this, zCrew,0));
        //list.Add(new CrewStateTransitionWithinCrew(this, zCrew, 3));
        //list.Add(new CrewStateLogoFadeIn(this, zCrew));
		this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateFadeOutMembers(this, zCrew, -1, 0.4f, 0.08f));
        list.Add(new CrewStateLeaderStrikeActive(this,zCrew));
        list.Add(new CrewStateDelay(this, 1));
        this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateLeaderStrike(this, zCrew, zTimes));
        string bossWinRace = CrewChatter.GetBossWinRace(zCrew, zTimes + 1);
		this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, bossWinRace, false,"right"));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateDelay(this, 1.7f));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateShowNextButton(this));
		this.LoadCharactersForCrew(zCrew, delegate
		{
			this.charactersSlots[zCrew].SetNumCharacterSlotsToCross(3);
            //this.charactersSlots[zCrew].GetMainCharacterGraphic().SetStrikeFramesActive();
		});
	}

	public void SetupLeaderDefeated(int zCrew, int zTimes)
	{
		Log.AnEvent(Events.TierCompleted);
		//int leader2 = zCrew + 1;
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
        for (int i = 0; i < zTimes; i++)
        {
            list.Add(new CrewStateLeaderStrike(this, zCrew, i, true));
        }
        list.Add(new CrewStatePlayAnimation(this, zCrew, -1, "Crew_Enter2"));
        //list.Add(new CrewStateIntroductionFadeIn(this, zCrew, 0));
        //list.Add(new CrewStateTransitionWithinCrew(this, zCrew, 3));
        //list.Add(new CrewStateLogoFadeIn(this, zCrew));
        //this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateFadeOutMembers(this, zCrew, -1, 0.4f, 0.08f));
        #region Old State
        //int num = 1;
        //if (zCrew == 2)
        //{
        //    num = 0;
        //}
        //if (leader2 < 5)
        //{
        //    this.States.Add(list = new List<BaseCrewState>());
        //    list.Add(new CrewStateBossBanter(this, zCrew, leader2, 0f));
        //    int num2 = 1;
        //    while (CrewChatter.DoesBossDefeatedMessageExist(zCrew, num2))
        //    {
        //        string bossDefeatedMessage = CrewChatter.GetBossDefeatedMessage(zCrew, num2);
        //        this.States.Add(list = new List<BaseCrewState>());
        //        if (num2 % 2 == num)
        //        {
        //            list.Add(new CrewStateBossTalk(this, zCrew, true, 0f, bossDefeatedMessage, true));
        //            this.States.Add(list = new List<BaseCrewState>());
        //            list.Add(new CrewStateBossTalkFadeOut(this, zCrew));
        //        }
        //        else
        //        {
        //            list.Add(new CrewStateBossTalk(this, leader2, false, 0f, bossDefeatedMessage, true));
        //            this.States.Add(list = new List<BaseCrewState>());
        //            list.Add(new CrewStateBossTalkFadeOut(this, leader2));
        //        }
        //        num2++;
        //    }
        //    list.Add(new CrewStateLeaderDefeated(this, zCrew));
        //    list.Add(new CrewStateFadeOutLeader(this, zCrew));
        //    list.Add(new CrewStateLeaderCrossGrayOut(this, zCrew));
        //    this.States.Add(list = new List<BaseCrewState>());
        //    list.Add(new CrewStateDelay(this, 1.7f));
        //}
        #endregion

        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateLeaderStrikeActive(this,zCrew));
        list.Add(new CrewStateDelay(this, 1));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateLeaderStrike(this, zCrew, zTimes));
        string bossWinRace = CrewChatter.GetBossWinRace(zCrew, zTimes + 1);
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateBossTalk(this, zCrew, false, 0f, bossWinRace, false,"right"));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateDelay(this, 1.7f));
        this.States.Add(list = new List<BaseCrewState>());
        list.Add(new CrewStateShowNextButton(this));

        //this.ShouldTriggerHighStakesChallenge = true;
        //this.TierOfHighStakesTrigger = (eCarTier)zCrew;
        //this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateTierUnlocked(this));
		this.LoadCharactersForCrew(zCrew, delegate
		{
			this.charactersSlots[zCrew].SetNumCharacterSlotsToCross(3);
		});
        //if (leader2 < 5)
        //{
        //    this.LoadCharactersForCrew(leader2, delegate
        //    {
        //        for (int i = 0; i < 4; i++)
        //        {
        //            this.charactersSlots[leader2].SetActiveSlots(i, false);
        //        }
        //    });
        //}
	}

	public void SetupLeaderErrolDefeated(int zTimes)
	{
		int num = 1;
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateAllInvisible(this));
		list.Add(new CrewStateLogoFadeIn(this, 4));
		Log.AnEvent(Events.TierCompleted);
		for (int i = 0; i <= 4; i++)
		{
			bool flag = i == 4;
			this.States.Add(list = new List<BaseCrewState>());
			list.Add(new CrewStateLeaderCenter(this, i, 0f));
            string bossDefeatedMessage = CrewChatter.GetBossDefeatedMessage(4, num);
			this.States.Add(list = new List<BaseCrewState>());
            list.Add(new CrewStateBossTalk(this, i, true, 0f, bossDefeatedMessage, true));
			if (!flag)
			{
				this.States.Add(list = new List<BaseCrewState>());
				list.Add(new CrewStateLeaderFadeOut(this, i));
			}
			num++;
		}
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateLeaderDefeated(this, 4));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateFadeOutLeader(this, 4));
		list.Add(new CrewStateLeaderCrossGrayOut(this, 4));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateDelay(this, 1f));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateAgentPopup(this));
		list.Add(new CrewStateDeactivateCrew(this, 4));
		list.Add(new CrewStateRemoveLogo(this));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateCreateMysteryDonor(this, "CharacterCards/Crew/NitroContainer", "AdvisorPortraits.Nitro"));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorFadeIn(this, 1f));
		this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateMysteryDonorTalk(this, string.Format(LocalizationManager.GetTranslation("TEXT_POPUPS_ENDGAME_02"), CurrencyUtils.GetColouredCashString(25000))));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
		this.States.Add(list = new List<BaseCrewState>());
        //list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_POPUPS_ENDGAME_03")));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorFadeOut(this, 2f, true));
		for (int j = 4; j <= 8; j++)
		{
			this.States.Add(list = new List<BaseCrewState>());
            //list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_POPUPS_ENDGAME_0" + j)));
			this.States.Add(list = new List<BaseCrewState>());
			list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
		}
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateNitroFadeOut(this, 2f));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateDelay(this, 0.4f));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateDismissScreen(this));
		this.ShouldTriggerHighStakesChallenge = true;
		this.TierOfHighStakesTrigger = eCarTier.TIER_5;
		for (int k = 0; k < 4; k++)
		{
			this.LoadCharactersForCrew(k, null);
		}
		this.LoadCharactersForCrew(4, delegate
		{
			this.charactersSlots[4].SetNumCharacterSlotsToCross(3);
		});
	}

	private void GetRaceTeamMemberFromIndex(int inIndex, out string outName, out string outFilename)
	{
		switch (inIndex)
		{
		case 1:
			outFilename = "hireteam_character_engine";
			outName = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_ENGINE");
			return;
		case 2:
			outFilename = "hireteam_character_nos";
			outName = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_NITROUS");
			return;
		case 3:
			outFilename = "hireteam_character_tyre";
			outName = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_TYRES");
			return;
		case 4:
			outFilename = "hireteam_character_pr";
			outName = LocalizationManager.GetTranslation("TEXT_NAME_CONSUMABLES_PRAGENT");
			return;
		default:
			outFilename = "Frankie_Card_No_Frame";
			outName = LocalizationManager.GetTranslation("TEXT_NAME_FRANKIE");
			return;
		}
	}

	public void SetupRaceTeamIntroduction()
	{
		int i;
		string characterName;
		string str;
		List<BaseCrewState> list;
		for (i = 0; i < 5; i++)
		{
			this.GetRaceTeamMemberFromIndex(i, out characterName, out str);
			this.States.Add(list = new List<BaseCrewState>());
			list.Add(new CrewStateCreateRaceTeamMember(this, "CharacterCards/StargazerContainer", "CharacterCards/" + str, characterName));
			list.Add(new CrewStateFadeInRaceTeamMember(this, 1f));
			this.States.Add(list = new List<BaseCrewState>());
			list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_RACE_TEAM_INTRO_" + i)));
			this.States.Add(list = new List<BaseCrewState>());
			list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
			list.Add(new CrewStateFadeOutRaceTeamMember(this, 0.5f));
			this.States.Add(list = new List<BaseCrewState>());
			list.Add(new CrewStateDestroyRaceTeamMember(this));
		}
		this.GetRaceTeamMemberFromIndex(i, out characterName, out str);
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateCreateRaceTeamMember(this, "CharacterCards/StargazerContainer", "CharacterCards/" + str, characterName));
		list.Add(new CrewStateFadeInRaceTeamMember(this, 1f));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_RACE_TEAM_INTRO_" + i)));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
		i++;
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_RACE_TEAM_INTRO_" + i)));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateDismissScreen(this));
		this.OnSetup();
	}

	public void SetupCardShark()
	{
		List<BaseCrewState> list;
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateCreateCardShark(this, "CharacterCards/StargazerContainer", "CharacterCards/Frankie_Stargazer_card"));
		list.Add(new CrewStateMysteryDonorFadeIn(this, 1f));
		int num = UnityEngine.Random.Range(1, 23);
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_CARDSHARK_CHATTER_" + num + "_1")));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorFadeOut(this, 0.5f, false));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalk(this, LocalizationManager.GetTranslation("TEXT_CARDSHARK_CHATTER_" + num + "_2")));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateMysteryDonorTalkFadeOut(this));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStateNitroFadeOut(this, 1f));
		this.States.Add(list = new List<BaseCrewState>());
		list.Add(new CrewStatePrizeOMatic(this));
		this.OnSetup();
	}

	protected override void Update()
	{
		if (!this.isLoaded)
		{
			return;
		}

        if (Time.time - _timeStarted < 1)
        {
            return;
        }
		int count = this.States.Count;
		if (this.currentState < count)
		{
			bool flag = true;
			foreach (BaseCrewState current in this.States[this.currentState])
			{
				if (!current.OnMain())
				{
					flag = false;
				}
			}
			if (flag)
			{
				foreach (BaseCrewState current2 in this.States[this.currentState])
				{
					current2.OnExit();
                    if (ScreenManager.Instance.CurrentScreen != ScreenID.CrewProgression)
                    {
                        return;
                    }
                }
				this.currentState++;
				if (this.currentState < count && this.isLoaded)
				{
					foreach (BaseCrewState current3 in this.States[this.currentState])
					{
						current3.OnEnter();
					}
				}
				return;
			}
		}
		this.UpdateBasedOnOffset();
	}

	private void UpdateBasedOnOffset()
	{
		//Transform transform = base.transform.FindChild("Container");
		//Transform transform2 = base.transform.FindChild("CenterLeft");
        //float x = GameObjectHelper.To2DP(this.offsetX);
        //transform.transform.localPosition = transform2.localPosition + new UnityEngine.Vector3(x, 0f, 0f);
	}

	public void ShowRaceButton()
	{
		this.RaceButton.CurrentState = BaseRuntimeControl.State.Active;
	}

	public void ShowNextButton()
	{
		this.NextButton.CurrentState = BaseRuntimeControl.State.Active;
	}

	public void ShowNextSpeechButton()
	{
		this.NextSpeechButton.CurrentState = BaseRuntimeControl.State.Active;
	}

	public void OnRacePressed()
	{
        ScreenManager.Instance.PopScreen();
        if (CrewProgressionSetup.GetCrew(RaceEventInfo.Instance.CurrentEvent) == 0 && RaceEventInfo.Instance.CurrentEvent.IsFirstCrewMemberRace() && !PlayerProfileManager.Instance.ActiveProfile.AttemptedFirstCrewRace)
        {
            PlayerProfileManager.Instance.ActiveProfile.AttemptedFirstCrewRace = true;
            Log.AnEvent(Events.PriorTo1stCrewRace);
        }
        if (RaceEventInfo.Instance.CurrentEvent.IsRelay)
        {
            ScreenManager.Instance.PushScreen(ScreenID.RelayResults);
            return;
        }
        PinDetail worldTourPinPinDetail = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
        if (worldTourPinPinDetail == null || !worldTourPinPinDetail.ActivateVSLoadingScreen())
        {
            ScreenManager.Instance.PushScreen(ScreenID.VS);
            //SceneManagerFrontend.ButtonStart();
        }
	}

	public void OnNextPressed()
	{
        var showTierUnlockScreen = false;
        var currentEvent = RaceEventInfo.Instance.CurrentEvent;
	    if (currentEvent.IsBossRace())
	    {
	        var crewBattleEvents = currentEvent.Parent as CrewBattleEvents;
	        if (crewBattleEvents.NumOfEvents() - crewBattleEvents.NumEventsComplete() == 0
	            && currentEvent.GetTierEvent().GetCarTier() != eCarTier.TIER_5)
	        {
	            showTierUnlockScreen = true;
	        }
	    }

	    if (showTierUnlockScreen)
	    {
	        ScreenManager.Instance.PushScreenWithFakedHistory(ScreenID.TierUnlocked
	            , new[] { ScreenID.Home,ScreenID.Workshop});
	    }
	    else
	    {
            ScreenManager.Instance.PopScreen();
        }
	}

	public void OnNextSpeechPressed()
	{
		if (this.OnNextSpeechButtonPressed != null)
		{
			this.OnNextSpeechButtonPressed();
		}
		this.NextSpeechButton.CurrentState = BaseRuntimeControl.State.Hidden;
	}

	protected override void OnEnterPressed()
	{
		if (this.RaceButton.CurrentState == BaseRuntimeControl.State.Active)
		{
			this.OnRacePressed();
		}
		else if (this.NextButton.CurrentState == BaseRuntimeControl.State.Active)
		{
			this.OnNextPressed();
		}
		else if (this.NextSpeechButton.CurrentState == BaseRuntimeControl.State.Active)
		{
			this.OnNextSpeechPressed();
		}
	}

    public void PlayAnimation(string animationName)
    {
        Animator.Play(animationName);
    }

    public bool IsAnimationFinished(string animationName)
    {
        if (Animator.IsInTransition(0))
            return false;
        var stateInfo = Animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(animationName) && stateInfo.normalizedTime >= 1;
    }
}
