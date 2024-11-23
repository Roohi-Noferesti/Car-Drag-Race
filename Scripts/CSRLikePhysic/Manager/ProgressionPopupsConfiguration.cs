using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ProgressionPopupsConfiguration:ScriptableObject
{
	public List<PopupStatusData> PopupsData = new List<PopupStatusData>();
}
