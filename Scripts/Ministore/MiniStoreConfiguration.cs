using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MiniStoreConfiguration:ScriptableObject
{
	public Dictionary<string, MiniStoreLayout> CheapestGoldWithGoldOnly;

	public Dictionary<string, MiniStoreLayout> CheapestCashWithCashOnly;

	public Dictionary<string, MiniStoreLayout> CheapestCashWithNearestGold;

	public Dictionary<string, MiniStoreLayout> AffordableGoldWithNearestGold;

	public Dictionary<string, MiniStoreLayout> AffordableGoldWithNearestCashAndGold;



	public MiniStoreLayoutKeyValue[] CheapestGoldWithGoldOnlyData;
	public MiniStoreLayoutKeyValue[] CheapestCashWithCashOnlyData;
	public MiniStoreLayoutKeyValue[] CheapestCashWithNearestGoldData;
	public MiniStoreLayoutKeyValue[] AffordableGoldWithNearestGoldData;
	public MiniStoreLayoutKeyValue[] AffordableGoldWithNearestCashAndGoldData;

	public void Initialize()
	{
		CheapestGoldWithGoldOnly = new Dictionary<string, MiniStoreLayout>();
		foreach (var keyValue in CheapestGoldWithGoldOnlyData)
		{
			CheapestGoldWithGoldOnly.Add(keyValue.Name, keyValue.Layout);
		}
		
		CheapestCashWithCashOnly = new Dictionary<string, MiniStoreLayout>();
		foreach (var keyValue in CheapestCashWithCashOnlyData)
		{
			CheapestCashWithCashOnly.Add(keyValue.Name, keyValue.Layout);
		}
		
		CheapestCashWithNearestGold = new Dictionary<string, MiniStoreLayout>();
		foreach (var keyValue in CheapestCashWithNearestGoldData)
		{
			CheapestCashWithNearestGold.Add(keyValue.Name, keyValue.Layout);
		}
		
		AffordableGoldWithNearestGold = new Dictionary<string, MiniStoreLayout>();
		foreach (var keyValue in AffordableGoldWithNearestGoldData)
		{
			AffordableGoldWithNearestGold.Add(keyValue.Name, keyValue.Layout);
		}
		
		AffordableGoldWithNearestCashAndGold = new Dictionary<string, MiniStoreLayout>();
		foreach (var keyValue in AffordableGoldWithNearestCashAndGoldData)
		{
			AffordableGoldWithNearestCashAndGold.Add(keyValue.Name, keyValue.Layout);
		}
	}

	public void Setup()
	{
        CheapestGoldWithGoldOnlyData = new MiniStoreLayoutKeyValue[CheapestGoldWithGoldOnly.Count];
		int i = 0;
		foreach (var keyvalue in CheapestGoldWithGoldOnly)
		{
            CheapestGoldWithGoldOnlyData[i] = new MiniStoreLayoutKeyValue()
				{Name = keyvalue.Key, Layout = keyvalue.Value};
			i++;
		}

        CheapestCashWithCashOnlyData = new MiniStoreLayoutKeyValue[CheapestCashWithCashOnly.Count];
		i = 0;
		foreach (var keyvalue in CheapestCashWithCashOnly)
		{
            CheapestCashWithCashOnlyData[i] = new MiniStoreLayoutKeyValue()
				{Name = keyvalue.Key, Layout = keyvalue.Value};
			i++;
		}

        CheapestCashWithNearestGoldData = new MiniStoreLayoutKeyValue[CheapestCashWithNearestGold.Count];
		i = 0;
		foreach (var keyvalue in CheapestCashWithNearestGold)
		{
            CheapestCashWithNearestGoldData[i] = new MiniStoreLayoutKeyValue()
				{Name = keyvalue.Key, Layout = keyvalue.Value};
			i++;
		}
		
		AffordableGoldWithNearestGoldData = new MiniStoreLayoutKeyValue[AffordableGoldWithNearestGold.Count];
		i = 0;
		foreach (var keyvalue in AffordableGoldWithNearestGold)
		{
            AffordableGoldWithNearestGoldData[i] = new MiniStoreLayoutKeyValue()
				{Name = keyvalue.Key, Layout = keyvalue.Value};
			i++;
		}
		
		AffordableGoldWithNearestCashAndGoldData = new MiniStoreLayoutKeyValue[AffordableGoldWithNearestCashAndGold.Count];
		i = 0;
		foreach (var keyvalue in AffordableGoldWithNearestCashAndGold)
		{
            AffordableGoldWithNearestCashAndGoldData[i] = new MiniStoreLayoutKeyValue()
				{Name = keyvalue.Key, Layout = keyvalue.Value};
			i++;
		}
	}
}

[Serializable]
public class MiniStoreLayoutKeyValue
{
	public string Name;

	public MiniStoreLayout Layout;

    public MiniStoreLayoutKeyValue() { }
}
