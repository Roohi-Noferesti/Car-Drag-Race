using UnityEngine;
using UnityEngine.UI;

public class BubbleMessageSprites : MonoBehaviour
{
	public Image TopLeft;

    public Image Top;

    public Image Left;

    public Image Middle;

    public Image Nipple;

	public float NippleYPadding;

	public float NippleYOffset;

	public void SetSpritesActive(bool value)
	{
        //this.TopLeft.gameObject.SetActive(value);
        //this.Top.gameObject.SetActive(value);
        //this.Left.gameObject.SetActive(value);
        //this.Middle.gameObject.SetActive(value);
		this.Nipple.gameObject.SetActive(value);
	}
}
