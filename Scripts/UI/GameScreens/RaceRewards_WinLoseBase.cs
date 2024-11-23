using System;
using UnityEngine;

public abstract class RaceRewards_WinLoseBase : MonoBehaviour
{
	public abstract void Setup(RaceResultsTrackerState resultsData);
	public abstract void Hide(bool hide);

    public Action OnAnimationEnd;

    public void AnimationEnd()
    {
        if (OnAnimationEnd != null)
        {
            OnAnimationEnd.Invoke();
        }
    }
}
