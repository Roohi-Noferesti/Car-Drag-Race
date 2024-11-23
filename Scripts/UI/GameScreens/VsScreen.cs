using System;
using System.Collections;
using System.Linq;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class VsScreen : ZHUDScreen
{
    [SerializeField] private VsScreenPlayerUI m_playerUi;
    [SerializeField] private VsScreenPlayerUI m_opponentUI;
    [SerializeField] private int m_opponentStarThreshold = 100;

    //private HudScreenEventArgs m_screenEvent;
    private Texture m_profileTexture;
    private int m_tryLogRaceCount;
    private Texture m_originalBackground;
    private bool m_wait;

    [SerializeField]
    private TextMeshProUGUI m_difficultyLabel;

    [SerializeField] private RawImage m_background;
    [SerializeField] private Texture m_crewTexture;

    public override ScreenID ID
    {
        get { return ScreenID.VS; }
    }

    protected override void Awake()
    {
        base.Awake();
        if (m_profileTexture == null)
        {
            m_profileTexture = m_opponentUI.PlayerImage;
            m_originalBackground = m_background.texture;
        }
    }

    public void ButtonStart()
    {
        SceneManagerFrontend.ButtonStart();
    }

    public override bool Wait(bool startingUp)
    {
        if (startingUp)
        {
            return m_wait;
        }
        return false;
    }

    private void SetActiveObjects(bool active)
    {
        foreach (Transform t in transform)
        {
            if (t.name != "BG" && t.name!= "Mechanic_Button" && t.name!= "Next_Button")
            {
                t.gameObject.SetActive(active);
            }
        }
    }

    private void OnlineRaceGameEvents_LogRaceInitiateCompleted(long arg1, string arg2, string imageurl)
    {
        //LogUtility.Log("Log race inited.Opponent : " + arg1 + ":" + arg2);

        //if (!RaceEventInfo.Instance.CurrentEvent.IsCrewBattle())
        //{
        //    m_opponentUI.PlayerName = arg2;
        //    RaceEventInfo.Instance.OpponentName = arg2;

        //    if (!RaceEventInfo.Instance.IsCrewRaceEvent)
        //    {
        //        RaceEventInfo.Instance.OpponentImageUrl = imageurl;
        //    }
        //    if (m_screenEvent != null)
        //    {
        //        m_screenEvent.Wait = false;
        //    }
        //    RefershUI(false);
        //}
        //else
        //{
        //    var playerStar = PlayerProfileManager.Instance.ActiveProfile.PlayerStar;
        //    m_opponentUI.Star = GetRandomStarForOpponent(playerStar);
        //}
    }

    public override void OnCreated(bool zAlreadyOnStack)
    {
        //visibility changing
        base.OnCreated(zAlreadyOnStack);
        m_wait = true;
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        PreRaceCarGarageSetup.Instance.Setup(activeProfile.DisplayName, RaceEventInfo.Instance.GetRivalName());
        CoroutineManager.Instance.StartCoroutine(WaitForCarsToBeSetup());
        SetActiveObjects(false);

    }

    public override void OnAfterActivate()
    {
        base.OnAfterActivate();
        //visibility changed
        SetActiveObjects(true);
        Animator.Play(OpenAnimationName);
        CoroutineManager.Instance.StartCoroutine(GameCountDown());
    }

    IEnumerator WaitForCarsToBeSetup()
    {
        while (!PreRaceCarGarageSetup.Instance.IsHumanCarFullySetup || !PreRaceCarGarageSetup.Instance.IsAiCarFullySetup)
        {
            yield return new WaitForEndOfFrame();
        }

        SetupAvatar();
    }

    private void SetupAvatar()
    {
        if (RaceEventInfo.Instance.CurrentEvent.IsWorldTourRace())
        {
            var eventData = RaceEventInfo.Instance.CurrentEvent;
            var pin = eventData.GetWorldTourPinPinDetail();
            string eventPaneBossSprite = pin.GetEventPaneBossSprite();
            if (!string.IsNullOrEmpty(eventPaneBossSprite))
            {
                Texture2D crewMemberEventPaneTexture =
                    TierXManager.Instance.GetCrewMemberEventPaneTexture(eventPaneBossSprite);
                m_opponentUI.PlayerImage = crewMemberEventPaneTexture;
            }
            m_wait = false;
            RefershUI();

            //if (!string.IsNullOrEmpty(pin.TemplateName))
            //{
            //    Texture2D sprite;
            //    if (string.IsNullOrEmpty(pin.GetEventPaneBossSprite()))
            //    {
            //        sprite = null;
            //    }
            //    else
            //    {
            //        string bossSprite = pin.GetEventPaneBossSprite();
            //        sprite = TierXManager.Instance.GetBossTexture(bossSprite);
            //    }

            //    m_opponentUI.PlayerImage = sprite;
            //    m_wait = false;
            //    RefershUI();
            //}

        }
        else if (RaceEventInfo.Instance.CurrentEvent.IsCrewBattle())
        {
            m_background.texture = m_crewTexture;
            //Debug.Log("The race is crew Battle " + Time.time);
            var currentEvent = RaceEventInfo.Instance.CurrentEvent;
            var zTier = RaceEventInfo.Instance.CurrentEventTier;
            string tierNumber = CarTierHelper.TierToString[(int)zTier];
            if (currentEvent.IsCrewRace())
            {
                int member = currentEvent.GetProgressionRaceEventNumber() + 1;
                if (member == 4)
                {
                    member = 3;
                }
                //var opponentName = LocalizationManager.GetTranslation(CrewChatter.GetMemberName((int) zTier, member));
                //RaceEventInfo.Instance.OpponentName = opponentName;
                var crewMemberPath = string.Concat("CrewPortraitsTier", tierNumber, ".Crew ",
                    member," Tumb") + (zTier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":"");
                TexturePack.RequestTextureFromBundle(crewMemberPath, (tex) =>
                {
                    m_opponentUI.PlayerImage = tex;
                    m_wait = false;
                    RefershUI();
                });
            }
            else
            {
                //var opponentName = LocalizationManager.GetTranslation(CrewChatter.GetLeaderName((int) zTier));
                //RaceEventInfo.Instance.OpponentName = opponentName;
                TexturePack.RequestTextureFromBundle("CrewPortraitsTier" + tierNumber + ".Boss Card Tumb"  + (zTier==eCarTier.TIER_2 && BasePlatform.ActivePlatform.ShouldShowFemaleHair? "_World":""), (tex) =>
                {
                    m_opponentUI.PlayerImage = tex;
                    m_wait = false;
                    RefershUI();
                });
            }
        }
        else
        {
            //m_opponentUI.PlayerImage = m_profileTexture;
            SetupRandomAvatar();

            if (m_originalBackground != null)
            {
                m_background.texture = m_originalBackground;
            }
        }
    }

    private void SetupRandomAvatar()
    {
        var randomAvatar = AvatarIDs.IDs[Random.Range(0, AvatarIDs.IDs.Count())];
        ResourceManager.LoadAssetAsync<Texture2D>(randomAvatar, ServerItemBase.AssetType.avatar, true, tex =>
        {
            m_opponentUI.PlayerImage = tex;
            m_wait = false;
            RefershUI();
        });
    }


    //private IEnumerator _testEnumerator()
    //{
    //    yield return new WaitForSecondsRealtime(1);
    //    m_screenEvent.Wait = false;
    //    RefershUI(true);
    //}

    private IEnumerator GameCountDown()
    {
        //var penaltyCost = GameDatabase.Instance.Currencies.GetFuelCostForEvent(RaceEventInfo.Instance.CurrentEvent);
        //FuelManager.Instance.SpendFuel(penaltyCost);
        yield return new WaitForSeconds(3);
        ButtonStart();
    }

    private void RefershUI()
    {
        var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
        m_playerUi.PlayerName = !string.IsNullOrEmpty(activeProfile.DisplayName)? activeProfile.DisplayName : "DefaultPlayer";
        //ImageUtility.LoadImage(PlayerProfileManager.Instance.ActiveProfile.ImageUrl, UserImageSize.Size_100,
        //    (res, tex) =>
        //    {
        //        if (res)
        //            m_playerUi.PlayerImage = tex;
        //    });

        var playerAvatarID = string.IsNullOrEmpty(activeProfile.AvatarID) ? "avatar_1" : activeProfile.AvatarID;
        ResourceManager.LoadAssetAsync<Texture2D>(playerAvatarID, ServerItemBase.AssetType.avatar, true, tex =>
        {
            m_playerUi.PlayerImage = tex;
        });

        m_playerUi.Star = activeProfile.PlayerStar;
        var playerCarName = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.LocalPlayerCarDBKey).ShortName;
        m_playerUi.CarName = LocalizationManager.GetTranslation(playerCarName);
        //if (RaceEventInfo.Instance.CurrentEvent.IsSMPRaceEvent())
        //{
        //    m_playerUi.CarRateText = "?";
        //}
        //else
        //{
            m_playerUi.CarRate = RaceEventInfo.Instance.HumanCarGarageInstance.CurrentPPIndex;
        //}
        m_playerUi.CarImage = PreRaceCarGarageSetup.Instance.HumanSnapShot;


        m_opponentUI.PlayerName = RaceEventInfo.Instance.GetRivalName();//RaceEventInfo.Instance.IsCrewRaceEvent || RaceEventInfo.Instance.IsWorldTourRaceHasOverrideDriver()
            //?RaceEventInfo.Instance.GetRivalName():LocalizationManager.FixRTL_IfNeeded(RaceEventInfo.Instance.GetRivalName());//RaceEventInfo.Instance.IsCrewRaceEvent ||
            //                      string.IsNullOrEmpty(RaceEventInfo.Instance.OpponentName)
            //? RaceEventInfo.Instance.OpponentName
            //: LocalizationManager.FixRTL_IfNeeded(RaceEventInfo.Instance.OpponentName);

        //if (!RaceEventInfo.Instance.IsCrewRaceEvent)// && !string.IsNullOrEmpty(RaceEventInfo.Instance.OpponentImageUrl))
        //{
        //    //ImageUtility.LoadImage(RaceEventInfo.Instance.OpponentImageUrl, UserImageSize.Size_100, (res, tex) =>
        //    //{
        //    //    if (res)
        //    //        m_opponentUI.PlayerImage = tex;
        //    //});

        //    SetupRandomAvatar();
        //}
        if (RaceEventInfo.Instance.CurrentEvent.IsCrewBattle())
            m_opponentUI.Star = Random.Range(1000, 15000);
        else
        {
            var playerStar = PlayerProfileManager.Instance.ActiveProfile.PlayerStar;
            m_opponentUI.Star = GetRandomStarForOpponent(playerStar);
        }
        var currentEvent = RaceEventInfo.Instance.CurrentEvent;
        var opponentCarName = CarDatabase.Instance.GetCar(RaceEventInfo.Instance.OpponentCarDBKey).ShortName;
        m_opponentUI.CarName = LocalizationManager.GetTranslation(opponentCarName);
        //if (currentEvent.IsSMPRaceEvent())
            m_opponentUI.CarRateText = "?";
        //else
        //{
        //    m_opponentUI.CarRate = currentEvent.GetAIPerformancePotentialIndex();
        //}
        m_opponentUI.CarImage = PreRaceCarGarageSetup.Instance.AiSnapShotReverse;

        var difficultyValue = RaceEventDifficulty.Instance.GetRating(RaceEventInfo.Instance.CurrentEvent);
        //var difficultyValue = (RaceEventDifficulty.Rating)RaceEventInfo.Instance.CurrentEvent.GetRaceGroupIndex();
        if (m_difficultyLabel!=null)
            m_difficultyLabel.text = LocalizationManager.GetTranslation(difficultyValue.ToString());
    }

    private int GetRandomStarForOpponent(int playerStar)
    {
        var minStar = playerStar < 300 ? playerStar : playerStar - m_opponentStarThreshold;
        return Random.Range(minStar, playerStar + m_opponentStarThreshold);
    }
}

[Serializable]
public class VsScreenPlayerUI
{
    [SerializeField] private RawImage m_playerImage;
    [SerializeField] private TextMeshProUGUI m_playerName;
    [SerializeField] private TextMeshProUGUI m_playerStar;
    [SerializeField] private RawImage m_carImage;
    [SerializeField] private TextMeshProUGUI m_carName;
    [SerializeField] private TextMeshProUGUI m_carRate;

    public Texture PlayerImage
    {
        get { return m_playerImage.texture; }
        set { m_playerImage.texture = value; }
    }

    public string PlayerName
    {
        set { m_playerName.text = value; }
    }

    public int Star
    {
        set { m_playerStar.text = value.ToString("##,##0").ToNativeNumber(); }
    }

    public Texture2D CarImage
    {
        set { m_carImage.texture = value; }
    }

    public string CarName
    {
        set { m_carName.text = value; }
    }

    public int CarRate
    {
        set { m_carRate.text = value.ToNativeNumber(); }
    }

    public string CarRateText
    {
        set { m_carRate.text = value; }
    }

}
