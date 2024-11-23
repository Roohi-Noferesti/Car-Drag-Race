using DataSerialization;

public class DisableAllMapPinEventPopupDataAction : PopupDataActionBase
{
    public override void Execute(EligibilityConditionDetails details)
    {
        var eventType = EnumHelper.FromString<ProgressionMapPinEventType>(details.StringValue);
        CareerModeMapScreen.Instance.DisableEverythingExceptEvent(eventType, details.Tier, details.FloatValue);

    }
}
