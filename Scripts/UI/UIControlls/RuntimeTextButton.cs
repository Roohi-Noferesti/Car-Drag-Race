using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class RuntimeTextButton : RuntimeButton
{
    public TextMeshProUGUI spriteText;
	public void SetText(string zText, bool alreadyLocalised, bool makeUpper = true)
	{
	    if (spriteText == null)
	        return;
		if (zText == string.Empty)
		{
			this.spriteText.text = zText;
		}
		else if (alreadyLocalised)
		{
			if (makeUpper)
			{
				this.spriteText.text = zText.ToUpper();
			}
			else
			{
				this.spriteText.text = zText;
			}
		}
		else if (makeUpper)
		{
			this.spriteText.text = LocalizationManager.GetTranslation(zText).ToUpper();
		}
		else
		{
			this.spriteText.text = LocalizationManager.GetTranslation(zText);
		}
	}

    public void Init(string zText, UnityAction buttonAction, List<GameObject> zExtras)
	{
        if (spriteText == null)
        {
            spriteText = GetComponentInChildren<TextMeshProUGUI>();
        }
		this.SetText(zText, false, true);
		this.spriteText.gameObject.SetActive(true);
        base.Init(buttonAction, zExtras);
	}

    void Reset()
    {
        spriteText = GetComponentInChildren<TextMeshProUGUI>();
    }
}
