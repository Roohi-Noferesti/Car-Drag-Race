using UnityEngine;

public class AICompetitor : RaceCompetitor
{
	private AIPlayer AIPlayer
	{
		get;
		set;
	}

	public AICompetitor()
	{
		base.CompetitorType = eRaceCompetitorType.AI_COMPETITOR;
		this.AIPlayer = null;
	}

	public override void SetupPlayerInfo()
	{
		base.PlayerInfo = new AIPlayerInfo();
	}

    public override void SetupRaceEvents()
    {
        RaceController instance = RaceController.Instance;
        instance.Events.HandleEvent("RaceStart", CarPhysics.RaceStartedEventHandler);
    }

    public override void SetUpObjectReferences(GameObject gameObject,CarPhysics carPhysics)
	{
        base.SetUpObjectReferences(gameObject, carPhysics);
		this.AIPlayer = gameObject.GetComponent<AIPlayer>();
	}

	public override void UpdateEnterStateFixedUpdate()
	{
		this.UpdatePreCountDownRevving();
	}

	public override void UpdatePreCountDownFixedUpdate()
	{
		this.UpdatePreCountDownRevving();
	}

	private void UpdatePreCountDownRevving()
	{
		this.AIPlayer.DoUpdate();
		base.CarPhysics.DriverInputs = this.AIPlayer.DriverInputs;
		base.CarPhysics.RunCarPhysics();
	}

	public override void UpdateGridFixedUpdate()
	{
		this.AIPlayer.DoUpdate();
		base.CarPhysics.DriverInputs = this.AIPlayer.DriverInputs;
		base.CarPhysics.RunCarPhysics();
	}

	public override void UpdateRaceFixedUpdate()
	{
		if (!this.AIPlayer.CanStartEvent())
		{
			return;
		}
		this.AIPlayer.DoUpdate();
		DriverInputs driverInputs;
		driverInputs.GearChangeUp = this.AIPlayer.DriverInputs.GearChangeUp;
		if (base.CarPhysics.GearBox.CurrentGear == 0)
		{
			driverInputs.GearChangeUp = true;
		}
		driverInputs.GearChangeDown = this.AIPlayer.DriverInputs.GearChangeDown;
		driverInputs.Throttle = 1f;
		if (driverInputs.GearChangeUp)
		{
			driverInputs.Throttle = 0f;
		}
		driverInputs.Nitrous = this.AIPlayer.DriverInputs.Nitrous;
		base.CarPhysics.DriverInputs = driverInputs;
		base.CarPhysics.RunCarPhysics();
	}

	public override void UpdatePostRaceFixedUpdate()
	{
		this.AIPlayer.DoUpdate();
		base.CarPhysics.DriverInputs = this.AIPlayer.DriverInputs;
		base.CarPhysics.RunCarPhysics();
	}

	public override void Destroy()
	{
		base.Destroy();
		Object.Destroy(this.AIPlayer);
	}
}
