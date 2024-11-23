using System.Collections.Generic;

public class CompetitorManager
{
	public static CompetitorManager Instance;

	public List<RaceCompetitor> CompetitorList = new List<RaceCompetitor>();

	private bool hasCleanedUp;

	public RaceCompetitor LocalCompetitor
	{
		get
		{
			return (this.CompetitorList.Count <= 0) ? null : this.CompetitorList[0];
		}
	}

	private RaceCompetitor localCompetitor
	{
		get
		{
			return (this.CompetitorList.Count <= 0) ? null : this.CompetitorList[0];
		}
	}

	public RaceCompetitor OtherCompetitor
	{
		get
		{
			return this.otherCompetitor;
		}
	}

	private RaceCompetitor otherCompetitor
	{
		get
		{
			return (this.CompetitorList.Count <= 1) ? null : this.CompetitorList[1];
		}
	}

	public static void Create()
	{
		if (Instance == null)
		{
			Instance = new CompetitorManager();
			Instance.Clear();
		}
	}

	public void Clear()
	{
		this.CompetitorList.Clear();
		this.hasCleanedUp = false;
	}

	public bool IsLocalCompetitorOnly()
	{
		return this.CompetitorList.Count == 1;
	}

	public bool HasCleanedUp()
	{
		return this.hasCleanedUp;
	}

	public void ResetCarVisuals()
	{
        this.localCompetitor.CarVisuals.RaceVisuals.Reset();
        if (this.OtherCompetitor != null)
        {
            this.OtherCompetitor.CarVisuals.RaceVisuals.Reset();
        }
	}

	public string GetLocalPlayerName()
	{
        if (this.localCompetitor == null)
        {
            return string.Empty;
        }
        return this.localCompetitor.PlayerInfo.DisplayName;
	}

	public void StopAudio()
	{
		if (this.localCompetitor != null)
		{
			this.localCompetitor.StopAudio();
		}
		if (this.otherCompetitor != null)
		{
			this.otherCompetitor.StopAudio();
		}
	}

	public void SwitchToRaceWheels()
	{
        this.localCompetitor.CarVisuals.SwichToRaceWheels();
        if (this.otherCompetitor != null)
        {
            this.otherCompetitor.CarVisuals.SwichToRaceWheels();
        }
	}

	public void DestroyCompetitorComponents()
	{
		if (this.otherCompetitor != null)
		{
			this.otherCompetitor.Destroy();
		}
		this.localCompetitor.Destroy();
		this.hasCleanedUp = true;
	}

	public void AddCompetitor(RaceCompetitor zCompetitor)
	{
		this.CompetitorList.Add(zCompetitor);
	}

	public void AddCompetitor(eRaceCompetitorType competitorType)
	{
		switch (competitorType)
		{
		case eRaceCompetitorType.LOCAL_COMPETITOR:
			this.CompetitorList.Add(new LocalCompetitor());
			break;
        case eRaceCompetitorType.AI_COMPETITOR:
            this.CompetitorList.Add(new AICompetitor());
            break;
        case eRaceCompetitorType.NETWORK_COMPETITOR:
            this.CompetitorList.Add(new NetworkCompetitor());
            break;
		}
	}

	public void UpdateEnterStateFixedUpdate()
	{
		foreach (RaceCompetitor current in this.CompetitorList)
		{
			current.UpdateEnterStateFixedUpdate();
		}
	}

	public void UpdatePreCountDownFixedUpdate()
	{
		foreach (RaceCompetitor current in this.CompetitorList)
		{
			current.UpdatePreCountDownFixedUpdate();
		}
	}

	public void UpdateGridFixedUpdate()
	{
		foreach (RaceCompetitor current in this.CompetitorList)
		{
			current.UpdateGridFixedUpdate();
		}
	}

	public void UpdateRaceFixedUpdate()
	{
		foreach (RaceCompetitor current in this.CompetitorList)
		{
			current.UpdateRaceFixedUpdate();
		}
	}

	public void UpdatePostRaceFixedUpdate()
	{
		foreach (RaceCompetitor current in this.CompetitorList)
		{
			current.UpdatePostRaceFixedUpdate();
		}
	}

    public void EnableVisualAnimation(bool value)
    {
        this.localCompetitor.CarVisuals.EnableAnimation(value);
        if (this.OtherCompetitor != null)
        {
            this.OtherCompetitor.CarVisuals.EnableAnimation(value);
        }
    }
}
