using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IAPBundlePopupCarWidget : BundleOfferWidget, IBundleOwner
{
	public RawImage CarImage;

	public TextMeshProUGUI TierText;

	public GameObject LoadingAnimation;

	public Animation EntryAnimation;

    //private string[] tierToLogo = new string[]
    //{
    //    "tier1_icon",
    //    "tier2_icon",
    //    "tier3_icon",
    //    "tier4_icon",
    //    "tier5_icon",
    //    "tier5_icon"
    //};

	public override void Setup(IBundleOfferWidgetInfo placementInfo)
	{
        //base.gameObject.transform.localPosition = placementInfo.Position;
        //this.OfferSprite.transform.parent.localPosition = placementInfo.SpritePosition;
        //this.OfferSprite.gameObject.SetActive(false);
        this.CarImage.gameObject.SetActive(false);
        //this.LoadingAnimation.transform.localPosition = placementInfo.SpritePosition;
		this.LoadingAnimation.gameObject.SetActive(true);
		string carNiceName = CarDatabase.Instance.GetCarNiceName(placementInfo.CarDBKey);
		this.OfferDesc.text = carNiceName;
        //this.OfferDesc.transform.localPosition = placementInfo.DescPosition;
		this.OfferDesc.gameObject.SetActive(false);
        //this.TierLogo.gameObject.SetActive(false);
		this.TierText.gameObject.SetActive(false);
		this.LoadOfferCar(placementInfo.CarDBKey);
	}


    public void LoadOfferCar(string carDBKey)
    {
        CarGarageInstance carGarageInstance = new CarGarageInstance();
        var carInfo = CarDatabase.Instance.GetCar(carDBKey);
        carGarageInstance.SetupNewGarageInstance(carInfo);
        CarSnapshotManager.Instance.CarSlot = AsyncBundleSlotDescription.AICar;
        CarSnapshotManager.Instance.LiverySlot = AsyncBundleSlotDescription.AICarLivery;
        CarSnapshotManager.Instance.SnapshotType = CarSnapshotType.VSScreenLeft;
        CarSnapshotManager.Instance.SnapshotSize = 512;
        string textID = CarInfo.ConvertCarTierEnumToString(carGarageInstance.CurrentTier);
        this.TierText.text = LocalizationManager.GetTranslation(textID);
        Texture2D texture2D = CarSnapshotManager.Instance.LoadSnapshotFromCache(carGarageInstance);
        if (texture2D != null)
        {
            this.DisplayOfferCarTexture(texture2D);
        }
        else
        {
            CarSnapshotManager.Instance.GenerateSnapshot(carGarageInstance, delegate(Texture2D snapshotTexture)
            {
                this.DisplayOfferCarTexture(snapshotTexture);
            });
        }
        //this.LoadTierIcon(this.tierToLogo[(int)carGarageInstance.CurrentTier]);
    }

	private void DisplayOfferCarTexture(Texture2D snapshotTexture)
	{
        this.CarImage.gameObject.SetActive(true);
	    CarImage.texture = snapshotTexture;
        //this.OfferSprite.SetTexture(snapshotTexture);
        //this.OfferSprite.SetPixelDimensions(snapshotTexture.width, snapshotTexture.height);
        //this.OfferSprite.SetLowerLeftPixel(0, snapshotTexture.height - 1);
        //this.OfferSprite.gameObject.GetComponent<MeshRenderer>().enabled = true;
        //this.OfferSprite.gameObject.SetActive(true);
		this.LoadingAnimation.gameObject.SetActive(false);
		this.OfferDesc.gameObject.SetActive(true);
        this.TierText.gameObject.SetActive(true);
        CarSnapshotManager.Instance.ResetToDefaults();
        //this.EntryAnimation.Play();
	}

	private void LoadTierIcon(string assetName)
	{
		AssetProviderClient.Instance.RequestAsset(assetName, new BundleLoadedDelegate(this.TierIconLoaded), this);
	}

	private void TierIconLoaded(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
	{
        //UnityEngine.Object[] array = zAssetBundle.LoadAllAssets();
        //Texture2D texture2D = array[0] as Texture2D;
        //if (array.Length > 1)
        //{
        //    texture2D = (array[1] as Texture2D);
        //}
        ////this.TierLogo.gameObject.renderer.material.SetTexture("_MainTex", texture2D);
        ////this.TierLogo.Setup((float)texture2D.width / 200f, (float)texture2D.width / 200f, new Vector2(0f, (float)(texture2D.height - 1)), new Vector2((float)texture2D.width, (float)texture2D.height));
        //this.CarImage.gameObject.transform.localPosition = new Vector3(-2f, 0.12f, 0f);
        //AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
        //this.CarImage.gameObject.SetActive(true);
        //this.TierText.gameObject.SetActive(true);
	}
}
