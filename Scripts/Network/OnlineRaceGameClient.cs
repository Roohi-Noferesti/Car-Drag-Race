using System;
using ExitGames.Client.Photon;

public class OnlineRaceGameClient : IPhotonPeerListener
{

    public event Action<OperationResponse> OperationResponse;
    public event Action<StatusCode> StatusChanged;
    public event Action<DebugLevel, string> DebugReturned;
    public event Action<EventData> Event;


    public void DebugReturn(DebugLevel level, string message)
    {
        if (DebugReturned != null)
            DebugReturned.Invoke(level, message);
    }

    public void OnOperationResponse(OperationResponse operationResponse)
    {
        if (OperationResponse != null)
            OperationResponse.Invoke(operationResponse);
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (StatusChanged != null)
            StatusChanged.Invoke(statusCode);
    }

    public void OnEvent(EventData eventData)
    {
        if (Event != null)
            Event.Invoke(eventData);
    }
}
