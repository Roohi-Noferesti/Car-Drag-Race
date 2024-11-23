using System.Collections.Generic;
using Metrics;

public interface IAnalytics
{
    event AnalyticsFinishedInitializationHandler InitializationFinished;
    void Init();

    void SetUserID();

    bool IsValidEventID(Events eventName);

    void LogAnEvent_Internal(Events eventName, Dictionary<Parameters, string> data);

    void UserManager_LoggedInEvent();
}
