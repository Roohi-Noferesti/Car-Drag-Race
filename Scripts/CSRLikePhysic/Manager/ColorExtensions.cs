using DataSerialization;
using System;
using UnityEngine;

public static class ColorExtensions
{
	public static UnityEngine.Color AsUnityColor(this DataSerialization.Color col)
	{
		return new UnityEngine.Color(col.r, col.g, col.b, col.a);
	}
}
