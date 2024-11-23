using System;
using System.Collections;
using System.Linq;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

public class CarClassScreen : ZHUDScreen
{
    public static eCarTier SelectedTier
    {
        get { return _selectedTier; }
        private set { _selectedTier = value; }
    }

    [SerializeField] private ClassUI m_class1;
    [SerializeField] private ClassUI m_class2;
    [SerializeField] private Sprite[] m_sprites;
    [SerializeField] private Toggle[] m_toggles;
    private float m_lastToggleTime;

    private bool m_animToggle;
    private static eCarTier _selectedTier = eCarTier.TIER_1;
    private BubbleMessage m_bubbleMessage;

    public override ScreenID ID
    {
        get
        {
            return ScreenID.CarClass;
        }
    }

    private void Init(bool visible)
    {
        m_animToggle = false;
        m_class1.Sprite = GetSprite(SelectedTier);
        m_class2.Sprite = GetSprite(SelectedTier);
        m_class1.Play("Opened");
        m_class2.Play("Closed");
        if (visible)
        {
            if (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar)
            {
                BringUpBubble();
            }
        }
    }

    private void BringUpBubble()
    {
        var fClassToggle = m_toggles.First();
        Vector3 position = fClassToggle.rectTransform().GetUpperPoint() + new Vector3(-0.007f, 0, 0);
        m_bubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TAP_F_CLASS_TO_ENTER", false, position,
            BubbleMessage.NippleDir.DOWN, 0, BubbleMessageConfig.DuringRace
             , true,true, fClassToggle.rectTransform());
    }

    private IEnumerator _initDelayed(bool visible)
    {
        yield return new WaitForEndOfFrame();
        Init(visible);
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (!activeProfile.HasVisitedManufacturerScreen)
        {
            activeProfile.HasVisitedManufacturerScreen = true;
            PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
        }
        //if (gameObject.activeInHierarchy)
        //    StartCoroutine(_initDelayed(true));
        //else
        //{
        //    Init(true);
        //}
    }

    public override void OnAfterActivate()
    {
        base.OnAfterActivate();
        if (gameObject.activeInHierarchy)
            StartCoroutine(_initDelayed(true));
        else
        {
            Init(true);
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (gameObject.activeInHierarchy)
            StartCoroutine(_initDelayed(false));
        else
        {
            Init(false);
        }
    }

    private eCarTier GetSelectedTier()
    {
        for (int i = 0; i < m_toggles.Length; i++)
        {
            if (m_toggles[i].isOn)
                return (eCarTier) i;
        }
        return eCarTier.TIER_1;
    }

    public void ToggleClass()
    {
        if (Time.time - m_lastToggleTime < 0.3F)
            return;
        m_lastToggleTime = Time.time;
        var carClass = GetSelectedTier();

        if (carClass != SelectedTier)
        {
            SelectedTier = carClass;
            m_animToggle = !m_animToggle;
            if (m_animToggle)
            {
                m_class2.Sprite = GetSprite(SelectedTier);
                m_class1.Play("Close");
                m_class2.Play("Open");
            }
            else
            {
                m_class1.Sprite = GetSprite(SelectedTier);
                m_class1.Play("Open");
                m_class2.Play("Close");
            }
        }
        else
        {
            OpenShowRoom();
        }
    }

    public void OpenShowRoom()
    {
        if (m_bubbleMessage != null)
        {
            m_bubbleMessage.OnDestroyEvent += (b) => { ScreenManager.Instance.PushScreen(ScreenID.Showroom); };
            m_bubbleMessage.Dismiss();
        }
        else
        {
            if (SelectedTier == eCarTier.TIER_X)
            {
                ShowroomScreen.Init.screenMode = ShowroomMode.WorldTour_Cars;
            }
            else
            {
                ShowroomScreen.Init.CurrentTierManufacturer = SelectedTier;
            }

            ScreenManager.Instance.PushScreen(ScreenID.Showroom);
        }
    }

    private Sprite GetSprite(eCarTier tier)
    {
        return m_sprites[(int) tier];
    }


    public override bool IgnoreHardwareBackButton()
    {
        return !PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar;
    }
}

[Serializable]
public class ClassUI
{
    [SerializeField] private Animator m_animator;
    [SerializeField] private Image m_image;

    public Sprite Sprite
    {
        set
        {
            m_image.sprite = value;
            m_image.SetNativeSize();
        }
    }

    public void Play(string animName)
    {
        m_animator.Play(animName);
    }
}
