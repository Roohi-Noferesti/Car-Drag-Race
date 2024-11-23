using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RevenueTrackingConfiguration:ScriptableObject
{
    public ProductPrice[] Prices;

	public float PricePoint2Revenue = 0.7f;

	public bool Enabled = true;

	public bool DebugLogOutEnabled = true;

	public bool ApsalarSendEventEnabled = true;
}