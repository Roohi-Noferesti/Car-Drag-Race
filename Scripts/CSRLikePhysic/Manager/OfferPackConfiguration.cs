using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OfferPackConfiguration:ScriptableObject
{
	public List<OfferPackData> Offers = new List<OfferPackData>();
}
