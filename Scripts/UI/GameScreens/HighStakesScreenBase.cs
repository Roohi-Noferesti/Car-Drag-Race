using System;
using I2.Loc;
using TMPro;
using UnityEngine;

public abstract class HighStakesScreenBase : ZHUDScreen
{
	public TextMeshProUGUI Title;

	public TextMeshProUGUI Body;

	public DataDrivenObject DataDrivenNode;

	public MainCharacterGraphic MainCharacter;

	public TextMeshProUGUI Caption;

	public static eCarTier TierForChallenge;

	public static bool IsSetupForWorldTourHighStakes;

	public static RaceEventData WorldTourHighStakesRaceEvent;

	public int SetUpScreenForWorldTourHighstakesScene(NarrativeScene scene)
	{
		this.DataDrivenNode.gameObject.SetActive(false);
		NarrativeSceneCharacter leader = scene.CharactersDetails.CharacterGroups[0].Leader;
		this.MainCharacter.gameObject.SetActive(false);
		TexturePack.RequestTextureFromBundle(leader.CardTextureName, delegate(Texture2D tex)
		{
			this.MainCharacter.LoadProtrait(tex, null);
			this.MainCharacter.gameObject.SetActive(true);
		});
		this.Caption.text = LocalizationManager.GetTranslation(leader.Name);
        this.Title.text = LocalizationManager.GetTranslation(scene.SceneDetails.Title).ToUpper();
        //LocalisationManager.AdjustText(this.Title, 2.1f);
		int highStakesGoldEntryCost = scene.SceneDetails.HighStakesGoldEntryCost;
		NarrativeSceneLine narrativeSceneLine = scene.Lines[0];
		if (narrativeSceneLine == null)
		{
			return highStakesGoldEntryCost;
		}
		IGameState gameState = new GameStateFacade();
		this.Body.text = string.Format(narrativeSceneLine.StateData.StateDetails.GetTranslatedMessage(gameState), CurrencyUtils.GetColouredGoldString(highStakesGoldEntryCost));
		return highStakesGoldEntryCost;
	}
}
