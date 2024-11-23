using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpotPrizeBarBox : MonoBehaviour
{
	public TextMeshProUGUI Content;

	public Image Left;

    public Image Middle;

    public Image Right;

    public Image Point;

	public float Padding;

	public void SetText(string text)
	{
		this.Content.text = text;
        ////this.Middle.SetSize(this.Content.TotalWidth + this.Padding, this.Middle.height);
	    float num = 0;//0.5f * this.Middle.width;
		this.Left.transform.localPosition = new Vector3(-num, 0f, 0f);
		this.Right.transform.localPosition = new Vector3(num, 0f, 0f);
	}
}
