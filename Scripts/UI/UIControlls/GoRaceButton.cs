using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GoRaceButton : MonoBehaviour
{
	public enum eState
	{
		Normal,
		Disabled
	}

	public GameObject NormalState;

	public GameObject DisabledState;

	public Button button;

	public TextMeshProUGUI RaceText;

	public string ButtonText = "TEXT_GO_RACE";

	public bool TextTranslated;

	public GoRaceButton.eState DefaultState;

	//private GoRaceButton.eState currentState;

    private List<Image> sprites = new List<Image>();

	private void Awake()
	{
		this.button = base.gameObject.GetComponent<Button>();
        //this.button.width = 1.1f;
		//this.sprites.AddRange(this.NormalState.GetComponentsInChildren<Image>());
        //this.sprites.AddRange(this.DisabledState.GetComponentsInChildren<Image>());
		if (this.TextTranslated)
		{
			this.RaceText.text = this.ButtonText;
		}
		else
		{
            this.RaceText.text = LocalizationManager.GetTranslation(this.ButtonText);
		}
	}

	private void Start()
	{
		this.SetState(this.DefaultState);
	}

	private void Update()
	{
        //if (this.button.controlState == UIButton.CONTROL_STATE.NORMAL && this.currentState != GoRaceButton.eState.Normal)
        //{
        //    this.SetState(GoRaceButton.eState.Normal);
        //}
        //else if ((this.button.controlState == UIButton.CONTROL_STATE.DISABLED || this.button.controlState == UIButton.CONTROL_STATE.ACTIVE) && this.currentState != GoRaceButton.eState.Disabled)
        //{
        //    this.SetState(GoRaceButton.eState.Disabled);
        //}
	}

	public void SetState(GoRaceButton.eState zState)
	{
		return;
		if (zState != GoRaceButton.eState.Normal)
		{
			if (zState == GoRaceButton.eState.Disabled)
			{
				this.NormalState.SetActive(false);
				this.DisabledState.SetActive(true);
			}
		}
		else
		{
			this.NormalState.SetActive(true);
			this.DisabledState.SetActive(false);
		}
		//this.currentState = zState;
	}

	public void ChangeAllAlphas(float alphaValue)
	{
        //foreach (PackedSprite current in this.sprites)
        //{
        //    Color color = current.color;
        //    color.a = alphaValue;
        //    current.SetColor(color);
        //}
	}
}
