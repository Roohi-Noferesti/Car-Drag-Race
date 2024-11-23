using System;
using System.Collections.Generic;
using UnityEngine;

public class StatListItem : ListItem
{
	public Transform[] StatNodes;

	public GameObject StatBoxPrefab;

	public void Create(List<StatBox.StatType> statsToShow)
	{
		for (int i = 0; i < this.StatNodes.Length; i++)
		{
			if (statsToShow.Count == 0)
			{
				return;
			}
			Transform parent = this.StatNodes[i];
			GameObject gameObject = UnityEngine.Object.Instantiate(this.StatBoxPrefab) as GameObject;
			gameObject.transform.parent = parent;
			gameObject.transform.localPosition = Vector3.zero;
			StatBox component = gameObject.GetComponent<StatBox>();
			StatBox.StatType stat = statsToShow[0];
			statsToShow.RemoveAt(0);
			component.SetStat(stat);
		}
	}
}
