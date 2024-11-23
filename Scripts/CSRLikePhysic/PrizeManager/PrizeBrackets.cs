using System;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PrizeBrackets : MonoBehaviour
{
    public PrizeBracketBox[] prizeBracketBoxes;

    private int leaderboardID = -1;

    private SeasonEventMetadata eventMetadata;

    public GameObject SeasonEndedOverlay;

    public TextMeshProUGUI SeasonEndedTitle;

    private bool waitingForLeaderboard;

    public void Setup()
    {
        if (!NetworkReplayManager.Instance.Leaderboard.RPBoundsAvailable())
        {
            this.waitingForLeaderboard = true;
            PrizeBracketBox[] array = this.prizeBracketBoxes;
            for (int i = 0; i < array.Length; i++)
            {
                PrizeBracketBox box = array[i];
                this.SetupEmptyBracketBox(box);
            }
            return;
        }
        if (!SeasonServerDatabase.Instance.IsAnySeasonActive())
        {
            this.SetupSeasonEndedMessage();
            return;
        }
        this.leaderboardID = SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
        RtwLeaderboardStatusItem leaderboardStatusForID = SeasonServerDatabase.Instance.GetLeaderboardStatusForID(this.leaderboardID);
        if (leaderboardStatusForID == null)
        {
            return;
        }
        this.eventMetadata = GameDatabase.Instance.SeasonEvents.GetEvent(leaderboardStatusForID.event_id);
        if (this.eventMetadata == null)
        {
            return;
        }
        if (SeasonCountdownManager.HasEventEnded(this.eventMetadata.ID))
        {
            this.SetupSeasonEndedMessage();
            return;
        }
        IOrderedEnumerable<RtwLeaderboardPrizeData> source = from p in leaderboardStatusForID.prizes
                                                             orderby p.requirement
                                                             select p;
        for (int j = 0; j < this.prizeBracketBoxes.Length; j++)
        {
            PrizeBracketBox prizeBracketBox = this.prizeBracketBoxes[j];
            if (j < source.Count<RtwLeaderboardPrizeData>())
            {
                RtwLeaderboardPrizeData rtwLeaderboardPrizeData = source.ElementAt(j);
                SeasonPrizeMetadata prize = GameDatabase.Instance.SeasonPrizes.GetPrize(rtwLeaderboardPrizeData.prize_id);
                if (prize != null)
                {
                    this.SetupBracketBox(rtwLeaderboardPrizeData, prize, prizeBracketBox);
                }
            }
            else
            {
                prizeBracketBox.LoadingSpinner.SetActive(false);
                this.SetupEmptyBracketBox(prizeBracketBox);
            }
        }
    }

    private void Update()
    {
        if (this.waitingForLeaderboard && NetworkReplayManager.Instance.Leaderboard.RPBoundsAvailable())
        {
            this.waitingForLeaderboard = false;
            this.Setup();
        }
    }

    private void SetupBracketBox(RtwLeaderboardPrizeData rtwPrize, SeasonPrizeMetadata prizeMetadata, PrizeBracketBox box)
    {
        box.LoadingSpinner.SetActive(false);
        TextMeshProUGUI[] componentsInChildren = box.PercentileSticker.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            TextMeshProUGUI TextMeshProUGUI = array[i];
            TextMeshProUGUI.text = this.LocalisedTopPercentile(rtwPrize.requirement);
        }
        box.BeatLabel.text = LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_BEAT_RP");
        int rPBound = NetworkReplayManager.Instance.Leaderboard.GetRPBound(rtwPrize.requirement);
        if (rPBound >= 0)
        {
            box.BeatRPValue.text = CurrencyUtils.GetRankPointsString(rPBound, true, false);
        }
        switch (prizeMetadata.Type)
        {
            case SeasonPrizeMetadata.ePrizeType.Car:
                this.SetupCarImage(box, prizeMetadata);
                break;
            case SeasonPrizeMetadata.ePrizeType.Gold:
                {
                    int amount = -1;
                    if (prizeMetadata.GetPrizeDataAsInt(out amount))
                    {
                        this.SetupGoldPrize(box, amount);
                    }
                    break;
                }
        }
    }

    private void SetupEmptyBracketBox(PrizeBracketBox box)
    {
        TextMeshProUGUI[] componentsInChildren = box.PercentileSticker.GetComponentsInChildren<TextMeshProUGUI>();
        TextMeshProUGUI[] array = componentsInChildren;
        for (int i = 0; i < array.Length; i++)
        {
            TextMeshProUGUI TextMeshProUGUI = array[i];
            TextMeshProUGUI.text = string.Empty;
        }
        box.BeatLabel.text = string.Empty;
        box.BeatRPValue.text = string.Empty;
    }

    private void SetupSeasonEndedMessage()
    {
        this.SeasonEndedOverlay.SetActive(true);
        this.SeasonEndedTitle.text = LocalizationManager.GetTranslation("TEXT_SEASON_ENDED");
        PrizeBracketBox[] array = this.prizeBracketBoxes;
        for (int i = 0; i < array.Length; i++)
        {
            PrizeBracketBox prizeBracketBox = array[i];
            prizeBracketBox.BracketRoot.SetActive(false);
        }
    }

    private void SetupCarImage(PrizeBracketBox box, SeasonPrizeMetadata prizeMetadata)
    {
        if (box == null || prizeMetadata.Type != SeasonPrizeMetadata.ePrizeType.Car)
        {
            return;
        }
        if (this.eventMetadata != null && this.eventMetadata.SeasonCarImageBundle != string.Empty)
        {
            TexturePack.RequestTextureFromBundle(this.eventMetadata.SeasonCarImageBundle + ".Prize Car", delegate(Texture2D texture)
            {
                this.ApplyPrizeTexture(box.PrizeImage, texture);
            });
            if (prizeMetadata.AwardedCarIsPro)
            {
                EliteCarOverlay eliteCarOverlay = EliteCarOverlay.Create(false);
                eliteCarOverlay.Setup(Vector3.zero, box.ProCarOverlayParent, 0.3f, 0.25f);
            }
        }
        else if (this.eventMetadata == null)
        {
        }
    }

    private void SetupGoldPrize(PrizeBracketBox box, int amount)
    {
        if (box == null)
        {
            return;
        }
        box.GoldOnlyPrize.gameObject.SetActive(true);
        box.GoldOnlyPrize.text = CurrencyUtils.GetGoldString(amount);
    }

    private void ApplyPrizeTexture(Image sprite, Texture2D texture)
    {
        if (sprite != null && texture != null)
        {
            //sprite.SetTexture(texture);
            //sprite.StartFade();
            //sprite.gameObject.SetActive(true);
            //sprite.Setup(sprite.width, sprite.width / (float)(texture.width / texture.height), new Vector2(0f, (float)texture.height), new Vector2((float)texture.width, (float)texture.height));
        }
    }

    private string LocalisedTopPercentile(int percentile)
    {
        return string.Format(LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_TOP_PERCENT"), percentile);
    }
}
