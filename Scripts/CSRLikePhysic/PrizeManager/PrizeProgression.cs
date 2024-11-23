using System;

public abstract class PrizeProgression
{
	public abstract string LocalisationTag
	{
		get;
	}

	public static void AddProgress(PrizeProgressionType type, float quantity)
	{
        if (MultiplayerUtils.SelectedMultiplayerMode != MultiplayerMode.EVENT)
        {
            return;
        }
        MultiplayerEventData data = MultiplayerEvent.Saved.Data;
        if (data == null || data.PrizeProgression.TypeEnum != type)
        {
            return;
        }
        float maxMilestone = data.GetMaxMilestone();
        float progression = MultiplayerEvent.Saved.GetProgression();
        if (progression >= maxMilestone)
        {
            return;
        }
        if (progression + quantity >= maxMilestone)
        {
            MultiplayerEvent.Saved.SetProgression(maxMilestone);
        }
        else
        {
            MultiplayerEvent.Saved.AddProgression(quantity);
        }
	}

	public abstract string FormatQuantity(float quantity);

	public string GetDescription()
	{
	    //return LocalizationManager.GetTranslation(this.LocalisationTag);
	    return null;
	}
}
