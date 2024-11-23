using System;
using UnityEngine;

public class SMPNetworkManager : MonoBehaviour
{
    #region OLD
    /*
	public enum EP2PConnectionType
	{
		None,
		NAT,
		Proxy,
		Bot
	}

	public enum EIPConnectionType
	{
		IPv4,
		IPv6
	}

	public enum ESMPServerType
	{
		BuildAppropriate,
		ForceStaging,
		ForceProduction,
		AlternateIPv4,
		AlternateIPv6,
		AWSTestStaging,
		PNInternal
	}

	public enum ESMPServerDisconnectionType
	{
		LostConnection,
		IntentionallyDisconnected
	}

	public delegate void SMPVoidEventDelegate();

	public delegate void SMPBoolEventDelegate(bool success);

	public delegate void SMPServerDisconnectionDelegate(ESMPServerDisconnectionType disconnectionType);

	private const float SMPAutoRaceStartTime = 5f;

    public static SMPNetworkManager Instance;

	private string cachedServerIPAddress;

	public ushort natPunchthroughServerPort;

	public ushort lobbyServerPort;

	public short socketFamily = 2;

	private ESMPServerType _serverType;

	public bool tryNATFirst = true;

	private bool m_HasReceivedChallenge;

	private UDPProxyMessageHandler m_UDPProxyMessageHandler;

	private NatPunchthroughClient m_PunchthroughClientPlugin;

	private UDPProxyClient m_UDPProxyPlugin;

	private AddressOrGUID m_PunchthroughServerGUID;

	private RakPeerInterface m_PunchthroughPeer;

	private AddressOrGUID m_OpponentGUID;

	private bool m_InitiatedNATPunchthrough;

	private bool m_AttemptingToConnectToOpponent;

	public RakNetGUID m_PunchthroughLocalRakNetGUID;

	public RPCRouter rpcRouter;

	public string lastGameConnectionID;

	public PublicKey NatAndLobbyPublicKey;

	private bool m_ConnectedToServer;

	private SMPRaceControllerRPCs m_RaceControllerRPCs;

	private float RemainingAutoRaceStartTime;

	public SMPLastConnnectionInfo lastConnectionInfo = new SMPLastConnnectionInfo();

	private bool m_HasAttemptedAlternateSocketFamily;

	private bool m_AttemptAlternateSocketFamilyNextUpdate;

    public event SMPVoidEventDelegate OnConnectedToServer;

    public event SMPVoidEventDelegate OnConnectionToServerFailed;

    public event SMPServerDisconnectionDelegate OnDisconnectedFromServer;

    public event SMPVoidEventDelegate OnSMPPlayerJoined;

    public event SMPVoidEventDelegate OnSMPPlayerLeft;

    public event SMPVoidEventDelegate OnSMPPlayerDisconnected;

    public event SMPBoolEventDelegate OnJoinRemoteGame;

    public event SMPVoidEventDelegate OnSMPApplicationWillResignActive;

    public event SMPVoidEventDelegate OnSMPApplicationDidEnterBackground;

    public event SMPVoidEventDelegate OnSMPApplicationDidBecomeActive;

	public string loadBalancerAddress
	{
		get
		{
			switch (serverType)
			{
			case ESMPServerType.ForceStaging:
				return "staging.raknet.csr2.zynga.com";
			case ESMPServerType.ForceProduction:
				return "production.raknet.csr2.zynga.com";
			case ESMPServerType.AlternateIPv4:
				return "139.59.190.182";
			case ESMPServerType.AlternateIPv6:
				return "2a03:b0c0:1:a1::581:b001";
			case ESMPServerType.AWSTestStaging:
				return "54.68.62.26";
			case ESMPServerType.PNInternal:
				return "10.104.65.5";
			}
			return Endpoint.GetSMPServer();
		}
	}

	public string lobbyServerIP
	{
		get
		{
			return cachedServerIPAddress;
		}
	}

	public ESMPServerType serverType
	{
		get
		{
			return _serverType;
		}
		set
		{
			_serverType = value;
			CSRRoomsClient.Instance.Disconnect();
			DeregisterAsHost();
		}
	}

	public SMPHostInfo OpponentGameInfo
	{
		get;
		set;
	}

	public string ActiveServerAddress
	{
		get;
		private set;
	}

	public EP2PConnectionType P2PConnectionType
	{
		get;
		private set;
	}

	public RakNetGUID localRakNetGUID
	{
		get
		{
			if (SMPConfigManager.General.UseLobbyProxy)
			{
				return CSRRoomsClient.Instance.localLobbyRakNetGUID;
			}
			return m_PunchthroughLocalRakNetGUID;
		}
	}

	public bool IsConnectedToServer
	{
		get
		{
			return m_ConnectedToServer;
		}
	}

	public bool SMPYouLeftRace
	{
		get;
		set;
	}

	public bool SMPOpponentLeftRace
	{
		get;
		set;
	}

	public bool SMPYouLostFocus
	{
		get;
		set;
	}

	public bool SMPOpponentLostFocus
	{
		get;
		set;
	}

	public DateTime SMPYouLostFocusTime
	{
		get;
		set;
	}

	public DateTime SMPLastRegainedFocusTime
	{
		get;
		set;
	}

	public DateTime SMPOpponentLostFocusTime
	{
		get;
		set;
	}

	public float SMPYouTotalWithoutFocusTime
	{
		get;
		set;
	}

	public bool SMPRaceInvalidated
	{
		get;
		set;
	}

	public bool SMPRaceDiscrepancy
	{
		get;
		set;
	}

	public int SMPCurrentRaceStake
	{
		get;
		set;
	}

	public EIPConnectionType LastUsedIPProtocol
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
		m_HasReceivedChallenge = false;
		OpponentGameInfo = new SMPHostInfo();
		rpcRouter = new RPCRouter();
		byte[] remoteServerPublicKey = {
			147,
			24,
			184,
			155,
			91,
			150,
			152,
			66,
			251,
			219,
			112,
			226,
			175,
			94,
			10,
			149,
			122,
			117,
			131,
			1,
			95,
			175,
			221,
			96,
			217,
			160,
			49,
			252,
			125,
			225,
			197,
			114,
			41,
			113,
			28,
			47,
			230,
			249,
			31,
			94,
			126,
			136,
			144,
			120,
			50,
			127,
			113,
			77,
			73,
			184,
			162,
			51,
			241,
			152,
			37,
			106,
			7,
			254,
			135,
			92,
			111,
			101,
			165,
			240
		};
		NatAndLobbyPublicKey = new PublicKey();
		NatAndLobbyPublicKey.remoteServerPublicKey = remoteServerPublicKey;
		NatAndLobbyPublicKey.publicKeyMode = PublicKeyMode.PKM_USE_KNOWN_PUBLIC_KEY;
		m_ConnectedToServer = false;
		ActiveServerAddress = "0.0.0.0";
		P2PConnectionType = EP2PConnectionType.None;
		LastUsedIPProtocol = EIPConnectionType.IPv4;
		natPunchthroughServerPort = 0;
		lobbyServerPort = 0;
		cachedServerIPAddress = string.Empty;
		m_RaceControllerRPCs = gameObject.AddComponent<SMPRaceControllerRPCs>();
		m_RaceControllerRPCs.enabled = false;
		iOSEvents.ApplicationWillResignActiveEvent += new iOSEvents_Delegate(this.OnApplicationWillResignActive);
		iOSEvents.ApplicationDidBecomeActiveEvent += new iOSEvents_Delegate(this.OnApplicationDidBecomeActive);
		iOSEvents.ApplicationDidEnterBackgroundEvent += new iOSEvents_Delegate(this.OnApplicationDidEnterBackground);
		_serverType = ESMPServerType.BuildAppropriate;
		m_AttemptingToConnectToOpponent = false;
		m_HasAttemptedAlternateSocketFamily = false;
		m_AttemptAlternateSocketFamilyNextUpdate = false;
	}

	private void OnDestroy()
	{
		DeregisterAsHost();
		ShutdownRakPeer();
		iOSEvents.ApplicationWillResignActiveEvent -= new iOSEvents_Delegate(this.OnApplicationWillResignActive);
		iOSEvents.ApplicationDidBecomeActiveEvent -= new iOSEvents_Delegate(this.OnApplicationDidBecomeActive);
		iOSEvents.ApplicationDidEnterBackgroundEvent -= new iOSEvents_Delegate(this.OnApplicationDidEnterBackground);
		Instance = null;
	}

	private void StartupRakPeer()
	{
		if (m_PunchthroughPeer != null)
		{
			return;
		}
		try
		{
			m_PunchthroughPeer = RakPeerInterface.GetInstance();
		}
		catch (Exception var_0_1C)
		{
		}
		SocketDescriptor socketDescriptors = new SocketDescriptor(0, string.Empty);
		uint maxConnections = 2u;
		m_PunchthroughPeer.Startup(maxConnections, socketDescriptors, 1u);
		m_PunchthroughClientPlugin = new NatPunchthroughClient();
		m_PunchthroughPeer.AttachPlugin(m_PunchthroughClientPlugin);
		m_PunchthroughPeer.SetMaximumIncomingConnections(2);
		m_UDPProxyPlugin = new UDPProxyClient();
		m_PunchthroughPeer.AttachPlugin(m_UDPProxyPlugin);
		m_UDPProxyMessageHandler = new UDPProxyMessageHandler();
		m_UDPProxyPlugin.SetResultHandler(m_UDPProxyMessageHandler);
		m_PunchthroughLocalRakNetGUID = m_PunchthroughPeer.GetMyGUID();
		lastConnectionInfo.clientIPAddress = m_PunchthroughPeer.GetLocalIP(0u);
	}

	private void ShutdownRakPeer()
	{
		if (m_PunchthroughPeer == null)
		{
			return;
		}
		m_UDPProxyPlugin.SetResultHandler(null);
		m_UDPProxyMessageHandler = null;
		m_PunchthroughPeer.DetachPlugin(m_PunchthroughClientPlugin);
		m_PunchthroughPeer.DetachPlugin(m_UDPProxyPlugin);
		RakPeerInterface.DestroyInstance(m_PunchthroughPeer);
		m_PunchthroughPeer = null;
		m_PunchthroughClientPlugin = null;
		m_UDPProxyPlugin = null;
		m_ConnectedToServer = false;
		ActiveServerAddress = "0.0.0.0";
		P2PConnectionType = EP2PConnectionType.None;
	}

	private void OnApplicationWillResignActive()
	{
		if (OnSMPApplicationWillResignActive != null)
		{
			OnSMPApplicationWillResignActive();
		}
	}

	private void OnApplicationDidEnterBackground()
	{
		if (OnSMPApplicationDidEnterBackground != null)
		{
			OnSMPApplicationDidEnterBackground();
		}
	}

	private void OnApplicationDidBecomeActive()
	{
		if (OnSMPApplicationDidBecomeActive != null)
		{
			OnSMPApplicationDidBecomeActive();
		}
	}

	public void ResetSMPRaceVariables(bool bResetStakeValue = true)
	{
		SMPYouLeftRace = false;
		SMPOpponentLeftRace = false;
		SMPRaceInvalidated = false;
		SMPRaceDiscrepancy = false;
		SMPYouLostFocus = false;
		SMPOpponentLostFocus = false;
		SMPYouLostFocusTime = DateTime.MinValue;
		SMPLastRegainedFocusTime = DateTime.MinValue;
		SMPOpponentLostFocusTime = DateTime.MinValue;
		SMPYouTotalWithoutFocusTime = 0f;
		RemainingAutoRaceStartTime = 5f;
		if (bResetStakeValue)
		{
			SMPCurrentRaceStake = 0;
		}
	}

	public bool TriggerAutoCountdownStart()
	{
		return RemainingAutoRaceStartTime <= 0f;
	}

	public float UpdateAutoRaceStartTimer()
	{
		RemainingAutoRaceStartTime -= Time.deltaTime;
		return RemainingAutoRaceStartTime;
	}

	public void RegisterAsHost()
	{
		if (SMPConfigManager.General.UseLobbyProxy)
		{
			return;
		}
		if (m_PunchthroughServerGUID != null || m_OpponentGUID != null)
		{
			return;
		}
		if (natPunchthroughServerPort == 0)
		{
			natPunchthroughServerPort = SMPConfigManager.General.NATPortv4;
			lobbyServerPort = SMPConfigManager.General.LobbyPortv4;
		}
		m_HasAttemptedAlternateSocketFamily = false;
		m_AttemptAlternateSocketFamilyNextUpdate = false;
		socketFamily = PlayerProfileManager.Instance.ActiveProfile.SMPLastSuccessfulSocketFamily;
		StartupRakPeer();
		ConnectToPunchthroughPeer();
	}

	private void ConnectToPunchthroughPeer()
	{
		ConnectionAttemptResult connectionAttemptResult = m_PunchthroughPeer.Connect(loadBalancerAddress, natPunchthroughServerPort, string.Empty, 0, NatAndLobbyPublicKey);
		if (connectionAttemptResult != ConnectionAttemptResult.CONNECTION_ATTEMPT_STARTED)
		{
			ConnectionToServerFailed();
		}
	}

	private ConnectionAttemptResult AttemptAlternateConnection()
	{
		if (!m_HasAttemptedAlternateSocketFamily && SMPConfigManager.General.EnableAlternateSocketFamilyRetries)
		{
			ShutdownRakPeer();
			ToggleConnectionType();
			StartupRakPeer();
			m_HasAttemptedAlternateSocketFamily = true;
			return m_PunchthroughPeer.Connect(loadBalancerAddress, natPunchthroughServerPort, string.Empty, 0, NatAndLobbyPublicKey);
		}
		return ConnectionAttemptResult.CANNOT_RESOLVE_DOMAIN_NAME;
	}

	public void DeregisterAsHost()
	{
		if (m_PunchthroughPeer != null && m_PunchthroughServerGUID != null)
		{
			LeaveGame(PacketPriority.LOW_PRIORITY);
			m_PunchthroughPeer.CloseConnection(m_PunchthroughServerGUID, true);
			m_PunchthroughServerGUID = null;
			m_OpponentGUID = null;
		}
	}

	private void ToggleConnectionType()
	{
		if (socketFamily == 0)
		{
			socketFamily = 2;
		}
		else
		{
			socketFamily = 0;
		}
	}

	private void Update()
	{
		if (m_PunchthroughPeer != null)
		{
			bool flag = false;
			Packet packet = m_PunchthroughPeer.Receive();
			if (packet != null)
			{
				ushort port = packet.systemAddress.GetPort();
				if (port == natPunchthroughServerPort)
				{
					HandlePunchthroughPacket(packet, out flag);
				}
				else
				{
					HandleGamePeerPacket(packet);
				}
				m_PunchthroughPeer.DeallocatePacket(packet);
			}
			if (flag)
			{
				ShutdownRakPeer();
				StartupRakPeer();
			}
			if (m_AttemptAlternateSocketFamilyNextUpdate)
			{
				m_AttemptAlternateSocketFamilyNextUpdate = false;
				ConnectionAttemptResult connectionAttemptResult = AttemptAlternateConnection();
				if (connectionAttemptResult != ConnectionAttemptResult.CONNECTION_ATTEMPT_STARTED)
				{
					ConnectionToServerFailed();
					flag = true;
				}
			}
		}
	}

	private void HandlePunchthroughPacket(Packet p, out bool packetCausedDisconnection)
	{
		packetCausedDisconnection = false;
		DefaultMessageIDTypes defaultMessageIDTypes = (DefaultMessageIDTypes)p.data[0];
		DefaultMessageIDTypes defaultMessageIDTypes2 = defaultMessageIDTypes;
		switch (defaultMessageIDTypes2)
		{
		case DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
			cachedServerIPAddress = p.systemAddress.ToString(false);
			m_PunchthroughServerGUID = new AddressOrGUID(p.guid);
			ActiveServerAddress = p.systemAddress.ToString(false);
			if (ActiveServerAddress.Contains(":"))
			{
				LastUsedIPProtocol = EIPConnectionType.IPv6;
			}
			else
			{
				LastUsedIPProtocol = EIPConnectionType.IPv4;
			}
			lastConnectionInfo.lastActualServerIP = cachedServerIPAddress;
			ConnectedToServer();
			CSRRoomsClient.Instance.Connect();
			return;
		case DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED:
			lastConnectionInfo.lastActualServerIP = p.systemAddress.ToString(false);
			if (m_HasAttemptedAlternateSocketFamily)
			{
				ConnectionToServerFailed();
				packetCausedDisconnection = true;
			}
			else
			{
				m_AttemptAlternateSocketFamilyNextUpdate = true;
			}
			return;
		case DefaultMessageIDTypes.ID_ALREADY_CONNECTED:
		case DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION:
		        switch (defaultMessageIDTypes2)
			{
			case DefaultMessageIDTypes.ID_NAT_TARGET_NOT_CONNECTED:
			case DefaultMessageIDTypes.ID_NAT_TARGET_UNRESPONSIVE:
			case DefaultMessageIDTypes.ID_NAT_CONNECTION_TO_TARGET_LOST:
				FailedToConnectToOpponent();
				return;
			default:
				if (defaultMessageIDTypes2 != DefaultMessageIDTypes.ID_NAT_RESPOND_BOUND_ADDRESSES)
				{
					return;
				}
				return;
			}
			break;
		case DefaultMessageIDTypes.ID_NO_FREE_INCOMING_CONNECTIONS:
			lastConnectionInfo.lastActualServerIP = p.systemAddress.ToString(false);
			ConnectionToServerFailed();
			packetCausedDisconnection = true;
			return;
		case DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
			DisconnectedFromServer(ESMPServerDisconnectionType.IntentionallyDisconnected);
			packetCausedDisconnection = true;
			return;
		case DefaultMessageIDTypes.ID_CONNECTION_LOST:
			if (m_ConnectedToServer)
			{
				ZTrackMetricsHelper.LogSMPDisconnectError(false);
			}
			DisconnectedFromServer(ESMPServerDisconnectionType.LostConnection);
			packetCausedDisconnection = true;
			return;
		}
		goto IL_33;
	}

	private void HandleGamePeerPacket(Packet p)
	{
		DefaultMessageIDTypes defaultMessageIDTypes = (DefaultMessageIDTypes)p.data[0];
		DefaultMessageIDTypes defaultMessageIDTypes2 = defaultMessageIDTypes;
		switch (defaultMessageIDTypes2)
		{
		case DefaultMessageIDTypes.ID_CONNECTION_REQUEST:
		case DefaultMessageIDTypes.ID_NEW_INCOMING_CONNECTION:
			OpponentConnected(new AddressOrGUID(p));
			return;
		case DefaultMessageIDTypes.ID_REMOTE_SYSTEM_REQUIRES_PUBLIC_KEY:
		case DefaultMessageIDTypes.ID_OUR_SYSTEM_REQUIRES_SECURITY:
		case DefaultMessageIDTypes.ID_PUBLIC_KEY_MISMATCH:
		case DefaultMessageIDTypes.ID_OUT_OF_BAND_INTERNAL:
		case DefaultMessageIDTypes.ID_SND_RECEIPT_ACKED:
		case DefaultMessageIDTypes.ID_SND_RECEIPT_LOSS:
		case DefaultMessageIDTypes.ID_ALREADY_CONNECTED:
		        switch (defaultMessageIDTypes2)
			{
			case DefaultMessageIDTypes.ID_NAT_TARGET_NOT_CONNECTED:
				Crittercism.LeaveBreadcrumb("SMP: PvP Packet ID_NAT_TARGET_NOT_CONNECTED. m_InitiatedNATPunchthrough = " + m_InitiatedNATPunchthrough);
				FailedToConnectToOpponent();
				return;
			case DefaultMessageIDTypes.ID_NAT_TARGET_UNRESPONSIVE:
				Crittercism.LeaveBreadcrumb("SMP: PvP Packet ID_NAT_TARGET_UNRESPONSIVE. m_InitiatedNATPunchthrough = " + m_InitiatedNATPunchthrough);
				FailedToConnectToOpponent();
				return;
			case DefaultMessageIDTypes.ID_NAT_CONNECTION_TO_TARGET_LOST:
			        if (defaultMessageIDTypes2 != DefaultMessageIDTypes.ID_RPC_PLUGIN)
				{
					return;
				}
				rpcRouter.InvokeRPC(p);
				return;
			case DefaultMessageIDTypes.ID_NAT_ALREADY_IN_PROGRESS:
				Crittercism.LeaveBreadcrumb("SMP: PvP Packet ID_NAT_ALREADY_IN_PROGRESS. m_InitiatedNATPunchthrough = " + m_InitiatedNATPunchthrough);
				FailedToConnectToOpponent();
				return;
			case DefaultMessageIDTypes.ID_NAT_PUNCHTHROUGH_FAILED:
				if (m_InitiatedNATPunchthrough)
				{
					m_InitiatedNATPunchthrough = false;
					AttemptConnectionViaProxy();
				}
				Crittercism.LeaveBreadcrumb("SMP: PvP Packet ID_NAT_PUNCHTHROUGH_FAILED. m_InitiatedNATPunchthrough = " + m_InitiatedNATPunchthrough);
				return;
			case DefaultMessageIDTypes.ID_NAT_PUNCHTHROUGH_SUCCEEDED:
				if (m_InitiatedNATPunchthrough)
				{
					m_PunchthroughPeer.Connect(p.systemAddress.ToString(), p.systemAddress.GetPort(), string.Empty, 0);
					m_InitiatedNATPunchthrough = false;
				}
				P2PConnectionType = EP2PConnectionType.NAT;
				Crittercism.LeaveBreadcrumb("SMP: PvP Packet ID_NAT_PUNCHTHROUGH_SUCCEEDED. m_InitiatedNATPunchthrough = " + m_InitiatedNATPunchthrough);
				return;
			}
			goto IL_6D;
		case DefaultMessageIDTypes.ID_CONNECTION_REQUEST_ACCEPTED:
			SuccessfullyConnectedToOpponent(new AddressOrGUID(p.systemAddress));
			return;
		case DefaultMessageIDTypes.ID_CONNECTION_ATTEMPT_FAILED:
			FailedToConnectToOpponent();
			return;
		case DefaultMessageIDTypes.ID_NO_FREE_INCOMING_CONNECTIONS:
			FailedToConnectToOpponent();
			return;
		case DefaultMessageIDTypes.ID_DISCONNECTION_NOTIFICATION:
			OpponentDisconnected();
			return;
		case DefaultMessageIDTypes.ID_CONNECTION_LOST:
			OpponentConnectionLost();
			return;
		}
		goto IL_4C;
	}

	public void ConnectedToServer()
	{
		if (m_PunchthroughServerGUID != null && !m_ConnectedToServer)
		{
			if (OnConnectedToServer != null)
			{
				OnConnectedToServer();
			}
			m_ConnectedToServer = true;
			lastConnectionInfo.lastNatConnectionSucceded = true;
		}
	}

	public void ConnectionToServerFailed()
	{
		lastConnectionInfo.lastNatConnectionSucceded = false;
		if (OnConnectionToServerFailed != null)
		{
			OnConnectionToServerFailed();
		}
	}

	public void DisconnectedFromServer(ESMPServerDisconnectionType disconnectionType)
	{
		ActiveServerAddress = "0.0.0.0";
		if (m_ConnectedToServer)
		{
			if (OnDisconnectedFromServer != null)
			{
				OnDisconnectedFromServer(disconnectionType);
			}
			m_ConnectedToServer = false;
		}
	}

	public bool JoinGame()
	{
		if (m_OpponentGUID != null)
		{
			return false;
		}
		ZTrackMetricsHelper.LogSMPTryP2PConnection(OpponentGameInfo.IsBot);
		if (OpponentGameInfo.IsBot)
		{
			P2PConnectionType = EP2PConnectionType.Bot;
			return true;
		}
		m_AttemptingToConnectToOpponent = true;
		if (SMPConfigManager.General.UseLobbyProxy)
		{
			return true;
		}
		bool flag = SMPConfigManager.General.ForceProxyAllMobileConnections && (OpponentGameInfo.DataConnectionType == NetworkReachability.ReachableViaCarrierDataNetwork || PolledNetworkState.GetReachabilityStatus() == NetworkReachability.ReachableViaCarrierDataNetwork);
		if (tryNATFirst && !flag)
		{
			return AttemptConnectionViaNAT();
		}
		return AttemptConnectionViaProxy();
	}

	public void JoinGameWithBot(SMPHostInfo opponentInfo)
	{
		OpponentGameInfo = opponentInfo;
	}

	private bool AttemptConnectionViaNAT()
	{
		if (m_PunchthroughPeer != null && m_PunchthroughClientPlugin != null)
		{
			P2PConnectionType = EP2PConnectionType.NAT;
			Crittercism.LeaveBreadcrumb("SMP: AttemptConnectionViaNAT.");
			SystemAddress facilitator = new SystemAddress(cachedServerIPAddress, natPunchthroughServerPort);
			m_InitiatedNATPunchthrough = true;
			m_PunchthroughClientPlugin.OpenNAT(OpponentGameInfo.GUID, facilitator);
			return true;
		}
		return false;
	}

	private bool AttemptConnectionViaProxy()
	{
		if (m_PunchthroughPeer != null && m_UDPProxyPlugin != null)
		{
			P2PConnectionType = EP2PConnectionType.Proxy;
			Crittercism.LeaveBreadcrumb("SMP: AttemptConnectionViaProxy.");
			SystemAddress proxyCoordinator = new SystemAddress(cachedServerIPAddress, natPunchthroughServerPort);
			m_UDPProxyPlugin.RequestForwarding(proxyCoordinator, RakNet.UNASSIGNED_SYSTEM_ADDRESS, OpponentGameInfo.GUID, 7000u);
			return true;
		}
		return false;
	}

	public void LeaveGame(PacketPriority priority = PacketPriority.LOW_PRIORITY)
	{
		if (m_OpponentGUID != null)
		{
			Crittercism.LeaveBreadcrumb("SMP: LeaveGame.");
			if (m_PunchthroughPeer != null)
			{
				m_PunchthroughPeer.CloseConnection(m_OpponentGUID, true, 0, priority);
				if (P2PConnectionType == EP2PConnectionType.Proxy)
				{
					CSRRoomsClient.Instance.StartCountdownToLocalPlayerReadiness();
				}
				ResetNatPlugin();
			}
			m_OpponentGUID = null;
			m_RaceControllerRPCs.enabled = false;
		}
	}

	public void LeaveGameWithImmediatePriority()
	{
		LeaveGame(PacketPriority.IMMEDIATE_PRIORITY);
	}

	public void LeaveGameImmediate()
	{
		LeaveGameWithImmediatePriority();
		SMPYouLeftRace = true;
	}

	public bool IsConnectedToOpponent()
	{
		return m_OpponentGUID != null;
	}

	public bool IsTryingToConnectToOpponent()
	{
		return m_AttemptingToConnectToOpponent;
	}

	public void SuccessfullyConnectedToOpponent(AddressOrGUID opponentGUID)
	{
		m_AttemptingToConnectToOpponent = false;
		if (m_OpponentGUID == null)
		{
			m_OpponentGUID = opponentGUID;
		}
		Crittercism.LeaveBreadcrumb("SMP: SuccessfullyConnectedToOpponent. Connection Type: " + P2PConnectionType);
		m_RaceControllerRPCs.enabled = true;
		lastGameConnectionID = GetLatestGameConnectionID(m_OpponentGUID.rakNetGuid.g.ToString());
		ZTrackMetricsHelper.LogSMPP2PConnectionResult(OpponentGameInfo.IsBot, true);
		if (OnJoinRemoteGame != null)
		{
			OnJoinRemoteGame(true);
		}
	}

	public void FailedToConnectToOpponent()
	{
		m_AttemptingToConnectToOpponent = false;
		Crittercism.LeaveBreadcrumb("SMP: FailedToConnectToOpponent. Connection Type: " + P2PConnectionType);
		P2PConnectionType = EP2PConnectionType.None;
		if (m_InitiatedNATPunchthrough)
		{
			m_InitiatedNATPunchthrough = false;
		}
		ZTrackMetricsHelper.LogSMPP2PConnectionResult(OpponentGameInfo.IsBot, false);
		if (OnJoinRemoteGame != null)
		{
			OnJoinRemoteGame(false);
		}
	}

	public void OpponentConnected(AddressOrGUID opponentGUID)
	{
		if (m_OpponentGUID == null)
		{
			m_OpponentGUID = opponentGUID;
			Crittercism.LeaveBreadcrumb("SMP: OpponentConnected. Connection Type: " + P2PConnectionType);
			m_RaceControllerRPCs.enabled = true;
			lastGameConnectionID = GetLatestGameConnectionID(m_OpponentGUID.rakNetGuid.g.ToString());
			if (OnSMPPlayerJoined != null)
			{
				OnSMPPlayerJoined();
			}
		}
	}

	private void OpponentDisconnected()
	{
		m_OpponentGUID = null;
		Crittercism.LeaveBreadcrumb("SMP: OpponentDisconnected. Connection Type: " + P2PConnectionType);
		if (P2PConnectionType == EP2PConnectionType.Proxy)
		{
			CSRRoomsClient.Instance.StartCountdownToLocalPlayerReadiness();
		}
		ResetNatPlugin();
		P2PConnectionType = EP2PConnectionType.None;
		m_RaceControllerRPCs.enabled = false;
		if (CSRRoomsClient.Instance.ConnectedToRoomServer)
		{
			if (OnSMPPlayerLeft != null)
			{
				OnSMPPlayerLeft();
			}
		}
		else if (OnSMPPlayerDisconnected != null)
		{
			OnSMPPlayerDisconnected();
		}
	}

	private void OpponentConnectionLost()
	{
		m_OpponentGUID = null;
		Crittercism.LeaveBreadcrumb("SMP: OpponentConnectionLost. Connection Type: " + P2PConnectionType);
		if (P2PConnectionType == EP2PConnectionType.Proxy)
		{
			CSRRoomsClient.Instance.StartCountdownToLocalPlayerReadiness();
		}
		ResetNatPlugin();
		P2PConnectionType = EP2PConnectionType.None;
		m_RaceControllerRPCs.enabled = false;
		if (OnSMPPlayerDisconnected != null)
		{
			OnSMPPlayerDisconnected();
		}
	}

	private void ResetNatPlugin()
	{
		if (SMPConfigManager.General.ResetNATPluginAfterOpponentDisconnect)
		{
			m_PunchthroughPeer.DetachPlugin(m_PunchthroughClientPlugin);
			m_PunchthroughClientPlugin = new NatPunchthroughClient();
			m_PunchthroughPeer.AttachPlugin(m_PunchthroughClientPlugin);
		}
	}

	public void SetOpponentInfoFromJson(string opponent_game_info_json)
	{
		OpponentGameInfo = new SMPHostInfo();
		OpponentGameInfo.SerializeFromJson(opponent_game_info_json);
	}

	public void SetOpponentToChallenge(SMPHostInfo opponent_game_info)
	{
		string json_str = opponent_game_info.SerializeToJson();
		OpponentGameInfo.SerializeFromJson(json_str);
	}

	public void UpdateOpponentMaxCash(int updatedMaxCashValue)
	{
		if (OpponentGameInfo != null)
		{
			OpponentGameInfo.UpdateMaxCashValue(updatedMaxCashValue);
		}
	}

	public void SetBotOpponentToChallenge(SMPHostInfo opponent_game_info)
	{
		OpponentGameInfo = opponent_game_info;
	}

	public void ReceivedRaceChallenge(string opponent_game_info_json)
	{
		SetOpponentInfoFromJson(opponent_game_info_json);
		m_HasReceivedChallenge = true;
	}

	public bool HasReceivedChallenge()
	{
		return m_HasReceivedChallenge;
	}

	public void ClearReceivedChallenge()
	{
		m_HasReceivedChallenge = false;
	}

	private string GetLatestGameConnectionID(string opponentGUID)
	{
		string str = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
		str += "|";
		return str + opponentGUID;
	}

	public SMPRaceControllerRPCs GetRaceControllerRPCs()
	{
		if (m_RaceControllerRPCs.enabled)
		{
			return m_RaceControllerRPCs;
		}
		return null;
	}

	public void RPC(string functionName, params object[] args)
	{
		if (m_OpponentGUID != null)
		{
			RakNet4.BitStream bitStream;
			rpcRouter.FillBitstreamForRPC(functionName, out bitStream, args);
			m_PunchthroughPeer.Send(bitStream, PacketPriority.HIGH_PRIORITY, PacketReliability.RELIABLE_ORDERED, '\0', m_OpponentGUID, false);
		}
	}

	public void RPCImmediate(string functionName, params object[] args)
	{
		if (m_OpponentGUID != null)
		{
			RakNet4.BitStream bitStream;
			rpcRouter.FillBitstreamForRPC(functionName, out bitStream, args);
			m_PunchthroughPeer.Send(bitStream, PacketPriority.IMMEDIATE_PRIORITY, PacketReliability.RELIABLE_ORDERED, '\0', m_OpponentGUID, false);
		}
	}
     */
    #endregion

    public static SMPNetworkManager Instance;

    public bool SMPYouLeftRace
    {
        get;
        set;
    }

    public bool SMPOpponentLeftRace
    {
        get;
        set;
    }

    public bool SMPRaceInvalidated
    {
        get;
        set;
    }

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }
        Instance = this;

        PolledNetworkState.JustWentOffline += PolledNetworkState_JustWentOffline;
    }

    void PolledNetworkState_JustWentOffline()
    {
        if (RaceController.RaceIsRunning())
        {
            SMPYouLeftRace = true;
            SMPRaceInvalidated = true;
        }
    }


    public void ResetSMPRaceVariables(bool bResetStakeValue = true)
    {
        SMPYouLeftRace = false;
        SMPOpponentLeftRace = false;
        SMPRaceInvalidated = false;
    }
}
