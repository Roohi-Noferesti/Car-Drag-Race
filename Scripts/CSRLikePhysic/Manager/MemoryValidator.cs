using System;
using System.Collections.Generic;
using UnityEngine;

public class MemoryValidator : MonoBehaviour
{
	private Dictionary<Type, IMangledDataItem> mangledDataItems = new Dictionary<Type, IMangledDataItem>();

	public static MemoryValidator Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		this.mangledDataItems.Add(typeof(MangledCashEarned), new MangledCashEarned());
		this.mangledDataItems.Add(typeof(MangledCashSpent), new MangledCashSpent());
		this.mangledDataItems.Add(typeof(MangledGoldEarned), new MangledGoldEarned());
        this.mangledDataItems.Add(typeof(MangledGoldSpent), new MangledGoldSpent());
        //this.mangledDataItems.Add(typeof(MangledGachaTokensEarned), new MangledGachaTokensEarned());
        //this.mangledDataItems.Add(typeof(MangledGachaTokensSpent), new MangledGachaTokensSpent());
        this.mangledDataItems.Add(typeof(MangledGachaBronzeKeysEarned), new MangledGachaBronzeKeysEarned());
        this.mangledDataItems.Add(typeof(MangledGachaBronzeKeysSpent), new MangledGachaBronzeKeysSpent());
        this.mangledDataItems.Add(typeof(MangledGachaSilverKeysEarned), new MangledGachaSilverKeysEarned());
        this.mangledDataItems.Add(typeof(MangledGachaSilverKeysSpent), new MangledGachaSilverKeysSpent());
        this.mangledDataItems.Add(typeof(MangledGachaGoldKeysEarned), new MangledGachaGoldKeysEarned());
        this.mangledDataItems.Add(typeof(MangledGachaGoldKeysSpent), new MangledGachaGoldKeysSpent());
        //this.mangledDataItems.Add(typeof(MangledIAPCashSpent), new MangledIAPCashSpent());
        //this.mangledDataItems.Add(typeof(MangledIAPGoldSpent), new MangledIAPGoldSpent());
        //this.mangledDataItems.Add(typeof(MangledIAPGachaBronzeKeysSpent), new MangledIAPGachaBronzeKeysSpent());
        //this.mangledDataItems.Add(typeof(MangledIAPGachaSilverKeysSpent), new MangledIAPGachaSilverKeysSpent());
        //this.mangledDataItems.Add(typeof(MangledIAPGachaGoldKeysSpent), new MangledIAPGachaGoldKeysSpent());
        //this.mangledDataItems.Add(typeof(MangledRP), new MangledRP());
        //this.mangledDataItems.Add(typeof(MangledRacesInCrewEarned), new MangledRacesInCrewEarned());
		MemoryValidator.Instance = this;
	}

	public void Mangle<T>(MangleInvoker invoker) where T : IMangledDataItem
	{
		IMangledDataItem mangledDataItem = this.mangledDataItems[typeof(T)];
		mangledDataItem.Mangle(invoker);
	}

	public void InitialiseMangle<T>(long initialValue, MangleInvoker invoker) where T : IMangledDataItem
	{
		IMangledDataItem mangledDataItem = this.mangledDataItems[typeof(T)];
		mangledDataItem.InitialiseMangle(initialValue, invoker);
	}

	private void Update()
	{
		if (PlayerProfileManager.Instance == null)
		{
			return;
		}
		if (PlayerProfileManager.Instance.ActiveProfile == null)
		{
			return;
		}
		foreach (Type current in this.mangledDataItems.Keys)
		{
			IMangledDataItem mangledDataItem = this.mangledDataItems[current];
			if (!mangledDataItem.Validate())
			{
				mangledDataItem.Reset();
			}
		}
	}

	public void TriggerMetricsEvent<T>() where T : IMangledDataItem
	{
		IMangledDataItem mangledDataItem = this.mangledDataItems[typeof(T)];
		mangledDataItem.TriggerMetricsEvent();
	}
}
