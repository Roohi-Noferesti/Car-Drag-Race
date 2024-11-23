using System;
using UnityEngine;

[Serializable]
public class TutorialConfiguration:ScriptableObject
{
	public string TutorialCar;

    public string TutorialCarBodyShader;
    public string TutorialCarHeadlightShader;
    public string TutorialCarRingShader;
    public string TutorialCarSticker;
    public string TutorialCarSpoiler;

	public string TutorialCarLivery;

	public string FirstDailyBattleCar;

    public string FirstDailyBattleCarBodyShader;
    public string FirstDailyBattleCarHeadlightShader;
    public string FirstDailyBattleCarRingShader;
    public string FirstDailyBattleCarSticker;
    public string FirstDailyBattleCarSpoiler;

	public string FirstDailyBattleCarLivery;

	public string FirstDailyBattleCarAIDriver;

	public TutorialUpgradeSet FirstDailyBattleCarUpgrades;

	public float BadThrottleMessageThreshold;

	public int AllHardRacesConditionalFrequency;

    public int Tutorial2PPDifference;

    public int Tutorial3PPDifference;
    public byte Tutorial3PlayerNitrousLevel;
    public int InitialGoldReward;
    public int InitialCashReward;

    [Header("Additional Conditions")] 
    public bool IsOn;

    public bool ShouldLockScreen;

}
