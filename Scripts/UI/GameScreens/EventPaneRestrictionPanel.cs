using EventPaneRestriction;
using UnityEngine;

public class EventPaneRestrictionPanel : MonoBehaviour
{
	public EventPaneRestrictionBubble RestrictionBubble;

	public RuntimeTextButton RestrictionButton;

	private IRestriction activeRestriction;

	public bool IsRestrictionActive
	{
		get
		{
			return this.activeRestriction != null;
		}
	}

	public void Setup(RaceEventData raceData, bool fuelEnough)
	{
		this.activeRestriction = RestrictionHelper.GetActiveRestriction(raceData);
		this.CompleteSetup();
	}

    //public void Setup(MultiplayerEventData data)
    //{
    //    this.activeRestriction = RestrictionHelper.GetActiveRestriction(data);
    //    this.CompleteSetup();
    //}

	public void Setup(ModeInfo data)
	{
		this.activeRestriction = RestrictionHelper.GetActiveRestriction(data);
		this.CompleteSetup();
	}

	private void CompleteSetup()
	{
		if (!this.IsRestrictionActive)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		this.RestrictionBubble.ClearRestrictions();
        this.activeRestriction.AddRestrictionBubbleGraphics(this.RestrictionBubble);
		this.RestrictionBubble.Finalise();
	    this.RestrictionButton.SetText(this.activeRestriction.RestrictionButtonText(), false, true);
	}

	public void ForceDisableRestrictions()
	{
		this.activeRestriction = null;
		base.gameObject.SetActive(false);
	}

	public void AnimateRestriction()
	{
		//this.RestrictionBubble.AnimateRestriction();
	}

	public void ChangeAllTints(float alpha)
	{
		this.RestrictionBubble.ChangeAllTints(alpha);
        //this.RestrictionButton.ChangeAllAlphas(alpha);
	}

	public void NextPressed()
	{
	    if (activeRestriction != null)
	        this.activeRestriction.RestrictionButtonPressed();
	}
}
