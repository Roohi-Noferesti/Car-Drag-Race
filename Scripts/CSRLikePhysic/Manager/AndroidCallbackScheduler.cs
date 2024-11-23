using System;
using System.Collections.Generic;
using UnityEngine;

public class AndroidCallbackScheduler : MonoBehaviour
{
	private Queue<Action> taskQueue = new Queue<Action>();

	public static AndroidCallbackScheduler Create()
	{
		GameObject gameObject = new GameObject("AndroidCallbackScheduler");
		AndroidCallbackScheduler result = gameObject.AddComponent<AndroidCallbackScheduler>();
		DontDestroyOnLoad(gameObject);
		return result;
	}

	public void Update()
	{
		Queue<Action> obj = this.taskQueue;
		lock (obj)
		{
			while (this.taskQueue.Count != 0)
			{
				this.taskQueue.Dequeue()();
			}
		}
	}

	public void QueueTask(Action task)
	{
		Queue<Action> obj = this.taskQueue;
		lock (obj)
		{
			this.taskQueue.Enqueue(task);
		}
	}
}
