using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RankingEntry : ListItem
{
    //public WindowPaneBatched BackgroundPane;

    public Transform TextParent;

	public TextMeshProUGUI TextName;

	public float Width;

	private List<TextMeshProUGUI> TextEntries = new List<TextMeshProUGUI>();

	private float FontSize;

	protected override void OnDestroy()
	{
		foreach (TextMeshProUGUI current in this.TextEntries)
		{
			UnityEngine.Object.Destroy(current.gameObject);
		}
		base.OnDestroy();
	}

	public void SetText(GameObject TextMeshProUGUIPrefab, string Text, float PercentOffset, TextAlignmentOptions alignment, float maxWidth = 0f)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(TextMeshProUGUIPrefab) as GameObject;
		TextMeshProUGUI component = gameObject.GetComponent<TextMeshProUGUI>();
		float num = PercentOffset / 100f;
		component.transform.parent = this.TextParent;
		component.text = Text;
		component.transform.localPosition = new Vector3(num * this.Width, 0f, 0f);
        //component.SetCharacterSize(this.FontSize);
        //component.SetAlignment(alignment);
        //component.maxWidth = maxWidth;
        //component.multiline = false;
        //TextMeshProUGUI.Anchor_Pos anchor = TextMeshProUGUI.Anchor_Pos.Middle_Center;
        //if (alignment == TextMeshProUGUI.Alignment_Type.Left)
        //{
        //    anchor = TextMeshProUGUI.Anchor_Pos.Middle_Left;
        //}
        //else if (alignment == TextMeshProUGUI.Alignment_Type.Right)
        //{
        //    anchor = TextMeshProUGUI.Anchor_Pos.Middle_Right;
        //}
        //component.SetAnchor(anchor);
		this.TextEntries.Add(component);
	}

	public void SetFontSize(float size)
	{
        //foreach (TextMeshProUGUI current in this.TextEntries)
        //{
        //    current.SetCharacterSize(size);
        //}
		this.FontSize = size;
	}

	public static string GetOrdinal(int rank)
	{
		int num = rank % 10;
		if (num == 1 && rank != 11)
		{
			return "st";
		}
		if (num == 2 && rank != 12)
		{
			return "nd";
		}
		if (num == 3 && rank != 13)
		{
			return "rd";
		}
		return "th";
	}

	public void UpdateSize()
	{
        //this.BackgroundPane.Width = this.Width;
        //this.BackgroundPane.UpdateSize();
	}
}
