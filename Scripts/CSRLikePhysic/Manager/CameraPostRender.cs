using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraPostRender : MonoBehaviour
{
	private static CameraPostRender _instance;

	private Dictionary<string, Action> _actions = new Dictionary<string, Action>();

	public static CameraPostRender Instance
	{
		get
		{
			Camera camera = FindBestCamera();
			CameraPostRender component = camera.GetComponent<CameraPostRender>();
			if (component != _instance || component == null)
			{
				Dictionary<string, Action> dictionary = new Dictionary<string, Action>();
				if (_instance != null)
				{
					foreach (KeyValuePair<string, Action> current in _instance._actions)
					{
						dictionary.Add(current.Key, current.Value);
					}
					DestroyImmediate(_instance);
					_instance = null;
				}
				_instance = camera.gameObject.AddComponent<CameraPostRender>();
				foreach (KeyValuePair<string, Action> current2 in dictionary)
				{
					_instance._actions.Add(current2.Key, current2.Value);
				}
			}
			return _instance;
		}
	}

	private static Camera FindBestCamera()
	{
		List<Camera> list = new List<Camera>
		{
			Camera.main
		};
        //if (GUICamera.Instance != null)
        //{
        //    list.Add(GUICamera.Instance.gameObject.camera);
        //}
		list.RemoveAll((Camera q) => q == null);
        //(from q in list
        //orderby q.depth
        //select q);
		list.Reverse();
		return (list.Count <= 0) ? Camera.main : list[0];
	}

	public void AddProcess(string uniqueId, Action action)
	{
		if (this._actions.ContainsKey(uniqueId))
		{
			return;
		}
		this._actions.Add(uniqueId, action);
		base.enabled = true;
	}

	private void OnPostRender()
	{
		Dictionary<string, Action> actions = this._actions;
		this._actions = new Dictionary<string, Action>();
		foreach (KeyValuePair<string, Action> current in actions)
		{
			current.Value();
		}
		if (this._actions.Count == 0)
		{
			base.enabled = false;
		}
	}
}
