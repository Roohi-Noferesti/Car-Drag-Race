using System;
using System.Collections.Generic;

public class PlayerListItemRow
{
	public enum SlideDirection
	{
		Left,
		Right
	}

	public PlayerListItemRow.SlideDirection slideDirection;

	public List<PlayerListItem> playerListItems = new List<PlayerListItem>();

	public void Setup(PlayerListItemRow.SlideDirection slideDirection, List<PlayerListItem> items)
	{
		this.slideDirection = slideDirection;
		this.playerListItems = items;
	}

	public void AssignAnimations()
	{
		string introAnimation = string.Empty;
		PlayerListItemRow.SlideDirection slideDirection = this.slideDirection;
		if (slideDirection != PlayerListItemRow.SlideDirection.Left)
		{
			if (slideDirection == PlayerListItemRow.SlideDirection.Right)
			{
				introAnimation = "PlayerListItemSlideRight";
			}
		}
		else
		{
			introAnimation = "PlayerListItemSlideLeft";
		}
		foreach (PlayerListItem current in this.playerListItems)
		{
			current.AssignIntroAnimation(introAnimation);
		}
	}

	public void StartPlayingSlideAnimations()
	{
		PlayerListItem item = null;
		PlayerListItemRow.SlideDirection slideDirection = this.slideDirection;
		if (slideDirection != PlayerListItemRow.SlideDirection.Left)
		{
			if (slideDirection == PlayerListItemRow.SlideDirection.Right)
			{
				item = this.playerListItems[this.playerListItems.Count - 1];
			}
		}
		else
		{
			item = this.playerListItems[0];
		}
		this.PlaySingleItemSlideAnimation(item);
	}

	private void PlaySingleItemSlideAnimation(PlayerListItem item)
	{
		item.OnSlideInAnimHalfWay += new OnSlideInAnimEvent(this.HandleOnSlideInAnimHalfWay);
		item.PlayAssignedAnimation();
	}

	private void HandleOnSlideInAnimHalfWay(PlayerListItem item)
	{
		int num = this.playerListItems.IndexOf(item);
		int num2 = -1;
		PlayerListItemRow.SlideDirection slideDirection = this.slideDirection;
		if (slideDirection != PlayerListItemRow.SlideDirection.Left)
		{
			if (slideDirection == PlayerListItemRow.SlideDirection.Right)
			{
				num2 = num - 1;
			}
		}
		else
		{
			num2 = num + 1;
		}
		if (num2 >= 0 && num2 < this.playerListItems.Count)
		{
			this.PlaySingleItemSlideAnimation(this.playerListItems[num2]);
		}
	}
}
