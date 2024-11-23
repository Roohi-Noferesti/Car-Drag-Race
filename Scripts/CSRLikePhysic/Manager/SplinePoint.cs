using System.Diagnostics;
using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Spline/SplinePoint")]
public class SplinePoint : MonoBehaviour
{
	public float innerSize = 1f;

	public float outerSize = 2f;

	public float speed = 1f;

	public Spline Spline
	{
		get
		{
			return base.transform.parent.GetComponent<Spline>();
		}
	}

	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
		Gizmos.DrawIcon(base.transform.position, "SplinePoint-Green.png");
	}
}
