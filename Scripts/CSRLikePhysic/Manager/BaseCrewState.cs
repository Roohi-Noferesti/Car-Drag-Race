using System;
using System.Collections.Generic;
using UnityEngine;

public class BaseCrewState
{
	public enum NarrativeCrewStateType
	{
		IntroductionFadeIn,
		TransitionWithinCrew,
		LogoFadeIn,
		FadeOutMembers,
		BossAndMember,
		BossTalk,
		ShowRaceButton,
		ShowNextButton,
		AgentPopup,
		AllInvisible,
		BossBanter,
		BossTalkFadeOut,
		DeactivateCrew,
		Delay,
		DismissScreen,
		FadeOut,
		FadeOutLeader,
		FadeOutText,
		IntroductionPan,
		IntroductionPanBack,
		LeaderCenter,
		LeaderCrossFadeOut,
		LeaderCrossGrayOut,
		LeaderDefeated,
		LeaderFadeOut,
		LeaderStrike,
		LogoFadeOut,
		MarkMemberAsDefeated,
		TierUnlocked,
		CreateCardShark,
		PrizeOMatic,
		FadeOutRaceTeamMember,
		FadeInRaceTeamMember,
		DestroyRaceTeamMember,
		CreateRaceTeamMember,
		RemoveLogo,
		NitroFadeOut,
		MysteryDonorTalkFadeOut,
		MysteryDonorTalk,
		MysteryDonorFadeOut,
		MysteryDonorFadeIn,
		Credits,
		CreateMysteryDonor,
		FadeInNewsText,
		FadeOutNewsText,
		LeaderSide,
		DismissScreenWithSuperNitrousCheck,
	    PlayAnimation
	}

	protected CrewProgressionScreen parentScreen;

	protected float timeInState;

	protected IGameState gameState = new GameStateFacade();

