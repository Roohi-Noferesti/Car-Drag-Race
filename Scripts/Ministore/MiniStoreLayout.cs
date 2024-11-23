using System;
using System.Collections.Generic;

[Serializable]
public class MiniStoreLayout
{
	public List<string> products;

	public List<string> stickers;

	public List<bool> glowing;

	public MiniStoreLayout GenerateCopy()
	{
		return new MiniStoreLayout
		{
			products = new List<string>(this.products),
			stickers = new List<string>(this.stickers),
			glowing = new List<bool>(this.glowing)
		};
	}
}
