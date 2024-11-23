using System;
using System.Collections.Generic;
using I2.Loc;
using KingKodeStudio;
using UnityEngine;

public abstract class BaseIAPOffersController : MonoBehaviour
{
	public TimeSpan AvailabilityTimer;

	public DateTime FirstSeenTime;

	public TimeSpan ValidityDuration;

	public string OfferItem;

	public int CurrentOfferDiscount;

	private DateTime expireTime;

	private BubbleMessage Message;

	private string lastMessage = string.Empty;

	private bool IsShowingMessage;

	protected bool OfferActive;

	protected List<ScreenID> ValidBubbleMessageScreens = new List<ScreenID>();

	private void Awake()
	{
		this.IsShowingMessage = false;
		this.OfferActive = false;
	}

	public bool IsOfferActive()
	{
		return this.OfferActive;
	}

	private void Update()
	{
		this.expireTime = this.FirstSeenTime.Add(this.ValidityDuration);
		this.AvailabilityTimer = this.expireTime.Subtract(GTDateTime.Now);
		if (this.ValidBubbleMessageScreens.Contains(ScreenManager.Instance.CurrentScreen) && this.OfferActive)
		{
			this.StartBubbleMessage();
		}
		else
		{
			this.DismissBubbleMessage();
		}
		if (this.Message != null && this.OfferActive)
		{
			string timeRemainingMessage = this.GetTimeRemainingMessage();
			if (!timeRemainingMessage.Equals(this.lastMessage))
			{
				if (timeRemainingMessage.Length > this.lastMessage.Length)
				{
					if (!GarageCameraManager.Instance.IsZoomedIn)
					{
						this.DismissBubbleMessage();
						this.StartBubbleMessage();
						this.lastMessage = timeRemainingMessage;
					}
				}
				else
				{
					this.Message.Text.text = timeRemainingMessage;
					this.lastMessage = timeRemainingMessage;
				}
			}
		}
	}

	public void DisplayOfferIcon()
	{
        //CommonUI.Instance.NavBar.StarterPackIcon.gameObject.SetActive(true);
		this.OfferActive = true;
	}

	public void HideOfferIcon()
	{
        //CommonUI.Instance.NavBar.StarterPackIcon.gameObject.SetActive(false);
		this.OfferActive = false;
	}

	public void StartBubbleMessage()
	{
		if (this.IsShowingMessage || !this.ValidBubbleMessageScreens.Contains(ScreenManager.Instance.CurrentScreen) || AppStore.Instance.ShouldHideIAPInterface)
		{
			return;
		}
		
        Vector3 position = CommonUI.Instance.ShopButtonPoint.rectTransform().position + new Vector3(0f, -0.05f, 0f);
        if (this.Message == null)
        {
	        this.Message = BubbleManager.Instance.ShowMessage(this.GetTimeRemainingMessage(), true, position,
		        BubbleMessage.NippleDir.UP, 1f, BubbleMessageConfig.ThemeStyle.SMALL,
		        BubbleMessageConfig.PositionType.BOX_RELATIVE, 22);
            this.Message.GetParentTransform().parent = CommonUI.Instance.ShopButtonPoint.rectTransform();
            this.IsShowingMessage = true;
        }
	}

	public void DismissBubbleMessage()
	{
		if (!this.IsShowingMessage)
		{
			return;
		}
		if (this.Message != null)
		{
			this.Message.Dismiss();
			this.Message = null;
			this.IsShowingMessage = false;
		}
	}

	public virtual bool TimerHasExpired()
	{
		return this.AvailabilityTimer <= TimeSpan.Zero;
	}

	public virtual string GetTimeRemainingMessage()
	{
		int days = this.AvailabilityTimer.Days;
		int hours = this.AvailabilityTimer.Hours;
		int minutes = this.AvailabilityTimer.Minutes;
		int seconds = this.AvailabilityTimer.Seconds;
		string result;
		if (this.TimerHasExpired())
		{
		    result = LocalizationManager.GetTranslation("TEXT_STARTER_PACK_LAST_CHANCE");
		}
		else if (days != 0)
		{
			result = string.Format(LocalizationManager.GetTranslation("TEXT_STARTER_PACK_DAYS_HOURS"), days, hours);
		}
		else if (hours != 0)
		{
			result = string.Format(LocalizationManager.GetTranslation("TEXT_STARTER_PACK_HOURS_MINUTES"), hours, minutes);
		}
		else
		{
			result = string.Format(LocalizationManager.GetTranslation("TEXT_STARTER_PACK_MINUTES_SECONDS"), minutes, seconds);
		}
		return result;
	}

	public string GetCancelText()
	{
		if (this.TimerHasExpired())
		{
			return LocalizationManager.GetTranslation("TEXT_BUTTON_MISS_OUT");
		}
		return LocalizationManager.GetTranslation("TEXT_BUTTON_MAYBE_LATER");
	}

	public abstract void CleanUp();
}
