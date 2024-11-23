public struct ItemTypeId
{
	public string Clss;

	public string Itm;

	public ItemTypeId(string c)
	{
		this.Clss = c;
		this.Itm = string.Empty;
	}

	public ItemTypeId(string c, string i)
	{
		this.Clss = c;
		this.Itm = i;
	}
}
