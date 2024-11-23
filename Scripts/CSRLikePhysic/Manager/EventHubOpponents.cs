using DataSerialization;
using System;
using System.Collections.Generic;
using UnityEngine;

public class EventHubOpponents : MonoBehaviour
{
	private List<EventHubOpponent> opponents = new List<EventHubOpponent>();

	public float YOffset;

	public float XOffset;

	public void CreateOpponents(ThemeOptionLayoutDetails details)
	{
		GameObject original = Resources.Load("Career/CarParent") as GameObject;
		IGameState gameState = new GameStateFacade();
		details.ProgressionSnapshots.Initialise();
		List<CarOverride> snapshotList = details.ProgressionSnapshots.GetSnapshotList(gameState);
		for (int i = 0; i < snapshotList.Count; i++)
		{
			GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(original);
			gameObject.transform.parent = base.gameObject.transform;
			float y = (i % 2 != 0) ? this.YOffset : (-this.YOffset);
			float x = -((float)(snapshotList.Count / 2) * this.XOffset) + (float)i * this.XOffset;
			gameObject.transform.localPosition = new UnityEngine.Vector3(x, y, 0f);
			EventHubOpponent component = gameObject.GetComponent<EventHubOpponent>();
			if (component != null)
			{
				component.SetupFromCarOverride(snapshotList[i]);
				this.opponents.Add(component);
			}
		}
	}

	public void InitOpponentsAnimation()
	{
		foreach (EventHubOpponent current in this.opponents)
		{
            //AnimationUtils.PlayFirstFrame(current.animation);
		}
	}

	public void PlayOpponentAnimation(int i)
	{
        //AnimationUtils.PlayAnim(this.opponents[i].animation);
	}

	public void UpdateFromEvent(RaceEventData race)
	{
		if (race == null)
		{
			foreach (EventHubOpponent current in this.opponents)
			{
				current.ShowAsUsed(false);
			}
		}
		else if (race.GetWorldTourPinPinDetail().WorldTourScheduledPinInfo.ChoiceScreen != null)
		{
			bool flag = false;
			foreach (EventHubOpponent current2 in this.opponents)
			{
				if (!flag && !current2.isSet)
				{
					flag = true;
					current2.ShowAsUsed(true);
				}
				else
				{
					current2.ShowAsUsed(false);
				}
			}
		}
		else if (!race.IsRelay)
		{
			foreach (EventHubOpponent current3 in this.opponents)
			{
				current3.ShowAsUsed(current3.UsesCar(race.AICar));
			}
		}
		else
		{
			foreach (EventHubOpponent o in this.opponents)
			{
				o.ShowAsUsed(race.Group.RaceEvents.Find((RaceEventData r) => o.UsesCar(r.AICar)) != null);
			}
		}
	}
}
