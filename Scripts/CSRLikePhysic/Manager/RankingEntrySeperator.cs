using System;
using UnityEngine;

public class RankingEntrySeperator : ListItem
{
	public Transform seperatorParent;

	public void SetWidth(float width)
	{
		this.seperatorParent.localPosition = new Vector3(width * 0.5f, this.seperatorParent.localPosition.y, this.seperatorParent.localPosition.z);
	}
}
