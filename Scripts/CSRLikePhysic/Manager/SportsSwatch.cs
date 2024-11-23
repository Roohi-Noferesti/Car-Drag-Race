using System;
using UnityEngine;
using UnityEngine.UI;

public class SportsSwatch : MonoBehaviour
{
	public Image TopSwatch;

	public Image MiddleSwatch;

	public Image BottomSwatch;

	public void SetupSwatch(SportsSwatchType swatch, Color colour)
	{
		this.TopSwatch.gameObject.SetActive(false);
		this.MiddleSwatch.gameObject.SetActive(false);
		this.BottomSwatch.gameObject.SetActive(false);
		switch (swatch)
		{
		case SportsSwatchType.TOP:
			this.TopSwatch.gameObject.SetActive(true);
		        this.TopSwatch.color = colour;
			break;
		case SportsSwatchType.MIDDLE:
			this.MiddleSwatch.gameObject.SetActive(true);
            this.MiddleSwatch.color = colour;
			break;
		case SportsSwatchType.BOTTOM:
			this.BottomSwatch.gameObject.SetActive(true);
            this.BottomSwatch.color = colour;
			break;
		}
	}
}
