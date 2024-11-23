using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
	[Serializable]
	public class ThemeLayout
	{
		[ProtoMember(1)]
		public string Name;

		[ProtoMember(2)]
		public string Description;

		[ProtoMember(3)]
		public string ID;

		[ProtoMember(4)]
		public string Localisation;

		[ProtoMember(5)]
		public Color Colour;

		[ProtoMember(6)]
		public Color GlowColour;

		[ProtoMember(7)]
		public DifficultyDeltas DifficultyDeltas;

		[ProtoMember(8, IsRequired = true)]
		public bool IsOverviewTheme;

		[ProtoMember(9, IsRequired = true)]
		public bool ShowTitle = true;

		[ProtoMember(10, IsRequired = true)]
		public bool ShowDescription = true;

		[ProtoMember(11, IsRequired = true)]
		public bool CheckForProgression = true;

		[ProtoMember(12, IsRequired = true)]
		public bool AllowForRightJustifiedPins;

		[ProtoMember(13, IsRequired = true)]
		public bool UseButtonsForPins;

		[ProtoMember(14, IsRequired = true)]
		public bool ShowEventPane = true;

		[ProtoMember(15, IsRequired = true)]
		public bool ShowObjective = true;

		[ProtoMember(16, IsRequired = true)]
		public bool ShowTierText = true;

		[ProtoMember(17, IsRequired = true)]
		public bool ShowSelectedThemeDescription;

		[ProtoMember(18)]
		public string ThemeToLoadOnBackOut = "TierX_Overview";

		[ProtoMember(19, IsRequired = true)]
		public bool CanSwipe;

		[ProtoMember(20)]
		public List<TimelinePinDetails> PreviousPinsLayout = new List<TimelinePinDetails>();

		[ProtoMember(21)]
		public List<TimelinePinDetails> NextPinsLayout = new List<TimelinePinDetails>();

		[ProtoMember(22)]
		public List<PinTemplate> PinTemplates = new List<PinTemplate>();

		[ProtoMember(23)]
		public ThemeProgressionData Progression = new ThemeProgressionData();

		[ProtoMember(24)]
		public List<PinDetail> PinDetails = new List<PinDetail>();

		[ProtoMember(25)]
		public List<RestrictionRaceHelperOverride> RestrictionRaceHelperOverrides = new List<RestrictionRaceHelperOverride>();

		[ProtoMember(26)]
		public string[] EventIDsForAnimation = new string[0];

		[ProtoMember(27)]
		public string OutroAnimFlag = "";

		[ProtoMember(28)]
		public WorldTourProgressLayout ProgressLayout;

		[ProtoMember(29)]
		public WorldTourBossPinDetais BossPinDetails;

		[ProtoMember(30)]
		public Dictionary<string, ThemeOptionLayoutDetails> ThemeOptionLayoutDetails = new Dictionary<string, ThemeOptionLayoutDetails>();

		[ProtoMember(31)]
		public PopupData CarAwardPopupData = PopupData.CreateDontShowPopupData();

		[ProtoMember(32)]
		public string ThemePrizeCar;

		[ProtoMember(33)]
		public bool ThemePrizeCarIsElite;

		[ProtoMember(34)]
		public List<ThemeTransition> Transitions = new List<ThemeTransition>();

		public override string ToString()
		{
			string text = this.Name + "\n";
			text = text + this.Description + "\n";
			text = text + this.ID + "\n";
			text = text + this.Localisation + "\n";
			text = text + this.IsOverviewTheme + "\n";
			text += "Pins\n";
			foreach (PinDetail current in this.PinDetails)
			{
				text = text + " " + current.PinID + "\n";
			}
			return text;
		}


#if UNITY_EDITOR
        public void DoSerializationStaff()
	    {
	        foreach (var pinDetail in PinDetails)
	        {
	            pinDetail.DoSerializationStaff();
	        }

	        foreach (var pinTemplate in PinTemplates)
	        {
	            pinTemplate.DoSerializationStaff();
	        }
        }



	    public void DoDeSerializationStaff()
	    {
	        foreach (var pinDetail in PinDetails)
	        {
	            pinDetail.DoDeSerializationStaff();
	        }

	        foreach (var pinTemplate in PinTemplates)
	        {
	            pinTemplate.DoDeSerializationStaff();
	        }
	    }
#endif
    }
}
