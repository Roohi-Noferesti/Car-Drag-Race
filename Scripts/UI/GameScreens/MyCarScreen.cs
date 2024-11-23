using System.Collections;
using System.Linq;
using UnityEngine;

public class MyCarScreen : ZHUDScreen
{
    [SerializeField] private MyCarScrollerController m_scroller;
    public static string OnLoadCar;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.CarSelect;
        }
    }

    void m_scroller_SelectedItemClicked(string obj)
    {
        if (!string.IsNullOrEmpty(m_scroller.SelectedID))
            CarInfoUI.Instance.SetCurrentCarIDKey(m_scroller.SelectedID);
        Close();
    }

    void m_scroller_SelectedIndexChanged(int obj)
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!string.IsNullOrEmpty(m_scroller.SelectedID))
        {
            activeProfile.SelectCar(m_scroller.SelectedID);
        }
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        m_scroller.SelectedIndexChanged += m_scroller_SelectedIndexChanged;
        m_scroller.SelectedItemClicked += m_scroller_SelectedItemClicked;
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var playerCars = activeProfile.CarsOwned.OrderBy(x => !PlayerProfileManager.Instance.ActiveProfile.HasSeenCar(x.CarDBKey))
            .ThenBy(y => y.Tier)
            .ThenBy(i => i.CurrentPPIndex).Cast<ICarSimpleSpec>().ToArray();
        m_scroller.SetData(playerCars);
        m_scroller.Reload();
        //var currentCarKey = !string.IsNullOrEmpty(OnLoadCar) ? OnLoadCar : activeProfile.CurrentlySelectedCarDBKey;
        //CoroutineManager.Instance.StartCoroutine(_toggleSelected(currentCarKey));
    }


    public override void OnAfterActivate()
    {
        base.OnAfterActivate();
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        var currentCarKey = !string.IsNullOrEmpty(OnLoadCar) ? OnLoadCar : activeProfile.CurrentlySelectedCarDBKey;
        m_scroller.SelectedID = currentCarKey;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        m_scroller.SelectedIndexChanged -= m_scroller_SelectedIndexChanged;
        m_scroller.SelectedItemClicked -= m_scroller_SelectedItemClicked;
        //OnLoadCar = null;
    }

    private IEnumerator _toggleSelected(string selectedCar)
    {
        yield return new WaitForSeconds(0.2F);
    }

    public void GoViewCar(string carId, bool value)
    {
        m_scroller.SelectedID = carId;
    }
}
