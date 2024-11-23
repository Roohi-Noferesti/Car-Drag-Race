using System;
using KingKodeStudio;
using TMPro;
using UnityEngine;

public class EventHubProgressLayout : MonoBehaviour
{
	private const float extraSpacing = 0.08f;

	public float titleWidth;

	public float progressionWidth;

	public float height;

    //public PackedSprite Left;

    //public PackedSprite LeftBorder;

    //public PackedSprite LeftMiddle;

    //public PackedSprite CenterMiddle;

    //public PackedSprite RightMiddle;

    //public PackedSprite TopBorder;

    //public PackedSprite BottomBorder;

    //public PackedSprite Right;

    //public PackedSprite RightBorder;

	public TextMeshProUGUI EventNameText;

    public TextMeshProUGUI ProgressionText;

	public void UpdateLayout()
	{
        //this.Left.height = this.height;
        //this.LeftBorder.height = this.height;
        //this.LeftMiddle.height = this.height;
        //this.CenterMiddle.height = this.height;
        //this.RightMiddle.height = this.height;
        //this.Right.height = this.height;
        //this.RightBorder.height = this.height;
        //this.LeftMiddle.width = this.titleWidth;
        //this.RightMiddle.width = this.progressionWidth;
        //float num = this.titleWidth + this.progressionWidth;
        //this.TopBorder.width = num;
        //this.BottomBorder.width = num;
        //this.LeftMiddle.transform.localPosition = new Vector3(this.Left.transform.localPosition.x + this.Left.width, 0f, 0f);
        //this.CenterMiddle.transform.localPosition = new Vector3(this.LeftMiddle.transform.localPosition.x + this.titleWidth - this.CenterMiddle.width, 0f, -0.05f);
        //this.RightMiddle.transform.localPosition = new Vector3(this.CenterMiddle.transform.localPosition.x + this.CenterMiddle.width, 0f, 0f);
        //this.Right.transform.localPosition = new Vector3(this.RightMiddle.transform.localPosition.x + this.progressionWidth, 0f, 0f);
        //this.RightBorder.transform.localPosition = new Vector3(this.RightMiddle.transform.localPosition.x + this.progressionWidth + this.Right.width, 0f, -0.1f);
        //this.TopBorder.transform.localPosition = new Vector3(this.Left.transform.localPosition.x + this.Left.width, 0f, -0.1f);
        //this.BottomBorder.transform.localPosition = new Vector3(this.Left.transform.localPosition.x + this.Left.width, this.BottomBorder.height / 2f - this.height, -0.1f);
        //this.EventNameText.transform.localPosition = new Vector3((this.LeftMiddle.width - this.CenterMiddle.width) / 2f, -this.height / 2f - this.TopBorder.height, -0.1f);
        //this.ProgressionText.transform.localPosition = new Vector3(this.RightMiddle.width / 2f, -this.height / 2f - this.TopBorder.height, -0.1f);
        //float num2 = num + this.Left.width + this.Right.width;
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen != null)
		{
            //base.transform.localPosition = new Vector3((-num2 - careerModeMapScreen.eventPane.PaneWidthTight) / 2f, -CommonUI.Instance.NavBar.GetHeightTight() / 2f + 0.08f, -0.3f);
		}
	}

	public void SetColour(Color colour)
	{
        //this.LeftBorder.renderer.material.SetColor("_Tint", colour);
        //this.CenterMiddle.renderer.material.SetColor("_Tint", colour);
        //this.RightMiddle.renderer.material.SetColor("_Tint", colour);
        //this.TopBorder.renderer.material.SetColor("_Tint", colour);
        //this.BottomBorder.renderer.material.SetColor("_Tint", colour);
        //this.Right.renderer.material.SetColor("_Tint", colour);
        //this.RightBorder.renderer.material.SetColor("_Tint", colour);
	}

	public void UpdateContent(string eventName, string progression)
	{
		this.EventNameText.text = eventName.ToUpper();
        //this.titleWidth = this.EventNameText.TotalWidth + 0.08f;
        this.ProgressionText.text = progression;
        //this.progressionWidth = this.ProgressionText.TotalWidth;
		this.UpdateLayout();
	}
}
