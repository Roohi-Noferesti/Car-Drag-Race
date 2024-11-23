using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SeasonEventsMetadata:ScriptableObject
{
	public List<SeasonEventMetadata> Events = new List<SeasonEventMetadata>();
}
