using System;
using UnityEngine;

public class LoadingIconRotator : MonoBehaviour
{
	public int chunks = 4;

	public float time = 0.1f;

	private float _timer;

	private int currentChunk;

	private void Update()
	{
		float num = this.time / (float)this.chunks;
		this._timer += Time.deltaTime;
		if (this._timer >= num)
		{
			this._timer = 0f;
			this.currentChunk--;
			this.currentChunk %= this.chunks;
		}
		base.transform.localEulerAngles = new Vector3(0f, 0f, (float)this.currentChunk * (360f / (float)this.chunks));
	}
}
