using System;
using UnityEngine;

public class CrewStateMysteryDonorTalk : BaseCrewState
{
	//private string text;

	private bool showButton;

	private bool nextButtonPressed;

	private bool hasShownButton;

	private float timeToShowButton = 0.25f;

	public CrewStateMysteryDonorTalk(CrewProgressionScreen zParentScreen, string zText) : base(zParentScreen)
	{
		//this.text = zText;
		this.showButton = true;
	}

	public CrewStateMysteryDonorTalk(NarrativeSceneStateConfiguration config) : base(config)
	{
		//this.text = config.StateDetails.GetTranslatedMessage(this.gameState);
		this.showButton = config.StateDetails.ShowButton;
	}

	public GameObject GetNitroContainer()
	{
		return this.parentScreen.GetContainer().transform.Find("MysteryDonor").gameObject;
	}

    //public NitroContainer GetNitroComponent()
    //{
    //    return this.GetNitroContainer().GetComponent<NitroContainer>();
    //}

	public override void OnEnter()
	{
        //NitroContainer nitroComponent = this.GetNitroComponent();
        //nitroComponent.Leader.ShowText(this.text, 0f, true);
		this.nextButtonPressed = false;
	}

	private void OnNextSpeechButtonPressed()
	{
		this.nextButtonPressed = true;
	}

	public override bool OnMain()
	{
		base.OnMain();
		if (!this.showButton)
		{
			return true;
		}
		if (!this.hasShownButton)
		{
			if (this.timeInState < this.timeToShowButton)
			{
				return false;
			}
			CrewProgressionScreen expr_38 = this.parentScreen;
			expr_38.OnNextSpeechButtonPressed = (Action)Delegate.Combine(expr_38.OnNextSpeechButtonPressed, new Action(this.OnNextSpeechButtonPressed));
			this.parentScreen.ShowNextSpeechButton();
			this.hasShownButton = true;
			return false;
		}
		else
		{
			if (this.nextButtonPressed)
			{
				CrewProgressionScreen expr_7E = this.parentScreen;
				expr_7E.OnNextSpeechButtonPressed = (Action)Delegate.Remove(expr_7E.OnNextSpeechButtonPressed, new Action(this.OnNextSpeechButtonPressed));
				return true;
			}
			return false;
		}
	}

	public override void OnExit()
	{
	}
}
