using System;
using UnityEngine;

[AddComponentMenu("GT/Visuals/CarLiveryProjector")]
public class CarLiveryProjector : MonoBehaviour
{
	public Texture2D liveryTexture;

	public Color liveryTint = Color.white;

	public bool enableSymmetryX;

	public Vector2 textureFlip
	{
		get
		{
			return new Vector2(Mathf.Sign(base.transform.localScale.x), Mathf.Sign(base.transform.localScale.y));
		}
	}
}
