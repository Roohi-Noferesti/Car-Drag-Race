using UnityEngine;
using UnityEngine.UI;

public class NitrousDisplay : MonoBehaviour
{
    public float currentNitrous
    {
        set { NitrousCanSprite.fillAmount = value; }
    }

	public bool anyNitrousAvailable;

    [SerializeField]
	private Image NitrousCanSprite;

	public void Reset()
	{
		this.NitrousCanSprite = base.GetComponent<Image>();
	    NitrousCanSprite.fillAmount = 0;
	}
}
