using System;
using UnityEngine;

[Serializable]
public class IBundleOfferWidgetInfo
{
	public string OfferType = string.Empty;

	public string CarDBKey = string.Empty;

	//public string ShopItem = string.Empty;
	
	public string[] _ShopItem;

	public string ShopItem
	{
		get
		{
			if(_ShopItem!=null && ProductManager.Instance.discount < _ShopItem.Length)
				return _ShopItem[ProductManager.Instance.discount];
			else
				return String.Empty;
		}
		set { _ShopItem[ProductManager.Instance.discount] = value; }
	}

	public Vector3 Position = Vector3.zero;

	public Vector3 SpritePosition = Vector3.zero;

	public Vector3 DescPosition = Vector3.zero;
}
