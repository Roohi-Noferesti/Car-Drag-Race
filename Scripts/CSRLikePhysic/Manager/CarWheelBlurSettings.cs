using System;

[Serializable]
public class CarWheelBlurSettings
{
	public RadialBlurSettings RadialBlur1 = new RadialBlurSettings();

	public RadialBlurSettingsDefaultOff RadialBlur2 = new RadialBlurSettingsDefaultOff();

	public LinearBlurSettings LinearBlur = new LinearBlurSettings();
}
