using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatList : MonoBehaviour
{
	public Image BottomLeftSprite;

	public Image BottomRightSprite;

	public Image BottomMidSprite;

	public Image TopLeftSprite;

	public Image TopRightSprite;

	public Image TopMidSprite;

	public Image MidLeftSprite;

	public Image MidRightSprite;

    public Image MidSprite;

	public TextMeshProUGUI TitleText;

	public float MidTilePixelSize;

	public float EntryHeight;

	public bool UseDivider;

	public GameObject ListEntryTextPrefab;

	private StatListItemListText[] textEntries;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void SetTitle(string title)
	{
		this.TitleText.text = title;
	}

	public void SetEntryText(int entry, string text)
	{
		this.SetEntryText(entry, text, string.Empty);
	}

	public void SetEntryText(int entry, string left, string right)
	{
        this.textEntries[entry].LeftText.text = left;
        this.textEntries[entry].RightText.text = right;
	}

	public void SetEntryTitle(string title)
	{
		this.TitleText.text = title;
	}

	public void SetEntryCount(int count)
	{
		if (this.textEntries != null)
		{
			for (int i = 0; i < this.textEntries.Length; i++)
			{
				UnityEngine.Object.Destroy(this.textEntries[i].gameObject);
			}
		}
		this.textEntries = new StatListItemListText[count];
		float num = this.EntryHeight * (float)count;
		float y = this.MidTilePixelSize * (float)count;
        //this.MidLeftSprite.Setup(this.MidLeftSprite.width, num);
        //this.MidRightSprite.Setup(this.MidRightSprite.width, num);
        //this.MidSprite.Setup(this.MidSprite.width, num, Vector2.zero, new Vector2(this.MidSprite.PixelSize.x, y));
		Vector3 b = new Vector3(0f, -num);
		this.BottomLeftSprite.transform.localPosition = this.MidLeftSprite.transform.localPosition + b;
		this.BottomMidSprite.transform.localPosition = this.MidSprite.transform.localPosition + b;
		this.BottomRightSprite.transform.localPosition = this.MidRightSprite.transform.localPosition + b;
		for (int j = 0; j < count; j++)
		{
			StatListItemListText statListItemListText = this.InstantiateEntryText();
			if (j < count - 1)
			{
				statListItemListText.UseDivider = this.UseDivider;
			}
			else
			{
				statListItemListText.UseDivider = false;
			}
			statListItemListText.transform.parent = this.MidSprite.transform.parent;
			statListItemListText.transform.localPosition = new Vector3(0f, -this.EntryHeight * (float)j);
			this.textEntries[j] = statListItemListText;
		}
	}

	private StatListItemListText InstantiateEntryText()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.ListEntryTextPrefab) as GameObject;
		return gameObject.GetComponent<StatListItemListText>();
	}
}
