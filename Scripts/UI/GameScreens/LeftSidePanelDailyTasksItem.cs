using System;
using I2.Loc;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LeftSidePanelDailyTasksItem : MonoBehaviour
{
	public delegate void OnCollectPressedDelegate(LeftSidePanelDailyTasksItem item);

    public RuntimeTextButton Button_Collect;

	public LeftSidePanelDailyTasksItem.OnCollectPressedDelegate OnCollectPressedCallback;

	public Toggle Toggle_Complete;

	protected string m_ObjectiveID = string.Empty;

	[SerializeField]
	private TextMeshProUGUI[] Label_Description;

	[SerializeField]
    private TextMeshProUGUI Label_TimeLimit;

	[SerializeField]
	private Image Label_TimeLimitSprite;

	[SerializeField]
    private TextMeshProUGUI[] Label_Title;

	public GameObject m_marker;

    [SerializeField]
    private Slider[] ProgressBar_TaskCompletion;

	[SerializeField]
    private TextMeshProUGUI[] CurrentProgressStep;

	[SerializeField]
    private TextMeshProUGUI[] Label_CompletionMarker;

	[SerializeField]
	private Image[] Sprite_StatusBarAchievementIcon;

	[SerializeField]
	private Image[] Sprite_StatusBarCurrencyIcon;

	[SerializeField]
	private GameObject[] Object_FusionPartIcon;

	[SerializeField]
    private TextMeshProUGUI[] Label_StatusBarCurrencyValue;

	public string Description
	{
		set
		{
			Array.ForEach<TextMeshProUGUI>(this.Label_Description, delegate(TextMeshProUGUI x)
			{
			//	x.SetText(value);
            x.GetComponent<RTLTextMeshPro>().text = value;
                x.GetComponent<RTLTextMeshPro>().UpdateText();
			});
		}
	}

	public virtual string TimeLimit
	{
		set
		{
			this.Label_TimeLimit.SetText(value);
			if (string.IsNullOrEmpty(value))
			{
				this.Label_TimeLimitSprite.gameObject.SetActive(false);
			}
			else
			{
				this.Label_TimeLimitSprite.gameObject.SetActive(true);
			}
		}
	}

	public string Title
	{
		set
		{
			Array.ForEach<TextMeshProUGUI>(this.Label_Title, delegate(TextMeshProUGUI x)
			{
                //	x.SetText(value);
                x.GetComponent<RTLTextMeshPro>().text = value;
                x.GetComponent<RTLTextMeshPro>().UpdateText();
            });
		}
	}

	public virtual float TaskCompletion
	{
		set
		{
			float locScale = value;
			float num = 0;
			if (value > 0f && value < num)
			{
				locScale = Mathf.Clamp(locScale, num, 1f);
			}
            Array.ForEach<Slider>(this.ProgressBar_TaskCompletion, delegate(Slider x)
            {
                x.value = locScale;
            });
			if (value == 0f)
			{
				this.m_marker.SetActive(false);
			}
			else
			{
				this.m_marker.SetActive(true);
			}
		}
	}

	public int CurrentProgress
	{
		set
		{
			Array.ForEach<TextMeshProUGUI>(this.CurrentProgressStep, delegate(TextMeshProUGUI x)
			{
			    x.text = string.Format("{0}",value);//LocalisationManager.CultureFormat(value, "N0"));
			});
		}
	}

	public float CurrentProgressOverride
	{
		set
		{
			Array.ForEach<TextMeshProUGUI>(this.CurrentProgressStep, delegate(TextMeshProUGUI x)
			{
                x.text = string.Format("{0}", value); //LocalisationManager.CultureFormatDecimal(value, "N");
			});
		}
	}

	public string CompletionMarker
	{
		set
		{
			Array.ForEach<TextMeshProUGUI>(this.Label_CompletionMarker, delegate(TextMeshProUGUI x)
			{
				x.SetText(value);
			});
		}
	}

	public string StatusBarAchievementIcon
	{
		set
		{
			Array.ForEach<Image>(this.Sprite_StatusBarAchievementIcon, delegate(Image x)
			{
                //x.SetSpriteName(value);
			});
		}
	}

	public string StatusBarCurrencyIcon
	{
		set
		{
			Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
			{
                //x.SetSpriteName(value);
			});
		}
	}

	public string StatusBarCurrencyValue
	{
		set
		{
			Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
            {
                x.text = value;
			});
		}
	}

	public virtual void SetObjectiveID(string ID)
	{
		this.m_ObjectiveID = ID;
	}

	public virtual string GetObjectiveID()
	{
		return this.m_ObjectiveID;
	}

    public void SetRewardType(ERewardType rewardType)
    {
        Array.ForEach<GameObject>(this.Object_FusionPartIcon, delegate(GameObject x)
        {
            x.SetActive(rewardType == ERewardType.FusionUpgrade);
        });
        Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
        {
            x.gameObject.SetActive(rewardType != ERewardType.FusionUpgrade);
        });
        Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
        {
            x.gameObject.SetActive(rewardType != ERewardType.FusionUpgrade);
        });
        ERewardType rewardType2 = rewardType;
        switch (rewardType2)
        {
            case ERewardType.LockupKey:
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("GatchaKey");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("GachaKeyBronze");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("GachaBalanceBoldBronze");
                });
                break;

            case ERewardType.SilverGachaKey:
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("GatchaKey");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("GachaKeySilver");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("GachaBalanceBoldSilver");
                });
                break;

            case ERewardType.GoldGachaKey:
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("GatchaKey");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("GachaKeyGold");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("GachaBalanceBoldGold");
                });
                break;

            case ERewardType.FuelPip:
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("FuelIcon");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("SpriteFuelGlow");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("FuelExtraBold");
                });
                break;

            case ERewardType.Car:
            case ERewardType.EvoUpgrade:
            case ERewardType.FusionUpgrade:
            case ERewardType.TestRun:
            case ERewardType.Crate:
            case ERewardType.FreeUpgrade:
                if (rewardType2 == ERewardType.Cash)
                {
                    Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                    {
                        //x.SetSpriteName("SoftCurrencyIconFlat");
                    });
                    Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                    {
                        //x.SetPreset("SpriteSilver");
                    });
                    Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                    {
                        //x.SetPreset("SoftCurrencyExtraBold");
                    });
                    break;

                }
                if (rewardType2 != ERewardType.Gold)
                {
                    break;

                }
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("HardCurrencyFlatIcon");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("SpriteGold");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("HardCurrencyExtraBold");
                });
                break;

            case ERewardType.RP:
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("EPIcon");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("SpriteEP");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("EPExtraBold");
                });
                break;

            case ERewardType.CrewRP:
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetSpriteName("EPIcon");
                });
                Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
                {
                    //x.SetPreset("SpriteEP");
                });
                Array.ForEach<TextMeshProUGUI>(this.Label_StatusBarCurrencyValue, delegate(TextMeshProUGUI x)
                {
                    //x.SetPreset("EPExtraBold");
                });
                break;
        }
        //float fontScale = LocalisationManager.GetLanguageScale(LocalisationManager.GetChosenLanguage().ToString());
        //Array.ForEach<Image>(this.Sprite_StatusBarCurrencyIcon, delegate(Image x)
        //{
        //    x.transform.localScale = new Vector3(fontScale, fontScale, 0f);
        //});
    }

    private void Awake()
	{
		if (this.Button_Collect != null)
		{
			this.Button_Collect.SetText(LocalizationManager.GetTranslation("TEXT_BUTTON_GETPRIZE"), true,false);
            Button_Collect.AddValueChangedDelegate(this.OnButton_Collect);
		}
	}

	private void OnButton_Collect()
	{
		if (this.OnCollectPressedCallback != null)
		{
			this.OnCollectPressedCallback(this);
		}
	}

	public void EnableCollectButton(bool state)
	{
		if (this.Button_Collect != null)
		{
			this.Button_Collect.gameObject.SetActive(state);
		}
	}
}
