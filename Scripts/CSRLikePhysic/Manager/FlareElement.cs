using System;
using UnityEngine;
using UnityEngine.UI;

public class FlareElement : MonoBehaviour
{
	public enum ElementNumber
	{
		Large,
		Medium1,
		Medium2,
		Small1,
		Small2,
		Small3,
		Small4,
		Small5,
		Small6,
		Small7,
		Small8
	}

	public FlareElement.ElementNumber ElementChosen;

	public float ElementPosition;

	public float ElementSize;

	private Vector2 PictureSize = new Vector2(256f, 512f);

	private float LineMinRange = -1f;

	private float LineMaxRange = 1f;

	private float yAxisPosition;

	private void Start()
	{
		this.SetUpElement(this.ElementChosen);
	}

	private void SetUpElement(FlareElement.ElementNumber eNumb)
	{
        //Image component = base.gameObject.GetComponent<Image>();
		Vector2 zero = Vector2.zero;
		Vector2 zero2 = Vector2.zero;
		switch (eNumb)
		{
		case FlareElement.ElementNumber.Large:
			zero.y = this.PictureSize.y / 2f;
			zero2.Set(this.PictureSize.x, this.PictureSize.y / 2f);
			break;
		case FlareElement.ElementNumber.Medium1:
			zero.Set(0f, 3f * this.PictureSize.y / 4f);
			zero2.Set(this.PictureSize.x / 2f, this.PictureSize.y / 4f);
			break;
		case FlareElement.ElementNumber.Medium2:
			zero.Set(0f, this.PictureSize.y);
			zero2.Set(this.PictureSize.x / 2f, this.PictureSize.y / 4f);
			break;
		case FlareElement.ElementNumber.Small1:
			zero.Set(this.PictureSize.x / 2f, 5f * this.PictureSize.y / 8f);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small2:
			zero.Set(this.PictureSize.x / 2f, 6f * this.PictureSize.y / 8f);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small3:
			zero.Set(this.PictureSize.x / 2f, 7f * this.PictureSize.y / 8f);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small4:
			zero.Set(this.PictureSize.x / 2f, this.PictureSize.y);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small5:
			zero.Set(3f * this.PictureSize.x / 4f, 5f * this.PictureSize.y / 8f);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small6:
			zero.Set(3f * this.PictureSize.x / 4f, 6f * this.PictureSize.y / 8f);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small7:
			zero.Set(3f * this.PictureSize.x / 4f, 7f * this.PictureSize.y / 8f);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		case FlareElement.ElementNumber.Small8:
			zero.Set(3f * this.PictureSize.x / 4f, this.PictureSize.y);
			zero2.Set(this.PictureSize.x / 4f, this.PictureSize.y / 8f);
			break;
		}
        //component.Setup(this.ElementSize, this.ElementSize, zero, zero2);
		this.checkPositionvalue();
	}

	public void SetYPosition(float yPosition)
	{
		this.yAxisPosition = yPosition;
	}

	public float GetYPosition()
	{
		return this.yAxisPosition;
	}

	private void checkPositionvalue()
	{
		if (this.ElementPosition < this.LineMinRange)
		{
			this.ElementPosition = this.LineMinRange;
		}
		else if (this.ElementPosition > this.LineMaxRange)
		{
			this.ElementPosition = this.LineMaxRange;
		}
	}
}
