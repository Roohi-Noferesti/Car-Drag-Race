using DataSerialization;
using System;
using I2.Loc;

[Serializable]
public class MessageOption
{
	private string message = string.Empty;

	public ConditionallySelectedString ConditionalMessage = ConditionallySelectedString.CreateEmpty();

	public EligibilityRequirements Requirements = EligibilityRequirements.CreateAlwaysEligible();

	public string Message
	{
		get
		{
			return this.ConditionalMessage.GetText(new GameStateFacade()) ?? LocalizationManager.GetTranslation(this.message);
		}
		set
		{
			this.message = value;
		}
	}

	public void Initialise()
	{
		this.Requirements.Initialise();
	}
}
