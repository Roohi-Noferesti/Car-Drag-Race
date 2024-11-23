using DataSerialization;
using System;

[Serializable]
public class ProgressionMapPinsData
{
    public string Name;

    public string EventTypeString;

    public ProgressionMapPinEventType EventType
    {
        get
        {
            return EnumHelper.FromString<ProgressionMapPinEventType>(EventTypeString);
        }
    }

    public eCarTier Tier = eCarTier.BASE_EVENT_TIER;

	public EligibilityRequirements ShowingRequirements = EligibilityRequirements.CreateNeverEligible();

	public EligibilityRequirements OverrideHighlightRequirements = EligibilityRequirements.CreateNeverEligible();

	public void Initialise()
	{
		this.ShowingRequirements.Initialise();
		this.OverrideHighlightRequirements.Initialise();
	}
}
