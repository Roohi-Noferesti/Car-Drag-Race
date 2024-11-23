using System;
using System.Collections.Generic;
using UnityEngine;

public class NavBarInfoPaneManager : MonoBehaviour, IPersistentUI
{
	private List<NavBarInfoPane> MessageWindows = new List<NavBarInfoPane>();

	public GameObject PrefabToCreate;

	private bool isShowing;

	private void OnDisable()
	{
		for (int i = this.MessageWindows.Count - 1; i >= 0; i--)
		{
			NavBarInfoPane zNavBarInfoPane = this.MessageWindows[i];
			this.MessageWindows.RemoveAt(i);
			this.DestroyInfoPane(zNavBarInfoPane);
		}
	}

	private void OnDestroy()
	{
	}

	public void Show(bool show)
	{
		this.isShowing = show;
		foreach (NavBarInfoPane current in this.MessageWindows)
		{
			current.Show(show);
		}
	}

	public void OnScreenChanged(ScreenID screen)
	{
		if (!this.isShowing || !NavBarAnimationManager.Instance.IsShowing())
		{
			for (int i = this.MessageWindows.Count - 1; i >= 0; i--)
			{
				this.MessageWindows[i].KillNow();
			}
		}
		for (int j = this.MessageWindows.Count - 1; j >= 0; j--)
		{
			NavBarInfoPane navBarInfoPane = this.MessageWindows[j];
			navBarInfoPane.OnScreenChanged(screen);
			if (navBarInfoPane.HasFinished())
			{
				this.MessageWindows.RemoveAt(j);
				this.DestroyInfoPane(navBarInfoPane);
			}
		}
	}

	private void DestroyInfoPane(NavBarInfoPane zNavBarInfoPane)
	{
		UnityEngine.Object.Destroy(zNavBarInfoPane.transform.parent.gameObject);
	}

	public bool PaneWillOverlapExisting(NavBarInfoPane newpane)
	{
		foreach (NavBarInfoPane current in this.MessageWindows)
		{
			if (current.transform.localPosition.x + current.width / 2f >= newpane.transform.localPosition.x - newpane.width / 2f || current.transform.localPosition.x - current.width / 2f <= newpane.transform.localPosition.x + newpane.width / 2f)
			{
				return true;
			}
		}
		return false;
	}

	public List<NavBarInfoPane> GetOverlapExisting(NavBarInfoPane newpane)
	{
		List<NavBarInfoPane> list = new List<NavBarInfoPane>();
		foreach (NavBarInfoPane current in this.MessageWindows)
		{
			if (current.transform.localPosition.x + current.width / 2f >= newpane.transform.localPosition.x - newpane.width / 2f || current.transform.localPosition.x - current.width / 2f <= newpane.transform.localPosition.x + newpane.width / 2f)
			{
				list.Add(current);
			}
		}
		return list;
	}

	public void Update()
	{
	}

	private NavBarInfoPane InstantiateInfoPane()
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(this.PrefabToCreate, Vector3.zero, Quaternion.identity) as GameObject;
		gameObject.transform.parent = base.gameObject.transform;
		return gameObject.GetComponentsInChildren<NavBarInfoPane>(true)[0];
	}

	public NavBarInfoPane NewForScreen(string str, bool alreadyLocalised, float XPos, ScreenID screenhome, Action button = null, bool separateNipple = false)
	{
		if (!this.isShowing || !NavBarAnimationManager.Instance.IsShowing())
		{
			return null;
		}
		NavBarInfoPane navBarInfoPane = this.InstantiateInfoPane();
		navBarInfoPane.SetForThisScreen(str, alreadyLocalised, XPos, screenhome, button, separateNipple);
		if (this.PaneWillOverlapExisting(navBarInfoPane))
		{
			List<NavBarInfoPane> overlapExisting = this.GetOverlapExisting(navBarInfoPane);
			foreach (NavBarInfoPane current in overlapExisting)
			{
				current.FinishNice();
			}
		}
		this.MessageWindows.Add(navBarInfoPane);
		return navBarInfoPane;
	}

	public NavBarInfoPane NewForTime(string str, bool alreadyLocalised, float XPos, float SecondsOnScreen, Action button = null, bool separateNipple = false)
	{
		if (!this.isShowing || !NavBarAnimationManager.Instance.IsShowing())
		{
			return null;
		}
		NavBarInfoPane navBarInfoPane = this.InstantiateInfoPane();
		navBarInfoPane.SetForTime(str, alreadyLocalised, XPos, SecondsOnScreen, button, separateNipple);
		if (!this.PaneWillOverlapExisting(navBarInfoPane))
		{
			this.MessageWindows.Add(navBarInfoPane);
		}
		else
		{
			navBarInfoPane.KillNow();
		}
		return navBarInfoPane;
	}
}
