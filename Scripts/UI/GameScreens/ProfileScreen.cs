using System.Collections;
using System.Globalization;
using System.IO;
using System.Linq;
using Fabric;
using I2.Loc;
using KingKodeStudio;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProfileScreen : ZHUDScreen
{
    private string ProfileSnapshotPath;
    private ProfileShare ProfileSharer;
    [SerializeField] private TextMeshProUGUI m_nameText;
    [SerializeField] private RawImage m_image;
    [SerializeField] private TextMeshProUGUI m_startText;
    [SerializeField] private TextMeshProUGUI m_rankText;
    [SerializeField] private TextMeshProUGUI m_levelText;
    [SerializeField] private TextMeshProUGUI m_winCountText;
    [SerializeField] private TextMeshProUGUI m_loseCountText;
    [SerializeField] private TextMeshProUGUI m_totalDistanceText;
    [SerializeField] private TextMeshProUGUI m_goldSpentText;
    [SerializeField] private TextMeshProUGUI m_cashSpentText;
    [SerializeField] private TextMeshProUGUI m_bestQuarterTimeText;
    [SerializeField] private TextMeshProUGUI m_bestQuarterTimeTextLabel;
    [SerializeField] private TextMeshProUGUI m_bestHalfTimeText;
    [SerializeField] private TextMeshProUGUI m_bestHalfTimeTextLabel;
    [SerializeField] private TextMeshProUGUI m_totalPlayTimeText;
    [SerializeField] private TextMeshProUGUI m_totalGarageTimeText;
    [SerializeField] private ProfileCarScrollerController m_scroller;

    [SerializeField] private RuntimeButton m_modifyButton;
    [SerializeField] private RuntimeButton m_shareButton;
    //private Texture m_defaultPlayerTexture;

    private static PlayerProfileData m_user;

    public override ScreenID ID
    {
        get { return ScreenID.Profile; }
    }


    protected override void Awake()
    {
        base.Awake();
        //m_defaultPlayerTexture = m_image.texture;
        m_modifyButton.AddValueChangedDelegate(OnModifyButton);

        this.ProfileSharer = gameObject.GetComponent<ProfileShare>();
        this.ProfileSnapshotPath = Path.Combine(Application.persistentDataPath, "ProfileShare.png");
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        base.OnCreated(zAlreadyOnStack);
        RefreshUI();
    }

    public static void SetUser(PlayerProfileData user)
    {
        m_user = user;
    }

    private void RefreshUI()
    {
        var ids = m_user.CarsOwned.Cast<ICarSimpleSpec>().OrderBy(c => c.PPIndex).ToArray();
        var userName = m_user.DisplayName;//LocalizationManager.FixRTL_IfNeeded(m_user.DisplayName);
        m_nameText.text = userName;
        m_levelText.text = GameDatabase.Instance.XPEvents.CurrentLevelForXP(m_user.PlayerXP).ToString();
        m_startText.text = m_user.PlayerStar.ToString();
        //var car = CarDatabase.Instance.GetCar(m_user.CurrentlySelectedCarDBKey);
        var leagueIndexForPlayerStar = (int) GameDatabase.Instance.StarDatabase.CurrentLeagueForStar(m_user.PlayerStar);
        m_rankText.text =
            GameDatabase.Instance.StarDatabase.GetLeagueLocalizationName(
                (LeagueData.LeagueName) leagueIndexForPlayerStar);
        m_winCountText.text = m_user.RacesWon.ToString();
        m_loseCountText.text = m_user.RacesLost.ToString();
        RefreshTimePlaying(m_user.TotalPlayTime);
        RefreshTimeInGarage(m_user.TotalGarageTime);
        RefreshDistanceTravelled(m_user.TotalDistanceTravelled);
        RefreshBestHalfMileTime(m_user.BestOverallHalfMileTime);
        RefreshBestQuarterMile(m_user.BestOverallQuarterMileTime);
        if (m_goldSpentText != null)
            m_goldSpentText.text = CurrencyUtils.GetGoldStringWithIcon(m_user.GoldSpent);
        if (m_cashSpentText != null)
            m_cashSpentText.text = CurrencyUtils.GetCashString(m_user.CashSpent);
        m_scroller.SetCars(ids);
        m_scroller.Reload();

        //ImageUtility.LoadImage(m_user.ImageUrl, UserImageSize.Size_50, (res, tex) =>
        //{
        //    if (res)
        //    {
        //        m_image.texture = tex;
        //    }
        //    else
        //    {
        //        m_image.texture = m_defaultPlayerTexture;
        //    }
        //});

        var avatarID = string.IsNullOrEmpty(m_user.AvatarID) ? "avatar_1" : m_user.AvatarID;
        ResourceManager.LoadAssetAsync<Texture2D>(avatarID, ServerItemBase.AssetType.avatar, true,
            tex => { m_image.texture = tex; });
        
        m_shareButton.gameObject.SetActive(BuildType.CanShowShareButton() && m_user.Username == UserManager.Instance.currentAccount.Username);
    }

    private void RefreshTimePlaying(int totalPlayTime)
    {
        var format = LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES");
        var num = totalPlayTime / 60;
        var num2 = totalPlayTime % 60;
        var zValue = string.Format(format, num, num2);
        m_totalPlayTimeText.text = zValue;
    }

    private void RefreshTimeInGarage(int totalGarageTime)
    {
        var format = LocalizationManager.GetTranslation("TEXT_UNITS_TIME_HOURS_AND_MINUTES");
        var num = totalGarageTime / 60;
        var num2 = totalGarageTime % 60;
        var zValue = string.Format(format, num, num2);
        m_totalGarageTimeText.text = zValue;
    }


    private void RefreshDistanceTravelled(float distanceTraveled)
    {
        var distanceTraveledInKm = distanceTraveled / 1000;
        string zValue;
        if (distanceTraveledInKm <= 0f)
        {
            zValue = LocalizationManager.GetTranslation("TEXT_NOT_APPLICABLE_ABBREVIATION");
        }
        else
        {
            if (PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit)
            {
                var format = LocalizationManager.GetTranslation("TEXT_UNITS_DISTANCE_IN_MILE");
                var distanceInMile = distanceTraveledInKm * 0.621371;
                zValue = string.Format(format, distanceInMile.ToString(CultureInfo.CurrentCulture));
            }
            else
            {
                var format = LocalizationManager.GetTranslation("TEXT_UNITS_DISTANCE_IN_METERS");
                zValue = string.Format(format, distanceTraveledInKm);
            }
        }

        m_totalDistanceText.text = zValue;
    }


    private void RefreshBestHalfMileTime(float bestOverallHalfMileTime)
    {
        string zValue;
        if (bestOverallHalfMileTime == 0f)
        {
            zValue = LocalizationManager.GetTranslation("TEXT_NOT_APPLICABLE_ABBREVIATION");
        }
        else
        {
            var format = LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS");
            zValue = string.Format(format, bestOverallHalfMileTime);
        }

        m_bestHalfTimeText.text = zValue;

        m_bestHalfTimeTextLabel.text = LocalizationManager.GetTranslation(
            PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit
                ? "TEXT_HALF_MILE_BEST"
                : "TEXT_HALF_MILE_BEST_IN_METER");
    }


    private void RefreshBestQuarterMile(float bestOverallQuarterMileTime)
    {
        string zValue;
        if (bestOverallQuarterMileTime == 0f)
        {
            zValue = LocalizationManager.GetTranslation("TEXT_NOT_APPLICABLE_ABBREVIATION");
        }
        else
        {
            var format = LocalizationManager.GetTranslation("TEXT_UNITS_TIME_SECONDS_WITH_MS");
            zValue = string.Format(format, bestOverallQuarterMileTime);
        }

        m_bestQuarterTimeText.text = zValue;

        m_bestQuarterTimeTextLabel.text = LocalizationManager.GetTranslation(
            PlayerProfileManager.Instance.ActiveProfile.UseMileAsUnit
                ? "TEXT_QUARTER_MILE_BEST"
                : "TEXT_QUARTER_MILE_BEST_IN_METER");
    }

    private void OnModifyButton()
    {
        if (!UserManager.Instance.isLoggedIn || !PolledNetworkState.IsNetworkConnected)
        {
            PopUpDatabase.Common.ShowNoInternetConnectionPopup();
            return;
        }

        if (m_user.Username == UserManager.Instance.currentAccount.Username)
            ScreenManager.Instance.PushScreen(ScreenID.ChooseName);
    }


    public void ShareProfile()
    {
        //this.fullScreenFlash.StartFlashAnimation();
        EventManager.Instance.PostEvent("Snapshot", EventAction.PlaySound, null, null);
        //CameraPostRender.Instance.AddProcess("Profile render profile share snapshot", delegate
        //{
        //    Texture2D texture2D = this.ProfileSharer.Render();
        //    byte[] bytes = texture2D.EncodeToPNG();
        //    File.WriteAllBytes(this.ProfileSnapshotPath, bytes);
        //    base.StartCoroutine(this.ShareProfileImage());
        //});
        ScreenCapture.CaptureScreenshot("ProfileShare.png");
        base.StartCoroutine(this.ShareProfileImage());
    }

    IEnumerator ShareProfileImage()
    {
        #if UNITY_ANDROID
            var message = LocalizationManager.GetTranslation("TEXT_TWITTER_STAT_SHARE");
            BasePlatform.ActivePlatform.ShareImage(
                string.Format(
                    message.Replace("{LINK}", SocialController.Instance.GetTwitterGameURL()),
                    BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty,
                ProfileSnapshotPath, message);

            yield return null;
        #else
            //Keyframe keyframe = fullScreenFlash.ExplosionCurve[fullScreenFlash
            //    .ExplosionCurve.length - 1];
            yield return new WaitForSeconds(1); //keyframe.time);
            //fullScreenFlash.StopFlashAnimation();
            var message = LocalizationManager.GetTranslation("TEXT_TWITTER_STAT_SHARE");
            BasePlatform.ActivePlatform.ShareImage(
                string.Format(
                    message.Replace("{LINK}", SocialController.Instance.GetTwitterGameURL()),
                    BasePlatform.ActivePlatform.GetTargetAppStoreFriendlyName()), string.Empty,
                ProfileSnapshotPath);
        #endif
        
        
    }
}
