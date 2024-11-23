using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativeSceneStandardLogo : MonoBehaviour, INarrativeSceneLogo
{
	public RawImage Logo;

	public TextMeshProUGUI Title;

    public TextMeshProUGUI SubTitle;

	public void SetOptionalStrings(string[] strings)
	{
		this.Title.text = ((strings.Length <= 0) ? string.Empty : strings[0]);
        this.SubTitle.text = ((strings.Length <= 1) ? string.Empty : strings[1]);
        //this.Title.gameObject.transform.localPosition = new Vector3(this.Logo.width + 0.08f, -0.04f, 0f);
		//this.SubTitle.gameObject.transform.localPosition = this.Title.gameObject.transform.localPosition + new Vector3(0f, -0.31f, 0f);
	}

	public void SetPreferredColour(Color colour)
	{
        this.Logo.color = colour;
        this.Title.color = colour;
        this.SubTitle.color = colour;
    }
}
