public abstract class Z2HInitBase
{
	public string name;

	public bool initTakesAWhile
	{
		get;
		private set;
	}

    public Z2HInitBase(bool inTakes, string inName)
	{
		this.initTakesAWhile = inTakes;
		this.name = inName;
	}

	public abstract void Process();
}
