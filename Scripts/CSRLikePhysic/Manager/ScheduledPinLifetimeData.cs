using System;

[Serializable]
public class ScheduledPinLifetimeData
{
    public string LifetimeGroup = string.Empty;

    public int RaceCountFirstShownAt = -1;

    public int RaceCountLastRacedAt = -1;

    public void UpdateRaceCountFirstShownAt(int raceCount)
    {
        if (this.RaceCountFirstShownAt < 0)
        {
            this.RaceCountFirstShownAt = raceCount;
        }
    }

    public void UpdateRaceCountLastRacedAt(int raceCount)
    {
        this.RaceCountLastRacedAt = raceCount;
    }

    public void ResetRaceCountFirstShownAt()
    {
        this.RaceCountFirstShownAt = -1;
    }

    public bool HasRecordedProgress()
    {
        return this.RaceCountFirstShownAt != -1 || this.RaceCountLastRacedAt != -1;
    }
}
