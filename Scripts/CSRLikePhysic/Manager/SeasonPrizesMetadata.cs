using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SeasonPrizesMetadata:ScriptableObject
{
	public List<SeasonPrizeMetadata> Prizes = new List<SeasonPrizeMetadata>();
}
