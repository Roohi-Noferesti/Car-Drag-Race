using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CostContainerBlueButton : MonoBehaviour
{
	public TextMeshProUGUI TitleText;

	public RuntimeTextButton BlueButton;

	private void Awake()
	{
        //this.BlueButton.ForceAwake();
	}

	public void Setup(string title, string buttonText, BaseRuntimeControl.State state, UnityAction callBack, bool makeButtonUpper = true)
	{
        //if (this._characterSize == 0f)
        //{
        //    this._characterSize = this.TitleText.characterSize;
        //}
        //this.TitleText.SetCharacterSize(this._characterSize);
        //if (!string.IsNullOrEmpty(title))
        //{
        //    float width = this.TitleText.GetWidth(title);
        //    this.TitleText.SetCharacterSize(Mathf.Min(this._characterSize, this._characterSize * this.TitleText.maxWidth / width));
        //}
        if (this.TitleText!=null)
            this.TitleText.text = title;
        this.BlueButton.SetText(buttonText, true, makeButtonUpper);
        this.BlueButton.CurrentState = state;
	    this.BlueButton.SetCallback(callBack);
	}

    public void SetupFree(string title, UnityAction method)
    {
        this.Setup(title, LocalizationManager.GetTranslation("TEXT_COST_BOX_HEADING_FREE"), BaseRuntimeControl.State.Active, method,
            false);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
