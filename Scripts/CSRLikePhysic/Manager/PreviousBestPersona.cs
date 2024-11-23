using System;
using I2.Loc;

public class PreviousBestPersona : LocalPersona
{
	private string _numberPlate = string.Empty;

	public override string GetDisplayName()
	{
		return LocalizationManager.GetTranslation("TEXT_PREVIOUS_BEST");
	}

	public override string GetNumberPlate()
	{
		return this._numberPlate;
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		jsonDict.TryGetValue("np", out this._numberPlate);
		this._numberPlate = NumberPlate.SafeString(this._numberPlate);
	}
}
