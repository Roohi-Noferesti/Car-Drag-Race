using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchCollection
{
	private List<GenericTouch> previousTouchBuffer = new List<GenericTouch>();

	private List<GenericTouch> touchBuffer = new List<GenericTouch>();

	public int TouchCount
	{
		get
		{
			return this.touchBuffer.Count;
		}
	}

	public GenericTouch GetTouchFromIndex(int zIndex)
	{
		if (zIndex < this.touchBuffer.Count && zIndex >= 0)
		{
			return this.touchBuffer[zIndex];
		}
		return GenericTouch.Empty;
	}

	public GenericTouch GetTouchFromTouchID(int zTouchID)
	{
		foreach (GenericTouch current in this.touchBuffer)
		{
			if (current.TouchID == zTouchID)
			{
				return current;
			}
		}
		return GenericTouch.Empty;
	}

	public void Update()
	{
		if (this.previousTouchBuffer.Count > 0)
		{
			this.previousTouchBuffer.Clear();
		}
		if (this.touchBuffer.Count > 0)
		{
			this.previousTouchBuffer.AddRange(this.touchBuffer);
			this.touchBuffer.Clear();
		}

		if (false)//Input.multiTouchEnabled)
		{
			//Touch[] touches = Input.touches;
			//for (int i = 0; i < touches.Length; i++)
			//{
			//	Touch zTouch = touches[i];
			//	GenericTouch item = TouchHelper.CreateFromUnityTouch(zTouch, this.previousTouchBuffer);
			//	this.touchBuffer.Add(item);
			//}
		}
	    bool flag = true;//!Input.multiTouchEnabled;
		if (flag && MouseTouchDetector.Touching)
		{
			this.touchBuffer.Add(MouseTouchDetector.MouseTouch);
		}
	}
}