	private static Dictionary<BaseCrewState.NarrativeCrewStateType, Type> stateMapping = new Dictionary<BaseCrewState.NarrativeCrewStateType, Type>
	{
		{
			BaseCrewState.NarrativeCrewStateType.IntroductionFadeIn,
			typeof(CrewStateIntroductionFadeIn)
		},
		{
			BaseCrewState.NarrativeCrewStateType.TransitionWithinCrew,
			typeof(CrewStateTransitionWithinCrew)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeOutMembers,
			typeof(CrewStateFadeOutMembers)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LogoFadeIn,
			typeof(CrewStateLogoFadeIn)
		},
		{
			BaseCrewState.NarrativeCrewStateType.BossTalk,
			typeof(CrewStateBossTalk)
		},
		{
			BaseCrewState.NarrativeCrewStateType.BossAndMember,
			typeof(CrewStateBossAndMember)
		},
		{
			BaseCrewState.NarrativeCrewStateType.ShowRaceButton,
			typeof(CrewStateShowRaceButton)
		},
		{
			BaseCrewState.NarrativeCrewStateType.AgentPopup,
			typeof(CrewStateAgentPopup)
		},
		{
			BaseCrewState.NarrativeCrewStateType.AllInvisible,
			typeof(CrewStateAllInvisible)
		},
		{
			BaseCrewState.NarrativeCrewStateType.BossBanter,
			typeof(CrewStateBossBanter)
		},
		{
			BaseCrewState.NarrativeCrewStateType.BossTalkFadeOut,
			typeof(CrewStateBossTalkFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.DeactivateCrew,
			typeof(CrewStateDeactivateCrew)
		},
		{
			BaseCrewState.NarrativeCrewStateType.Delay,
			typeof(CrewStateDelay)
		},
		{
			BaseCrewState.NarrativeCrewStateType.DismissScreen,
			typeof(CrewStateDismissScreen)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeOut,
			typeof(CrewStateFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeOutLeader,
			typeof(CrewStateFadeOutLeader)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeOutText,
			typeof(CrewStateFadeOutText)
		},
		{
			BaseCrewState.NarrativeCrewStateType.IntroductionPan,
			typeof(CrewStateIntroductionPan)
		},
		{
			BaseCrewState.NarrativeCrewStateType.IntroductionPanBack,
			typeof(CrewStateIntroductionPanBack)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderCenter,
			typeof(CrewStateLeaderCenter)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderCrossFadeOut,
			typeof(CrewStateLeaderCrossFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderCrossGrayOut,
			typeof(CrewStateLeaderCrossGrayOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderDefeated,
			typeof(CrewStateLeaderDefeated)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderFadeOut,
			typeof(CrewStateLeaderFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderStrike,
			typeof(CrewStateLeaderStrike)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LogoFadeOut,
			typeof(CrewStateLogoFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.MarkMemberAsDefeated,
			typeof(CrewStateMarkMemberAsDefeated)
		},
		{
			BaseCrewState.NarrativeCrewStateType.ShowNextButton,
			typeof(CrewStateShowNextButton)
		},
		{
			BaseCrewState.NarrativeCrewStateType.TierUnlocked,
			typeof(CrewStateTierUnlocked)
		},
		{
			BaseCrewState.NarrativeCrewStateType.CreateCardShark,
			typeof(CrewStateCreateCardShark)
		},
		{
			BaseCrewState.NarrativeCrewStateType.PrizeOMatic,
			typeof(CrewStatePrizeOMatic)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeOutRaceTeamMember,
			typeof(CrewStateFadeOutRaceTeamMember)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeInRaceTeamMember,
			typeof(CrewStateFadeInRaceTeamMember)
		},
		{
			BaseCrewState.NarrativeCrewStateType.DestroyRaceTeamMember,
			typeof(CrewStateDestroyRaceTeamMember)
		},
		{
			BaseCrewState.NarrativeCrewStateType.CreateRaceTeamMember,
			typeof(CrewStateCreateRaceTeamMember)
		},
		{
			BaseCrewState.NarrativeCrewStateType.RemoveLogo,
			typeof(CrewStateRemoveLogo)
		},
		{
			BaseCrewState.NarrativeCrewStateType.NitroFadeOut,
			typeof(CrewStateNitroFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.MysteryDonorTalkFadeOut,
			typeof(CrewStateMysteryDonorTalkFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.MysteryDonorTalk,
			typeof(CrewStateMysteryDonorTalk)
		},
		{
			BaseCrewState.NarrativeCrewStateType.MysteryDonorFadeOut,
			typeof(CrewStateMysteryDonorFadeOut)
		},
		{
			BaseCrewState.NarrativeCrewStateType.MysteryDonorFadeIn,
			typeof(CrewStateMysteryDonorFadeIn)
		},
		{
			BaseCrewState.NarrativeCrewStateType.Credits,
			typeof(CrewStateCredits)
		},
		{
			BaseCrewState.NarrativeCrewStateType.CreateMysteryDonor,
			typeof(CrewStateCreateMysteryDonor)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeInNewsText,
			typeof(CrewStateFadeInNewsText)
		},
		{
			BaseCrewState.NarrativeCrewStateType.FadeOutNewsText,
			typeof(CrewStateFadeOutNewsText)
		},
		{
			BaseCrewState.NarrativeCrewStateType.LeaderSide,
			typeof(CrewStateLeaderSide)
		},
		{
			BaseCrewState.NarrativeCrewStateType.DismissScreenWithSuperNitrousCheck,
			typeof(CrewStateDismissScreenWithSuperNitrousCheck)
		},
	    {
	        BaseCrewState.NarrativeCrewStateType.PlayAnimation,
	        typeof(CrewStatePlayAnimation)
	    }

    };

	public BaseCrewState(CrewProgressionScreen zParentScreen)
	{
		this.Reset();
		this.parentScreen = zParentScreen;
	}

	public BaseCrewState(NarrativeSceneStateConfiguration config)
	{
		this.Reset();
		this.parentScreen = config.ParentScreen;
	}

	public virtual void OnEnter()
	{
	}

	public virtual bool OnMain()
	{
		this.timeInState += Time.deltaTime;
		return false;
	}

	public virtual void OnExit()
	{
	}

	private void Reset()
	{
		this.parentScreen = null;
		this.timeInState = 0f;
	}

	public static BaseCrewState MakeStateFromConfiguration(BaseCrewState.NarrativeCrewStateType type, NarrativeSceneStateConfiguration config)
	{
		if (!BaseCrewState.stateMapping.ContainsKey(type))
		{
			return null;
		}
		Type type2 = BaseCrewState.stateMapping[type];
		object[] args = new object[]
		{
			config
		};
		return (BaseCrewState)Activator.CreateInstance(type2, args);
	}
}
