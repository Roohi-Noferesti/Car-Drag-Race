using System;
using System.Collections.Generic;
using System.Linq;
using DataSerialization;
using Vector3 = UnityEngine.Vector3;

[Serializable]
public class TutorialBubble
{
	public string Name = string.Empty;

    public List<TutorialBubbleParent> PossibleParents = new List<TutorialBubbleParent>();

	public EligibilityRequirements BubbleRequirements = EligibilityRequirements.CreateAlwaysEligible();

	public List<string> ShowEventNames = new List<string>();

	public List<string> DismissEventNames = new List<string>();

	public string Text = string.Empty;

	public Vector3 Offset = Vector3.zero;

	public string NippleDir = "DOWN";

	public float NipplePos = 0.5f;

	public string ThemeStyle = "SMALL";

	public string PositionType = "BOX_RELATIVE";

	public float FontSize = 0.16f;

	public float Delay;

	private HashSet<TutorialBubblesEvent> showEvents;

	private HashSet<TutorialBubblesEvent> dismissEvents;
    public bool useBackdrop;

    public int ID
	{
		get
		{
			return HashName(this.Name);
		}
	}

	public BubbleMessage.NippleDir NippleDirEnum
	{
		get
		{
			return EnumHelper.FromString<BubbleMessage.NippleDir>(this.NippleDir);
		}
	}

	public BubbleMessageConfig.ThemeStyle ThemeStyleEnum
	{
		get
		{
			return EnumHelper.FromString<BubbleMessageConfig.ThemeStyle>(this.ThemeStyle);
		}
	}

	public BubbleMessageConfig.PositionType PositionTypeEnum
	{
		get
		{
			return EnumHelper.FromString<BubbleMessageConfig.PositionType>(this.PositionType);
		}
	}

	public HashSet<TutorialBubblesEvent> ShowEvents
	{
		get
		{
			if (this.showEvents == null)
			{
				this.showEvents = new HashSet<TutorialBubblesEvent>(from name in this.ShowEventNames
				select EnumHelper.FromString<TutorialBubblesEvent>(name));
			}
			return this.showEvents;
		}
	}

	public HashSet<TutorialBubblesEvent> DismissEvents
	{
		get
		{
			if (this.dismissEvents == null)
			{
				this.dismissEvents = new HashSet<TutorialBubblesEvent>(from name in this.DismissEventNames
				select EnumHelper.FromString<TutorialBubblesEvent>(name));
			}
			return this.dismissEvents;
		}
	}

	public static int HashName(string name)
	{
		return name.GetHashCode();
	}

	public bool IsEligible(IGameState gs)
	{
        return this.BubbleRequirements.IsEligible(gs) && this.GetEligibleParent(gs) != null;
	}

    public TutorialBubbleParent GetEligibleParent(IGameState gs)
    {
        return (from p in this.PossibleParents
                where p.IsEligible(gs)
                select p).FirstOrDefault<TutorialBubbleParent>();
    }
}
