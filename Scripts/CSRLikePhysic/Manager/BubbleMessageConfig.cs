public class BubbleMessageConfig
{
	public enum ThemeStyle
	{
		SMALL,
		BIG,
		BLUE
	}

	public enum PositionType
	{
		BOX_RELATIVE,
		NIPPLE_POINT
	}


	public const float DefaultFontSize = 0.16f;

	public float FontSize = 36;

	public ThemeStyle Theme;

	public PositionType PosType;

	public bool PlayGlowSwipeAnimation;

	public static BubbleMessageConfig DuringRace
	{
		get
		{
			return new BubbleMessageConfig
			{
				Theme = ThemeStyle.BLUE,
				PosType = PositionType.NIPPLE_POINT,
				PlayGlowSwipeAnimation = true
			};
		}
	}


    public static BubbleMessageConfig Frontend
    {
        get
        {
            return new BubbleMessageConfig
            {
                Theme = ThemeStyle.BLUE,
                PosType = PositionType.NIPPLE_POINT,
                PlayGlowSwipeAnimation = true
            };
        }
    }
}
