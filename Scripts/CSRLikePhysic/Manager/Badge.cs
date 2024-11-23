using DataSerialization;
using System;
using UnityEngine;

[System.Serializable]
public class Badge
{
	public const string TexturePack = "BadgesTexturePack";

	public string ID = string.Empty;

	public string TextureName = string.Empty;

	public UnityEngine.Vector3 AvatarPortraitOffset = UnityEngine.Vector3.zero;

	public EligibilityRequirements Requirements = EligibilityRequirements.CreateNeverEligible();
}
