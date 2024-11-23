using System;
using TMPro;
using UnityEngine;

public class StatListItemListText : MonoBehaviour
{
	public TextMeshProUGUI LeftText;

    public TextMeshProUGUI RightText;

	public bool UseDivider;

	public GameObject Divider;

	private void Start()
	{
		if (this.UseDivider)
		{
			this.Divider.SetActive(true);
            //Vector3 localPosition = this.LeftText.transform.localPosition;
            //localPosition.y -= this.LeftText.characterSize + 0.01f;
            //this.Divider.transform.localPosition = localPosition;
		}
	}

	private void Update()
	{
	}
}
