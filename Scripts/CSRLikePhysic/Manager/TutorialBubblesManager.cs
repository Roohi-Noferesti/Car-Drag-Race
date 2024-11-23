using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using UnityEngine;
using UnityEngine.EventSystems;

public class TutorialBubblesManager : MonoBehaviour
{
	private Dictionary<BubbleMessage, TutorialBubble> activeBubbles = new Dictionary<BubbleMessage, TutorialBubble>();

	private float timeSinceLastScreenChange;

	public static TutorialBubblesManager Instance
	{
		get;
		private set;
	}

	private void Awake()
	{
		if (Instance != null)
		{
			return;
		}
		Instance = this;
	}

	private void Start()
	{
        //GestureEventSystem.Instance.Tap += new GestureEventSystem.GestureEventHandler(this.OnScreenTap);
	}

	private void OnDestroy()
	{
        //GestureEventSystem.Instance.Tap -= new GestureEventSystem.GestureEventHandler(this.OnScreenTap);
	}

    //private void OnScreenTap(GenericTouch touchData)
    //{
    //    this.TriggerEvent(TutorialBubblesEvent.ScreenTap);
    //}

	private void OnDestroyBubble(BubbleMessage bubble)
	{
		bubble.OnDestroyEvent = (BubbleMessage.OnDestroyDelegate)Delegate.Remove(bubble.OnDestroyEvent, new BubbleMessage.OnDestroyDelegate(this.OnDestroyBubble));
		if (this.activeBubbles.ContainsKey(bubble))
		{
			this.activeBubbles.Remove(bubble);
		}
	}

	public void OnScreenChanged()
	{
		foreach (BubbleMessage current in this.activeBubbles.Keys)
		{
			current.KillNow();
		}
		this.activeBubbles.Clear();
		this.timeSinceLastScreenChange = 0f;
	}

	public void TriggerEvent(TutorialBubblesEvent tbEvent)
	{
		if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend)
		{
			return;
		}
		List<BubbleMessage> list = (from b in this.activeBubbles.Keys
		where this.activeBubbles[b].DismissEvents.Contains(tbEvent)
		select b).ToList<BubbleMessage>();
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		foreach (BubbleMessage current in list)
		{
			TutorialBubble tutorialBubble = this.activeBubbles[current];
			current.Dismiss();
			activeProfile.SetTutorialBubbleDismissed(tutorialBubble.ID);
			this.activeBubbles.Remove(current);
		}
        var iD = ScreenManager.Instance.CurrentScreen;
        IGameState gs = new GameStateFacade();
	    List<TutorialBubble> tutorialBubblesForScreen =
	        GameDatabase.Instance.TutorialBubblesConfiguration.GetTutorialBubblesForScreen(iD, gs);
        this.ShowTutorialBubbles((from b in tutorialBubblesForScreen
                                  where b.ShowEvents.Contains(tbEvent)
                                  select b).ToList<TutorialBubble>());
        if (list.Count > 0)
        {
            this.TriggerEvent(TutorialBubblesEvent.TutorialBubblesDismissed);
            PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
        }
	}

    private void ShowTutorialBubbles(List<TutorialBubble> bubbles)
    {
        if (bubbles.Count < 1)
        {
            return;
        }
        //RectTransform[] source = ScreenManager.Active.GetComponentsInChildren<RectTransform>(true);
        RectTransform[] source = UnityEngine.Object.FindObjectsOfType(typeof(RectTransform)) as RectTransform[];
        IGameState gs = new GameStateFacade();
        Dictionary<TutorialBubble, TutorialBubbleParent> dictionary = (from b in bubbles
                                                                       select new KeyValuePair<TutorialBubble, TutorialBubbleParent>(b, b.GetEligibleParent(gs)) into p
                                                                       where p.Value != null
                                                                       select p).ToDictionary((KeyValuePair<TutorialBubble, TutorialBubbleParent> p) => p.Key, (KeyValuePair<TutorialBubble, TutorialBubbleParent> p) => p.Value);
        var parentNames = from p in dictionary
                                          select p.Value.Names[0];
        var lookup = (from go in source
            where parentNames.Contains(go.name)
            select go).ToLookup(go => go.name, go => go);

        foreach (TutorialBubble current in dictionary.Keys)
        {
            TutorialBubbleParent tutorialBubbleParent = dictionary[current];
            if (lookup.Contains(tutorialBubbleParent.Name))
            {
                //List<RectTransform> list = lookup[tutorialBubbleParent.Name].ToList<RectTransform>();
                List<RectTransform> list2 = new List<RectTransform>();
                foreach (var pName in tutorialBubbleParent.Names)
                {
                    var obj = source.FirstOrDefault(s => s.name == pName);
                    if (obj != null)
                        list2.Add(obj);
                }
                int num = tutorialBubbleParent.Selector;
                if (num < 0 || num >= list2.Count)
                {
                    num = 0;
                }
                base.StartCoroutine(this.ShowTutorialBubbleDelayed(current, num,list2.ToArray()));
            }
        }
    }

    private IEnumerator ShowTutorialBubbleDelayed(TutorialBubble tutorialBubble,int selector, params RectTransform[] parents)
    {
        ScreenManager.Instance.Interactable = false;
        yield return new WaitForSeconds(tutorialBubble.Delay);
        ShowTutorialBubble(tutorialBubble,selector, parents);
        ScreenManager.Instance.Interactable = true;
    }

    private void ShowTutorialBubble(TutorialBubble tutorialBubble,int selector, params RectTransform[] parents)
	{
	    var activeScreen = ScreenManager.Instance.ActiveScreen as ZHUDScreen;
        if (activeScreen != null && activeScreen.BlockingTutorialBubbles)
        {
            return;
        }
        var position = tutorialBubble.NippleDirEnum == BubbleMessage.NippleDir.DOWN
            ? parents[selector].GetUpperPoint()
            : parents[selector].GetBottomPoint();
	    BubbleMessage bubbleMessage = BubbleManager.Instance.ShowMessage(tutorialBubble.Text, false,
            position + tutorialBubble.Offset, tutorialBubble.NippleDirEnum, tutorialBubble.NipplePos,
	        tutorialBubble.ThemeStyleEnum, tutorialBubble.PositionTypeEnum, /*tutorialBubble.FontSize*/36
            , tutorialBubble.useBackdrop,true, parents);
	    bubbleMessage.OnDestroyEvent += OnDestroyBubble;
        if(parents.Length>0)
            bubbleMessage.GetParentTransform().SetParent(parents[selector].transform,false);
		this.activeBubbles.Add(bubbleMessage, tutorialBubble);
		PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		activeProfile.IncrementTutorialBubbleSeenCount(tutorialBubble.ID);
		PlayerProfileManager.Instance.RequestConvenientSaveActiveProfile();
	}

	private void Update()
	{
		this.timeSinceLastScreenChange += Time.deltaTime;
	}

}
