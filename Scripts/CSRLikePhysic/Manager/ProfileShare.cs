using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ProfileShare : MonoBehaviour
{
	private const int Width = 1024;

	private const int Height = 700;

	private const int CarSnapshotSize = 512;

	private const float SpriteUnitConversion = 0.003f;

	private const int StatsInRow = 3;

	private const AsyncBundleSlotDescription CarSlot = AsyncBundleSlotDescription.HumanCar;

	private const AsyncBundleSlotDescription LiverySlot = AsyncBundleSlotDescription.HumanCarLivery;

	public Camera RenderCamera;

	public AvatarPicture Avatar;

	public TextMeshProUGUI UserNameText;

	public CarSnapshotGeneric CarSnapshot;

	public TextMeshProUGUI CarName;

	public Transform StatsLocation;

	public GameObject StatBoxRowPrefab;

	public float RowSpacing;

	public StatBox.StatType[] StatsToDisplay;

	public List<Renderer> ObjectsToTint = new List<Renderer>();

	private Color tintColor;

	private bool avatarReady;

	private bool snapshotReady;

	private void Start()
	{
		CarGarageInstance currentCar = PlayerProfileManager.Instance.ActiveProfile.GetCurrentCar();
		//this.CarSnapshot.Setup(currentCar, new Action(this.CarSnapshotLoaded));
		//this.tintColor = GameDatabase.Instance.Colours.GetTierColour(currentCar.CurrentTier);
		//PlayerInfo info = MultiplayerProfileScreen.Info;
		//this.Avatar.onAvatarLoaded += new OnAvatarLoaded(this.AvatarLoaded);
		//info.Persona.SetupAvatarPicture(this.Avatar);
		//this.UserNameText.text = info.DisplayName;
		//this.PopulateStatBoxes();
        //this.CarName.text = CarDatabase.Instance.GetCarNiceName(currentCar.CarDBKey);
		//this.ApplyTint();
	}

	private void ApplyTint()
	{
		foreach (Material current in this.ObjectsToTint.SelectMany((Renderer r) => r.materials))
		{
			current.SetColor("_Tint", this.tintColor);
		}
	}

	private void PopulateStatBoxes()
	{
		for (int i = 0; i < this.StatsToDisplay.Length; i += 3)
		{
			List<StatBox.StatType> statsInRow = this.StatsToDisplay.Skip(i).Take(3).ToList<StatBox.StatType>();
			float offset = this.RowSpacing * (float)i / 3f;
			this.CreateStatRow(offset, statsInRow);
		}
	}

	private void CreateStatRow(float offset, List<StatBox.StatType> statsInRow)
	{
		GameObject gameObject = (GameObject)UnityEngine.Object.Instantiate(this.StatBoxRowPrefab);
		gameObject.transform.SetParent(this.StatsLocation, false);
		Vector3 localPosition = new Vector3(0f, -offset, 0f);
		gameObject.transform.localPosition = localPosition;
		StatBox[] componentsInChildren = gameObject.GetComponentsInChildren<StatBox>();
		if (componentsInChildren.Length >= statsInRow.Count)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (i < statsInRow.Count)
				{
					componentsInChildren[i].SetStat(statsInRow[i]);
					componentsInChildren[i].SetTint(this.tintColor);
				}
				else
				{
					componentsInChildren[i].gameObject.SetActive(false);
				}
			}
		}
	}

	private void AvatarLoaded()
	{
		this.avatarReady = true;
		this.Avatar.onAvatarLoaded -= new OnAvatarLoaded(this.AvatarLoaded);
	}

	private void CarSnapshotLoaded()
	{
		this.snapshotReady = true;
	}

	public bool IsReadyToShare()
	{
		return this.avatarReady && (this.snapshotReady || this.CarSnapshot.SnapshotIsLoaded);
	}

	public Texture2D Render()
	{
	    RenderTexture temporary = RenderTexture.GetTemporary(1024, 700, 0, RenderTextureFormat.ARGB32);
		RenderTexture.active = temporary;
		this.RenderCamera.targetTexture = temporary;
		this.RenderCamera.Render();
		Texture2D texture2D = new Texture2D(temporary.width, temporary.height, TextureFormat.RGB24, false);
		texture2D.ReadPixels(new Rect(0f, 0f, (float)temporary.width, (float)temporary.height), 0, 0);
		texture2D.Apply(false);
		RenderTexture.active = null;
		this.RenderCamera.targetTexture = null;
		RenderTexture.ReleaseTemporary(temporary);
		return texture2D;
	}
}
