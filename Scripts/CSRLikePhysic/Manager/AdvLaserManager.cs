using System;
using System.Collections.Generic;
using UnityEngine;

public class AdvLaserManager : MonoBehaviour
{
	public List<AdvancedLaser> _LaserList = new List<AdvancedLaser>();

	private void Awake()
	{
	}

	private void Update()
	{
		Vector3 forward = Camera.main.transform.forward;
		float alpha = -Mathf.Min(Vector3.Dot(Vector3.forward.normalized, forward.normalized), 0f);
		AdvancedLaser.SetAlpha(alpha);
		if (Mathf.Sin(Time.timeSinceLevelLoad * 1.5f) > 0.8f)
		{
			AdvancedLaser.FlickerEnabled = true;
			foreach (AdvancedLaser current in this._LaserList)
			{
                //current.animation["Take 001"].speed = Mathf.Lerp(current.animation["Take 001"].speed, 0.2f, Time.deltaTime * 6f);
			}
		}
		else
		{
			AdvancedLaser.FlickerEnabled = false;
			foreach (AdvancedLaser current2 in this._LaserList)
			{
                //current2.animation["Take 001"].speed = Mathf.Lerp(current2.animation["Take 001"].speed, 0.6f, Time.deltaTime);
			}
		}
	}

	public void DetectLasers()
	{
		Component[] componentsInChildren = base.transform.GetComponentsInChildren<AdvancedLaser>(true);
		this._LaserList = new List<AdvancedLaser>();
		Component[] array = componentsInChildren;
		for (int i = 0; i < array.Length; i++)
		{
			AdvancedLaser advancedLaser = (AdvancedLaser)array[i];
			advancedLaser.LockPosition();
			advancedLaser.GetComponent<Animation>()["Take 001"].time = UnityEngine.Random.Range(0f, advancedLaser.GetComponent<Animation>()["Take 001"].length);
			this._LaserList.Add(advancedLaser);
		}
	}

	private void LateUpdate()
	{
		if (this._LaserList.Count == 0)
		{
			return;
		}
		if (AdvancedLaser.ColourChanged)
		{
			foreach (AdvancedLaser current in this._LaserList)
			{
				current.UpdateColours();
			}
			AdvancedLaser.ColourChanged = false;
		}
	}
}
