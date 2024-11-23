using UnityEngine;

public struct Hermite
{
	private float blend1;

	private float blend2;

	private float blend3;

	private float blend4;

	public Hermite(float mu)
	{
		float num = mu * mu;
		float num2 = mu * mu * mu;
		this.blend1 = 2f * num2 - 3f * num + 1f;
		this.blend2 = -2f * num2 + 3f * num;
		this.blend3 = num2 - 2f * num + mu;
		this.blend4 = num2 - num;
	}

	public Vector3 Interpolate(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
	{
		Vector3 a = (p2 - p0) * 0.5f;
		Vector3 a2 = (p3 - p1) * 0.5f;
		return p1 * this.blend1 + p2 * this.blend2 + a * this.blend3 + a2 * this.blend4;
	}

	public float Interpolate(float p0, float p1, float p2, float p3)
	{
		float num = (p2 - p0) * 0.5f;
		float num2 = (p3 - p1) * 0.5f;
		return p1 * this.blend1 + p2 * this.blend2 + num * this.blend3 + num2 * this.blend4;
	}
}
