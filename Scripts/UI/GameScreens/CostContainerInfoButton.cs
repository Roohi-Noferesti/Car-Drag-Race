using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CostContainerInfoButton : MonoBehaviour
{
	public enum Icons
	{
		Multiplayer,
		WorldTour,
		RaceYourFriends,
		CSRClassics,
		International,
		None
	}

	public TextMeshProUGUI TitleText;

	public RuntimeTextButton BlueButton;

	public GameObject[] IconSprites;

	//public Vector3 NoButtonTextPos;

	//private Vector3 defaultTextBoxPos;

	private void Awake()
	{
        //this.BlueButton.ForceAwake();
		//this.defaultTextBoxPos = this.TitleText.gameObject.transform.localPosition;
	}

	public void Setup(string title, Icons icon, string buttonText, BaseRuntimeControl.State state, UnityAction method, bool makeButtonUpper = true)
	{
		this.TitleText.text = title;
		if (!string.IsNullOrEmpty(buttonText))
		{
			//this.TitleText.gameObject.transform.localPosition = this.defaultTextBoxPos;
			this.BlueButton.gameObject.SetActive(true);
            this.BlueButton.SetText(buttonText, true, makeButtonUpper);
            this.BlueButton.CurrentState = state;
            this.BlueButton.SetCallback(method);
		}
		else
		{
			//this.TitleText.gameObject.transform.localPosition = this.NoButtonTextPos;
			this.BlueButton.gameObject.SetActive(false);
		}
		for (int num = 0; num != this.IconSprites.Length; num++)
		{
			if (this.IconSprites[num] != null)
			{
				this.IconSprites[num].gameObject.SetActive(num == (int)icon);
			}
		}
	}
}
