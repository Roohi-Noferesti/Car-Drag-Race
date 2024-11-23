using System;
using System.Collections.Generic;
using UnityEngine;

public static class GameObjectHelper
{
	public static T InstantiatePrefab<T>(GameObject prefab, GameObject parent) where T : Component
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(prefab, Vector3.zero, Quaternion.identity) as GameObject;
	    gameObject.transform.SetParent(parent.transform, false);
		gameObject.transform.localPosition = Vector3.zero;
		gameObject.transform.localScale = Vector3.one;
		return gameObject.GetComponent<T>();
	}

	public static string GetFullName(GameObject go)
	{
		if (go.transform.parent != null)
		{
			return GameObjectHelper.GetFullName(go.transform.parent.gameObject) + "/" + go.name;
		}
		return go.name;
	}

	public static List<GameObject> GetAllRootGameObjects()
	{
		GameObject[] array = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[];
		List<GameObject> list = new List<GameObject>();
		GameObject[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			GameObject gameObject = array2[i];
			if (gameObject.transform.parent == null)
			{
				list.Add(gameObject);
			}
		}
		return list;
	}

	public static float SquareTransition(float zRatio)
	{
		float num = 1f - zRatio;
		return num * num;
	}

	public static float To2DP(float zVal)
	{
		return (float)Math.Round((double)zVal, 2);
	}

	public static float To2DPMultipleOf2(float zVal)
	{
		zVal = GameObjectHelper.To2DP(zVal);
		return (float)Math.Round((double)(zVal / 0.02f)) * 0.02f;
	}

	public static Vector3 To2DP(Vector3 zVal)
	{
		return new Vector3((float)Math.Round((double)zVal.x, 2), (float)Math.Round((double)zVal.y, 2), (float)Math.Round((double)zVal.z, 2));
	}

	public static void SetLocalX(GameObject zFrom, float zNewX)
	{
		zFrom.transform.localPosition = new Vector3(zNewX, zFrom.transform.localPosition.y, zFrom.transform.localPosition.z);
	}

	public static void SetLocalY(GameObject zFrom, float zNewY)
	{
		zFrom.transform.localPosition = new Vector3(zFrom.transform.localPosition.x, zNewY, zFrom.transform.localPosition.z);
	}

	public static void SetLocalZ(GameObject zFrom, float zNewZ)
	{
		zFrom.transform.localPosition = new Vector3(zFrom.transform.localPosition.x, zFrom.transform.localPosition.y, zNewZ);
	}

	public static void SetLocalXY(GameObject zFrom, float zNewX, float zNewY)
	{
		zFrom.transform.localPosition = new Vector3(zNewX, zNewY, zFrom.transform.localPosition.z);
	}

	public static void MakeLocalPositionPixelPerfect(GameObject zGo)
	{
		GameObjectHelper.MakeLocalPositionPixelPerfect(zGo.transform);
	}

	public static void MakeLocalPositionPixelPerfect(Transform zTrans)
	{
		zTrans.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zTrans.localPosition);
	}

	public static Vector3 MakeLocalPositionPixelPerfect(Vector3 zPosition)
	{
		float num = zPosition.x;
		float num2 = zPosition.y;
		float z = zPosition.z;
		num = (float)Math.Round((double)num, 2);
		num2 = (float)Math.Round((double)num2, 2);
		return new Vector3(num, num2, z);
	}

	public static float MakeLocalPositionPixelPerfect(float zScaler)
	{
		return (float)Math.Round((double)zScaler, 2);
	}

	public static Vector2 MakeLocalPositionPixelPerfect(Vector2 zPleaseGodNo)
	{
		Debug.Break();
		Vector2 result = new Vector2((float)Math.Round((double)zPleaseGodNo.x, 2), (float)Math.Round((double)zPleaseGodNo.y, 2));
		return result;
	}
}
