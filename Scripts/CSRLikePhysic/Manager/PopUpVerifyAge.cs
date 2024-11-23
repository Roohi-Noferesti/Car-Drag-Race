using System;
using System.Collections.Generic;
using System.Globalization;
using I2.Loc;
using TMPro;
using UnityEngine;

public class PopUpVerifyAge : PopUpDialogue
{
	public TMP_Dropdown yearDropDown;

    protected override void Start()
    {
        base.Start();
        
        LocalizationManager.CurrentLanguage = Application.systemLanguage.ToString();
        
        if(BuildType.IsAppTuttiBuild)
            LocalizationManager.CurrentLanguage = "Chinese";

        List<string> options = new List<string>();
        var currentYear = CurrentYear;
        for (int i = currentYear+1; i >= 1900; i--)
        {
            var option = string.Format("{0} - {1}", (i - 1), i);
            options.Add(option);
        }

        yearDropDown.AddOptions(options);

        yearDropDown.value = 0;

        yearDropDown.onValueChanged.AddListener(DropDown_ValueChanged);
        ConfirmButton.CurrentState = BaseRuntimeControl.State.Disabled;
    }


    private void DropDown_ValueChanged(int value)
    {
        if (CurrentYear-SelectedYear < 3)
        {
            ConfirmButton.CurrentState = BaseRuntimeControl.State.Disabled;
        }
        else
        {
            ConfirmButton.CurrentState = BaseRuntimeControl.State.Active;
        }
    }

    private int SelectedYear
    {
        get
        {
            return CurrentYear - yearDropDown.value;
        }
    }


    //We want to ensure current year is greater that 2021
    private int CurrentYear
    {
        get { return Mathf.Max(2021, GTDateTime.Now.Year); }
    }

    public void OnConfirmAge()
    {
        AgeVerificationManager.Instance.UserYearOfBirth = SelectedYear;
        AgeVerificationManager.Instance.HasAgeVerified = true;
        AgeVerificationManager.Instance.SaveChanges();
        PopUpManager.Instance.KillPopUp();
    }
}
