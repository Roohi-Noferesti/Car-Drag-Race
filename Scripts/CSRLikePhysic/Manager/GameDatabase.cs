using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;

public class GameDatabase
{
    private enum GameDatabaseInitialisationMode
    {
        INGAME,
        EDITOR
    }

    private static GameDatabase _instance;

    public static GameDatabaseReadyDelegate GameDatabaseReadyEvent;

    private List<ConfigurationAssetLoader> databases = new List<ConfigurationAssetLoader>();

    public static GameDatabase Instance
    {
        get { return _instance; }
    }

    public CareerDatabase Career { get; private set; }

    public AdDatabase Ad { get; private set; }

    public LegalDatabase Legal
    {
        get;
        private set;
    }

    public AIPlayersDatabase AIPlayers { get; private set; }

    public RPBonusDatabase RPBonus
    {
        get;
        private set;
    }

    public CarsDatabase Cars { get; private set; }

    public CurrenciesDatabase Currencies { get; private set; }

    public DifficultyDatabase Difficulty { get; private set; }

    public NamesDatabase Names { get; private set; }

    public OnlineDatabase Online { get; private set; }

    public SocialDatabase Social
    {
        get;
        private set;
    }

    public XPEventsDatabase XPEvents { get; private set; }

    public StarDatabase StarDatabase { get; private set; }

    public FriendsDatabase Friends
    {
        get;
        private set;
    }

    public RevenueTrackingDatabase RevenueTracking { get; private set; }

    public PricesDatabase Prices { get; private set; }

    public IAPDatabase IAPs { get; private set; }

    public TutorialDatabase Tutorial { get; private set; }

    public CarePackagesDatabase CarePackages
    {
        get;
        private set;
    }

    public DealDatabase Deal { get; private set; }

    public DailyBattleRewardDatabase DailyBattle { get; private set; }

    public RelayDatabase Relay { get; private set; }

    public TierColourDatabase Colours { get; private set; }

    public MultiplayerEventsDatabase MultiplayerEvents
    {
        get;
        private set;
    }

    public ProgressionPopupsDatabase ProgressionPopups { get; private set; }

    public ProgressionMapTextsDatabase ProgressionMapTexts
    {
        get;
        private set;
    }

    public ProgressionMapPinsDatabase ProgressionMapPins { get; private set; }

    public TutorialBubblesDatabase TutorialBubbles { get; private set; }

    public FlowConditionsDatabase FlowConditions { get; private set; }

    public ObjectiveDatabase Objective { get; private set; }

    public SeasonEventDatabase SeasonEvents
    {
        get;
        private set;
    }

    public SeasonPrizeDatabase SeasonPrizes
    {
        get;
        private set;
    }

    public BundleOffersDatabase BundleOffers
    {
        get;
        private set;
    }

    public OfferPackDatabase OfferPacks
    {
        get;
        private set;
    }

    public MiniStoreDatabase MiniStore { get; private set; }

    public BadgesDatabase Badges
    {
        get;
        private set;
    }


    public Z2HDifficultyDatabase Z2HDifficultyDatabase { get; private set; }

# if UNITY_EDITOR
    public EventDebugDatabase DebugDatabase { get; private set; }
#endif

    public CareerConfiguration CareerConfiguration
    {
        get { return this.Career.Configuration; }
    }


    public AdConfiguration AdConfiguration
    {
        get { return this.Ad.Configuration; }
    }

    public LegalConfiguration LegalConfiguration
    {
        get
        {
            return this.Legal.Configuration;
        }
    }

    public AIPlayersConfiguration AIPlayersConfiguration
    {
        get { return this.AIPlayers.Configuration; }
    }

    public RPBonusConfiguration RPBonusConfiguration
    {
        get
        {
            return this.RPBonus.Configuration;
        }
    }

    public CarsConfiguration CarsConfiguration
    {
        get { return this.Cars.Configuration; }
    }

    public CurrenciesConfiguration CurrenciesConfiguration
    {
        get { return this.Currencies.Configuration; }
    }

    public DifficultyConfiguration DifficultyConfiguration
    {
        get { return this.Difficulty.Configuration; }
    }

    public NamesConfiguration NamesConfiguration
    {
        get { return this.Names.Configuration; }
    }

    public OnlineConfiguration OnlineConfiguration
    {
        get { return this.Online.Configuration; }
    }

    public SocialConfiguration SocialConfiguration
    {
        get
        {
            return this.Social.Configuration;
        }
    }

    public XPEventsConfiguration XPEventsConfiguration
    {
        get { return this.XPEvents.Configuration; }
    }

