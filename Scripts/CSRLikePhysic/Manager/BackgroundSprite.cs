using UnityEngine;
using UnityEngine.UI;

public class BackgroundSprite : MonoBehaviour
{
	//private Image sprite;

	private void Start()
	{
		this.SetupSize();
		this.Hide();
	}

	private void SetupSize()
	{
        //this.sprite = base.GetComponent<Image>();
        //this.sprite.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenHeight, Vector2.zero, Vector2.zero);
	}

	public void Show()
	{
		base.gameObject.SetActive(true);
		this.SetupSize();
	}

	public void Hide()
	{
		base.gameObject.SetActive(false);
	}
}
