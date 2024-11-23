using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PrefabPlaceholder : MonoBehaviour
{
	public GameObject PrefabToCreate;

	public GameObject PrefabToCreateAndroidOverride;

	public string PathOfPrefabToCreate;

	public string PathOfPrefabToCreateAndroidOverride;

	public bool DelayCreationUntilStart;

	public bool DoNothingForProductionBuilds;

	public bool DoNothingForDebugMenuBuilds;

	public float ForceScale;

	public bool ForceTransform;

	public Vector3 ForceTransformVector = new Vector3(0f, 0f, 0f);

	private bool _createdFlag;

	private bool _awake;

	private bool _started;

	private GameObject _instantiatedGameObject;

	private Dictionary<Type, MonoBehaviour> _cachedItems = new Dictionary<Type, MonoBehaviour>();

	public GameObject GetGameObject()
	{
		if (this.DoNothingForProductionBuilds)
		{
			return null;
		}
		if (this.DoNothingForDebugMenuBuilds)
		{
			return null;
		}
		return this._instantiatedGameObject;
	}

	public T GetBehaviourOnPrefab<T>() where T : MonoBehaviour
	{
		if (this.DoNothingForProductionBuilds)
		{
			return (T)((object)null);
		}
		if (this.DoNothingForDebugMenuBuilds)
		{
			return (T)((object)null);
		}
		if (this.DelayCreationUntilStart && !this._started)
		{
			return (T)((object)null);
		}
		this.Create();
		if (!this._instantiatedGameObject)
		{
			if (string.IsNullOrEmpty(this.PathOfPrefabToCreate))
			{
				if (this.PrefabToCreate != null)
				{
				}
			}
			return (T)((object)null);
		}
		MonoBehaviour monoBehaviour = null;
		if (!this._cachedItems.ContainsKey(typeof(T)))
		{
			monoBehaviour = this._instantiatedGameObject.GetComponent<T>();
			if (!monoBehaviour)
			{
				monoBehaviour = this._instantiatedGameObject.GetComponentsInChildren<T>(true)[0];
			}
			this._cachedItems.Add(typeof(T), monoBehaviour);
		}
		else
		{
			this._cachedItems.TryGetValue(typeof(T), out monoBehaviour);
		}
		if (!monoBehaviour)
		{
			return (T)((object)null);
		}
		return monoBehaviour as T;
	}

	private void Awake()
	{
		if (this.DoNothingForProductionBuilds)
		{
			return;
		}
		if (this.DoNothingForDebugMenuBuilds)
		{
			return;
		}
		if (this._awake)
		{
			return;
		}
		this._awake = true;
		if (!this.DelayCreationUntilStart)
		{
			this.Create();
		}
	}

	private void Start()
	{
		if (this.DoNothingForProductionBuilds)
		{
			return;
		}
		if (this.DoNothingForDebugMenuBuilds)
		{
			return;
		}
		if (this._started)
		{
			return;
		}
		this._started = true;
		this.Create();
	}

	private void Create()
	{
		if (this.DoNothingForProductionBuilds)
		{
			return;
		}
		if (this.DoNothingForDebugMenuBuilds)
		{
			return;
		}
		if (this._createdFlag)
		{
			return;
		}
		this._createdFlag = true;
		if (!string.IsNullOrEmpty(this.PathOfPrefabToCreateAndroidOverride))
		{
			Object original = Resources.Load(this.PathOfPrefabToCreateAndroidOverride);
			this._instantiatedGameObject = (Instantiate(original, Vector3.zero, Quaternion.identity) as GameObject);
		}
		else if (!string.IsNullOrEmpty(this.PathOfPrefabToCreate))
		{
			Object original2 = Resources.Load(this.PathOfPrefabToCreate);
			this._instantiatedGameObject = (Instantiate(original2, Vector3.zero, Quaternion.identity) as GameObject);
		}
		else if (this.PrefabToCreateAndroidOverride != null)
		{
			this._instantiatedGameObject = (Instantiate(this.PrefabToCreateAndroidOverride, Vector3.zero, Quaternion.identity) as GameObject);
		}
		else
		{
			if (!(this.PrefabToCreate != null))
			{
				return;
			}
			this._instantiatedGameObject = (Instantiate(this.PrefabToCreate, Vector3.zero, Quaternion.identity) as GameObject);
		}
		this._instantiatedGameObject.transform.parent = base.gameObject.transform;
        //if (this._instantiatedGameObject.GetComponent<EZScreenPlacement>() == null)
        //{
        //    this._instantiatedGameObject.transform.localPosition = Vector3.zero;
        //}
		if (this.ForceScale > 0f)
		{
			this._instantiatedGameObject.transform.localScale = new Vector3(this.ForceScale, this.ForceScale, this.ForceScale);
		}
		if (this.ForceTransform)
		{
			this._instantiatedGameObject.transform.localPosition = this.ForceTransformVector;
		}
	}
}
