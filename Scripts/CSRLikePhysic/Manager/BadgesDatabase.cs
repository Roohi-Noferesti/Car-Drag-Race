using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BadgesDatabase : ConfigurationAssetLoader
{
	public BadgesConfiguration Configuration
	{
		get;
		private set;
	}

	public BadgesDatabase() : base(GTAssetTypes.configuration_file, "BadgesConfiguration")
	{
		this.Configuration = new BadgesConfiguration();
	}

    //protected override void ProcessEventAssetDataString(string assetDataString)
    //{
    //    this.Configuration = JsonConverter.DeserializeObject<BadgesConfiguration>(assetDataString);
    //    this.Configuration.Initialise();
    //}

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        this.Configuration = (BadgesConfiguration) scriptableObject;
        this.Configuration.Initialise();
    }

	public List<Badge> GetBadgesWithIDs(List<string> networkIds)
	{
		Dictionary<string, Badge> dict = this.Configuration.Badges.ToDictionary((Badge badge) => badge.ID);
		return (from nid in networkIds.Where(new Func<string, bool>(dict.ContainsKey))
		select dict[nid]).ToList<Badge>();
	}

	public List<Badge> GetObtainedBadges(IGameState gameState)
	{
		return (from b in this.Configuration.Badges
		where b.Requirements.IsEligible(gameState)
		select b).ToList<Badge>();
	}
}
