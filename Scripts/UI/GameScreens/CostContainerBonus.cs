using System.Collections;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class CostContainerBonus : MonoBehaviour
{
    public TextMeshProUGUI TitleText;
    public void Setup(int title)
    {
        if (TitleText!=null)
            TitleText.text = string.Format(LocalizationManager.GetTranslation("TEXT_LIVERY_BONUS"), title,"<sprite=0>");
        
    }
    
    public void SetState(bool state)
    {
        TitleText.gameObject.transform.parent.gameObject.SetActive(state);
    }
    
}
