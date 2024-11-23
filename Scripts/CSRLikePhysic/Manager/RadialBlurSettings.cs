using System;
using UnityEngine;

[Serializable]
public class RadialBlurSettings
{
	public Vector2 Position = new Vector2(0.25f, 0.5f);

	public float Radius = 0.5f;

	[Range(0f, 360f)]
	public float Angle = 60f;

	public int BlurPasses = 2;
}
