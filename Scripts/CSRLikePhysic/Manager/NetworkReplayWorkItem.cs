using System;

public abstract class NetworkReplayWorkItem
{
    public PlayerReplay PlayerReplay
    {
        get;
        private set;
    }

    public int TryCount
    {
        get;
        set;
    }

    public NetworkReplayWorkItem()
    {
    }

    public NetworkReplayWorkItem(PlayerReplay playerReplay)
    {
        this.PlayerReplay = playerReplay;
        this.TryCount = 0;
    }

    public abstract ReplayType Type();

    public abstract void Upload(WebClientDelegate2 uploadComplete);

    public abstract bool ProcessContent(string content);

    public virtual void ToJson(JsonDict jsonDict)
    {
        jsonDict.Set("playerReplayJson", this.PlayerReplay.ToJson());
        jsonDict.Set("tryCount", this.TryCount);
    }

    public virtual void FromJson(JsonDict jsonDict)
    {
        this.PlayerReplay = PlayerReplay.CreateFromJson(jsonDict.GetString("playerReplayJson"), new LocalPlayerInfo());
        this.TryCount = jsonDict.GetInt("tryCount");
    }

    public override bool Equals(object obj)
    {
        NetworkReplayWorkItem networkReplayWorkItem = obj as NetworkReplayWorkItem;
        return networkReplayWorkItem != null && !(networkReplayWorkItem.PlayerReplay.ToJson() != this.PlayerReplay.ToJson()) && networkReplayWorkItem.TryCount == this.TryCount && networkReplayWorkItem.Type() == this.Type();
    }

    public override int GetHashCode()
    {
        throw new NotSupportedException();
    }

    public override string ToString()
    {
        return string.Format("[NetworkReplayWorkItem: PlayerReplay={0}, TryCount={1}, Type={2}]", this.PlayerReplay.ToJson(), this.TryCount, this.Type());
    }
}