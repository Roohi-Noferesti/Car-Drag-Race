using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("GT/Visuals/CarLightFlare")]
public class CarLightFlareData : MonoBehaviour
{
	public float AngleLimit = 45f;

	public AnimationCurve ScaleCurve;

	private void Awake()
	{
		CarLightFlare carLightFlare = base.gameObject.AddComponent<CarLightFlare>();
		carLightFlare.SetAngleLimit(this.AngleLimit);
		carLightFlare.ScaleCurve = this.ScaleCurve;
		Destroy(this);
	}

	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(base.transform.position, base.transform.position + base.transform.forward * 0.3f);
	}
}
