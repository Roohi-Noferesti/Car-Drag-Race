using System.Linq;
using I2.Loc;
using UnityEngine;

public class StarDatabase : ConfigurationAssetLoader
{
    public StarConfiguration Configuration;

    public StarDatabase()
        : base(GTAssetTypes.configuration_file, "StarConfiguration")
    {

    }

    protected override void ProcessEventAssetDataString(ScriptableObject scriptableObject)
    {
        Configuration = (StarConfiguration) scriptableObject;
    }

    public LeagueData.LeagueName GetPlayerLeague()
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile == null)
        {
            return 0;
        }
        return activeProfile.GetPlayerLeague();
    }

    private LeagueData GetLeageData(LeagueData.LeagueName league)
    {
        return Configuration.Leagues.FirstOrDefault(l => l.Name == league);
    }

    public string GetLeagueLocalizationName(LeagueData.LeagueName league)
    {
        return LocalizationManager.GetTranslation(GetLeageData(league).LocalizedName);
    }

    public LeagueData.LeagueName CurrentLeagueForStar(int playerStar)
    {
        int i;
        for (i = 1; i < Configuration.Leagues.Length; i++)
        {
            if (playerStar < this.StarTotalAtEndOfLeague(i))
            {
                return (LeagueData.LeagueName) i;
            }
        }
        return (LeagueData.LeagueName) Mathf.Clamp(i, 0, 4);
    }

    public int StarTotalAtEndOfLeague(int leagueIndex)
    {
        leagueIndex = Mathf.Clamp(leagueIndex, 0, 4);
        return Configuration.Leagues[leagueIndex].Star;
    }

    public LeagueData.LeagueName GetLeagueForPlayerStar()
    {
        var playerStar = this.GetPlayerStar();
        return this.CurrentLeagueForStar(playerStar);
    }

    public int GetPlayerStar()
    {
        PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        if (activeProfile == null)
        {
            return 0;
        }
        return activeProfile.GetPlayerStar();
    }

    public void LeagueChangePlayerToStar()
    {
        var currentLeagueForStar =  this.GetLeagueForPlayerStar();
        PlayerProfileManager.Instance.ActiveProfile.ChangePlayerLeague(currentLeagueForStar);
        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    }
}
