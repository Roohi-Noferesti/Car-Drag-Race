using System;
using UnityEngine;
using UnityEngine.UI;

public abstract class CarSnapshotBase : MonoBehaviour, IBundleOwner
{
	public int SnapshotSize = 256;

	public CarSnapshotType SnapshotType;

	public AsyncBundleSlotDescription CarSlot;

	public AsyncBundleSlotDescription LiverySlot = AsyncBundleSlotDescription.HumanCarLivery;

	public GameObject LoadingSpinner;

	public bool UseFallback = true;

	protected Action loadedCallback;

	private CarSnapshotRequest request;

	public bool SnapshotIsLoaded
	{
		get;
		private set;
	}

	private void ShowLoadingSpinner()
	{
		if (this.LoadingSpinner != null)
		{
			this.LoadingSpinner.gameObject.SetActive(true);
		}
	}

	private void HideLoadingSpinner()
	{
		if (this.LoadingSpinner != null)
		{
			this.LoadingSpinner.gameObject.SetActive(false);
		}
	}

	private void SetCarSnapshot(Texture2D tex)
	{
        RawImage component = base.gameObject.GetComponent<RawImage>();
		if (component == null)
		{
			return;
		}
		int num = (int)((float)this.SnapshotSize * CarSnapshotManager.GetAspectRatio(this.SnapshotType));
	    component.texture = tex;
        //component.SetPixelDimensions(this.SnapshotSize, num);
        //component.SetLowerLeftPixel(0, num - 1);
        //component.SetUVs(new Rect(0f, 0f, 1f, 1f));
		base.gameObject.GetComponent<MeshRenderer>().enabled = true;
		this.SnapshotIsLoaded = true;
		this.request = null;
		if (this.loadedCallback != null)
		{
			this.loadedCallback();
		}
		this.HideLoadingSpinner();
	}

	protected void LoadCarSnapshot(CarGarageInstance garageInstance, RaceEventData race = null)
	{
		base.gameObject.GetComponent<MeshRenderer>().enabled = false;
		this.ShowLoadingSpinner();
		if (this.UseLowQualityAssets(this.UseFallback))
		{
			string carDBKey = garageInstance.CarDBKey;
			BundleLoadedDelegate zReadyDelegate = delegate(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner)
			{
				this.LoadedCarAvatarAsset(zAssetID, zAssetBundle, zOwner, garageInstance);
			};
			AssetProviderClient.Instance.RequestAsset("PlayerListCarTex_" + carDBKey, zReadyDelegate, this);
		}
		else
		{
			this.request = new CarSnapshotRequest
			{
				GarageInstance = garageInstance,
				SnapshotSize = this.SnapshotSize,
				CarSlot = this.CarSlot,
				LiverySlot = this.LiverySlot,
				SnapshotType = this.SnapshotType,
				ResultCallback = new Action<Texture2D>(this.SetCarSnapshot)
			};
			CarSnapshotQueue.Instance.RequestSnapshot(this.request);
		}
	}

	private bool UseLowQualityAssets(bool fallback = true)
	{
		return fallback && BaseDevice.ActiveDevice.DeviceQuality == AssetQuality.Low;
	}

	private void OnDestroy()
	{
		if (this.request != null)
		{
			CarSnapshotQueue.Instance.CancelSnapshot(this.request);
			this.request = null;
		}
		Material material = base.GetComponent<Renderer>().material;
		Texture2D texture2D = material.mainTexture as Texture2D;
		if (texture2D != null && this.UseLowQualityAssets(true))
		{
			Resources.UnloadAsset(texture2D);
		}
		UnityEngine.Object.Destroy(material);
	}

	public void LoadedCarAvatarAsset(string zAssetID, AssetBundle zAssetBundle, IBundleOwner zOwner, CarGarageInstance garageInstance)
	{
		Image component = base.gameObject.GetComponent<Image>();
		Texture2D texture = zAssetBundle.mainAsset as Texture2D;
		if (component != null)
		{
			Material material = new Material(Shader.Find("CSR Gui/GuiAlphaColorReplace (Texture, Tint)"));
			material.SetTexture("_MainTex", texture);
			component.gameObject.SetActive(true);
            //component.Setup(-component.width, Mathf.Abs(0.5f * component.width), new Vector2(0f, 127f), new Vector2(256f, 128f));
            //component.SetUVs(new Rect(0f, 0f, 1f, 1f));
            //component.SetMaterial(material);
			if (garageInstance.UseColorOverride)
			{
				Color color = garageInstance.ColorOverride;
				color.a = 1f;
                //component.renderer.material.SetColor("_Tint", color);
			}
			else if (zAssetBundle.Contains(zAssetID + "_Colours"))
			{
                TextAsset coloursAsset = zAssetBundle.LoadAsset<TextAsset>(zAssetID + "_Colours");
                //Color color = ColorUtils.GetCarColour(coloursAsset, garageInstance.CarDBKey);
                //color.a = 1f;
                //component.renderer.material.SetColor("_Tint", color);
			}
			base.gameObject.GetComponent<MeshRenderer>().enabled = true;
			this.SnapshotIsLoaded = true;
			base.gameObject.transform.localScale = Vector3.one;
			this.HideLoadingSpinner();
		}
		AssetProviderClient.Instance.ReleaseAsset(zAssetID, zOwner);
	}
}