    public StarConfiguration StarConfiguration
    {
        get { return this.StarDatabase.Configuration; }
    }

    public FriendsConfiguration FriendsConfiguration
    {
        get
        {
            return this.Friends.Configuration;
        }
    }

    public RevenueTrackingConfiguration RevenueTrackingConfiguration
    {
        get
        {
            return this.RevenueTracking.Configuration;
        }
    }

    public PricesConfiguration PricesConfiguration
    {
        get { return this.Prices.Configuration; }
    }

    public IAPConfiguration IAPConfiguration
    {
        get { return this.IAPs.Configuration; }
    }

    public TutorialConfiguration TutorialConfiguration
    {
        get { return this.Tutorial.Configuration; }
    }

    public CarePackagesConfiguration CarePackagesConfiguration
    {
        get
        {
            return this.CarePackages.Configuration;
        }
    }

    public DealConfiguration DealConfiguration
    {
        get { return this.Deal.Configuration; }
    }

    public ProgressionPopupsConfiguration ProgressionPopupsConfiguration
    {
        get { return this.ProgressionPopups.Configuration; }
    }

    public ProgressionMapTextsConfiguration ProgressionMapTextsConfiguration
    {
        get
        {
            return this.ProgressionMapTexts.Configuration;
        }
    }

    public ProgressionMapPinsConfiguration ProgressionMapPinsConfiguration
    {
        get { return this.ProgressionMapPins.Configuration; }
    }

    public TutorialBubblesConfiguration TutorialBubblesConfiguration
    {
        get { return this.TutorialBubbles.Configuration; }
    }

    public FlowConditionsConfiguration FlowConditionsConfiguration
    {
        get { return this.FlowConditions.Configuration; }
    }

    public BundleOffersPopUpConfiguration IAPBundlesConfiguration
    {
        get
        {
            return this.BundleOffers.Configuration;
        }
    }

    public RelayConfiguration RelayConfiguration
    {
        get { return this.Relay.Configuration; }
    }

    public OfferPackConfiguration OfferPackConfiguration
    {
        get
        {
            return this.OfferPacks.Configuration;
        }
    }

    public MultiplayerEventsConfiguration MultiplayerEventsConfiguration
    {
        get
        {
            return this.MultiplayerEvents.Configuration;
        }
    }

    public Z2HDifficultyConfiguration Z2HDifficultyConfiguration
    {
        get { return Z2HDifficultyDatabase.Configuration; }
    }

    public ObjectiveConfiguration ObjectiveConfiguration
    {
        get { return Objective.Configuration; }
    }

#if UNITY_EDITOR
    public EventDebugConfiguration EventDebugConfiguration
    {
        get { return DebugDatabase.Configuration; }
    }
#endif


