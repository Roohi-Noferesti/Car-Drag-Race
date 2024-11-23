using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using UnityEngine;

[Serializable]
public class NarrativeStateDetails
{
	public int SlotIndex = -1;

	public int SecondarySlotIndex = -1;

	public int MemberIndex = -1;

	public int CrewLeaderStrikeTimes = -1;

	private string message = string.Empty;

	public ConditionallySelectedString ConditionalMessage = ConditionallySelectedString.CreateEmpty();

	public List<MessageOption> MessageOptions = new List<MessageOption>();

	public bool IsBubbleMessagePositionRight;

	public float CharacterVerticalOffset;

	public bool ShowButton;

	public bool HasMessage = true;

	public bool FadeTextBox = true;

	public float FadeInTime = 0.6f;

	public float TimeBetweenFades = 0.08f;

	public float Delay = 0.8f;

	public float TimeToFade = 1f;

	public float PanTimePerCrew = 4.5f;

	public float PercentageTimeInSlow = 0.925f;

	public float DistanceForSlow = 0.2f;

	public float Greyness;

	public float TimeToScale = 2.5f;

	public float StartScale = 1.6f;

	public float EndScale = 1f;

	public float FadeOutTime = 0.4f;

	public float TotalTime = 1f;

	public string ContainerString = string.Empty;

	public string CharCardString = string.Empty;

	public string CharacterName = string.Empty;

	public bool ChangeName = true;

	public float TimeToWait;

	public string WorldTourThemeToLoadID = string.Empty;

    public string AnimationName = string.Empty;

    public string Message
	{
		get
		{
            //Just in editor
		    if (!Application.isPlaying)
		    {
		        return message;
		    }
			return this.ConditionalMessage.GetText(new GameStateFacade()) ?? LocalizationManager.GetTranslation(this.message);
		}
		set
		{
			this.message = value;
		}
	}

	public string GetTranslatedMessage(IGameState gameState)
	{
		if (this.MessageOptions == null || this.MessageOptions.Count <= 0)
		{
			return this.Message;
		}
		MessageOption messageOption = this.MessageOptions.FirstOrDefault((MessageOption m) => m.Requirements.IsEligible(gameState));
		if (messageOption != null)
		{
			return messageOption.Message;
		}
		return string.Empty;
	}

	public void Initialise()
	{
		this.MessageOptions.ForEach(delegate(MessageOption m)
		{
			m.Initialise();
		});
	}
}
