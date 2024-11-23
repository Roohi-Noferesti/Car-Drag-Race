using System;
using System.Collections.Generic;
using RotaryHeart.Lib.SerializableDictionary;

namespace DataSerialization
{
	[Serializable]
	public class PinDetail
	{
		public enum PinType
		{
			NORMAL,
			DAILYBATTLE,
			MECHANIC,
			PIZZAPIN,
			MULTIPLAYERPIN,
			WORKSHOPPIN
		}

		public enum TimelineDirection
		{
			None,
			Previous,
			Next
		}

		public enum TextureKeys
		{
			CarRender,
			CarRenderElite,
			EventPaneOverlay,
			EventPaneBackground,
			EventPaneBoss,
			PinOverlay,
			PinBoss,
			PinBackground,
			PinBranding,
			Cross
		}

		public string PinID;

		public string TemplateName = "";

		public Vector2 Position;

		public Vector2 PositionOffset;

		public string LoadingScreen = "";

		public string Title;

		public string EventDescription;

		public bool ProgressIndicator = true;

		public bool ShowSelectionArrow = true;

		public bool CanDisplayAsComplete;

		public bool IsSelectable = true;

		public bool HideTextBox;

		public int EventID;

		public int GroupID;

		public bool IsSuperNitrous;

		public bool IsProgressPin;

		public PinLock Lock = new PinLock();

		public LoadTierAction ClickAction;

		public PushScreenAction PushScreenAction;

		public Color Colour = Color.magenta;

		public Dictionary<string, TextureDetail> Textures = new Dictionary<string, TextureDetail>();

        public Dictionary<string, ConditionallySelectedString> ConditionallySelectedStrings = new Dictionary<string, ConditionallySelectedString>();

		public float Greyness;

		public string CompletedTitle;

		[NonSerialized]
		public ScheduledPin WorldTourScheduledPinInfo;

		[NonSerialized]
		public TimelineDirection CurrentTimelineDirection;

		[NonSerialized]
		public string Label = string.Empty;

		[NonSerialized]
		public TimelinePinDetails TimelineDetails;

#if UNITY_EDITOR
	    //public StringTextureDetailsDictionary _texturesSerializable =
	    //    StringTextureDetailsDictionary.New<StringTextureDetailsDictionary>();

        public StringTextureDetailsDictionaryV2 _texturesSerializableV2 =new StringTextureDetailsDictionaryV2();
        public void DoSerializationStaff()
	    {
	        //foreach (var textureDetail in Textures)
	        //{
	        //    _texturesSerializable.dictionary.Add(textureDetail.Key, textureDetail.Value);
         //   }


            foreach (var textureDetail in Textures)
            {
                _texturesSerializableV2.Add(textureDetail.Key, textureDetail.Value);
            }
        }

	    public void DoDeSerializationStaff()
	    {
	        Textures = new Dictionary<string, TextureDetail>();

         //   foreach (var textureDetail in _texturesSerializable.dictionary)
	        //{
	        //    Textures.Add(textureDetail.Key, textureDetail.Value);
	        //}

            foreach (var textureDetail in _texturesSerializableV2)
            {
                Textures.Add(textureDetail.Key, textureDetail.Value);
            }
        }


        [System.Serializable]
        public class StringTextureDetailsDictionaryV2:SerializableDictionaryBase<string,TextureDetail>
        {
            
        }
#endif
    }
}
