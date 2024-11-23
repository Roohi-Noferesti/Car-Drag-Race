public static class MemValidatorUtilities
{
	private static long MangleIt = 9223372036854775783L;

	public static long MangleValue(long value)
	{
		return ~value ^ MangleIt;
	}
}
