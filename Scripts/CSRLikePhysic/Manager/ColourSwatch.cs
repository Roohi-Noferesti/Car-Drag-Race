using System;
using UnityEngine;

[Serializable]
public class ColourSwatch
{
	public static readonly ColourSwatch Default = new ColourSwatch
	{
		Primary = Color.magenta,
		BarColor1 = Color.magenta,
		BarColor2 = Color.green
	};

	public Color Primary = Color.magenta;

	public Color BarColor1 = Color.magenta;

	public Color BarColor2 = Color.green;
}
