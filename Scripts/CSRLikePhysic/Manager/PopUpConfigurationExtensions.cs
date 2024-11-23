using DataSerialization;
using System;

public static class PopUpConfigurationExtensions
{
	public static void Initialise(this PopUpConfiguration puc)
	{
		foreach (PopupData current in puc.PopUpLookup.Values)
		{
			current.Initialise();
		}
	}
}
