using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/BaseBehaviour")]
public abstract class BaseBehaviour : MonoBehaviour
{
	public virtual float TimeToEnd(CameraState zResult)
	{
		return float.PositiveInfinity;
	}

	public abstract void OnActivate();

	public abstract void OnUpdate(ref CameraState zResult);
}
