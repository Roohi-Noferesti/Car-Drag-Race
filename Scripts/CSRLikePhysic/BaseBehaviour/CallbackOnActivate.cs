using UnityEngine;

public class CallbackOnActivate : BaseBehaviour
{
	public MonoBehaviour Script;

	public string MethodToCall;

	public override void OnActivate()
	{
		if (this.Script == null)
		{
			return;
		}
		this.Script.SendMessage(this.MethodToCall, SendMessageOptions.RequireReceiver);
	}

	public override void OnUpdate(ref CameraState zResult)
	{
	}
}
