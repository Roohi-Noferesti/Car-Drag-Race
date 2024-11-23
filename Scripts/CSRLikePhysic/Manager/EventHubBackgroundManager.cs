using DataSerialization;
using System;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.UI;

public class EventHubBackgroundManager : MonoBehaviour
{
	private const float extraOffset = 0.08f;

	public RawImage FancyBackground;

	public RawImage FancyBackgroundFloor;

	public RawImage FancyBackgroundBottom;

	public RawImage TopGlow;

	public RawImage BottomGlow;

	public EventHubProgressLayout ProgressionObject;

	public EventHubOpponents OpponentsObject;

	public RawImage Branding;

	public Material FancyMaterial;

	private float GetAspectRatio(RawImage s)
	{
	    //float height = s.height;
        //float width = s.width;
        //return width / height;
	    return 0;
	}

    public void Show(ThemeOptionLayoutDetails details)
	{
		this.FancyBackground.gameObject.SetActive(false);
		this.FancyBackgroundFloor.gameObject.SetActive(false);
		this.TopGlow.gameObject.SetActive(false);
		this.BottomGlow.gameObject.SetActive(false);
		string textureID = details.Background + "Background";
		Texture2D eventHubTexture = TierXManager.Instance.GetEventHubTexture(textureID);
		this.SetRawImageTexture(eventHubTexture, this.FancyBackground);
		if (details.UseBackgroundGlow)
		{
            //this.FancyBackground.SetMaterial(this.FancyMaterial);
            //this.FancyBackground.renderer.material.SetTexture("_StaticGlowTex", eventHubTexture);
		}
        //this.FancyBackground.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenWidth / this.GetAspectRatio(this.FancyBackground), this.FancyBackground.lowerLeftPixel, this.FancyBackground.pixelDimensions);
		string textureID2 = details.Background + "Floor";
		this.SetRawImageTexture(TierXManager.Instance.GetEventHubTexture(textureID2), this.FancyBackgroundFloor);
        //this.FancyBackground.transform.Translate(0f, -CommonUI.Instance.NavBar.GetHeight() - GUICamera.Instance.ScreenHeight / 3.5f, 0f);
		this.FancyBackgroundFloor.transform.position = this.FancyBackground.transform.position;
		this.FancyBackgroundFloor.transform.Translate(0f, 0.01f, 0.1f);
        //this.FancyBackgroundFloor.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenHeight / 4f, this.FancyBackgroundFloor.lowerLeftPixel, this.FancyBackgroundFloor.pixelDimensions);
		string textureID3 = details.Background + "Glow";
		Texture2D eventHubTexture2 = TierXManager.Instance.GetEventHubTexture(textureID3);
		this.SetRawImageTexture(eventHubTexture2, this.TopGlow);
		this.SetRawImageTexture(eventHubTexture2, this.BottomGlow);
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen != null)
		{
            //this.TopGlow.transform.localPosition = new UnityEngine.Vector3(-careerModeMapScreen.eventPane.PaneWidthTight / 2f, (-GUICamera.Instance.ScreenHeight - CommonUI.Instance.NavBar.GetHeight()) / 2f + 0.08f, 0f);
            //this.TopGlow.Setup(GUICamera.Instance.ScreenWidth - careerModeMapScreen.eventPane.PaneWidthTight, 0.1f, this.TopGlow.lowerLeftPixel, this.TopGlow.pixelDimensions);
            //this.BottomGlow.transform.localPosition = new UnityEngine.Vector3(-careerModeMapScreen.eventPane.PaneWidthTight / 2f, -GUICamera.Instance.ScreenHeight / 5f * 4f - 0.04f, 0f);
            //this.BottomGlow.Setup(GUICamera.Instance.ScreenWidth - careerModeMapScreen.eventPane.PaneWidthTight, 0.1f, this.BottomGlow.lowerLeftPixel, this.BottomGlow.pixelDimensions);
		}
        //this.FancyBackgroundBottom.transform.position = this.FancyBackgroundFloor.transform.position - new UnityEngine.Vector3(0f, this.FancyBackgroundFloor.height, 0f);
        //this.FancyBackgroundBottom.width = GUICamera.Instance.ScreenWidth;
        //this.FancyBackgroundBottom.height = GUICamera.Instance.ScreenHeight / 28f * 13f;
		this.ProgressionObject.SetColour(details.Colour.AsUnityColor());
		IGameState gameState = new GameStateFacade();
		this.ProgressionObject.UpdateContent(details.EventNameText.GetText(gameState), details.ProgressionText.GetText(gameState));
		string textureID4 = details.Background + "Branding";
		Texture2D eventHubTexture3 = TierXManager.Instance.GetEventHubTexture(textureID4);
		this.SetRawImageTextureAndResizeToFit(eventHubTexture3, this.Branding);
        //this.Branding.transform.Translate(-careerModeMapScreen.eventPane.PaneWidthTight / 2f, -CommonUI.Instance.NavBar.GetHeight() * 1.2f, 0f);
		if (details.ProgressionSnapshots != null)
		{
			this.OpponentsObject.CreateOpponents(details);
			this.OpponentsObject.transform.Translate(-careerModeMapScreen.eventPane.PaneWidthTight / 2f, -(this.OpponentsObject.transform.position.y - this.BottomGlow.transform.position.y) / 2f, 0f);
		}
		else
		{
			this.OpponentsObject.gameObject.SetActive(false);
		}
	}

	private void SetRawImageTextureAndResizeToFit(Texture2D texture, RawImage sprite)
	{
		if (sprite != null)
		{
			UnityEngine.Vector2 lowerleftPixel = new UnityEngine.Vector2(0f, (float)texture.height - 1f);
			UnityEngine.Vector2 pixeldimensions = new UnityEngine.Vector2((float)texture.width, (float)texture.height);
			float w = (float)texture.width / 200f;
			float h = (float)texture.height / 200f;
            sprite.texture = (texture);
            //sprite.Setup(w, h, lowerleftPixel, pixeldimensions);
            //sprite.StartFade();
			sprite.gameObject.SetActive(true);
		}
	}

	private void SetRawImageTexture(Texture2D texture, RawImage sprite)
	{
		if (sprite != null)
		{
			sprite.texture = (texture);
            //sprite.StartFade();
			sprite.gameObject.SetActive(true);
		}
	}

	public void UpdateProgressionSnapshots(RaceEventData race)
	{
		CareerModeMapScreen x = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (x != null)
		{
			this.OpponentsObject.UpdateFromEvent(race);
		}
	}
}
