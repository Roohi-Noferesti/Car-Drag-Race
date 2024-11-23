using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public abstract class BaseFriendsManager
{
	private const float SYNC_RETRY_TIME = 30f;

	private bool syncRequired;

	private bool isCurrentlySyncing;

	private float timeToRetryOnError;

    public event RYFStatusManager_Delegate OnFriendsListUpdate;

	protected BaseFriendsManager()
	{
	}

	public abstract string GetNetworkIDPrefix();

	public abstract string GetNetworkID();

	public abstract List<string> GetFriendIDs();

	public abstract string GetFriendName(string friendId);

	public abstract void PollFriends();

	public abstract void SetupFriendAvatar(string friendID, PersonaComponent persona);

	public void Update()
	{
		if (!this.syncRequired)
		{
			return;
		}
		if (this.isCurrentlySyncing)
		{
			return;
		}
		this.timeToRetryOnError -= Time.deltaTime;
		if (this.timeToRetryOnError <= 0f)
		{
			this.SyncFriendsNetwork();
		}
	}

	public bool IsFriendInThisNetwork(string friendId)
	{
		return this.GetFriendIDs().Contains(friendId);
	}

	private void SyncFriendsNetwork()
	{
		this.isCurrentlySyncing = true;
		FriendWebRequests.UpdateFriendNetwork(this.GetNetworkID(), this.GetFriendIDs(), this.GetNetworkIDPrefix(), new WebClientDelegate2(this.NetworkSyncCompleted));
	}

	private void NetworkSyncCompleted(string content, string error, int status, object userData)
	{
		if (status == 200)
		{
			PlayerProfileManager.Instance.ActiveProfile.FirstTimeFriendsUser = false;
			this.syncRequired = false;
		}
		else
		{
			this.timeToRetryOnError = 30f;
		}
		this.isCurrentlySyncing = false;
	}

	protected void ReadyToSync()
	{
		this.syncRequired = true;
		this.OnFriendsListUpdate();
	}

	protected void CancelSync()
	{
		this.syncRequired = false;
	}
}
