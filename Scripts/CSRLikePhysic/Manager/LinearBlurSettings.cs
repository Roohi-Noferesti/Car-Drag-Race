using System;
using UnityEngine;

[Serializable]
public class LinearBlurSettings
{
	public float Length = 0.1f;

	public Vector2 RectPosition = new Vector2(0.5f, 0.5f);

	public Vector2 RectSize = new Vector2(0.5f, 0.5f);

	public int BlurPasses = 1;
}
