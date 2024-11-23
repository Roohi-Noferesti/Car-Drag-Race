using System;

[Serializable]
public class ServerConfig
{
    public string ip;
    public bool wait;
    public int waitTime;
    public string version;
    public string whatsNews;

    public override string ToString()
    {
        return string.Format("ip:{0},wait:{1},waitTime:{2},version:{3},whatsNews:{4}", ip, wait, waitTime, version,
            whatsNews);
    }
}