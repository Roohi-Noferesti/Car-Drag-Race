using System.Linq;
using UnityEngine;

public class TierColourDatabase : ConfigurationAssetLoader
{
	private ColoursConfiguration ColourConfiguration;

	public TierColourDatabase() : base(GTAssetTypes.configuration_file, "TierColourConfiguration")
	{
		this.ColourConfiguration = null;
	}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
	{
	    this.ColourConfiguration = (ColoursConfiguration) scriptableObject;//JsonConverter.DeserializeObject<ColoursConfiguration>(assetDataString);
	}

	public Color GetTierColour(eCarTier tier)
	{
		return this.ColourConfiguration.TierColours.ElementAt((int)tier).Value;
	}

	public Color GetTierColour(string tier)
	{
		Color magenta = Color.magenta;
		this.ColourConfiguration.TierColours.TryGetValue(tier, out magenta);
		return magenta;
	}

	public Color GetTierColour(int tier)
	{
		return this.ColourConfiguration.TierColours.ElementAt(tier).Value;
	}

	public void SetTierXColour(Color c)
	{
		this.ColourConfiguration.TierColours["TX"] = c;
	}

	public int GetColourCount()
	{
		return this.ColourConfiguration.TierColours.Count;
	}
}
