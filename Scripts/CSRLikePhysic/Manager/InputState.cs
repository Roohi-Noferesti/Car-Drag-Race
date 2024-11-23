public class InputState
{
	public bool GearChangeUp;

	public bool GearChangeDown;

	public bool Throttle;

	public bool FakeThrottle;

	public bool Nitrous;

	public bool Record;

	public bool CatchAll;

	public void Reset()
	{
		this.GearChangeUp = false;
		this.GearChangeDown = false;
		this.Throttle = false;
		this.FakeThrottle = false;
		this.Nitrous = false;
		this.Record = false;
		this.CatchAll = false;
	}
}
