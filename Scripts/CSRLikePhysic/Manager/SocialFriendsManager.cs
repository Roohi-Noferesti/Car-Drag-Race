using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SocialFriendsManager : MonoBehaviour
{
	public const int DefaultPollFrequency = 600;

	private List<BaseFriendsManager> FriendsListProviders = new List<BaseFriendsManager>();

	private float elapsedSinceLastPoll;

	public float PollFrequency = 600f;

	public event RYFStatusManager_Delegate OnFriendsListUpdate
	{
		add
		{
			foreach (BaseFriendsManager current in this.FriendsListProviders)
			{
				current.OnFriendsListUpdate += value;
			}
		}
		remove
		{
			foreach (BaseFriendsManager current in this.FriendsListProviders)
			{
				current.OnFriendsListUpdate -= value;
			}
		}
	}

	public static SocialFriendsManager Instance
	{
		get;
		private set;
	}

	public string GetNetworkIDPrefix(string socialId)
	{
		foreach (BaseFriendsManager current in this.FriendsListProviders)
		{
			string networkIDPrefix = current.GetNetworkIDPrefix();
			if (socialId.StartsWith(networkIDPrefix))
			{
				return networkIDPrefix;
			}
		}
		return string.Empty;
	}

	private void Awake()
	{
		if (SocialFriendsManager.Instance != null)
		{
			return;
		}
		SocialFriendsManager.Instance = this;
        //this.FriendsListProviders.Add(new FacebookFriendsManager());
		this.FriendsListProviders.Add(new BAFriendsManager());
	}

	public void TriggerPollNextUpdate()
	{
		this.elapsedSinceLastPoll = this.PollFrequency;
	}

	public void Update()
	{
		this.elapsedSinceLastPoll += Time.deltaTime;
		if (this.elapsedSinceLastPoll >= this.PollFrequency)
		{
			this.elapsedSinceLastPoll = 0f;
			foreach (BaseFriendsManager current in this.FriendsListProviders)
			{
				current.PollFriends();
			}
		}
		foreach (BaseFriendsManager current2 in this.FriendsListProviders)
		{
			current2.Update();
		}
	}

	public bool IsFriendInOurNetworks(List<string> friendSocialIDs)
	{
		return friendSocialIDs != null && friendSocialIDs.Any((string id) => this.IsFriendInOurNetworks(id));
	}

	public bool IsFriendInOurNetworks(string friendSocialID)
	{
		return this.FriendsListProviders.Any((BaseFriendsManager network) => network.IsFriendInThisNetwork(friendSocialID));
	}

	public string GetFriendName(List<string> friendSocialIDs)
	{
		string friendId = null;
		BaseFriendsManager baseFriendsManager = null;
		if (this.FindAssociatedFriendNetwork(friendSocialIDs, out friendId, out baseFriendsManager))
		{
			return baseFriendsManager.GetFriendName(friendId);
		}
		if (friendSocialIDs != null && friendSocialIDs.Count > 0)
		{
			return friendSocialIDs.First<string>();
		}
		return string.Empty;
	}

	public void SetupFriendAvatar(List<string> friendSocialIDs, PersonaComponent persona)
	{
		string friendID = null;
		BaseFriendsManager baseFriendsManager = null;
		if (!this.FindAssociatedFriendNetwork(friendSocialIDs, out friendID, out baseFriendsManager))
		{
			persona.OnAvatarLoadFailed();
			return;
		}
		baseFriendsManager.SetupFriendAvatar(friendID, persona);
	}

	public void ClearAllFriends()
	{
		foreach (BaseFriendsManager current in this.FriendsListProviders)
		{
			FriendWebRequests.UpdateFriendNetwork(current.GetNetworkID(), new List<string>(), current.GetNetworkIDPrefix(), null);
		}
	}

	private bool FindAssociatedFriendNetwork(List<string> potentialFriendIDs, out string idForFriend, out BaseFriendsManager networkForFriend)
	{
		idForFriend = null;
		networkForFriend = null;
		if (potentialFriendIDs == null)
		{
			return false;
		}
		foreach (BaseFriendsManager provider in this.FriendsListProviders)
		{
			idForFriend = potentialFriendIDs.FirstOrDefault((string q) => provider.IsFriendInThisNetwork(q));
			if (!string.IsNullOrEmpty(idForFriend))
			{
				networkForFriend = provider;
				break;
			}
		}
		return idForFriend != null;
	}
}
