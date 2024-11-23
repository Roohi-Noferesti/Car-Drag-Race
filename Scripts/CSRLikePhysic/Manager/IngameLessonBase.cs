using UnityEngine;

public abstract class IngameLessonBase : MonoBehaviour
{
	public string stateName;

	public abstract void StateOnEnter();

	public abstract bool StateUpdate();

	public abstract void StateOnExit();

	protected float GetTotalRaceDistance()
	{
		if (RaceEventInfo.Instance.CurrentEvent == null)
		{
			return 500;
		}
		else
		{
			return (!RaceEventInfo.Instance.CurrentEvent.IsHalfMile) ? 402.325f : 804.65f;
		}
	}
}
