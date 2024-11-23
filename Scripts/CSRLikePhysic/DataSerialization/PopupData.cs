using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
    [ProtoContract]
	[Serializable]
	public class PopupData
	{
	    [ProtoMember(21, IsRequired = false)]
        public bool IsBig;

        [ProtoMember(1)]
		public string CharacterName = string.Empty;

        [ProtoMember(2)]
		public string CharacterTexture = string.Empty;

        [ProtoMember(3)]
		public string TitleText = string.Empty;

        [ProtoMember(4)]
		public List<FormatStringData> BodyText = new List<FormatStringData>();

        [ProtoMember(5)]
		public string StarterPackItem1 = string.Empty;

        [ProtoMember(6)]
		public string StarterPackItem2 = string.Empty;

        [ProtoMember(7)]
		public string StarterPackOfferItem = string.Empty;

        [ProtoMember(8)]
		public TimeSpan StarterPackValidityDuration = TimeSpan.Zero;

        [ProtoMember(9)]
		public string ConfirmButtonText = "TEXT_BUTTON_OK";

        [ProtoMember(10)]
		public EligibilityRequirements OkButtonRequirement = EligibilityRequirements.CreateAlwaysEligible();

        [ProtoMember(11)]
		public string CancelButtonText = "TEXT_BUTTON_CANCEL";

        [ProtoMember(12)]
		public EligibilityRequirements CancelButtonRequirement = EligibilityRequirements.CreateNeverEligible();

		[ProtoMember(13, IsRequired = true)]
		public bool SetupForCrewLeader = true;

        [ProtoMember(14, IsRequired = true)]
		public bool CheckForShowOnlyOnce;

        [ProtoMember(15, IsRequired = true)]
		public bool IsBodyTranslated = true;

        [ProtoMember(16)]
		public EligibilityRequirements PopupRequirements = EligibilityRequirements.CreateAlwaysEligible();

        [ProtoMember(17)]
		public List<PopupDataButtonAction> ConfirmActions = new List<PopupDataButtonAction>();

        [ProtoMember(18)]
		public List<PopupDataButtonAction> CancelActions = new List<PopupDataButtonAction>();

        [ProtoMember(20, IsRequired = true)]
        public bool HasBeenShown
        {
            get;
            set;
        }

	    public void Init()
		{
			this.PopupRequirements = null;
			this.OkButtonRequirement = null;
		}

		public static PopupData CreateDontShowPopupData()
		{
			return new PopupData
			{
				CheckForShowOnlyOnce = false,
				PopupRequirements = EligibilityRequirements.CreateNeverEligible()
			};
		}
	}
}
