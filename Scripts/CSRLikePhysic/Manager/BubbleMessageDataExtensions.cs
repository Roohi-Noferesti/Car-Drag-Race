using DataSerialization;

public static class BubbleMessageDataExtensions
{
	public static bool ShouldBeDisplayed(this BubbleMessageData bmd)
	{
		return !string.IsNullOrEmpty(bmd.Text);
	}
}
