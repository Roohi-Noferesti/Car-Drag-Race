using KingKodeStudio;

public static class BodyShaderIDCurrector
{
    public static string GetCurrcetBodyMaterialID(this string bodyShaderID)
    {
        string middlename;
        if (SceneLoadManager.Instance != null &&
            SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Frontend)
        {
            if (ScreenManager.Instance == null || ScreenManager.Instance.CurrentScreen != ScreenID.Showroom)
                middlename = "_Garage";
            else
                middlename = "_ShowRoom";
        }
        else
        {
            var raceTrack = RaceEventInfo.Instance!=null?RaceEventInfo.Instance.GetRaceTrack(): RaceEventInfo.RaceTrack.Night;
            if (raceTrack != RaceEventInfo.RaceTrack.Night)
                middlename = "_StreetDay";
            else
                middlename = "_StreetNight";
        }
        bodyShaderID = bodyShaderID.Insert(7, middlename);
        return bodyShaderID;
    }


    public static string GetCurrectRingMaterialID(this string ringShaderID)
    {
        if (SceneLoadManager.Instance != null &&
            SceneLoadManager.Instance.CurrentScene == SceneLoadManager.Scene.Race)
        {
            if (RaceEventInfo.Instance.GetRaceTrack()!=RaceEventInfo.RaceTrack.Night)
                return ringShaderID + "_Day";
        }
        return ringShaderID;
    }
}
