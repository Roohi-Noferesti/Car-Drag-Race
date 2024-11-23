using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public class RPStats : MonoBehaviour, IPersistentUI
{
	private const float _seasonDisplayRankTextYOffset = -0.05f;

	public TextMeshProUGUI uiTxtRP;

	public BigRankText RankText;

	public GameObject Offset;

    public TextMeshProUGUI SeasonText;

	private bool canUpdateRP = true;

	private int LastKnownRP = -1;

	private int CurrentRP = -1;

	private int CurrentDisplayRP = -1;

	private int _trackedRP = -1;

	private Vector3 _rankTextOriginalLocalPosition;

	private bool runRP;

	private float RPTime_Current;

	private float RPTime_Target;

	private int PreserveOldRPVal;

	private BubbleMessage respectBubbleMessage;

	private Vector3 respectBubbleMessageOffset = new Vector3(0f, -0.16f, -0.1f);

	public void RPLockedState(bool zLocked)
	{
		this.canUpdateRP = !zLocked;
	}

	private void Reset(int zRP)
	{
		this.CurrentRP = zRP;
		if (this.LastKnownRP < 0)
		{
			this.CurrentDisplayRP = this.CurrentRP;
			this.LastKnownRP = this.CurrentRP;
			this.uiTxtRP.text = CurrencyUtils.GetRankPointsString(this.CurrentRP, false, true);
		}
	}

	public void Awake()
	{
		Vector3 localPosition = this.Offset.transform.localPosition;
        //localPosition.x = -GUICamera.Instance.ScreenWidth / 4f;
		this.Offset.transform.localPosition = localPosition;
		NavBarAnimationManager.Instance.Subscribe(this.Offset);
		this._rankTextOriginalLocalPosition = this.RankText.gameObject.transform.localPosition;
	}

	public void OnDestroy()
	{
		this.KillRespectBubble();
	}

	public void Update()
	{
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		if (activeProfile == null)
		{
			return;
		}
		int num = this._trackedRP;
		if (this.canUpdateRP)
		{
			num = activeProfile.GetPlayerRP();
		}
		if (num != this._trackedRP)
		{
			this._trackedRP = num;
			this.Reset(num);
			this.UpdateSeasonText();
		}
		this.HandleRPAnim();
	}

	public void Start()
	{
	}

	public void OnScreenChanged(ScreenID newScreen)
	{
	}

	public void Show(bool zShow)
	{
		base.gameObject.SetActive(zShow);
		if (!zShow)
		{
			this.KillRespectBubble();
		}
	}

	public void OnEnable()
	{
		this.UpdateSeasonText();
	}

	public void SetRP(int newRP)
	{
		this.RankText.SetRank(newRP, BigRankText.Alignment.Right);
		this.UpdateSeasonText();
	}

	private void HandleRPAnim()
	{
		if (this.runRP)
		{
			this.RPTime_Current += Time.deltaTime;
			if (this.RPTime_Current >= this.RPTime_Target)
			{
				this.CurrentDisplayRP = this.CurrentRP;
				this.runRP = false;
			}
			else
			{
				float num = this.RPTime_Current / this.RPTime_Target;
				this.CurrentDisplayRP = (int)((float)this.CurrentRP * num + (float)this.PreserveOldRPVal * (1f - num));
			}
			CommonUI.Instance.RPStats.SetRP(this.CurrentDisplayRP);
		}
		if (this.CurrentRP != this.LastKnownRP)
		{
			this.runRP = true;
			this.RPTime_Current = 0f;
			this.RPTime_Target = (Mathf.Log10((float)Mathf.Abs(this.CurrentDisplayRP - this.CurrentRP)) + 1f) * 0.3f;
			this.PreserveOldRPVal = this.CurrentDisplayRP;
			this.LastKnownRP = this.CurrentRP;
		}
	}

	public void DisplayRespectBubble()
	{
		if (!base.gameObject.activeInHierarchy)
		{
			return;
		}
		if (this.respectBubbleMessage != null)
		{
			return;
		}
		Vector3 position = this.Offset.transform.position + this.respectBubbleMessageOffset;
		this.respectBubbleMessage = BubbleManager.Instance.ShowMessage("TEXT_TUTORIAL_MESSAGE_RESPECT", false, position, BubbleMessage.NippleDir.UP, 0.5f);
		this.respectBubbleMessage.GetParentTransform().parent = this.Offset.transform;
	}

	public void KillRespectBubble()
	{
		if (this.respectBubbleMessage == null)
		{
			return;
		}
		this.respectBubbleMessage.KillNow();
		this.respectBubbleMessage = null;
	}

	private void UpdateSeasonText()
	{
		int seasonLastPlayedEventID = PlayerProfileManager.Instance.ActiveProfile.SeasonLastPlayedEventID;
		SeasonEventMetadata @event = GameDatabase.Instance.SeasonEvents.GetEvent(seasonLastPlayedEventID);
		if (@event != null)
		{
			this.SeasonText.text = LocalizationManager.GetTranslation("TEXT_THIS_SEASON").ToUpper();
			this.RankText.gameObject.transform.localPosition = this._rankTextOriginalLocalPosition + new Vector3(0f, -0.05f, 0f);
		}
		else
		{
            this.SeasonText.text = string.Empty;
			this.RankText.gameObject.transform.localPosition = this._rankTextOriginalLocalPosition;
		}
	}
}
