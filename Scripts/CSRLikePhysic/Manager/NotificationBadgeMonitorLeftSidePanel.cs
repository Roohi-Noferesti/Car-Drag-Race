using Objectives;
using System;
using System.Linq;
using UnityEngine;

public class NotificationBadgeMonitorLeftSidePanel : MonoBehaviour
{
	public enum Type
	{
		NOT_SET = -1,
		SIDE_PANEL_TAB,
		CHAT_TAB,
		OBJECTIVES_TAB,
		PROFILE_TAB,
		SETTINGS_TAB,
		MYCARS_TAB
	}

	public Type type = Type.NOT_SET;

	public NotificationBadge Badge;

	private void Update()
	{
		int num;
		bool flag;
		this.GetBadgeNumberForType(out num, out flag);
		if (flag)
		{
			this.Badge.SetNotification(true, "!", 1, null);
		}
		else if (num == 0)
		{
			this.Badge.SetNotification(false, -1);
		}
		else
		{
			this.Badge.SetNotification(num);
		}
	}

	private void GetBadgeNumberForType(out int count, out bool isExclamationMark)
	{
		count = 0;
		isExclamationMark = false;
		if (this.type == NotificationBadgeMonitorLeftSidePanel.Type.SIDE_PANEL_TAB || this.type == NotificationBadgeMonitorLeftSidePanel.Type.OBJECTIVES_TAB)
		{
			count += ((!(ObjectiveManager.Instance == null)) ? ObjectiveManager.Instance.ObjectivesAwaitingCollectionCount() : 0);
		}
		if (this.type == NotificationBadgeMonitorLeftSidePanel.Type.MYCARS_TAB)
		{
			//cunt += ((!(ArrivalManager.Instance == null)) ? ArrivalManager.Instance.HowManyCarsAreOnOrder() : 0);
			count += ((!(PlayerProfileManager.Instance.ActiveProfile == null)) ? PlayerProfileManager.Instance.ActiveProfile.UnseenCarCount() : 0);
		}
		if (this.type == NotificationBadgeMonitorLeftSidePanel.Type.SIDE_PANEL_TAB || this.type == NotificationBadgeMonitorLeftSidePanel.Type.CHAT_TAB)
		{
		    bool isChatUnlocked = false;//PlayerCrewChatManager.Instance.IsChatUnlocked(ChatNetwork.ChatType.CREW_CHAT) || PlayerCrewChatManager.Instance.IsChatUnlocked(ChatNetwork.ChatType.GLOBAL_CHAT);
		    bool isBanned = false;//PlayerProfileManager.Instance.ActiveProfile.GetIsBanned();
			bool hasChat = isChatUnlocked && !isBanned;
			if (hasChat)
			{
                //count += ((!(PlayerProfileManager.Instance != null) || !PlayerProfileManager.Instance.ActiveProfile.UnseenChat) ? 0 : 1);
                //isExclamationMark = PlayerProfileManager.Instance.ActiveProfile.UnseenChat;
			}
		}
        //if ((this.type == NotificationBadgeMonitorLeftSidePanel.Type.SIDE_PANEL_TAB || this.type == NotificationBadgeMonitorLeftSidePanel.Type.SETTINGS_TAB) && HelpShiftController.Instance != null && HelpShiftController.Instance.helpShift != null)
        //{
        //    count += HelpShiftController.Instance.helpShift.getNotificationCount(false);
        //}
	}
}
