using UnityEngine;
using UnityEngine.UI;

public class BodyHeightScreen : ZHUDScreen
{
    [SerializeField] private Slider m_heightSlider;
    private bool m_changed;
    private bool m_initiated;

    public override ScreenID ID
    {
        get { return ScreenID.BodyHeight; }
    }

    protected override void Update()
    {
        base.Update();
        if (!m_initiated)
            return;
        var heightValue = m_heightSlider.value;
        m_changed = SceneManagerGarage.Instance.SetupHeight(heightValue);
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        var carVisual = SceneManagerGarage.Instance.currentCarVisuals;
        m_heightSlider.minValue = carVisual.MinBodyHeight;
        m_heightSlider.maxValue = carVisual.MaxBodyHeight;
        m_heightSlider.value = carVisual.BodyHeight;
        m_initiated = true;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        m_initiated = false;
        if (m_changed)
        {
            var carVisual = SceneManagerGarage.Instance.currentCarVisuals;
            PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar().BodyHeight = carVisual.BodyHeight;
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
    }
}
