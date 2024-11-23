using UnityEngine;

public class EditableCurve : ScriptableObject
{
    //[HideInInspector]
	public AnimationCurve animationCurve;

	public virtual void SetDefault()
	{
	}
}
