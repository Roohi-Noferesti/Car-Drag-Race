using UnityEngine;

public class StreakBarElement : MonoBehaviour
{
	public GameObject element;

	public GameObject elementSurround;

	private void Awake()
	{
		this.element.gameObject.SetActive(false);
		this.elementSurround.gameObject.SetActive(true);
	}

	public void SetLightOn(bool state)
	{
		this.element.gameObject.SetActive(state);
		this.elementSurround.gameObject.SetActive(!state);
	}
}
