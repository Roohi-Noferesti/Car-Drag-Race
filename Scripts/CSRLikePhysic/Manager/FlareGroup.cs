using System;
using UnityEngine;
using UnityEngine.UI;

public class FlareGroup : MonoBehaviour
{
	public FlareElement[] Elements;

	public Color GroupColor;

	private float oldAlpha;

	private void Awake()
	{
		this.Elements = base.GetComponentsInChildren<FlareElement>();
	}

	public void SaveAlpha()
	{
		this.oldAlpha = this.GroupColor.a;
	}

	public void InitialiseColor()
	{
		Color groupColor = this.GroupColor;
		groupColor.a = this.oldAlpha;
		this.GroupColor = groupColor;
	}

	public void UpdateColor()
	{
		//FlareElement[] elements = this.Elements;
		//for (int i = 0; i < elements.Length; i++)
		//{
		//	FlareElement flareElement = elements[i];
  //          flareElement.gameObject.GetComponent<Image>().renderer.material.SetColor("_Tint", this.GroupColor);
  //      }
	}

	public void FadeColorDelta(float deltaAlpha)
	{
		Color groupColor = this.GroupColor;
		groupColor.a = this.oldAlpha - deltaAlpha;
		this.GroupColor = groupColor;
	}

	public void FadeColorMultiply(float alpha)
	{
		this.GroupColor.a = this.oldAlpha * alpha;
	}
}
