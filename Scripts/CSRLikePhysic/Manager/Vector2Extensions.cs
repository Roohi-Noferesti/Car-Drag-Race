using DataSerialization;
using System;
using UnityEngine;

public static class Vector2Extensions
{
	public static UnityEngine.Vector2 AsUnityVector2(this DataSerialization.Vector2 vec2)
	{
		return new UnityEngine.Vector2(vec2.x, vec2.y);
	}

	public static UnityEngine.Vector3 AsUnityVector3(this DataSerialization.Vector2 vec2)
	{
		return new UnityEngine.Vector3(vec2.x, vec2.y, 0f);
	}
}
