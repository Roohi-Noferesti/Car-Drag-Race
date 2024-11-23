using System;
using ExitGames.Client.Photon;
using Z2HSharedLibrary.DatabaseEntity;
using Z2HSharedLibrary.Operation;

public class OnlineRaceGameEvents : MonoSingleton<OnlineRaceGameEvents>,IRaceGameEvents
{
    public static event Action<DebugLevel, string> DebugReturned;
    public static event Action<StatusCode> StatusChanged;
    public static event Action<DatabaseOperationCode, ResponseCode, string> OperationFailed;

    public void OnOperationFail(OperationResponse operationResponse)
    {
        if (OperationFailed != null)
        {
            OperationFailed.Invoke((DatabaseOperationCode) operationResponse.OperationCode, (ResponseCode) operationResponse.ReturnCode
                , operationResponse.DebugMessage);
        }
    }

    public void OnStatusChanged(StatusCode statusCode)
    {
        if (StatusChanged != null)
        {
            StatusChanged.Invoke(statusCode);
        }
    }

    public void OnDebugReturned(DebugLevel debugLevel, string message)
    {
        if (DebugReturned != null)
        {
            DebugReturned.Invoke(debugLevel, message);
        }
    }
}
