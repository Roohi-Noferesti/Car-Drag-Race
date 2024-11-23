public class Z2HIDInfo : Z2HInitBase
{
	private InitDelegate callMe;

	public Z2HIDInfo(InitDelegate inCallMe, bool inTakes, string inName) : base(inTakes, inName)
	{
		this.callMe = inCallMe;
	}

	public override void Process()
	{
		this.callMe();
	}
}
