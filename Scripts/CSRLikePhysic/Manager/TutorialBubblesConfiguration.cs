using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class TutorialBubblesConfiguration:ScriptableObject
{
    [SerializeField] private TutorialBubbleProgressionDictionary[] m_tutorialBubbleProgressionDictionary;
    public Dictionary<string, TutorialBubbleProgression> ScreenTutorialBubbleProgressions = new Dictionary<string, TutorialBubbleProgression>();

    public void Process()
    {
        foreach (var tutorialBubbleProgressionDictionary in m_tutorialBubbleProgressionDictionary)
        {
            ScreenTutorialBubbleProgressions.Add(tutorialBubbleProgressionDictionary.Key,
                tutorialBubbleProgressionDictionary.TutorialBubbleProgression);
        }
    }

#if UNITY_EDITOR
    public void AfterDeserialization()
    {
        int i = 0;
        m_tutorialBubbleProgressionDictionary = new TutorialBubbleProgressionDictionary[ScreenTutorialBubbleProgressions.Count];
        foreach (KeyValuePair<string, TutorialBubbleProgression> screenTutorialBubbleProgression in ScreenTutorialBubbleProgressions)
        {
            m_tutorialBubbleProgressionDictionary[i] = new TutorialBubbleProgressionDictionary()
            {
                Key = screenTutorialBubbleProgression.Key,
                TutorialBubbleProgression = screenTutorialBubbleProgression.Value
            };
            i++;
        }
    }
#endif

    public void Clear()
    {
        ScreenTutorialBubbleProgressions.Clear();
    }

    public List<TutorialBubble> GetTutorialBubblesForScreen(ScreenID screenID, IGameState gs)
	{
        string key = screenID.ToString();
        if (!this.ScreenTutorialBubbleProgressions.ContainsKey(key))
		{
			return new List<TutorialBubble>();
		}
        return this.ScreenTutorialBubbleProgressions[key].GetEligibleBubbles(gs);
	}
}

[Serializable]
public class TutorialBubbleProgressionDictionary
{
    public string Key;
    public TutorialBubbleProgression TutorialBubbleProgression;
}
