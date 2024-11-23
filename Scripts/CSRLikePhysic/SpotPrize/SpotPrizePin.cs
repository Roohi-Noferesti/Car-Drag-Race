using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpotPrizePin : MonoBehaviour
{
	private const string introAnimationName = "SpotPrizeIntro";

	public TextMeshProUGUI Description;

    public TextMeshProUGUI Milestone;

	public GameObject IconParent;

	public Image UnlockGlow;

	public SimpleLensFlare LensFlare;

	public OnIntroAnimationEvent introAnimationEvent;

	public void Setup(SpotPrizeData spotPrizeData, MultiplayerEventData multiplayerEvent, SpotPrizePinState state)
	{
        //GameObject prefab = Resources.Load("SpotPrizes/PinIcons/SpotPrize" + spotPrizeData.PrizeType) as GameObject;
        //GameObjectHelper.InstantiatePrefab<SpotPrizePin>(prefab, this.IconParent);
        //this.Description.Text = spotPrizeData.GetPinDescription();
        //this.Milestone.Text = multiplayerEvent.PrizeProgression.FormatQuantity(spotPrizeData.Milestone);
        //this.LensFlare.Startup();
        //this.LensFlare.Hide();
        //this.SetState(state);
        //this.UnlockGlow.SetColor(multiplayerEvent.Theme.GetSwatch().BarColor1);
	}

	public void SetState(SpotPrizePinState state)
	{
		this.UnlockGlow.gameObject.SetActive(state == SpotPrizePinState.Unlocked);
	}

	public void PrewarmIntroAnimation()
	{
		base.transform.localScale = Vector3.zero;
        //AnimationUtils.PlayFirstFrame(base.animation, "SpotPrizeIntro");
	}

	public void PlayIntroAnimation()
	{
        //AnimationUtils.PlayAnim(base.animation, "SpotPrizeIntro");
		AudioManager.Instance.PlaySound("Reward_StarSlide", null);
	}
}
