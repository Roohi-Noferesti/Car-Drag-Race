using DataSerialization;

public class FocusOnEventPopupDataAction : PopupDataActionBase
{
    public override void Execute(EligibilityConditionDetails details)
    {
        var eventType = EnumHelper.FromString<ProgressionMapPinEventType>(details.StringValue);
        if(CareerModeMapScreen.Instance != null)
            CareerModeMapScreen.Instance.FocusOnEvent(eventType, details.Tier, details.FloatValue);
    }
}
