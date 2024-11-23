public class NetworkReplayWorkItemFriends : NetworkReplayWorkItem
{
	public NetworkReplayWorkItemFriends(JsonDict json)
	{
		this.FromJson(json);
	}

	public NetworkReplayWorkItemFriends(PlayerReplay playerReplay) : base(playerReplay)
	{
	}

	public override ReplayType Type()
	{
		return ReplayType.RaceYourFriends;
	}

	public override void Upload(WebClientDelegate2 uploadComplete)
	{
        //FriendWebRequests.Result(base.PlayerReplay, uploadComplete);
	}

	public override bool ProcessContent(string content)
	{
		return content == "OK";
	}
}
