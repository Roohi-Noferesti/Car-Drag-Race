using System;

public class StatListItemExtra : StatListItem
{
	public StatList list;

	private int entries;

	protected override void Start()
	{
		this.entries = -1;
		base.Start();
	}

	public void SetListTitle(string title)
	{
		this.list.SetTitle(title);
	}

	public void SetListEntry(int index, string text)
	{
		this.list.SetEntryText(index, text);
	}

	public void SetListEntry(int index, string left, string right)
	{
		this.list.SetEntryText(index, left, right);
	}

	public void SetEntryCount(int entries)
	{
		this.list.SetEntryCount(entries);
		this.entries = entries;
	}

	public int GetEntryCount()
	{
		return this.entries;
	}
}
