using System;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

public class BubbleManager : MonoBehaviour
{
	public List<BubbleMessage> Messages = new List<BubbleMessage>();

    public bool isShowingBubble
    {
        get
        {
            return
                this.Messages.Any(
                    current =>
                        current.CurrentState != BubbleMessage.AnimState.HIDE &&
                        current.CurrentState != BubbleMessage.AnimState.FINISHED);
        }
    }

    public static BubbleManager Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
		}
		Instance = this;

        if (UICamera.Instance != null)
        {
            GetComponentInChildren<Canvas>().worldCamera = UICamera.Instance.Camera;
        }
	}

	private BubbleMessage InstantiateBubble()
	{
		Object original = Resources.Load("CommonUI/MessageWindow");
	    GameObject instancePopup = Instantiate(original, new Vector3(0,0,0), Quaternion.identity) as GameObject;
	    instancePopup.transform.SetParent(base.gameObject.transform,false);
		BubbleMessage bubbleMessage = instancePopup.GetComponentsInChildren<BubbleMessage>(true)[0];
		instancePopup.name = "Bubble Message";
	    bubbleMessage.rectTransform().anchoredPosition = new Vector2(-99999, -99999);
	    bubbleMessage.OnDestroyEvent += OnDestroyBubble;
		return bubbleMessage;
	}

	private void OnDestroyBubble(BubbleMessage zBubble)
	{
	    zBubble.OnDestroyEvent -= OnDestroyBubble;
		this.Messages.RemoveAll((BubbleMessage q) => q == zBubble);
	}

    public BubbleMessage ShowMessage(string str, bool alreadyLocalised, Vector3 position,
        BubbleMessage.NippleDir nippleDir, float nipplePos,
        BubbleMessageConfig.ThemeStyle theme = BubbleMessageConfig.ThemeStyle.SMALL,
        BubbleMessageConfig.PositionType positionType = BubbleMessageConfig.PositionType.BOX_RELATIVE,
        float fontsize = 36, bool useBackdrop = false, bool showNipple = true, params RectTransform[] targetTransform)
    {
		BubbleMessageConfig config = new BubbleMessageConfig
        {
            Theme = theme,
            PosType = positionType,
            FontSize = fontsize
        };
        return this.ShowMessage(str, alreadyLocalised, position, nippleDir, nipplePos, config,
            useBackdrop,true, targetTransform);
    }

    public BubbleMessage ShowMessage(string str, bool alreadyLocalised, Vector3 position,
        BubbleMessage.NippleDir nippleDir, float nipplePos, BubbleMessageConfig config
        , bool useBackdrop = false, bool showNipple = true, params RectTransform[] targetTransform)
    {
		BubbleMessage bubbleMessage = this.InstantiateBubble();
        bubbleMessage.name = str;
        bubbleMessage.Create(str, alreadyLocalised, position, nippleDir, nipplePos, config, useBackdrop,showNipple, targetTransform);
        this.Messages.Add(bubbleMessage);
        return bubbleMessage;
    }

	public IEnumerator ShowMessageDelayed(float delay, string str, bool alreadyLocalised, Transform parent, Vector3 offset, BubbleMessage.NippleDir nippleDir, float nipplePos, BubbleMessageConfig.ThemeStyle style,bool useBackdrop , Action<BubbleMessage> callback = null)
	{
	    yield return new WaitForSeconds(delay);
	    ShowMessage(str, alreadyLocalised, offset, nippleDir, nipplePos, style,
	        BubbleMessageConfig.PositionType.BOX_RELATIVE, 36,useBackdrop, parent.rectTransform());
	}

	public void GarageIsZoomingIn()
	{
		foreach (BubbleMessage current in this.Messages)
		{
			if (!current.PersistThroughGarageFade)
			{
                //current.ChangeAllTints(0f);
				current.SetActive(false);
			}
			else
			{
				current.MoveToGaragePosition();
			}
		}
	}

	public void GarageIsZoomingOut()
	{
		foreach (BubbleMessage current in this.Messages)
		{
			if (!current.PersistThroughGarageFade)
			{
                //current.ChangeAllTints(1f);
				current.SetActive(true);
			}
			else
			{
				current.ResetToDefaultPosition();
			}
		}
	}

	public void FadeAllOut()
	{
		foreach (BubbleMessage current in this.Messages)
		{
            //current.ChangeAllTints(0f);
			current.gameObject.SetActive(false);
		}
	}

	public void FadeAllIn()
	{
		foreach (BubbleMessage current in this.Messages)
		{
			current.gameObject.SetActive(true);
            //current.ChangeAllTints(1f);
		}
	}

	public void DismissMessages()
	{
		foreach (BubbleMessage current in this.Messages)
		{
			current.Dismiss();
		}
	}

	public void KillAllMessages()
	{
		foreach (BubbleMessage current in this.Messages)
		{
			current.KillNow();
		}
	}

	public void OnDisable()
	{
	}
}
