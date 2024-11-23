using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CarStatsElem : MonoBehaviour
{
	public Image ColouredBackground;

    public Image Background;

	public TextMeshProUGUI txtClass;

    public TextMeshProUGUI txtPI;

    public void Show(bool zShow)
    {
        base.gameObject.SetActive(PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar && zShow);
    }

    public void SetClipPos(float clipPos)
	{
        //Material material = this.ColouredBackground.renderer.material;
        //material.name = "TierPP BG ClipAbove";
        //material.SetFloat("_ClipPos", clipPos);
        //this.Background.renderer.sharedMaterial = material;
        //Material material2 = this.txtClass.renderer.material;
        //material2.SetFloat("_ClipPos", clipPos);
        //this.txtPI.renderer.sharedMaterial = material2;
	}

	private void ShowTheCorrectColoursForClass(eCarTier zClass)
	{
        //Color tierColour = GameDatabase.Instance.Colours.GetTierColour(zClass);
        //this.ColouredBackground.SetColor(tierColour);
	}

	public void Set(eCarTier zClass, int zPerformanceIndex)
	{
		this.Set(zClass, zPerformanceIndex.ToString());
	}

	public void Set(eCarTier zClass, string customText)
	{
	    this.txtClass.text = LocalizationManager.GetTranslation(CarInfo.ConvertCarTierEnumToShortString(zClass));
        this.txtPI.text = customText.ToNativeNumber();
		this.txtPI.gameObject.SetActive(true);
		this.ShowTheCorrectColoursForClass(zClass);
	}

	public void SetAlpha(float zAlpha)
	{
        //Color color = this.ColouredBackground.Color;
        //color.a = zAlpha;
        //this.ColouredBackground.SetColor(color);
        //color = this.Background.Color;
        //color.a = zAlpha;
        //this.Background.SetColor(color);
        //color = this.txtClass.Color;
        //color.a = zAlpha;
        //this.txtClass.SetColor(color);
        //color = this.txtPI.Color;
        //color.a = zAlpha;
        //this.txtPI.SetColor(color);
	}

	public void SetPositionWithTopLeftAnchor(float x, float y)
	{
        //GameObjectHelper.SetLocalX(base.gameObject, x + this.Background.width / 2f);
        //GameObjectHelper.SetLocalY(base.gameObject, y - this.Background.height / 2f);
	}
}
