using DataSerialization;
using System;
using System.Collections.Generic;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vector2 = UnityEngine.Vector2;

public class WorldTourProgressScreen : ZHUDScreen
{
	public TextMeshProUGUI InfoText;

	public TextMeshProUGUI CrewLeaderNameText;

	public TextMeshProUGUI CrewLeaderCarText;

	public DataDrivenPortrait ThemePrizeCar;

	public DataDrivenPortrait CrewLeader;

	public GameObject CrewMemberParent;

	public GameObject CrewLeaderParent;

	public List<GameObject> CrewMembers;

	public RawImage BackgroundIcon;

	public RawImage CrewIcon;

	public RawImage CrewStripeL;

	public RawImage CrewStripeR;

	private float unitConversion = 200f;

	public override ScreenID ID
	{
		get
		{
			return ScreenID.WorldTourProgress;
		}
	}

	public override void OnActivate(bool zAlreadyOnStack)
	{
		base.OnActivate(zAlreadyOnStack);
        WorldTourProgressLayout progressLayout = TierXManager.Instance.ThemeDescriptor.ProgressLayout;
        this.SetUpText(progressLayout.ProgressText);
        //this.SetThemeCar(TierXManager.Instance.ProgressTextures.ThemePrizeCar);
        this.SetupLeader(progressLayout.CrewLeaderName, progressLayout.CrewLeaderEvent);
        //this.SetupIcons();
        this.SetupCrewMembers();
    }

	private void SetUpText(string text)
	{
		this.InfoText.text = LocalizationManager.GetTranslation(text);
		//UnityEngine.Vector2 pos = new UnityEngine.Vector2(-0.09f, 0.35f);
		//UnityEngine.Vector2 vector = this.ConvertUIPinPosition(pos);
		//this.InfoText.gameObject.transform.localPosition = new UnityEngine.Vector3(vector.x, vector.y, 0.2f);
	}

	private void SetThemeCar(string carRenderPath)
	{
		UnityEngine.Vector2 pos = new UnityEngine.Vector2(-0.09f, -0.1f);
		UnityEngine.Vector2 vector = this.ConvertUIPinPosition(pos);
		this.ThemePrizeCar.gameObject.transform.localPosition = new UnityEngine.Vector3(vector.x, vector.y, 0.2f);
		this.ThemePrizeCar.Init(carRenderPath, string.Empty, null);
	}

	private void SetCrewMember(GameObject crewMember, int crewIndex)
	{
		ProgressCrewMember component = crewMember.GetComponent<ProgressCrewMember>();
		component.SetupCrewMember(TierXManager.Instance.ThemeDescriptor.ProgressLayout.CrewMembers[crewIndex]);
	}

	private void SetupSprite(RawImage sprite, Texture2D tex)
	{
	    sprite.texture = tex;
        //sprite.renderer.material.SetTexture("_MainTex", tex);
		//sprite.Setup((float)tex.width / this.unitConversion, (float)tex.height / this.unitConversion, new UnityEngine.Vector2(0f, (float)tex.height - 1f), new UnityEngine.Vector2((float)tex.width, (float)tex.height));
	}

	public void SetupLeader(string name, string eventName)
	{
		this.CrewLeader.Init(TierXManager.Instance.ProgressTextures.ProgressScreenCrewLeader, name, null);
		this.CrewLeaderNameText.text = LocalizationManager.GetTranslation(name);
		//this.CrewLeaderCarText.text = CarDatabase.Instance.GetCarNiceName(TierXManager.Instance.ThemeDescriptor.ThemePrizeCar).ToUpper();
		//UnityEngine.Vector2 pos = new UnityEngine.Vector2(-0.3f, -0.07f);
		//UnityEngine.Vector2 vector = this.ConvertUIPinPosition(pos);
		//this.CrewLeaderParent.transform.localPosition = new UnityEngine.Vector3(vector.x, vector.y, 0.2f);
	}

	private void SetupCrewMembers()
	{
		//UnityEngine.Vector2 pos = new UnityEngine.Vector2(0f, 0.3f);
		//UnityEngine.Vector2 vector = this.ConvertUIPinPosition(pos);
		//this.CrewMemberParent.gameObject.transform.localPosition = new UnityEngine.Vector3(vector.x, vector.y, 0.2f);
		for (int i = 0; i < this.CrewMembers.Count; i++)
		{
			this.SetCrewMember(this.CrewMembers[i], i);
		}
	}

	public void SetupIcons()
	{
		Texture2D tex = (BuildType.IsAppTuttiBuild && TierXManager.Instance.ThemeDescriptorPrefab.ProgressTextures.CrewBackgroundIcon_AppTutti!=null)?
			TierXManager.Instance.ProgressTextures.CrewBackgroundIcon_AppTutti:
			TierXManager.Instance.ProgressTextures.CrewBackgroundIcon;
		this.SetupSprite(this.BackgroundIcon, tex);
		tex = TierXManager.Instance.ProgressTextures.CrewIcon;
		UnityEngine.Vector2 pos = new UnityEngine.Vector2(0.06f, 0.58f);
		UnityEngine.Vector2 vector = this.ConvertUIPinPosition(pos);
		this.CrewIcon.gameObject.transform.localPosition = new UnityEngine.Vector3(vector.x, vector.y, 0.4f);
		this.SetupSprite(this.CrewIcon, tex);
		tex = TierXManager.Instance.ProgressTextures.CrewStripe;
		this.SetupSprite(this.CrewStripeL, tex);
		this.SetupSprite(this.CrewStripeR, tex);
		UnityEngine.Vector3 localPosition = this.CrewStripeR.transform.localPosition;
		//this.CrewStripeR.transform.localPosition = new UnityEngine.Vector3(this.CrewIcon.width, localPosition.y, localPosition.z);
	}

	private UnityEngine.Vector2 ConvertUIPinPosition(UnityEngine.Vector2 pos)
	{
	    //float num = 0.5f;
		//float num2 = GUICamera.Instance.ScreenHeight - num;
		//return new UnityEngine.Vector2(pos.x * GUICamera.Instance.ScreenWidth * 0.5f, pos.y * num2 * 0.5f - num * 0.5f);
	    return new Vector2();
	}
}
