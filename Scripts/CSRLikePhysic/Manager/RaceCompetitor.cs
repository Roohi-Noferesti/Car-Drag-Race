using UnityEngine;

public abstract class RaceCompetitor
{
    protected NetworkReplay RecordableReplayData;

	public CarPhysics CarPhysics
	{
		get;
		protected set;
	}

    public CarVisuals CarVisuals
    {
        get;
        protected set;
    }

	public Transform Transform
	{
		get;
		protected set;
	}

	public GameObject GameObject
	{
		get;
		protected set;
	}

    public RaceCar RaceCar
    {
        get;
        protected set;
    }

    public RaceCarAudio RaceCarAudio
    {
        get;
        protected set;
    }

	public eRaceCompetitorType CompetitorType
	{
		get;
		set;
	}

    public PlayerInfo PlayerInfo
    {
        get;
        protected set;
    }

	public RaceCompetitor()
	{
		this.SetupPlayerInfo();
	}

	public abstract void SetupPlayerInfo();

	public virtual void SetupRaceEvents()
	{
	}

	public virtual void RemoveRaceEvents()
	{
	}

	public virtual void UpdateEnterStateFixedUpdate()
	{
	}

	public virtual void UpdatePreCountDownFixedUpdate()
	{
		DriverInputs driverInputs;
		driverInputs.GearChangeUp = false;
		driverInputs.GearChangeDown = false;
		driverInputs.Throttle = 0f;
		driverInputs.Nitrous = false;
		this.CarPhysics.DriverInputs = driverInputs;
		this.CarPhysics.RunCarPhysics();
	}

	public virtual void UpdateGridFixedUpdate()
	{
	}

	public virtual void UpdateRaceFixedUpdate()
	{
	}

	public virtual void UpdatePostRaceFixedUpdate()
	{
	}

	public virtual void SetUpObjectReferences(GameObject gameObject,CarPhysics carPhysics)
	{
	    this.CarPhysics = carPhysics;//gameObject.GetComponent<CarPhysics>();
        this.CarVisuals = gameObject.GetComponent<CarVisuals>();
		this.Transform = gameObject.transform;
		this.GameObject = gameObject;
        this.RaceCar = gameObject.GetComponent<RaceCar>();
        this.RaceCarAudio = this.RaceCar.carAudio;
	}

	public virtual void Destroy()
	{
        Object.Destroy(this.RaceCar);
        Object.Destroy(this.CarVisuals.RaceVisuals);
	}

	public void StopAudio()
	{
        if (this.RaceCar != null)
        {
            this.RaceCar.StopAudio();
        }
	}
}
