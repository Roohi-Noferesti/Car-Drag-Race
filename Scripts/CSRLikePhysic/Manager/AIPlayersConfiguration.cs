using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class AIPlayersConfiguration:ScriptableObject
{
	public List<AIDriverData> AIDrivers;

	public List<string> AINumberPlates;
}
