using System;
using System.Collections;
using System.Collections.Generic;
using ExitGames.Client.Photon;
using UnityEngine;
using Z2HSharedLibrary.DatabaseEntity;
using Z2HSharedLibrary.Operation;

public class ClientConnectionManager : MonoSingleton<ClientConnectionManager>
{
    private enum State
    {
        Idle,
        Connecting,
        Disconnecting,
        CallingOperation
    }

    private class OperationAction
    {
        public DatabaseOperationCode OperationCode { get; private set; }
        public Action Action { get; private set; }

        public OperationAction(DatabaseOperationCode operationCode, Action action)
        {
            OperationCode = operationCode;
            Action = action;
        }
    }

    [SerializeField] private string[] m_serverAddress = {"127.0.0.1:4530"};
    [SerializeField] private string m_serverAppName = "RaceGameOnlineServer";
    [SerializeField] private int m_selectedServer = 0;
    [SerializeField] private bool m_debug;

    private OnlineRaceGameClient m_client;
    private float m_initialTimeout;
    private PhotonPeer m_peer;
    private IRaceGameEvents m_events;
    private Queue<OperationAction> m_operationQueue;
    private bool m_callingOperation;
    private DatabaseOperationCode m_callingOperationCode;
    private State m_state;
    private bool m_connecting;
    private bool m_disconnecting;
    private float m_lastOperationCallTime;


    public bool Connected
    {
        get { return m_peer!=null && m_peer.PeerState == PeerStateValue.Connected; }
    }
    public StatusCode Status { get; private set; }

    public string SelectedServerIP
    {
        get { return m_serverAddress[m_selectedServer]; }
    }



	// Use this for initialization
    protected override void Awake ()
	{
        base.Awake();

	    m_client = new OnlineRaceGameClient();
        m_client.StatusChanged += OnStatusChanged;
        m_client.OperationResponse += OnOperationResponse;
        m_client.Event += OnEvent;
        m_client.DebugReturned += OnDebugReturned;
        Application.runInBackground = false;
        m_operationQueue = new Queue<OperationAction>();
        m_events = OnlineRaceGameEvents.Instance;
        PolledNetworkState.JustWentOffline += PolledNetworkState_JustWentOffline;
        PolledNetworkState.JustCameOnline += PolledNetworkState_JustCameOnline;
	}

    void Start()
    {
        StartCoroutine(ServiceCoroutine());
    }

    private IEnumerator ServiceCoroutine()
    {
        while (true)
        {
            if (m_peer != null)
                m_peer.Service();
            yield return new WaitForSecondsRealtime(0.1F);
        }
    }

    private void OnDebugReturned(DebugLevel arg1, string arg2)
    {
        //if (m_debug)
        //    GTDebug.Log(GTLogChannel.Other,"OnDebugReturned : " + arg2);
        m_events.OnDebugReturned(arg1, arg2);
    }

    private void OnEvent(EventData obj)
    {
        //if (m_debug)
        //    GTDebug.Log(GTLogChannel.Other,"OnEvent : " + obj);
    }

    private void OnOperationResponse(OperationResponse obj)
    {
        //if (m_debug)
        //    GTDebug.Log(GTLogChannel.Other,"OnOperationResponse : " + obj);
        if (obj.ReturnCode != (int) ResponseCode.SUCCESS)
        {
            if(m_debug)
                GTDebug.Log(GTLogChannel.Other,"operation failed : " + (DatabaseOperationCode)obj.OperationCode+"   "+obj.DebugMessage);
            m_callingOperation = false;
            m_events.OnOperationFail(obj);
                m_operationQueue.Dequeue();
            return;
        }

        m_operationQueue.Dequeue();

        m_callingOperation = false;
    }

    private void OnStatusChanged(StatusCode obj)
    {
        Status = obj;
        if (obj == StatusCode.Disconnect)
        {
            m_connecting = false;
            m_disconnecting = false;
            m_callingOperation = false;
        }
        m_events.OnStatusChanged(obj);
    }

    void PolledNetworkState_JustWentOffline()
    {
        m_disconnecting = false;
        m_connecting = false;
        m_callingOperation = false;
        if (Connected)
        {
            Disconnect();
            m_state = State.Disconnecting;
        }
        else
        {
            m_state = State.Idle;
        }
    }

    void PolledNetworkState_JustCameOnline()
    {
        m_connecting = false;
        m_disconnecting = false;
        m_callingOperation = false;
        m_state = State.Idle;
        if (!ServerSynchronisedTime.Instance.ServerTimeValid
            && !ServerSynchronisedTime.Instance.RequestInProgress)
        {
            //ServerSynchronisedTime.Instance.RequestServerTime(null);
        }
    }

