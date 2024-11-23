using System.Collections.Generic;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBox : MonoBehaviour
{
	public enum StatType
	{
		WorldRank,
		Onlinewinlossratio,
		HalfMileBestTime,
		QuarterMileBestTime,
		TotalDistance,
		TotalTime,
		Level,
		TotalCashEver,
		TotalGoldEver,
		Numberofcarsowned,
		TotalGaragePP,
		TotalStarCount,
		BossesBeaten,
		Empty
	}

	private StatType stat = StatType.Empty;

	public TextMeshProUGUI StatNameText;

	public TextMeshProUGUI StatValueText;

	public PrefabPlaceholder StatTypePrefab;

	public Image boxLeft;

    public Image boxMid;

    public Image boxRight;

	private Color WinColor;

	private Color LossColor;

	public List<Renderer> ObjectsToTint = new List<Renderer>();

	public PlayerInfo info;

	private bool waitingToTintIcon;

	private Color tintColor;

	public StatType Stat
	{
		get
		{
			return this.stat;
		}
	}

	private void SetupPrefab(string PrefabToLoad)
	{
		this.StatTypePrefab.PrefabToCreate = (Resources.Load(PrefabToLoad) as GameObject);
	}

	public void SetStat(StatType inStat)
	{
		this.info = MultiplayerProfileScreen.Info;
		StatsPlayerInfoComponent component = this.info.GetComponent<StatsPlayerInfoComponent>();
		RTWPlayerInfoComponent component2 = this.info.GetComponent<RTWPlayerInfoComponent>();
		this.stat = inStat;
		switch (this.stat)
		{
		case StatType.WorldRank:
		{
			int worldRank = component2.WorldRank;
			this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_WORLD_RANK").ToUpper();
			this.SetupPrefab("Multiplayer/WorldIcon");
            this.StatValueText.text = ((worldRank <= 0) ? this.GetNotApplicableTranslatedString() : ("#" + worldRank.ToString("#,##0")));
			break;
		}
		case StatType.Onlinewinlossratio:
        this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_WIN_LOSE_RATIO").ToUpper();
			this.SetupPrefab("Multiplayer/WinLoss");
			this.WinColor = new Color(0.36f, 0.81f, 1f, 1f);
			this.LossColor = new Color(0.89f, 0.25f, 0.25f, 1f);
            this.StatValueText.text = string.Concat(new object[]
			{
				this.WinColor,
				component.TotalOnlineRacesWon.ToString("#,##0"),
				Color.white,
				" / ",
				this.LossColor,
				component.TotalOnlineRacesLost.ToString("#,##0")
			});
			break;
		case StatType.HalfMileBestTime:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_HALF_MILE_BEST_NEW").ToUpper();
			this.SetupPrefab("Multiplayer/HalfMileIcon");
            this.StatValueText.text = ((component.HalfMile <= 0f) ? this.GetNotApplicableTranslatedString() : component.HalfMile.ToString("0.000"));
			break;
		case StatType.QuarterMileBestTime:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_QUARTER_MILE_BEST_NEW").ToUpper();
			this.SetupPrefab("Multiplayer/QuarterMileIcon");
            this.StatValueText.text = ((component.QuarterMile <= 0f) ? this.GetNotApplicableTranslatedString() : component.QuarterMile.ToString("0.000"));
			break;
		case StatType.TotalDistance:
		{
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_TOTAL_DIST").ToUpper();
			this.SetupPrefab("Multiplayer/TotalDistanceIcon");
			float num = component.TotalDistanceTravelled / 1000f;
			float num2 = num / 1.6f;
            this.StatValueText.text = num2.ToString("0.00");
			break;
		}
		case StatType.TotalTime:
		{
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_TOTAL_TIME").ToUpper();
			this.SetupPrefab("Multiplayer/TotalTimeIcon");
			int totalPlayTime = component.TotalPlayTime;
			int num3 = totalPlayTime / 60;
			int num4 = totalPlayTime - num3 * 60;
            this.StatValueText.text = string.Format(LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_TIME_PLAYED"), num3.ToString(), num4.ToString());
			break;
		}
		case StatType.Level:
        this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_UI_XP_LEVEL").ToUpper();
			this.SetupPrefab("Multiplayer/Level");
            this.StatValueText.text = component.Level.ToString();
			break;
		case StatType.TotalCashEver:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_TOTAL_CASH").ToUpper();
			this.SetupPrefab("Multiplayer/CashIcon");
            this.StatValueText.text = component.TotalCash.ToString("#,##0");
			break;
		case StatType.TotalGoldEver:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_TOTAL_CREDS").ToUpper();
			this.SetupPrefab("Multiplayer/GoldIcon");
            this.StatValueText.text = component.TotalGold.ToString("#,##0");
			break;
		case StatType.Numberofcarsowned:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_CARS_OWNED").ToUpper();
			this.SetupPrefab("Multiplayer/CarIcon");
            this.StatValueText.text = component.CarsOwned.ToString("#,##0");
			break;
		case StatType.TotalGaragePP:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_GARAGE_PP");
			this.SetupPrefab("Multiplayer/GarageIcon");
            this.StatValueText.text = component.TotalGaragePP.ToString();
			break;
		case StatType.TotalStarCount:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_STARS");
			this.SetupPrefab("Multiplayer/StarCountIcon");
            this.StatValueText.text = ((component.StarCount <= 0) ? this.GetNotApplicableTranslatedString() : component.StarCount.ToString());
			break;
		case StatType.BossesBeaten:
            this.StatNameText.text = LocalizationManager.GetTranslation("TEXT_STATS_SCREEN_BOSSES_BEATEN");
			this.SetupPrefab("Multiplayer/Boss");
            this.StatValueText.text = string.Format("{0} / {1}", component.BossesBeaten.ToString(), GameDatabase.Instance.SocialConfiguration.BossEventsIDs.Count);
			break;
		case StatType.Empty:
            this.StatNameText.text = string.Empty;
            this.StatValueText.text = string.Empty;
			this.SetBgComponentsTransparency(0f);
			break;
		}
	}

	private string GetNotApplicableTranslatedString()
	{
		return LocalizationManager.GetTranslation("TEXT_NOT_APPLICABLE_ABBREVIATION");
	}

	public float GetBoxWidth()
	{
	    //return this.boxLeft.width + this.boxMid.width + this.boxRight.width;
	    return 0;
	}

    public void ResizeToWidth(float width)
	{
		if (width != this.GetBoxWidth())
		{
            //float num = this.boxMid.width + this.boxRight.width * 0.5f;
            //this.boxMid.width = width - (this.boxLeft.width + this.boxRight.width);
            //Vector3 localPosition = this.boxMid.transform.localPosition;
            //localPosition.x += this.boxMid.width;
            //this.boxRight.transform.localPosition = localPosition;
            //this.StatNameText.maxWidth = this.boxMid.width + this.boxRight.width * 0.9f;
            //this.StatValueText.maxWidth = this.boxMid.width + this.boxRight.width;
		}
	}

	private void SetBgComponentsTransparency(float alpha)
	{
        //Color color = new Color(1f, 1f, 1f, alpha);
        //this.boxLeft.renderer.material.SetColor("_Tint", color);
        //this.boxMid.renderer.material.SetColor("_Tint", color);
        //this.boxRight.renderer.material.SetColor("_Tint", color);
	}

	public void SetTint(Color color)
	{
		this.tintColor = color;
		IEnumerable<Material> enumerable = this.ObjectsToTint.SelectMany((Renderer r) => r.materials);
		foreach (Material current in enumerable)
		{
			current.SetColor("_Tint", color);
		}
		this.waitingToTintIcon = true;
	}

	private void Update()
	{
		if (this.waitingToTintIcon)
		{
            //GameObject gameObject = this.StatTypePrefab.GetGameObject();
            //if (gameObject != null)
            //{
            //    gameObject.renderer.material.SetColor("_Tint", this.tintColor);
            //}
		}
	}
}
