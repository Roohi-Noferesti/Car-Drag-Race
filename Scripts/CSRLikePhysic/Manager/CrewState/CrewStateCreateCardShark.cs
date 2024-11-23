using System;
using UnityEngine;

public class CrewStateCreateCardShark : BaseCrewState
{
	private string containerLocation;

	//private string charCardLocation;

	public CrewStateCreateCardShark(CrewProgressionScreen zParentScreen, string ContainerString, string CharacterCardString) : base(zParentScreen)
	{
		this.containerLocation = ContainerString;
		//this.charCardLocation = CharacterCardString;
	}

	public CrewStateCreateCardShark(NarrativeSceneStateConfiguration config) : base(config)
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
        //NitroContainer component = gameObject.GetComponent<NitroContainer>();
        //Texture2D zPortrait = Resources.Load(this.charCardLocation) as Texture2D;
        //component.Leader.LoadProtrait(zPortrait, this.parentScreen);
        //component.SetAlpha(0f, 0f);
        //component.SetMysteryDonorAlpha(0f);
        //Vector3 vector = new Vector3(1.6f, 0.12f, 0f);
        //vector = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //gameObject.transform.position = vector;
	}

	public override bool OnMain()
	{
		base.OnMain();
		return true;
	}

	public override void OnExit()
	{
	}
}
