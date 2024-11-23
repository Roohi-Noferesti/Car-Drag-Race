using System.Collections;
using DataSerialization;
using KingKodeStudio;
using UnityEngine;

public class GoToScreenPopupDataAction : PopupDataActionBase
{
    public override void Execute(EligibilityConditionDetails details)
    {
        if (details.FloatValue > 0)
        {
            //TODO
            CoroutineManager.Instance.StartCoroutine(_openScreenDelayed(details.ScreenID, details.FloatValue));
        }
        else
        {
            ScreenManager.Instance.PushScreen(details.ScreenID);
        }
    }

    private IEnumerator _openScreenDelayed(ScreenID screenID,float delay)
    {
        yield return new WaitForSeconds(delay);
        ScreenManager.Instance.PushScreen(screenID);
    }
}
