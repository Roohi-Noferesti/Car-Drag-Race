using System;
using System.Collections.Generic;

[Serializable]
public class RaceEventGroup
{
    public string EventGroupName;

    public List<RaceEventData> RaceEvents = new List<RaceEventData>();

	public bool IsGrindRelay;

	public bool IsGrindAutoHeadstart;

	public short GroupPriority;

	[NonSerialized]
	private RaceEventTopLevelCategory parent;

    public int EventGroupID;

	public RaceEventTopLevelCategory Parent
	{
		get
		{
			return this.parent;
		}
	}

	public void ProcessEvents(RaceEventTopLevelCategory zParent)
	{
		this.parent = zParent;
		foreach (RaceEventData current in this.RaceEvents)
		{
			current.Process(this, zParent);
		}
	}

	public int NumOfEvents()
	{
		return this.RaceEvents.Count;
	}

	public int NumEventsComplete()
	{
		int num = 0;
	    var activeProfile = PlayerProfileManager.Instance.ActiveProfile;
		foreach (RaceEventData current in this.RaceEvents)
		{
            if (activeProfile.IsEventCompleted(current.EventID))
			{
				num++;
			}
		}
		return num;
	}

    //public override string ToString()
    //{
    //    string text = string.Concat(new object[]
    //    {
    //        "\t\t Race Event Group : ",
    //        this.EventGroupName,
    //        "( Priority )",
    //        this.GroupPriority,
    //        "\n"
    //    });
    //    foreach (RaceEventData current in this.RaceEvents)
    //    {
    //        text += current.ToString();
    //    }
    //    return text;
    //}

	public static int CompareGroupPriority(RaceEventGroup zCmp1, RaceEventGroup zCmp2)
	{
		if (zCmp1.GroupPriority == zCmp2.GroupPriority)
		{
			return 0;
		}
		if (zCmp1.GroupPriority < zCmp2.GroupPriority)
		{
			return -1;
		}
		return 1;
	}
}
