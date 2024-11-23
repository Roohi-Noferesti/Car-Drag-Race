using System;
using System.Collections.Generic;
using System.Linq;

public class StarStats
{
	public Dictionary<StarType, int> NumStars = new Dictionary<StarType, int>();

	public int TotalStars
	{
		get
		{
			return this.NumStars.Values.Sum();
		}
	}

	public StarStats()
	{
		foreach (object current in Enum.GetValues(typeof(StarType)))
		{
			if ((int)current != 0 && (int)current != 4)
			{
				this.NumStars.Add((StarType)((int)current), 0);
			}
		}
	}
}
