using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpDialogue : MonoBehaviour
{
    private const float maxTitleTextWidth = 1.72f;

    private const float maxButtonTextWidth = 0.76f;

    private PopUpButtonAction m_cashedAction;

    public bool HasAnimator;
    public Sprite GoldIcon;
    public Sprite CashIcon;

    public TextMeshProUGUI Title;

    public TextMeshProUGUI BodyText;

    public RuntimeTextButton CancelButton;

    public RuntimeTextButton ConfirmButton;

    public Button SocialButton;

    public GameObject SocialButtonExtras;

    public Button FacebookButton;

    public Button TwitterButton;

    public GameObject ShareIcon;

    public GameObject AndroidShareIcon;

    public Text FacebookBonus;

    public Text TwitterBonus;

    public RuntimeButton CloseButton;

    public GameObject ButtonHolder;

    public GameObject CustomObject;

    public RawImage AgentImage;
    public RawImage ItemImage;

    public Button StopOutsideClicksObject;

    public Image BackgroundObject;

    public RawImage LeaderGraphic;

    //public WindowPaneBatched WindowPane;

    public bool ShouldCentreButtons;

    public TextMeshProUGUI CaptionText;

    public bool PositionsReactToNoSocial;

    protected PopUp popup;
    private List<TransformParentHolder> m_targeTransformParentHolders;

    public AudioSfx PopupBleeps;

    protected virtual void Start()
    {
        MenuAudio.Instance.playSound(PopupBleeps);
    }


    public PopUpID GetPopUpID()
    {
        if (this.popup == null)
        {
            return PopUpID.Invalid;
        }
        return this.popup.ID;
    }

    public virtual void Setup(PopUp pop)
    {
        this.popup = pop;
        //if (pop.HidePopup && this.WindowPane != null)
        //{
        //    this.WindowPane.Hide();
        //}
        if (!string.IsNullOrEmpty(pop.CustomPrefab))
        {
            GameObject original = (GameObject)Resources.Load(this.popup.CustomPrefab);
            this.CustomObject = (GameObject)Instantiate(original);
            this.CustomObject.transform.parent = base.gameObject.transform;
        }
        if (this.Title != null)
        {
            if (pop.TitleAlreadyTranslated || string.IsNullOrEmpty(pop.Title))
            {
                this.Title.text = this.popup.Title.ToUpper();
            }
            else
            {
                this.Title.text = LocalizationManager.GetTranslation(this.popup.Title).ToUpper();
            }
            //LocalisationManager.AdjustText(this.Title, 1.72f);
        }

  
        if (this.BodyText != null)
        {
            if (pop.BodyAlreadyTranslated || string.IsNullOrEmpty(pop.BodyText))
            {
                this.BodyText.text = this.popup.BodyText;
            }
            else
            {
        
                this.BodyText.text= LocalizationManager.GetTranslation(this.popup.BodyText);

            }
          //  this.BodyText.text = BodyText.GetComponent<RTLTextMeshPro>().GetFixedText(this.BodyText.text);


            //int displayLineCount = this.BodyText.GetDisplayLineCount();
            //if (displayLineCount > 9)
            //{
            //    float[] array = new float[]
            //    {
            //        0.82f,
            //        0.79f,
            //        0.75f
            //    };
            //    int num = Mathf.Min(displayLineCount - 10, 2);
            //    this.BodyText.SetCharacterSize(this.BodyText.characterSize * array[num]);
            //}
        }
        if (this.CancelButton != null)
        {
            if (!string.IsNullOrEmpty(this.popup.CancelText))
            {
                CancelButton.CurrentState = BaseRuntimeControl.State.Active;
                CancelButton.SetText(popup.CancelText, popup.CancelTextAlreadyTranslated, false);
                if (popup.UseCancelColor)
                    CancelButton.GetComponentInChildren<Image>().color = popup.CancelColor;
            }
            else
            {
                CancelButton.CurrentState = BaseRuntimeControl.State.Hidden;
            }
        }
        if (this.ConfirmButton != null)
        {
            if (!string.IsNullOrEmpty(this.popup.ConfirmText))
            {
                ConfirmButton.CurrentState = BaseRuntimeControl.State.Active;
                ConfirmButton.SetText(popup.ConfirmText, popup.ConfirmTextAlreadyTranslated,false);
                if (popup.UseConfirmColor)
                    ConfirmButton.GetComponentInChildren<Image>().color = popup.ConfirmColor;
            }
            else
            {
                ConfirmButton.CurrentState = BaseRuntimeControl.State.Hidden;
            }
        }
        if (this.SocialButton != null)
        {
            //this.SocialButton.ForceAwake();
        }
        if (this.popup.SocialAction == null)
        {
            if (this.SocialButton != null)
            {
                //this.SocialButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
            }
            if (this.SocialButtonExtras != null)
            {
                this.SocialButtonExtras.gameObject.SetActive(false);
            }
            //if (this.PositionsReactToNoSocial)
            //{
            //    if (this.ButtonHolder != null)
            //    {
            //        this.ButtonHolder.transform.localPosition = this.NoSocialButtonPos;
            //    }
            //    this.BodyText.transform.localPosition = this.NoSocialBodyPos;
            //}
        }
        else if (this.SocialButtonExtras != null)
        {
            this.ShareIcon.SetActive(false);
            this.AndroidShareIcon.SetActive(true);
        }
        //if (SocialController.Instance.TwitterIsDisabledForPosting && this.TwitterButton != null)
        //{
        //    this.TwitterButton.SetControlState(UIButton.CONTROL_STATE.DISABLED);
        //}
        //if (SocialController.Instance.FacebookIsDisabledForPosting && this.FacebookButton != null)
        //{
        //    this.FacebookButton.SetControlState(UIButton.CONTROL_STATE.DISABLED);
        //}
        if (this.TwitterBonus != null)
        {
            this.TwitterBonus.text = this.popup.TwitterRewardText;
        }
        if (this.FacebookBonus != null)
        {
            this.FacebookBonus.text = this.popup.FacebookRewardText;
        }
        if (this.CloseButton != null)
        {
            this.CloseButton.gameObject.SetActive(this.popup.hasCloseButton);
        }
        if (this.LeaderGraphic != null)
        {
            this.LeaderGraphic.texture = this.popup.Graphic;
            //this.LeaderGraphic.transform.localPosition = new Vector3(-0.94f, 0f, -0.48f);
            //Text component = this.LeaderGraphic.transform.FindChild("Name").GetComponent<Text>();
            //component.Text = LocalizationManager.GetTranslation((!this.popup.UseImageCaptionForCrewLeader) ? CrewChatter.GetLeaderName(this.popup.BossTier) : this.popup.ImageCaption);
            //component.gameObject.SetActive(true);
            //component.transform.localPosition = new Vector3(0f, -0.9f, 0f);
        }
        else if (this.AgentImage != null && this.popup.Graphic != null)
        {
            this.AgentImage.texture = this.popup.Graphic;
            //Vector2 lowerleftPixel = new Vector2(0f, (float)(this.popup.Graphic.height - 1));
            //this.Image.Setup(this.Image.width, this.Image.height, lowerleftPixel, new Vector2((float)this.popup.Graphic.width, (float)this.popup.Graphic.height));
            if (this.CaptionText != null)
            {
                if (string.IsNullOrEmpty(this.popup.ImageCaption))
                {
                    this.CaptionText.text = string.Empty;
                }
                else
                {
                    this.CaptionText.text = LocalizationManager.GetTranslation(this.popup.ImageCaption);
                }
            }
        }
        if (this.ItemImage != null && this.popup.ItemGraphic != null)
        {
            this.ItemImage.texture = this.popup.ItemGraphic;
            //Vector2 lowerleftPixel = new Vector2(0f, (float)(this.popup.Graphic.height - 1));
            //this.Image.Setup(this.Image.width, this.Image.height, lowerleftPixel, new Vector2((float)this.popup.Graphic.width, (float)this.popup.Graphic.height));

            if (this.CaptionText != null)
            {
                if (string.IsNullOrEmpty(this.popup.ImageCaption))
                {
                    this.CaptionText.text = string.Empty;
                    this.CaptionText.gameObject.SetActive(false);
                }
                else
                {
                    this.CaptionText.text = LocalizationManager.GetTranslation(this.popup.ImageCaption);
                }
            }

        }
        bool flag = false;
        float num2 = 0f;
        //if (this.popup.ShouldCentreIfOverflowsScreenBottom && this.WindowPane != null)
        //{
        //    float num3 = this.WindowPane.transform.position.y - this.WindowPane.Height;
        //    float num4 = 0.5f * (float)BaseDevice.ActiveDevice.GetScreenHeight() / ResolutionManager.PixelDensity;
        //    if (num3 < -num4)
        //    {
        //        num2 = 0.5f * this.WindowPane.Height - base.gameObject.transform.position.y;
        //        base.gameObject.transform.position = new Vector3(base.gameObject.transform.position.x, 0.5f * this.WindowPane.Height, base.gameObject.transform.position.z);
        //        this.StopOutsideClicksObject.transform.position = new Vector3(this.StopOutsideClicksObject.transform.position.x, num4, this.StopOutsideClicksObject.transform.position.z);
        //        flag = true;
        //    }
        //}
        if (this.BackgroundObject != null)
        {
            float z = -0.3544779f + this.popup.BackgroundOffsetZ;
            if (this.popup.ShouldCoverNavBar || flag)
            {
                this.BackgroundObject.transform.localPosition = new Vector3(0f, -num2, z);
            }
            else
            {
                //this.BackgroundObject.transform.localPosition = new Vector3(0f, -CommonUI.Instance.NavBar.GetHeightTight(), z);
            }
        }

        m_targeTransformParentHolders = new List<TransformParentHolder>();
        if (popup.TargetGameObjectNames != null)
        {
            var objects = FindObjectsOfType<RectTransform>();
            for (int i = 0; i < popup.TargetGameObjectNames.Length; i++)
            {
                var targetGameObjectName = popup.TargetGameObjectNames[i];
                if (!string.IsNullOrEmpty(targetGameObjectName) && objects.Length > 0)
                {
                    var target = objects.FirstOrDefault(o => o.name == targetGameObjectName);
                    if (target != null)
                    {
                        var holder = new TransformParentHolder(target, i > 0);
                        m_targeTransformParentHolders.Add(holder);
                        target.SetParent(transform, true);
                        target.SetAsLastSibling();
                    }
                }
            }
        }
    }

    public void DoButtonAction(PopUpButtonAction theAction, bool withDefault = true)
    {
        if (withDefault)
        {
            m_cashedAction = theAction;
            PopUpManager.Instance.DefaultButtonResponse();
        }
        if (!HasAnimator && theAction != null)
        {
            theAction();
        }
    }

    public virtual void OnCancel()
    {
        if (!string.IsNullOrEmpty(this.popup.CancelText) || this.popup.CancelAction != null)
        {
            this.DoButtonAction(this.popup.CancelAction, true);
        }
    }

    public virtual void OnConfirm()
    {
        if (!string.IsNullOrEmpty(this.popup.ConfirmText))
        {
            this.DoButtonAction(this.popup.ConfirmAction, true);
        }
    }

    public virtual void OnShareButton()
    {
        if (this.popup.SocialAction != null)
        {
            this.DoButtonAction(this.popup.SocialAction, false);
        }
    }

    public virtual void OnCloseButton()
    {
        if (this.popup.CloseAction != null)
        {
            if (this.popup.CloseActionHACKFORBILLBOARDMAP != null)
            {
                this.DoButtonAction(this.popup.CloseActionHACKFORBILLBOARDMAP, false);
            }
            this.DoButtonAction(this.popup.CloseAction, true);
        }
    }

    public void DisableOutsideClickArea()
    {
        //this.StopOutsideClicksObject.gameObject.SetActive(false);
    }

    public void DisableBackground()
    {
        //this.BackgroundObject.gameObject.SetActive(false);
    }

    public void preKill()
    {
        //if (this.WindowPane != null)
        //{
        //    this.WindowPane.Hide();
        //}

        if (HasAnimator && m_cashedAction != null)
        {
            m_cashedAction();
        }

        for (int i = 0; i < m_targeTransformParentHolders.Count; i++)
        {
            var targeTransformParentHolder = m_targeTransformParentHolders[i];
            targeTransformParentHolder.Reset(i > 0);
        }

        base.gameObject.SetActive(false);
    }

    protected bool HasConfirmButton()
    {
        return !string.IsNullOrEmpty(this.popup.ConfirmText);
    }

    protected bool HasCancelButton()
    {
        return !string.IsNullOrEmpty(this.popup.CancelText);
    }

    protected bool HasCancelAction()
    {
        return this.popup.CancelAction != null;
    }

    protected virtual void Update()
    {
        if (InputManager.BackButtonPressed())
        {
            if (this.popup.hasCloseButton)
            {
                this.OnCloseButton();
                MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
            }
            else if (this.popup.AllowBackButton && this.HasConfirmButton() && !this.HasCancelButton() && !this.HasCancelAction())
            {
                this.OnConfirm();
                MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
            }
            else if (this.popup.AllowBackButton && this.HasConfirmButton() && (this.HasCancelButton() || this.HasCancelAction()))
            {
                this.OnCancel();
                MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
            }
        }
        else if (InputManager.EnterPressed())
        {
            if (this.HasConfirmButton() && !this.HasCancelButton())
            {
                this.OnConfirm();
                MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
            }
            else if (this.HasConfirmButton() && this.HasCancelButton())
            {
                this.OnCancel();
                MenuAudio.Instance.playSound(AudioSfx.MenuClickBack);
            }
        }
    }

    public virtual void OnActivate(bool zAlreadyOnStack)
    {
    }
}
