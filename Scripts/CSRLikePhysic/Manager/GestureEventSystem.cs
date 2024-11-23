using UnityEngine;

public class GestureEventSystem : MonoBehaviour
{
	public delegate void GestureEventHandler(GenericTouch zTouchData);

	public bool PrintDebugInfo = true;

	private int touchIDOfCurrentDrag = -1;

    public event GestureEventHandler Tap;

    public event GestureEventHandler Flick;

    public event GestureEventHandler Drag;

    public event GestureEventHandler DragUpdate;

    public event GestureEventHandler DragComplete;

	public static GestureEventSystem Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	private void InvokeTapEvent(GenericTouch gt)
	{
		if (this.PrintDebugInfo)
		{
		}
		if (this.Tap != null)
		{
			this.Tap(gt);
		}
	}

	private void InvokeFlickEvent(GenericTouch gt)
	{
		if (this.PrintDebugInfo)
		{
		}
		if (this.Flick != null)
		{
			this.Flick(gt);
		}
	}

	private void InvokeDragEvent(GenericTouch gt)
	{
		if (this.PrintDebugInfo)
		{
		}
		if (this.Drag != null)
		{
			this.Drag(gt);
		}
	}

	private void InvokeDragUpdateEvent(GenericTouch gt)
	{
		if (this.PrintDebugInfo)
		{
		}
		if (this.DragUpdate != null)
		{
			this.DragUpdate(gt);
		}
	}

	private void InvokeDragCompleteEvent(GenericTouch gt)
	{
		if (this.PrintDebugInfo)
		{
		}
		if (this.DragComplete != null)
		{
			this.DragComplete(gt);
		}
	}

	public void UpdateContinuousGestures()
	{
		if (this.touchIDOfCurrentDrag != -1)
		{
			GenericTouch touchFromTouchID = TouchManager.Instance.TouchCollection.GetTouchFromTouchID(this.touchIDOfCurrentDrag);
			if (touchFromTouchID.VALID)
			{
				if (touchFromTouchID.Phase == TouchPhase.Ended || touchFromTouchID.Phase == TouchPhase.Canceled)
				{
					this.InvokeDragCompleteEvent(touchFromTouchID);
					this.touchIDOfCurrentDrag = -1;
					TouchManager.AttemptToUseButton("DRAG END");
				}
				else
				{
					this.InvokeDragUpdateEvent(touchFromTouchID);
				}
			}
		}
	}

	public void CheckTouchManagerForGestures()
	{
		while (TouchManager.Instance.IsGestureAvailable)
		{
			GestureSample gestureSample = TouchManager.Instance.ReadGesture();
			GenericTouch touchFromTouchID = TouchManager.Instance.TouchCollection.GetTouchFromTouchID(gestureSample.TouchID);
			switch (gestureSample.GestureType)
			{
			case GestureType.Tap:
				this.InvokeTapEvent(touchFromTouchID);
				break;
			case GestureType.Flick:
				this.InvokeFlickEvent(touchFromTouchID);
				break;
			case GestureType.Drag:
				this.InvokeDragEvent(touchFromTouchID);
				this.touchIDOfCurrentDrag = gestureSample.TouchID;
				break;
			}
		}
	}
}
