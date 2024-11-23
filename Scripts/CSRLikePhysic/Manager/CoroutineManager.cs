using System;
using System.Collections;
using UnityEngine;

public class CoroutineManager : MonoBehaviour
{
	public delegate IEnumerator CoroutineMethod();

	public static CoroutineManager Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	private IEnumerator RunCoroutine(CoroutineMethod coroutineMethod)
	{
		return coroutineMethod();
	}

	public void StartCoroutineDelegate(CoroutineMethod coroutineMethod)
	{
		base.StartCoroutine("RunCoroutine", coroutineMethod);
	}

    public void WaitForFrameThenExecute(int frameCount, Action action)
    {
        StartCoroutine(_waitForFrameThenExecute(frameCount, action));
    }

    private IEnumerator _waitForFrameThenExecute(int frameCount, Action action)
    {
        yield return new WaitForFrames(frameCount);
        action();
    }

    public void WaitForSecondsThenExecute(int seconds, Action action)
    {
        StartCoroutine(_waitForSecondsThenExecute(seconds, action));
    }

    private IEnumerator _waitForSecondsThenExecute(int seconds, Action action)
    {
        yield return new WaitForSeconds(seconds);
        action();
    }
}