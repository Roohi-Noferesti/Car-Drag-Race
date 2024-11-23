using System;
using UnityEngine;

[Serializable]
public class DifficultyConfiguration:ScriptableObject
{
	public DifficultySettings Settings = new DifficultySettings();

	public DifficultySettings EliteSettings = new DifficultySettings();
}
