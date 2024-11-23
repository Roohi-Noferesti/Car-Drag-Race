using System;
using UnityEngine;

public class CrewStateCreateMysteryDonor : BaseCrewState
{
	private string containerLocation;

	//private string charCardLocation;

	//private bool nitroLoaded;

	public CrewStateCreateMysteryDonor(CrewProgressionScreen zParentScreen, string ContainerString, string CharacterCardString) : base(zParentScreen)
	{
		this.containerLocation = ContainerString;
		//this.charCardLocation = CharacterCardString;
	}

	public CrewStateCreateMysteryDonor(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.containerLocation = config.StateDetails.ContainerString;
		//this.charCardLocation = config.StateDetails.CharCardString;
	}

	public override void OnEnter()
	{
		Transform container = this.parentScreen.GetContainer();
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(this.containerLocation)) as GameObject;
		gameObject.transform.parent = container;
		gameObject.name = "MysteryDonor";
        //NitroContainer nitroComponent = gameObject.GetComponent<NitroContainer>();
        //this.nitroLoaded = false;
        //TexturePack.RequestTextureFromBundle(this.charCardLocation, delegate(Texture2D nitroTexture2D)
        //{
        //    nitroComponent.Leader.LoadProtrait(nitroTexture2D, this.parentScreen);
        //    nitroComponent.SetAlpha(0f, 0f);
        //    this.nitroLoaded = true;
        //});
        //nitroComponent.SetMysteryDonorAlpha(0f);
        //float screenWidth = GUICamera.Instance.ScreenWidth;
        //Vector3 vector = this.parentScreen.transform.FindChild("CenterLeft").position + new Vector3(screenWidth * 0.5f - 0.8f, -0.12f, 0f);
        //vector = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //gameObject.transform.position = vector;
	}

	public override bool OnMain()
	{
	    return base.OnMain();
	    //return this.nitroLoaded;
	}

    public override void OnExit()
	{
	}
}
