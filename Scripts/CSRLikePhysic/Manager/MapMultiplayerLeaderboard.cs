using System;
using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapMultiplayerLeaderboard : MonoBehaviour
{
	private const float normalEntryExtraPadding = 0.1f;

	private const float entryHeightScale = 0.85f;

	private const float entryHeightYouScale = 0.28f;

	private const int leaderboardRequestTimeout = 10000;

	public GameObject[] PercentileStickers;

	public TextMeshProUGUI PercentileStickerText;

    public TextMeshProUGUI PercentileStickerShadow;

    public TextMeshProUGUI ChangeoverText;

	public GameObject LeaderboardEntryPrefab;

	public GameObject LeaderboardEntryTextPrefab;

	public GameObject LeaderboardUserEntryPrefab;

	public GameObject RelativeTo;

	public GameObject BottomMiddle;

	public Image BottomFade;

	public Image TopFade;

	public float LeftLeaderboardPadding;

	public float RightLeaderboardPadding;

	public float MaxLeaderboardWidth;

	public float OtherPlayerYShift = 0.2f;

	public float OtherPlayerXShift = 0.2f;

	public GameObject LoadingSpinner;

	public RuntimeButton LeaderboardButton;

	private bool waitingOnServer;

	private bool leaderboardConstructed;

	private List<GameObject> createdGameObjects = new List<GameObject>();

	private DateTime leaderboardLastReqTime;

	private void OnLeaderboard()
	{
		MenuAudio.Instance.playSound(AudioSfx.MenuClickForward);
		ScreenManager.Instance.PushScreen(ScreenID.Leaderboards);
	}

	public void ClearMiniLeaderboard()
	{
		this.leaderboardConstructed = false;
		GameObject[] percentileStickers = this.PercentileStickers;
		for (int i = 0; i < percentileStickers.Length; i++)
		{
			GameObject gameObject = percentileStickers[i];
			gameObject.SetActive(false);
		}
		this.RequestLeaderboard();
		base.gameObject.SetActive(false);
	}

	private void OnDestroy()
	{
		this.createdGameObjects.ForEach(delegate(GameObject x)
		{
			UnityEngine.Object.Destroy(x);
		});
		this.createdGameObjects.Clear();
	}

	private void Start()
	{
		GameObject[] percentileStickers = this.PercentileStickers;
		for (int i = 0; i < percentileStickers.Length; i++)
		{
			GameObject gameObject = percentileStickers[i];
			gameObject.SetActive(false);
		}
		this.RequestLeaderboard();
        //float num = GUICamera.Instance.ScreenWidth - this.LeftLeaderboardPadding - this.RightLeaderboardPadding;
        //num = Mathf.Min(num, this.MaxLeaderboardWidth) - 0.1f;
        //this.BottomFade.width = num;
        //this.TopFade.width = num;
        //this.LeaderboardButton.width = num;
	}

	private void RequestLeaderboard()
	{
		this.waitingOnServer = true;
		this.LoadingSpinner.gameObject.SetActive(true);
		this.ChangeoverText.gameObject.SetActive(false);
		this.leaderboardLastReqTime = GTDateTime.Now;
		RTWStatusManager.Instance.ForcePollServer();
		NetworkReplayManager.Instance.Leaderboard.RequestActiveLeaderboards(true, 4);
	}

	private string LocalisedTopPercentile(int percentile)
	{
		return string.Format(LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_TOP_PERCENT"), percentile);
	}

	private void Update()
	{
		CareerModeMapScreen careerModeMapScreen = ScreenManager.Instance.ActiveScreen as CareerModeMapScreen;
		if (careerModeMapScreen != null)
		{
            //EventPane eventPane = careerModeMapScreen.eventPane;
            //float x = (GUICamera.Instance.ScreenWidth - eventPane.PaneWidthTight) / 2f;
            //float y = -GUICamera.Instance.ScreenHeight;
            //this.BottomMiddle.transform.localPosition = new Vector3(x, y, 0f);
		}
		if (this.waitingOnServer && !NetworkReplayManager.Instance.Leaderboard.IsRequestingLeaderboards)
		{
			this.waitingOnServer = false;
			if (!NetworkReplayManager.Instance.Leaderboard.HadError)
			{
				this.LoadingSpinner.gameObject.SetActive(false);
				this.ConstructMiniLeaderboard();
			}
		}
		TimeSpan timeSpan = GTDateTime.Now - this.leaderboardLastReqTime;
		if (!this.leaderboardConstructed && timeSpan.TotalMilliseconds > 10000.0)
		{
			this.RequestLeaderboard();
		}
	}

	private GameObject ConstructEmptyGenericRankingEntryListItem()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.LeaderboardEntryPrefab) as GameObject;
        //RankingEntry component = gameObject.GetComponent<RankingEntry>();
        //float num = GUICamera.Instance.ScreenWidth - this.LeftLeaderboardPadding - this.RightLeaderboardPadding - this.OtherPlayerXShift;
        //num = Mathf.Min(num, this.MaxLeaderboardWidth) - 0.1f;
        //component.BackgroundPane.Height = this.OtherPlayerYShift;
        //component.Width = num;
        //component.BackgroundPane.Height *= 0.85f;
        //component.BackgroundPane.UpdateSize();
        //component.UpdateSize();
        //gameObject.transform.localPosition = new Vector3(-component.Width / 2f, 0f, 0f);
		gameObject.transform.parent = this.RelativeTo.transform;
		this.createdGameObjects.Add(gameObject);
		return gameObject;
	}

	private GameObject ConstructGenericRankingEntryListItem(int rank, string playerName, int respectPoints)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.LeaderboardEntryPrefab) as GameObject;
		RankingEntry component = gameObject.GetComponent<RankingEntry>();
        //float num = GUICamera.Instance.ScreenWidth - this.LeftLeaderboardPadding - this.RightLeaderboardPadding - this.OtherPlayerXShift;
        //num = Mathf.Min(num, this.MaxLeaderboardWidth) - 0.1f;
        //float maxWidth = num / 4f;
        //component.BackgroundPane.Height = this.OtherPlayerYShift;
        //component.Width = num;
        //component.BackgroundPane.Height *= 0.85f;
        //component.BackgroundPane.UpdateSize();
		component.UpdateSize();
		component.SetFontSize(0.09f);
		component.SetText(this.LeaderboardEntryTextPrefab, "#" + string.Format("{0:#,###0}", rank), 0.1f,TextAlignmentOptions.Left, 0f);
        component.SetText(this.LeaderboardEntryTextPrefab, CurrencyUtils.GetRankPointsString(respectPoints, true, false), 20f, TextAlignmentOptions.Center, 0f);
        //component.TextName.maxWidth = maxWidth;
		component.TextName.text = playerName;
		component.TextName.transform.localPosition = new Vector3(0.58f * component.Width, 0f, 0f);
		Vector3 localPosition = component.TextParent.transform.localPosition;
		localPosition.y = 0.127500013f;
		component.TextParent.transform.localPosition = localPosition;
		gameObject.transform.localPosition = new Vector3(-component.Width / 2f, 0f, 0f);
		gameObject.transform.parent = this.RelativeTo.transform;
		this.createdGameObjects.Add(gameObject);
		return gameObject;
	}

	private GameObject ConstructPlayerRankingEntryListItem(int rank, int respectPoints)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.LeaderboardUserEntryPrefab) as GameObject;
		RankingEntryYou component = gameObject.GetComponent<RankingEntryYou>();
		LocalPlayerInfo localPlayerInfo = new LocalPlayerInfo();
        //float num = GUICamera.Instance.ScreenWidth - this.LeftLeaderboardPadding - this.RightLeaderboardPadding;
        //num = Mathf.Min(num, this.MaxLeaderboardWidth);
        //float maxWidth = num / 4f;
		component.Avatar.transform.parent.gameObject.SetActive(false);
		component.RankText.text = "#" + string.Format("{0:#,###0}", rank);
        component.RespectPointText.text = CurrencyUtils.GetRankPointsString(respectPoints, true, false);
        //component.UserName.maxWidth = maxWidth;
        component.UserName.text = localPlayerInfo.DisplayName;
        //WindowPaneBatched component2 = component.GetComponent<WindowPaneBatched>();
        //component2.Height *= 0.28f;
        //component2.UpdateSize();
        //component.SetWidth(num);
        //gameObject.transform.localPosition = new Vector3(-num / 2f, 0f, 0f);
		gameObject.transform.parent = this.RelativeTo.transform;
		this.createdGameObjects.Add(gameObject);
		return gameObject;
	}

	private int GetLeaderboardID()
	{
		return SeasonServerDatabase.Instance.GetMostRecentActiveSeasonLeaderboardID();
	}

	private void ConstructMiniLeaderboard()
	{
		int leaderboardID = this.GetLeaderboardID();
		EventLeaderboard leaderboard = NetworkReplayManager.Instance.Leaderboard.GetLeaderboard(leaderboardID);
		int seasonPercentile = NetworkReplayManager.Instance.Response.currentSeasonRankPercentile;
		if (leaderboard == null)
		{
			this.ChangeoverText.gameObject.SetActive(true);
			RTWStatusManager.Instance.ForcePollServer();
			return;
		}
		GameObject gameObject = this.ConstructPlayerRankingEntryListItem(leaderboard.currentSeasonRank, leaderboard.currentSeasonRP);
		RankingEntryYou component = gameObject.GetComponent<RankingEntryYou>();
        //WindowPaneBatched component2 = component.GetComponent<WindowPaneBatched>();
        //gameObject.transform.localPosition = new Vector3(-component2.Width / 2f, 0f, 0f);
		if (leaderboard.peersAbove.Count > 0)
		{
			EventPeerRanking eventPeerRanking = leaderboard.peersAbove[0];
			GameObject gameObject2 = this.ConstructGenericRankingEntryListItem(eventPeerRanking.rank, eventPeerRanking.dname, eventPeerRanking.rp);
			RankingEntry component3 = gameObject2.GetComponent<RankingEntry>();
			gameObject2.transform.localPosition = new Vector3(-component3.Width / 2f, -this.OtherPlayerYShift * 0.85f - 0.03f, 0f);
			if (leaderboard.peersAbove.Count > 1)
			{
				GameObject gameObject3 = this.ConstructEmptyGenericRankingEntryListItem();
				gameObject3.transform.localPosition = new Vector3(-component3.Width / 2f, -this.OtherPlayerYShift * 0.85f - 0.16f, 0f);
			}
		}
		if (leaderboard.peersBelow.Count > 0)
		{
			EventPeerRanking eventPeerRanking2 = leaderboard.peersBelow[leaderboard.peersBelow.Count - 1];
			GameObject gameObject4 = this.ConstructGenericRankingEntryListItem(eventPeerRanking2.rank, eventPeerRanking2.dname, eventPeerRanking2.rp);
			RankingEntry component4 = gameObject4.GetComponent<RankingEntry>();
			gameObject4.transform.localPosition = new Vector3(-component4.Width / 2f, this.OtherPlayerYShift * 0.85f + 0.01f, 0.001f);
			if (leaderboard.peersBelow.Count > 1)
			{
				GameObject gameObject5 = this.ConstructEmptyGenericRankingEntryListItem();
				gameObject5.transform.localPosition = new Vector3(-component4.Width / 2f, this.OtherPlayerYShift * 0.85f + 0.13f, 0f);
				this.TopFade.gameObject.SetActive(true);
			}
		}
		else
		{
			this.TopFade.gameObject.SetActive(false);
		}
		if (leaderboard.currentSeasonRP == 0)
		{
			base.gameObject.SetActive(false);
		}
		else if (seasonPercentile != 0)
		{
			RtwLeaderboardStatusItem leaderboardStatusForID = SeasonServerDatabase.Instance.GetLeaderboardStatusForID(leaderboardID);
			if (leaderboardStatusForID == null)
			{
				return;
			}
			if (GameDatabase.Instance.SeasonEvents.GetEvent(leaderboardStatusForID.event_id) == null)
			{
				return;
			}
			int num = (from pair in (from p in leaderboardStatusForID.prizes
			orderby p.requirement
			select p).Select((RtwLeaderboardPrizeData prize, int idx) => new
			{
				prize = prize,
				idx = idx + 1
			})
			where seasonPercentile <= pair.prize.requirement
			select pair.idx).FirstOrDefault<int>() - 1;
			num = Mathf.Min(num, this.PercentileStickers.Count<GameObject>() - 1);
			GameObject gameObject6;
			if (num >= 0)
			{
				gameObject6 = this.PercentileStickers.ElementAt(num);
			}
			else
			{
				gameObject6 = this.PercentileStickers.Last<GameObject>();
			}
            //WindowPaneBatched component5 = gameObject.GetComponent<WindowPaneBatched>();
            //gameObject6.gameObject.transform.localPosition = gameObject.transform.localPosition + new Vector3(component5.Width * 0.93f, 0.12f, -0.2f);
            //this.PercentileStickerText.gameObject.transform.localPosition = gameObject.transform.localPosition + new Vector3(component5.Width * 0.93f, 0.12f, -0.21f);
			this.PercentileStickerText.text = this.LocalisedTopPercentile(seasonPercentile);
			gameObject6.SetActive(true);
		}
		this.LeaderboardButton.gameObject.SetActive(true);
		this.leaderboardConstructed = true;
	}
}
