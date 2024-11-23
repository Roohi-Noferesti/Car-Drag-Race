using System;
using System.Collections.Generic;
using ProtoBuf;

namespace DataSerialization
{
	[ProtoContract]
    [Serializable]
	public class PopUpConfiguration
	{
		[ProtoMember(1)]
		public Dictionary<string, PopupData> PopUpLookup = new Dictionary<string, PopupData>();


#if UNITY_EDITOR
	    //Used just for serialization in editor
	    [ProtoIgnore]
	    public List<PopUpDictionary> PopUpDictionary;

	    public void DoSerializationStaff()
	    {
	        PopUpDictionary = new List<PopUpDictionary>();

	        foreach (var popupData in PopUpLookup)
	        {
	            PopUpDictionary.Add(new PopUpDictionary()
	            {
	                Name = popupData.Key,
	                PopupData = popupData.Value
	            });
	        }
	    }


	    public void DoDeSerializationStaff()
	    {
	        PopUpLookup = new Dictionary<string, PopupData>();

            foreach (var popupData in PopUpDictionary)
            {
                PopUpLookup.Add(popupData.Name, popupData.PopupData);
	        }
	    }
#endif
    }


    [Serializable]
    public class PopUpDictionary
    {
        public string Name;
        public PopupData PopupData;
    }
}
