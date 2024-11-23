using System;
using UnityEngine;

public class CrewStateCreateRaceTeamMember : BaseCrewState
{
	private string containerLocation;

	//private string charCardLocation;

	//private string characterName;

	public CrewStateCreateRaceTeamMember(CrewProgressionScreen zParentScreen, string ContainerString, string CharacterCardString, string CharacterName) : base(zParentScreen)
	{
		this.containerLocation = ContainerString;
		//this.charCardLocation = CharacterCardString;
		//this.characterName = CharacterName;
	}

	public CrewStateCreateRaceTeamMember(NarrativeSceneStateConfiguration config) : base(config)
	{
		this.containerLocation = config.StateDetails.ContainerString;
		//this.charCardLocation = config.StateDetails.CharCardString;
		//this.characterName = config.StateDetails.CharacterName;
	}

	public override void OnEnter()
	{
		Transform container = this.parentScreen.GetContainer();
		GameObject gameObject = UnityEngine.Object.Instantiate(Resources.Load(this.containerLocation)) as GameObject;
		gameObject.transform.parent = container;
		gameObject.name = "MysteryDonor";
        //NitroContainer component = gameObject.GetComponent<NitroContainer>();
        //Texture2D texture2D = Resources.Load(this.charCardLocation) as Texture2D;
        //Texture2D zPortrait = Resources.Load("CharacterCards/Consumables_background_card") as Texture2D;
        //component.Leader.LoadProtrait(zPortrait, this.parentScreen);
        //component.SetAlpha(0f, 0f);
        //component.SetTextAlpha(0f);
        //component.SetMysteryDonorAlpha(0f);
        //component.MysteryDonor.SetTexture(texture2D);
        //component.MysteryDonor.Setup(component.MysteryDonor.width, component.MysteryDonor.height, component.MysteryDonor.lowerLeftPixel, new Vector2((float)texture2D.width, (float)texture2D.height));
        //component.SwitchName(this.characterName);
        //Vector3 vector = new Vector3(1.6f, 0.12f, 0f);
        //vector = GameObjectHelper.MakeLocalPositionPixelPerfect(vector);
        //gameObject.transform.localPosition = vector;
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
