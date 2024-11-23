using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GTLogConfiguration : ScriptableObject 
{
    [System.Serializable]
    public class LogStatus
    {
        public GTLogChannel Channel;
        public bool Active;
    }

    [System.Serializable]
    public class LogChannel
    {
        public string Name;
        public bool Active;
    }

    public List<LogStatus> Logs;

    public List<LogChannel> Channels;
}
