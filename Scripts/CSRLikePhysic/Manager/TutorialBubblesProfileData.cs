using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

public class TutorialBubblesProfileData
{
	private const string TUTORIAL_BUBBLES = "tbsd";

	private const string TUTORIAL_BUBBLE_ID = "tbid";

	private const string TUTORIAL_BUBBLE_SEEN_COUNT = "tbsc";

	private const string TUTORIAL_BUBBLE_DISMISSED = "tbhd";

	public Dictionary<int, TutorialBubbleProfileData> TutorialBubbles = new Dictionary<int, TutorialBubbleProfileData>();

    public void ToJson(ref JsonDict jsonDict)
    {
        jsonDict.SetObjectList<KeyValuePair<int, TutorialBubbleProfileData>>("tbsd", this.TutorialBubbles.ToList<KeyValuePair<int, TutorialBubbleProfileData>>(), new SetObjectDelegate<KeyValuePair<int, TutorialBubbleProfileData>>(this.SetTutorialBubbleData));
    }

    private void SetTutorialBubbleData(KeyValuePair<int, TutorialBubbleProfileData> tutorialBubbleData, ref JsonDict jsonDict)
    {
        jsonDict.Set("tbid", tutorialBubbleData.Key.ToString("X"));
        jsonDict.Set("tbsc", tutorialBubbleData.Value.SeenCount);
        jsonDict.Set("tbhd", tutorialBubbleData.Value.HasBeenDismissed);
    }

    public void FromJson(ref JsonDict jsonDict)
    {
        List<TutorialBubbleKeyValuePair> source = new List<TutorialBubbleKeyValuePair>();
        if (!jsonDict.TryGetObjectList<TutorialBubbleKeyValuePair>("tbsd", out source, new GetObjectDelegate<TutorialBubbleKeyValuePair>(this.GetTutorialBubbleKeyValuePair)))
        {
            this.TutorialBubbles = new Dictionary<int, TutorialBubbleProfileData>();
        }
        else
        {
            this.TutorialBubbles = (from p in source
                                    where p.Value.SeenCount > 0
                                    select p).ToDictionary((TutorialBubbleKeyValuePair p) => p.Key, (TutorialBubbleKeyValuePair p) => p.Value);
        }
    }

    private void GetTutorialBubbleKeyValuePair(JsonDict jsonDict, ref TutorialBubbleKeyValuePair tutorialBubbleKVP)
    {
        string s;
        jsonDict.TryGetValue("tbid", out s);
        if (int.TryParse(s, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out tutorialBubbleKVP.Key))
        {
            jsonDict.TryGetValue("tbsc", out tutorialBubbleKVP.Value.SeenCount);
            jsonDict.TryGetValue("tbhd", out tutorialBubbleKVP.Value.HasBeenDismissed);
        }
        else
        {
            tutorialBubbleKVP.Value.SeenCount = -1;
        }
    }

	public int GetTutorialBubbleSeenCount(int ID)
	{
		return (!this.TutorialBubbles.ContainsKey(ID)) ? 0 : this.TutorialBubbles[ID].SeenCount;
	}

	public void IncrementTutorialBubbleSeenCount(int ID)
	{
		if (!this.TutorialBubbles.ContainsKey(ID))
		{
			this.TutorialBubbles.Add(ID, new TutorialBubbleProfileData());
		}
		this.TutorialBubbles[ID].SeenCount++;
	}

	public bool HasDismissedTutorialBubble(int ID)
	{
		return this.TutorialBubbles.ContainsKey(ID) && this.TutorialBubbles[ID].HasBeenDismissed;
	}

	public void SetTutorialBubbleDismissed(int ID)
	{
		if (!this.TutorialBubbles.ContainsKey(ID))
		{
			this.TutorialBubbles.Add(ID, new TutorialBubbleProfileData());
		}
		this.TutorialBubbles[ID].HasBeenDismissed = true;
	}

	public void Reset()
	{
		this.TutorialBubbles.Clear();
	}
}
