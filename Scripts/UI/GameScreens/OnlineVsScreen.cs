public class OnlineVsScreen : VsScreen
{
    public override ScreenID ID
    {
        get { return ScreenID.OnlineVs; }
    }


    public override void OnActivate(bool zAlreadyOnStack)
    {
        base.OnActivate(zAlreadyOnStack);
        SMPNetworkManager.Instance.ResetSMPRaceVariables();
    }
}
