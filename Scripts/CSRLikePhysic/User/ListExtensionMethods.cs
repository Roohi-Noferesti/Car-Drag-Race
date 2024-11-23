using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public static class ListExtensionMethods
{
	private class DummyItem
	{
		public int v;

		public DummyItem(int val)
		{
			this.v = val;
		}
	}

	public static void Shuffle<T>(this List<T> list)
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		int count = list.Count;
		if (count <= 1)
		{
			return;
		}
		System.Random random = new System.Random();
		for (int i = count - 1; i > 0; i--)
		{
			int index = random.Next(i + 1);
			T value = list[index];
			list[index] = list[i];
			list[i] = value;
		}
	}

	public static T MaxItem<T, U>(this List<T> list, Func<T, U> selector) where U : IComparable<U>
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (list.Count == 0)
		{
			throw new InvalidOperationException("list is empty.");
		}
		T t = list[0];
		U other = selector(t);
		for (int i = 1; i < list.Count; i++)
		{
			T t2 = list[i];
			U u = selector(t2);
			if (u.CompareTo(other) > 0)
			{
				other = u;
				t = t2;
			}
		}
		return t;
	}

	public static U MaxValue<T, U>(this List<T> aList, Func<T, U> selector) where U : IComparable<U>
	{
		return selector(aList.MaxItem(selector));
	}

	public static T MinItem<T, U>(this List<T> list, Func<T, U> selector) where U : IComparable<U>
	{
		if (list == null)
		{
			throw new ArgumentNullException("list");
		}
		if (list.Count == 0)
		{
			throw new InvalidOperationException("list is empty.");
		}
		T t = list[0];
		U other = selector(t);
		for (int i = 1; i < list.Count; i++)
		{
			T t2 = list[i];
			U u = selector(t2);
			if (u.CompareTo(other) < 0)
			{
				other = u;
				t = t2;
			}
		}
		return t;
	}

	public static U MinValue<T, U>(this List<T> aList, Func<T, U> selector) where U : IComparable<U>
	{
		return selector(aList.MinItem(selector));
	}

	public static void ForEachWithIndex<T>(this IEnumerable<T> enumerable, Action<T, int> action)
	{
		int num = 0;
		foreach (T current in enumerable)
		{
			action(current, num++);
		}
	}

	[Conditional("UNITY_EDITOR")]
	public static void TestListExtensionMethodsPerformance()
	{
		UnityEngine.Debug.Log("List extension methods performance test!");
	}

	[Conditional("UNITY_EDITOR")]
	private static void TestMaxMethodsPerformance()
	{
		List<ListExtensionMethods.DummyItem> list = new List<ListExtensionMethods.DummyItem>();
		for (int i = 0; i < 1000000; i++)
		{
			list.Add(new ListExtensionMethods.DummyItem(i));
		}
		long ticks = DateTime.Now.Ticks;
		List<ListExtensionMethods.DummyItem> list2 = new List<ListExtensionMethods.DummyItem>(list);
		list2.Sort((ListExtensionMethods.DummyItem x, ListExtensionMethods.DummyItem y) => -1 * x.v.CompareTo(y.v));
		long ticks2 = DateTime.Now.Ticks;
		TimeSpan timeSpan = new TimeSpan(ticks2 - ticks);
		UnityEngine.Debug.Log("Copy + Sort: " + timeSpan.Milliseconds + " ms");
		ticks = DateTime.Now.Ticks;
		list.Sort((ListExtensionMethods.DummyItem x, ListExtensionMethods.DummyItem y) => -1 * x.v.CompareTo(y.v));
		ticks2 = DateTime.Now.Ticks;
		timeSpan = new TimeSpan(ticks2 - ticks);
		UnityEngine.Debug.Log("Sort: " + timeSpan.Milliseconds + " ms");
		ticks = DateTime.Now.Ticks;
		list.MaxItem((ListExtensionMethods.DummyItem q) => q.v);
		ticks2 = DateTime.Now.Ticks;
		timeSpan = new TimeSpan(ticks2 - ticks);
		UnityEngine.Debug.Log("Extension method MaxItem(): " + timeSpan.Milliseconds + " ms");
		ticks = DateTime.Now.Ticks;
		list.MaxValue((ListExtensionMethods.DummyItem q) => q.v);
		ticks2 = DateTime.Now.Ticks;
		timeSpan = new TimeSpan(ticks2 - ticks);
		UnityEngine.Debug.Log("Extension method MaxValue(): " + timeSpan.Milliseconds + " ms");
	}
}
