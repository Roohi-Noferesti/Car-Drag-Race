public class StarPersona : PersonaComponent
{
	private StarType type;

	public StarPersona(StarType type)
	{
		this.type = type;
	}

	protected override void RequestAvatar()
	{
		string avatarName = string.Format("StarAvatar{0}", (int)this.type);
		base.LoadCsrAvatarFromResources(avatarName);
	}

	public override string GetDisplayName()
	{
		string textID = string.Format("TEXT_FRIENDS_{0}_STAR_RACE_TIME", (int)this.type);
	    return textID;//LocalizationManager.GetTranslation(textID);
	}

	public override string GetNumberPlate()
	{
		return string.Format("{0} STAR", (int)this.type);
	}

    public override void SerialiseFromJson(JsonDict jsonDict)
    {
    }

    public override void SerialiseToJson(JsonDict jsonDict)
    {
    }
}
