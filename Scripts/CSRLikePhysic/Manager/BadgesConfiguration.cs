using System;
using System.Collections.Generic;
using UnityEngine;

public class BadgesConfiguration:ScriptableObject
{
	public List<Badge> Badges = new List<Badge>();

	public void Initialise()
	{
		this.Badges.ForEach(delegate(Badge b)
		{
			b.Requirements.Initialise();
		});
	}
}
