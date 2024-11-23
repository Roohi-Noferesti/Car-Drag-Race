using UnityEngine;

[System.Serializable]
public class LeagueData
{
    public enum LeagueName
    {
        None = -1,
        Regular = 0,
        Bronze = 1,
        Silver = 2,
        Golden = 3,
        Diamond = 4
    }

    public LeagueName Name;
    public string LocalizedName;
    public int Star;
}