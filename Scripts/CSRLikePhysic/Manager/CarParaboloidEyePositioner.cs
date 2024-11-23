using System;
using UnityEngine;

public class CarParaboloidEyePositioner : MonoBehaviour
{
	public int OffsetAmount = 4;

	private Vector3 _startPosition;

	private void Awake()
	{
		this._startPosition = base.transform.localPosition;
	}

	private void OnDisable()
	{
		base.transform.localPosition = this._startPosition;
	}

	private void Update()
	{
		Camera main = Camera.main;
		Vector3 position = base.transform.parent.position;
		position.y = 0f;
		Vector3 position2 = main.transform.position;
		position2.y = 0f;
		Vector3 lhs = position2 - position;
		lhs.Normalize();
		float num = Vector3.Dot(lhs, base.transform.forward);
		base.transform.localPosition = this._startPosition;
		base.transform.Translate(new Vector3(0f, 0f, (float)this.OffsetAmount * num), base.transform.parent.parent);
	}
}
