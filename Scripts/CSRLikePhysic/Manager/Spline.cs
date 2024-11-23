using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Spline/Spline")]
public class Spline : MonoBehaviour
{
	private struct HermitePoints
	{
		public SplinePoint p0;

		public SplinePoint p1;

		public SplinePoint p2;

		public SplinePoint p3;

		public float mu;

		public Vector3 position
		{
			get
			{
				Hermite hermite = new Hermite(this.mu);
				return hermite.Interpolate(this.p0.transform.position, this.p1.transform.position, this.p2.transform.position, this.p3.transform.position);
			}
		}

		public Vector3 tangent
		{
			get
			{
				HermiteTangent hermiteTangent = new HermiteTangent(this.mu);
				return hermiteTangent.Interpolate(this.p0.transform.position, this.p1.transform.position, this.p2.transform.position, this.p3.transform.position);
			}
		}

		public float innerSize
		{
			get
			{
				Hermite hermite = new Hermite(this.mu);
				return hermite.Interpolate(this.p0.innerSize, this.p1.innerSize, this.p2.innerSize, this.p3.innerSize);
			}
		}

		public float outerSize
		{
			get
			{
				Hermite hermite = new Hermite(this.mu);
				return hermite.Interpolate(this.p0.outerSize, this.p1.outerSize, this.p2.outerSize, this.p3.outerSize);
			}
		}

		public float speed
		{
			get
			{
				Hermite hermite = new Hermite(this.mu);
				return hermite.Interpolate(this.p0.speed, this.p1.speed, this.p2.speed, this.p3.speed);
			}
		}

		public HermitePoints(Spline spline, float globalT)
		{
			if (spline.closed)
			{
				while (globalT < 0f)
				{
					globalT += (float)spline.points.Count;
				}
				globalT %= (float)spline.points.Count;
			}
			else
			{
				globalT = Mathf.Clamp(globalT, 0f, (float)(spline.points.Count - 1));
			}
			float num = Mathf.Floor(globalT);
			this.mu = globalT - num;
			int num2 = (int)num;
			int num3 = num2 - 1;
			int num4 = num2 + 1;
			int num5 = num2 + 2;
			if (spline.closed)
			{
				num3 = (num3 + spline.points.Count) % spline.points.Count;
				num4 %= spline.points.Count;
				num5 %= spline.points.Count;
			}
			else
			{
				num3 = Mathf.Clamp(num3, 0, spline.points.Count - 1);
				num4 = Mathf.Clamp(num4, 0, spline.points.Count - 1);
				num5 = Mathf.Clamp(num5, 0, spline.points.Count - 1);
			}
			this.p0 = spline.points[num3];
			this.p1 = spline.points[num2];
			this.p2 = spline.points[num4];
			this.p3 = spline.points[num5];
		}
	}

	public List<SplinePoint> points = new List<SplinePoint>();

	public bool closed;

	private BakedSpline bakedSpline;

	public bool renderSpline = true;

	public bool renderTunnel = true;

	public bool renderBakedSpline;

	public BakedSpline BakedSpline
	{
		get
		{
			if (this.bakedSpline == null)
			{
				this.bakedSpline = new BakedSpline(this);
			}
			return this.bakedSpline;
		}
	}

	public float MaxT
	{
		get
		{
			if (this.closed)
			{
				return (float)this.points.Count;
			}
			return (float)(this.points.Count - 1);
		}
	}

	private void Start()
	{
		this.ResetBakedSpline();
	}

	private void DrawSpline()
	{
		if (!this.renderSpline)
		{
			return;
		}
		SplineRenderer.RenderSpline(this);
	}

	private void DrawTunnel()
	{
		if (!this.renderTunnel)
		{
			return;
		}
		SplineRenderer.RenderTunnel(this);
	}

	private void DrawBakedSpline()
	{
		if (!this.renderBakedSpline)
		{
			return;
		}
		SplineRenderer.RenderBakedSpline(this.BakedSpline);
	}

	public float GetPointTime(SplinePoint point)
	{
		return (float)this.points.IndexOf(point);
	}

	public Vector3 GetSplinePoint(float t)
	{
		HermitePoints hermitePoints = new HermitePoints(this, t);
		return hermitePoints.position;
	}

	public Vector3 GetSplineTangent(float t)
	{
		HermitePoints hermitePoints = new HermitePoints(this, t);
		return hermitePoints.tangent;
	}

	public void GetSplineData(float t, out Vector3 pos, out Vector3 tangent, out float innerSize, out float outerSize, out float speed)
	{
		HermitePoints hermitePoints = new HermitePoints(this, t);
		pos = hermitePoints.position;
		tangent = hermitePoints.tangent;
		innerSize = hermitePoints.innerSize;
		outerSize = hermitePoints.outerSize;
		speed = hermitePoints.speed;
	}

	public float FindNearestTime(Vector3 position)
	{
		float result = 0f;
		float num = 3.40282347E+38f;
		int num2 = 10;
		float num3 = 1f / (float)num2;
		for (float num4 = 0f; num4 < this.MaxT; num4 += num3)
		{
			float sqrMagnitude = (position - this.GetSplinePoint(num4)).sqrMagnitude;
			if (sqrMagnitude < num)
			{
				num = sqrMagnitude;
				result = num4;
			}
		}
		return result;
	}

	public void ResetBakedSpline()
	{
		this.bakedSpline = null;
	}
}
