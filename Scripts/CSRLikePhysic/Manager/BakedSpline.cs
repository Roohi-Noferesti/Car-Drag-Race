using System.Collections.Generic;
using UnityEngine;

public class BakedSpline
{
	public struct Point
	{
		public float globalT;

		public Vector3 pos;

		public Vector3 tangent;

		public float innerSize;

		public float outerSize;

		public float speed;

		public Point(float t, Vector3 pos, Vector3 tangent, float innerSize, float outerSize, float speed)
		{
			this.globalT = t;
			this.pos = pos;
			this.tangent = tangent;
			this.innerSize = innerSize;
			this.outerSize = outerSize;
			this.speed = speed;
		}
	}

	private class Builder
	{
		public List<Point> points;

		private Spline spline;

		private float sqrMaxSeparation;

		private float t;

		private Vector3 lastPos;

		private Point currentPoint;

		public Builder(Spline spline, float maxSeparation)
		{
			this.spline = spline;
			this.points = new List<Point>();
			this.sqrMaxSeparation = maxSeparation * maxSeparation;
			this.t = 0f;
			this.Build();
		}

		private void Build()
		{
			this.GetPoint();
			while (this.t < this.spline.MaxT)
			{
				this.AddPoint();
				this.FindNext();
			}
		}

		private void GetPoint()
		{
			this.currentPoint.globalT = this.t;
			this.spline.GetSplineData(this.t, out this.currentPoint.pos, out this.currentPoint.tangent, out this.currentPoint.innerSize, out this.currentPoint.outerSize, out this.currentPoint.speed);
		}

		private void AddPoint()
		{
			this.lastPos = this.currentPoint.pos;
			this.points.Add(this.currentPoint);
		}

		private void FindNext()
		{
			float num = this.t;
			float maxT = this.spline.MaxT;
			int num2 = 20;
			this.sqrMaxSeparation = 1f;
			float num3;
			do
			{
				this.t = num + (maxT - num) * 0.5f;
				this.GetPoint();
				float sqrMagnitude = (this.currentPoint.pos - this.lastPos).sqrMagnitude;
				if (sqrMagnitude < this.sqrMaxSeparation)
				{
					num = this.t;
				}
				else
				{
					maxT = this.t;
				}
				num2--;
				num3 = Mathf.Abs(sqrMagnitude - this.sqrMaxSeparation);
			}
			while (num3 > 0.01f && num2 > 0);
		}
	}

	public List<Point> points;

	public float length
	{
		get;
		private set;
	}

	public BakedSpline(Spline spline)
	{
		float maxSeparation = 0.1f;
		Builder builder = new Builder(spline, maxSeparation);
		this.points = builder.points;
		float num = 0f;
		for (int i = 0; i < this.points.Count - 1; i++)
		{
			num += (this.points[i].pos - this.points[i + 1].pos).magnitude;
		}
		this.length = num;
	}

	public float DistanceToT(float speed)
	{
		return speed / this.length;
	}

	public void GetPoint(float t, out Point point)
	{
		float num = t * (float)this.points.Count;
		int num2 = Mathf.FloorToInt(num);
		float mu = num - (float)num2;
		if (num2 >= this.points.Count - 1)
		{
			num2 = this.points.Count - 2;
			mu = 1f;
		}
		this.GetPoint(num2, mu, out point);
	}

	public void GetPoint(int pointIndex, float mu, out Point point)
	{
		if (pointIndex < 0)
		{
			pointIndex += this.points.Count;
		}
		pointIndex %= this.points.Count;
		int index = (pointIndex + 1) % this.points.Count;
		Point point2 = this.points[pointIndex];
		Point point3 = this.points[index];
		point.globalT = Mathf.Lerp(point2.globalT, point3.globalT, mu);
		point.pos = Vector3.Lerp(point2.pos, point3.pos, mu);
		point.tangent = Vector3.Lerp(point2.tangent, point3.tangent, mu);
		point.innerSize = Mathf.Lerp(point2.innerSize, point3.innerSize, mu);
		point.outerSize = Mathf.Lerp(point2.outerSize, point3.outerSize, mu);
		point.speed = Mathf.Lerp(point2.speed, point3.speed, mu);
	}

	public float PointToT(int pointIndex, float mu)
	{
		return ((float)pointIndex + mu) / (float)this.points.Count;
	}

	public Vector3 GetPointPos(int index)
	{
		return this.points[index].pos;
	}

	public void Search(Vector3 pos, int start, int end, out int pointIndex, out float mu)
	{
		pointIndex = 0;
		mu = 0f;
		if (!this.SearchWorker(pos, start, end, ref pointIndex, ref mu))
		{
			this.SearchWorker(pos, 0, 0, ref pointIndex, ref mu);
		}
	}

	private bool SearchWorker(Vector3 pos, int start, int end, ref int pointIndex, ref float mu)
	{
		start = this.FixupIndex(start);
		end = this.FixupIndex(end);
		int num = start;
		float num2 = 3.40282347E+38f;
		int num3 = -1;
		float num4 = 0f;
		do
		{
			Point point = this.points[num];
			Point point2 = this.points[(num + 1) % this.points.Count];
			float num5 = Vector3.Dot(pos - point.pos, point.tangent);
			float num6 = Vector3.Dot(point2.pos - pos, point2.tangent);
			if (num5 > 0f)
			{
				float num7 = num5 / (num5 + num6);
				Vector3 b = Vector3.Lerp(this.points[num].pos, this.points[(num + 1) % this.points.Count].pos, num7);
				float sqrMagnitude = (pos - b).sqrMagnitude;
				if (sqrMagnitude < num2)
				{
					num2 = sqrMagnitude;
					num3 = num;
					num4 = num7;
				}
			}
			num = this.FixupIndex(num + 1);
		}
		while (num != end);
		if (num3 >= 0)
		{
			pointIndex = num3;
			mu = num4;
			return true;
		}
		return false;
	}

	public int FixupIndex(int index)
	{
		while (index >= this.points.Count)
		{
			index -= this.points.Count;
		}
		while (index < 0)
		{
			index += this.points.Count;
		}
		return index;
	}
}