    public void CallOperation(DatabaseOperationCode DatabaseOperationCode, Dictionary<byte, object> parameters)
    {
        if (!PolledNetworkState.IsNetworkConnected)
        {
            if(m_debug)
                GTDebug.Log(GTLogChannel.Other,"register operation failed . because we are offline   " + DatabaseOperationCode);
            return;
        }
        var action = new Action(() =>
        {
            //GTDebug.Log(GTLogChannel.Other,"executing operation  " + DatabaseOperationCode+" ...");
            m_peer.OpCustom((byte) DatabaseOperationCode, parameters, true);
        });

            //GTDebug.Log(GTLogChannel.Other,DatabaseOperationCode + " opt added to reg queue " + Time.time);
        m_operationQueue.Enqueue(new OperationAction(DatabaseOperationCode, action));
            //GTDebug.Log(GTLogChannel.Other,"enqueue : " + DatabaseOperationCode + "  ,  " + Instance.m_operationQueue.Count);

        //if (!Connected)
        //{
        //    Instance.m_operationQueue.Enqueue(action);
        //    LogUtility.Log("Operation Failed for operation code '" + DatabaseOperationCode +
        //                   "'.Client is not connection to the server.Queuing ...");
        //    return;
        //}

    }

    private void Connect()
    {
        if (m_debug)
            GTDebug.Log(GTLogChannel.Other,"connecting to " + SelectedServerIP);
        Instance.m_peer = new PhotonPeer(Instance.m_client, ConnectionProtocol.Tcp);
        Instance.m_peer.Connect(SelectedServerIP, Instance.m_serverAppName);
    }

    private void Disconnect()
    {
        //GTDebug.Log(GTLogChannel.Other,"Disconnecting... ");
        if (Instance.m_peer != null)
        {
            Instance.m_peer.Disconnect();
        }
        m_disconnecting = true;
    }

    void Update()
    {
        if (!PolledNetworkState.IsNetworkConnected)
        {
            return;
        }

        switch (m_state)
        {
            case State.Idle:
                if (m_operationQueue.Count > 0)
                {
                    if (m_debug)                    
                        GTDebug.Log(GTLogChannel.Other,"Operation Count : " + m_operationQueue.Count + "   ,   OptCode:" + m_operationQueue.Peek().OperationCode + " . Go ot state ReadingConfig");
                    m_state = State.Connecting;
                }
                break;
            case State.Connecting:
                if (!Connected)
                {
                    if (!m_connecting)
                    {
                        if (m_debug)
                            GTDebug.Log(GTLogChannel.Other,"Trying to connect ");
                        Connect();
                        m_connecting = true;
                    }
                }
                else
                {
                    if (m_debug)
                        GTDebug.Log(GTLogChannel.Other,"Connected :  Go ot state CallingOperation");
                    m_state = State.CallingOperation;
                    m_connecting = false;
                }
                break;
            case State.CallingOperation:
                if (m_operationQueue.Count <= 0)
                {
                    if (m_debug)
                        GTDebug.Log(GTLogChannel.Other,"Operation Count : " + m_operationQueue.Count + " . Go ot state Disconnecting");
                    m_state = State.Disconnecting;
                    m_callingOperation = false;
                }
                else
                {
                    if (!m_callingOperation)
                    {
                        m_callingOperation = true;
                        var operationAction = m_operationQueue.Peek();
                        if (operationAction != null)
                        {
                            m_callingOperationCode = operationAction.OperationCode;
                            if (m_debug)
                                GTDebug.Log(GTLogChannel.Other,"trying to call opeartion " + m_callingOperationCode);
                            m_lastOperationCallTime = Time.time;
                            operationAction.Action();
                        }
                        else
                        {
                            m_callingOperation = false;
                            m_operationQueue.Dequeue();
                        }
                    }
                    else
                    {
                        if (Time.time - m_lastOperationCallTime > 30)
                        {
                            //Try to call last operation again because we reach timeout here
                            m_callingOperation = false;
                            m_lastOperationCallTime = Time.time;
                            m_state = State.Idle;
                            PolledNetworkState.ForceOffline();
                        }
                    }
                }
                break;
            case State.Disconnecting:
                if (!Connected)
                {
                    if (m_debug)
                        GTDebug.Log(GTLogChannel.Other,"disconnected go to state  Idle");
                    m_state = State.Idle;
                    m_disconnecting = false;
                }
                else
                {
                    if (!m_disconnecting)
                    {
                        if (m_debug)
                            GTDebug.Log(GTLogChannel.Other,"trying to disconnect");
                        Disconnect();
                        m_disconnecting = true;
                    }
                }
                break;
        }
    }
}

