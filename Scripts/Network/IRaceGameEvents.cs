using ExitGames.Client.Photon;

public interface IRaceGameEvents
{
    void OnOperationFail(OperationResponse operationResponse);
    void OnStatusChanged(StatusCode statusCode);
    void OnDebugReturned(DebugLevel debugLevel, string message);
}
