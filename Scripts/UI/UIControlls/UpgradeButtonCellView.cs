using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UpgradeButtonCellView : ImageButtonCellView
{
    [SerializeField]
    private GameObject[] m_upgradeObjects;

    [SerializeField] private TextMeshProUGUI m_titleText;



    public int UpgradeLevel
    {
        set
        {
            for (int i = 0; i < m_upgradeObjects.Length; i++)
            {
                m_upgradeObjects[i].GetComponent<Image>().enabled = i < value;
            }
        }
    }

	public override Sprite Icon
	{
		set
		{
			m_iconImage.sprite = value;
		}
	}

    public string Title
    {
        set { m_titleText.text = value; }
    }

    public eUpgradeType UpgradeType { get; set; }

    public void PlayFitAnimation()
    {
        GetComponent<Animator>().Play("Fit");
    }
}
