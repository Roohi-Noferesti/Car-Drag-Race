using System;
using System.Collections.Generic;

public class BAFriendsManager : BaseFriendsManager
{
	public override string GetNetworkIDPrefix()
	{
		return "ba";
	}

	public override string GetNetworkID()
	{
		return this.GetNetworkIDPrefix() + UserManager.Instance.currentAccount.UserID;
	}

	public override List<string> GetFriendIDs()
	{
		List<string> list = new List<string>();
		foreach (KeyValuePair<int, CachedFriendRaceData> current in LumpManager.Instance.FriendLumps)
		{
			if (current.Value.BelongsToService(this.GetNetworkIDPrefix()))
			{
				list.Add(current.Value.GetIdForService(this.GetNetworkIDPrefix()));
			}
		}
		return list;
	}

	public override string GetFriendName(string friendId)
	{
		return friendId;
	}

	public override void SetupFriendAvatar(string friendId, PersonaComponent persona)
	{
		persona.LoadDefaultCsrAvatarFromResources();
	}

	public override void PollFriends()
	{
	}

	private void SyncIfDataIsReady()
	{
	}
}
