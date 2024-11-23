using System.Collections;
using KingKodeStudio;
using UnityEngine;

public class ZHUDScreen : HUDScreen
{
    [SerializeField] private DashboardType m_dashboardType;
    public virtual DashboardType DashboardType
    {
        get { return m_dashboardType; }
    }

    private bool _screenActivatedForTutorialBubbles;
    protected bool firstUpdate;

    public bool BlockingTutorialBubbles
    {
        get;
        private set;
    }

    public override ScreenID ID
    {
        get { return ScreenID.Invalid; }
    }

    public bool ShowCarName;

    public void TriggerScreenActivateEventDelayed()
    {
        CoroutineManager.Instance.StartCoroutine(_triggerScreenActivateEventDelayed());
    }

    private IEnumerator _triggerScreenActivateEventDelayed()
    {
        yield return new WaitForEndOfFrame();
        TriggerScreenActivateEvent();
    }

    public void TriggerScreenActivateEvent()
    {
        if (this._screenActivatedForTutorialBubbles)
        {
            return;
        }
        if (this.BlockingTutorialBubbles)
        {
            return;
        }
        if (!GTSystemOrder.SystemsReady)
        {
            return;
        }
        TutorialBubblesManager.Instance.TriggerEvent(TutorialBubblesEvent.ScreenActivate);
        this._screenActivatedForTutorialBubbles = true;
    }

    public void BlockTutorialBubbles()
    {
        this.BlockingTutorialBubbles = true;
    }

    public void UnblockTutorialBubbles(bool triggerScreenActivate = true)
    {
        this.BlockingTutorialBubbles = false;
        if (triggerScreenActivate)
        {
            this.TriggerScreenActivateEvent();
        }
    }


    //ToDO
    //protected override void OnScreenVisibilityChanging(HudScreenEventArgs obj)
    //{
    //    base.OnScreenVisibilityChanging(obj);
    //    if (PopUpManager.Instance == null)
    //        return;

    //    if (PopUpManager.Instance.isShowingPopUp)
    //    {
    //        obj.Wait = true;
    //        CoroutineManager.Instance.StartCoroutine(_waitForPopup(obj));
    //    }
    //    if (!obj.Cancel)
    //    {
    //        MenuAudio.Instance.playSound(MenuBleeps.MenuClickBack);
    //    }
    //}

    //protected virtual void Update()
    //{
    //    if (m_eventArgs!=null && !PopUpManager.Instance.isShowingPopUp) {
    //        m_eventArgs.Wait = false;
    //        m_eventArgs = null;
    //    }
    //}

    public virtual void StartAnimOut()
    {

    }

    public virtual void StartAnimIn()
    {

    }
}

public enum DashboardType
{
    None,
    Normal,
    JustClose,
    JustNormal
}
