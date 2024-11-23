using UnityEngine;

internal class SplineRenderer
{
	private static Material lineMaterial;

	public static void RenderSpline(Spline spline)
	{
		SetMaterial();
		GL.Begin(1);
		GL.Color(Color.red);
		Vector3 p = spline.GetSplinePoint(0f);
		for (float num = 0.1f; num < spline.MaxT; num += 0.1f)
		{
			Vector3 splinePoint = spline.GetSplinePoint(num);
			GlLine(p, splinePoint);
			p = splinePoint;
		}
		if (spline.closed)
		{
			GlLine(p, spline.GetSplinePoint(0f));
		}
		for (float num2 = 0f; num2 < spline.MaxT; num2 += 0.25f)
		{
			Vector3 splinePoint2 = spline.GetSplinePoint(num2);
			Vector3 vector = spline.GetSplineTangent(num2);
			Vector3 vector2 = Vector3.Cross(vector, Camera.current.transform.forward);
			float d = 0.5f;
			vector = vector * d * 0.2f;
			vector2 = vector2 * d * 0.1f;
			if (num2 == 0f)
			{
				GlLine(splinePoint2 - vector2 * 2f, splinePoint2 + vector2 * 2f);
			}
			GlLine(splinePoint2, splinePoint2 - vector - vector2);
			GlLine(splinePoint2, splinePoint2 - vector + vector2);
		}
		GL.End();
	}

	public static void RenderTunnel(Spline spline)
	{
		SetMaterial();
		GL.Begin(1);
		BakedSpline bakedSpline = spline.BakedSpline;
		for (float num = 0f; num < 1f; num += bakedSpline.DistanceToT(5f))
		{
			RenderTunnelAtTime(bakedSpline, num);
		}
		GL.End();
	}

	public static void RenderTunnel(BakedSpline spline, int pointIndex, float mu)
	{
		SetMaterial();
		GL.Begin(1);
		RenderTunnelAtTime(spline, spline.PointToT(pointIndex, mu));
		GL.End();
	}

	private static void RenderTunnelAtTime(BakedSpline spline, float t)
	{
		BakedSpline.Point point;
		spline.GetPoint(t, out point);
		GL.Color(Color.red);
		RenderCircle(point.pos, point.tangent, point.innerSize);
		GL.Color(Color.blue);
		RenderCircle(point.pos, point.tangent, point.outerSize);
	}

	private static void RenderCircle(Vector3 pos, Vector3 splineForward, float size)
	{
		Vector3 vector = -Vector3.Cross(Camera.current.transform.up, splineForward).normalized;
		Vector3 normalized = Vector3.Cross(vector, splineForward).normalized;
		Vector3 a = vector * size;
		Vector3 a2 = normalized * size;
		int num = 20;
		for (int i = 0; i < num; i++)
		{
			float num2 = (float)i / (float)num;
			float num3 = (float)(i + 1) / (float)num;
			float f = num2 * 3.14159274f * 2f;
			float f2 = num3 * 3.14159274f * 2f;
			GlLine(pos + a * Mathf.Sin(f) + a2 * Mathf.Cos(f), pos + a * Mathf.Sin(f2) + a2 * Mathf.Cos(f2));
		}
	}

	public static void RenderBakedSpline(BakedSpline s)
	{
		SetMaterial();
		GL.Begin(1);
		GL.Color(Color.white);
		foreach (BakedSpline.Point current in s.points)
		{
			Vector3 b = Vector3.Cross(Vector3.up, current.tangent);
			GlLine(current.pos - b, current.pos + b);
		}
		GL.End();
	}

	private static void SetMaterial()
	{
		//if (!lineMaterial)
		//{
		//	lineMaterial = new Material("Shader \"Lines/Colored Blended\" { SubShader { Pass {   Blend SrcAlpha OneMinusSrcAlpha   ZWrite Off Cull Off Fog { Mode Off }   BindChannels {    Bind \"vertex\", vertex Bind \"color\", color } } } }");
		//	lineMaterial.hideFlags = HideFlags.HideAndDontSave;
		//	lineMaterial.shader.hideFlags = HideFlags.HideAndDontSave;
		//}
		//lineMaterial.SetPass(0);
	}

	private static void GlLine(Vector3 p1, Vector3 p2)
	{
		GL.Vertex3(p1.x, p1.y, p1.z);
		GL.Vertex3(p2.x, p2.y, p2.z);
	}
}