    public GameDatabase()
    {
        this.Career = new CareerDatabase();
        this.Ad = new AdDatabase();
        this.Legal = new LegalDatabase();
        this.AIPlayers = new AIPlayersDatabase();
        this.RPBonus = new RPBonusDatabase();
        this.Cars = new CarsDatabase();
        this.Currencies = new CurrenciesDatabase();
        this.Difficulty = new DifficultyDatabase();
        this.Names = new NamesDatabase();
        this.Online = new OnlineDatabase();
        this.Social = new SocialDatabase();
        this.XPEvents = new XPEventsDatabase();
        this.StarDatabase = new StarDatabase();
        this.Friends = new FriendsDatabase();
        this.RevenueTracking = new RevenueTrackingDatabase();
        this.Prices = new PricesDatabase();
        this.IAPs = new IAPDatabase();
        this.Tutorial = new TutorialDatabase();
        this.CarePackages = new CarePackagesDatabase();
        this.Deal = new DealDatabase();
        this.DailyBattle = new DailyBattleRewardDatabase();
        this.ProgressionPopups = new ProgressionPopupsDatabase();
        this.ProgressionMapTexts = new ProgressionMapTextsDatabase();
        this.ProgressionMapPins = new ProgressionMapPinsDatabase();
        this.TutorialBubbles = new TutorialBubblesDatabase();
        this.FlowConditions = new FlowConditionsDatabase();
        this.BundleOffers = new BundleOffersDatabase();
        this.Relay = new RelayDatabase();
        this.Colours = new TierColourDatabase();
        this.OfferPacks = new OfferPackDatabase();
        this.MiniStore = new MiniStoreDatabase();
        this.Badges = new BadgesDatabase();
        this.MultiplayerEvents = new MultiplayerEventsDatabase();
        this.SeasonEvents = new SeasonEventDatabase();
        this.SeasonPrizes = new SeasonPrizeDatabase();
        this.Z2HDifficultyDatabase = new Z2HDifficultyDatabase();
        this.Objective = new ObjectiveDatabase();
# if UNITY_EDITOR
        this.DebugDatabase = new EventDebugDatabase();
#endif
        this.databases.Add(this.Career);
        this.databases.Add(this.Ad);
        this.databases.Add(this.Legal);
        this.databases.Add(this.AIPlayers);
        this.databases.Add(this.RPBonus);
        this.databases.Add(this.Cars);
        this.databases.Add(this.Currencies);
        this.databases.Add(this.Difficulty);
        this.databases.Add(this.Names);
        this.databases.Add(this.Online);
        this.databases.Add(this.Social);
        this.databases.Add(this.XPEvents);
        this.databases.Add(this.StarDatabase);
        this.databases.Add(this.Friends);
        this.databases.Add(this.RevenueTracking);
        this.databases.Add(this.Prices);
        this.databases.Add(this.IAPs);
        this.databases.Add(this.Tutorial);
        this.databases.Add(this.CarePackages);
        this.databases.Add(this.Deal);
        this.databases.Add(this.DailyBattle);
        this.databases.Add(this.Relay);
        this.databases.Add(this.Colours);
        this.databases.Add(this.MultiplayerEvents);
        this.databases.Add(this.ProgressionPopups);
        this.databases.Add(this.ProgressionMapTexts);
        this.databases.Add(this.ProgressionMapPins);
        this.databases.Add(this.TutorialBubbles);
        this.databases.Add(this.FlowConditions);
        this.databases.Add(this.SeasonEvents);
        this.databases.Add(this.SeasonPrizes);
        this.databases.Add(this.BundleOffers);
        this.databases.Add(this.OfferPacks);
        this.databases.Add(this.MiniStore);
        this.databases.Add(this.Badges);
        this.databases.Add(this.Z2HDifficultyDatabase);
        this.databases.Add(this.Objective);
# if UNITY_EDITOR
        this.databases.Add(this.DebugDatabase);
#endif
    }

    public static void Create()
    {
        if (_instance == null)
        {
            _instance = new GameDatabase();
        }
    }

    public void ShutDown()
    {
        this.databases.ForEach(delegate(ConfigurationAssetLoader x)
        {
            x.Shutdown();
        });
    }

    public void Initialise()
    {
        this.ReloadConfigurationDatabases(GameDatabaseInitialisationMode.INGAME);
    }

    private void ReloadConfigurationDatabases(GameDatabaseInitialisationMode mode)
    {
        if (this.IsReady())
        {
            this.ShutDown();
        }
        if (mode == GameDatabaseInitialisationMode.INGAME)
        {
            this.databases.ForEach(delegate(ConfigurationAssetLoader x)
            {
                x.Initialise();
            });
        }
        else if (mode == GameDatabaseInitialisationMode.EDITOR)
        {
        }
        this.CallReadyEvent();
    }

    public bool IsReady()
    {
        return this.databases.TrueForAll((ConfigurationAssetLoader x) => x.IsValid);
    }

    public string WhichConfigurationDatabaseIsNotReady()
    {
        foreach (ConfigurationAssetLoader current in this.databases)
        {
            if (!current.IsValid)
            {
                return current.AssetID + "Database";
            }
        }
        return null;
    }

    public static void AddDatabaseReadyListener(GameDatabaseReadyDelegate listener)
    {
        GameDatabaseReadyEvent = (GameDatabaseReadyDelegate) Delegate.Combine(GameDatabaseReadyEvent, listener);
    }

    public static void RemoveDatabaseReadyListener(GameDatabaseReadyDelegate listener)
    {
        GameDatabaseReadyEvent = (GameDatabaseReadyDelegate) Delegate.Remove(GameDatabaseReadyEvent, listener);
    }

    private void CallReadyEvent()
    {
        if (GameDatabaseReadyEvent != null)
        {
            GameDatabaseReadyEvent();
        }
    }

    [Conditional("GT_DEBUG_LOGGING")]
    private void Validate(ConfigurationAssetLoader loader)
    {
        if (!loader.IsValid)
        {
        }
    }

#if UNITY_EDITOR
    private List<ScriptableObject> _list;

    public void Load()
    {
        DirectoryInfo directory = new DirectoryInfo(Application.dataPath + "/../" + "Assets/configuration/");
        directory.GetFiles("asset", SearchOption.AllDirectories);
        AssetDatabase.GetSubFolders("Assets/configuration/");
    }
#endif
}
