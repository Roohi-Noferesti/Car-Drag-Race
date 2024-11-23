using System;
using UnityEngine;

[Serializable]
public class LegalConfiguration:ScriptableObject
{
	public string TermsOfServiceURL = string.Empty;

	public string PrivacyPolicyURL = string.Empty;

	public int LatestVersion = -1;
}
