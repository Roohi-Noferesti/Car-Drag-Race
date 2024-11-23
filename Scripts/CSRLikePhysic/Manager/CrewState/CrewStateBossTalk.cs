using System;
using UnityEngine;

public class CrewStateBossTalk : BaseCrewState
{
	private const float timeForState = 3.5f;

    private int leader1;

    private bool isRight;

    private float yOffset;

    private string text;

	private bool showButton;

	private bool nextButtonPressed;

	private bool hasShownButton;

	private float timeToShowButton = 0.25f;

	private bool hasMessage = true;

    private string textAnimationName;

    private bool isTierX;

    public CrewStateBossTalk(CrewProgressionScreen zParentScreen, int zLeader1, bool zIsRight, float zYOffset,
        string zText, bool zShowButton, string textAnimationName = "left")
        : base(zParentScreen)
    {
        this.leader1 = zLeader1;
        this.isRight = zIsRight;
        //this.yOffset = zYOffset;
        this.text = zText;
        this.showButton = zShowButton;
        this.textAnimationName = textAnimationName;
    }

    public CrewStateBossTalk(NarrativeSceneStateConfiguration config) : base(config)
	{
        this.leader1 = config.StateDetails.SlotIndex;
        this.isRight = config.StateDetails.IsBubbleMessagePositionRight;
        //this.yOffset = config.StateDetails.CharacterVerticalOffset;
        this.hasMessage = config.StateDetails.HasMessage;
		this.text = ((!this.hasMessage) ? string.Empty : config.StateDetails.GetTranslatedMessage(this.gameState));
		this.showButton = config.StateDetails.ShowButton;
	    this.textAnimationName = config.StateDetails.AnimationName;
	    isTierX = true;
	}

    private GameObject GetLeader()
	{
	    return this.parentScreen.charactersSlots[this.leader1].GetMainCharacterGameObject();
    }

	public override void OnEnter()
	{
        MainCharacterGraphic component = this.GetLeader().GetComponent<MainCharacterGraphic>();
        //if (this.hasMessage)
        //{
        //    component.ShowText(this.text, this.yOffset, this.isRight);
        //}
        this.parentScreen.charactersSlots[leader1].SetTextAlpha(1);
        this.parentScreen.charactersSlots[leader1].SetChatText(this.text);
	    if (isTierX)
	    {
	        this.parentScreen.charactersSlots[leader1].PositionText(isRight);
	    }
	    else
	    {
	        this.parentScreen.charactersSlots[leader1].PlayTextAnimation(textAnimationName);
	    }

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
            this.parentScreen.OnNextSpeechButtonPressed += OnNextSpeechButtonPressed;
            this.parentScreen.ShowNextSpeechButton();
            this.hasShownButton = true;
            return false;
        }
        else
        {
            if (this.nextButtonPressed)
            {
                parentScreen.OnNextSpeechButtonPressed -= OnNextSpeechButtonPressed;
                return this.nextButtonPressed;
            }
            return false;
        }
	}

    public override void OnExit()
	{
	}
}
