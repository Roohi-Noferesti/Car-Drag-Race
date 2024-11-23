using ProtoBuf;
using System;
using System.Collections.Generic;

namespace DataSerialization
{
    [ProtoContract]
    [Serializable]
    public class ScheduledPin
    {
        [ProtoMember(1)]
        public string ID;

        [ProtoMember(2)]
        public string EventType;

        [ProtoMember(3)]
        public string LifetimeGroup = string.Empty;

        [ProtoMember(4)]
        public string PinID;

        [ProtoMember(5)]
        public PinSchedulerAIDriverOverrides AIDriverOverrides = PinSchedulerAIDriverOverrides.CreateEmpty();

        [ProtoMember(6)]
        public NarrativeSceneForEventData Narrative = NarrativeSceneForEventData.CreateEmpty();

        [ProtoMember(7)]
        public EligibilityRequirements Requirements = EligibilityRequirements.CreateAlwaysEligible();

        [ProtoMember(8)]
        public string OnPostRaceLostPopupID = string.Empty;

        [ProtoMember(9)]
        public string OnWorkshopPopupID = string.Empty;

        [ProtoMember(10)]
        public string OnMapPinTapPopupID = string.Empty;

        [ProtoMember(11)]
        public BubbleMessageData PinBubbleMessage = new BubbleMessageData();

        [ProtoMember(12)]
        public bool IsNextButtonLocked;

        [ProtoMember(13)]
        public bool AutoStart;

        [ProtoMember(14)]
        public string NextScreen = "Invalid";

        [ProtoMember(15)]
        public bool ShowAnimationIn;

        [ProtoMember(16)]
        public string AppearAnimationSelectionTypeString = "SELECT_FIRST";

        [ProtoMember(17)]
        public Vector3 AppearAnimationInitialScale = Vector3.one;

        [ProtoMember(18)]
        public List<string> AppearAnimations = new List<string>();

        [ProtoMember(19)]
        public List<CarOverride> CarOverrides = new List<CarOverride>();

        [ProtoMember(20)]
        public ProgressionVisualisation ProgressionVisualisation = new ProgressionVisualisation();

        [ProtoMember(21)]
        public string CarToAwardID;

        [ProtoMember(22)]
        public byte CarToAwardUpgradeLevel;

        [ProtoMember(23)]
        public ChoiceScreenInfo ChoiceScreen;

        [ProtoMember(24)]
        public int DefaultAutoSelectPriority;

        [ProtoMember(25)]
        public int LastSequenceAutoSelectPriority = 100;

        [ProtoMember(26)]
        public string SequenceReference;

        [ProtoIgnore]
        public ScheduledPin ReferrerPin
        {
            get;
            set;
        }

        [ProtoIgnore]
        public PinSequence ParentSequence
        {
            get;
            set;
        }

        [ProtoIgnore]
        public int Level
        {
            get;
            set;
        }

        [ProtoBeforeDeserialization]
        public void Init()
        {
            this.Requirements = null;
        }
    }
}
