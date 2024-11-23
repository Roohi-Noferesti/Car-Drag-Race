using System;
using UnityEngine;

public abstract class BaseDummyControl : MonoBehaviour
{
	public virtual void ForceAwake()
	{
        //GameObjectHelper.MakeLocalPositionPixelPerfect(base.gameObject);
	}

	public abstract GameObject GetControl();
}
