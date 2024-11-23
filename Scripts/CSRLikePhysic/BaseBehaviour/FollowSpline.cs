using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Behaviours/FollowSpline")]
public class FollowSpline : BaseBehaviour
{
	public Spline spline;

	public float speed = 5f;

	public bool rotation = true;

	public float rotationLookAhead = 1f;

	private float ratioAlongSpline;

	private Vector3 lastValidRotation;

	private BakedSpline bakedSpline;

	//private float PercentageToStartAlongSpline;

	public override void OnActivate()
	{
		if (this.spline == null)
		{
		}
		this.bakedSpline = this.spline.BakedSpline;
		//this.ratioAlongSpline = this.PercentageToStartAlongSpline / 100f;
	}

	public override void OnUpdate(ref CameraState zResult)
	{
		BakedSpline.Point point;
		this.bakedSpline.GetPoint(this.ratioAlongSpline, out point);
		this.ratioAlongSpline += this.speed * point.speed / this.bakedSpline.length * Time.deltaTime;
		float num = this.ratioAlongSpline + this.rotationLookAhead / this.bakedSpline.length;
		zResult.position = point.pos;
		if (this.rotation)
		{
			if (num > 1f)
			{
				if (this.lastValidRotation.magnitude > 0f)
				{
					zResult.rotation = Quaternion.LookRotation(this.lastValidRotation);
				}
			}
			else
			{
				BakedSpline.Point point2;
				this.bakedSpline.GetPoint(num, out point2);
				Vector3 forward = point2.pos - point.pos;
				this.lastValidRotation = forward;
				forward.Normalize();
				zResult.rotation = Quaternion.LookRotation(forward);
			}
		}
	}
}
