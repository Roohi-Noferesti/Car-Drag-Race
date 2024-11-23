using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GT/CameraSequencer/Shots/ShotSequence")]
public class ShotSequence : MonoBehaviour
{
	public List<BaseShot> shots;

	private int currentShot;

	public BaseShot ActiveShot
	{
		get
		{
			return this.shots[this.currentShot];
		}
	}

	public void Activate()
	{
		this.currentShot = 0;
		this.PlayShot(this.currentShot);
	}

	public bool OnUpdate(out CameraState zResult)
	{
		bool result = true;
		if (!this.shots[this.currentShot].OnUpdate(out zResult))
		{
			if (this.currentShot < this.shots.Count - 1)
			{
				this.PlayShot(this.currentShot + 1);
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public void PlayShot(int zShotIndex)
	{
		if (zShotIndex != 0)
		{
			SequenceManager.Instance.CallShotEnd();
		}
		this.shots[zShotIndex].Activate();
		this.currentShot = zShotIndex;
	}
}
