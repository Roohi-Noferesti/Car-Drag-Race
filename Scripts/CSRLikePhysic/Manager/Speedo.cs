using I2.Loc;
using TMPro;
using UnityEngine;

public class Speedo : MonoBehaviour
{
	private TextMeshProUGUI speedoText;

    [SerializeField]
	private TextMeshProUGUI unitText;


	public int lastSpeed;

	public int currentSpeed;

	public void Reset()
	{
		this.currentSpeed = 0;
		this.lastSpeed = -1;
        this.speedoText = base.gameObject.GetComponent<TextMeshProUGUI>();
        var unitKeyString = PlayerProfileManager.Instance != null && PlayerProfileManager.Instance.ActiveProfile != null
                                                                  && PlayerProfileManager.Instance.ActiveProfile
                                                                      .UseMileAsUnit
            ? "TEXT_RACE_HUD_SPEED_UNIT_MPH"
            : "TEXT_RACE_HUD_SPEED_UNIT_KPH";
        unitText.text = LocalizationManager.GetTranslation(unitKeyString);

    }

	private void Update()
	{
		if (this.currentSpeed != this.lastSpeed)
		{
			this.lastSpeed = this.currentSpeed;
			this.speedoText.text = string.Format("{0:0}", this.currentSpeed);
		}
	}
}
