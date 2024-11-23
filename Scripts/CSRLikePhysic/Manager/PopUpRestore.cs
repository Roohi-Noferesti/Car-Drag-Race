using System;
using I2.Loc;

public class PopUpRestore : PopUpDialogue
{
	public ProfileDescription descriptionLeft;

	public ProfileDescription descriptionRight;

	private void SelectedSide(ProfileDescription selectedProfile, ProfileDescription otherProfile)
	{
		if (otherProfile.Confirmation.activeInHierarchy)
		{
			base.DoButtonAction(new PopUpButtonAction(selectedProfile.RestoreProfile), true);
		}
		else if (selectedProfile.Confirmation.activeInHierarchy)
		{
			selectedProfile.ShowConfirmation(false);
			otherProfile.RestoreButton.SetText(LocalizationManager.GetTranslation("TEXT_BUTTON_RESTORE"), true, true);
		}
		else
		{
			otherProfile.ShowConfirmation(true);
            otherProfile.SetConfirmationText((!selectedProfile.IsNewGame()) ? LocalizationManager.GetTranslation("TEXT_RESTORE_CONFIRM") : LocalizationManager.GetTranslation("TEXT_NEWGAME_CONFIRM"));
            selectedProfile.RestoreButton.SetText(LocalizationManager.GetTranslation("TEXT_BUTTON_YES"), true, true);
            //otherProfile.CancelRestoreButton.ForceAwake();
            otherProfile.CancelRestoreButton.SetText(LocalizationManager.GetTranslation("TEXT_BUTTON_CANCEL"), true, true);
            //LocalisationManager.AdjustText(otherProfile.CancelRestoreButton.RuntimeText.spriteText, 0.76f);
		}
	}

	public void SelectedLeft()
	{
		this.SelectedSide(this.descriptionLeft, this.descriptionRight);
	}

	public void SelectedRight()
	{
		this.SelectedSide(this.descriptionRight, this.descriptionLeft);
	}
}
