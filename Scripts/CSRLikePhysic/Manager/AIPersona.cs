using System;
using DataSerialization;
using UnityEngine;

public class AIPersona : PersonaComponent
{
    private string m_cachedName;
	public override string GetDisplayName()
	{
        //RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
        //AIDriverData aIDriverData = GameDatabase.Instance.AIPlayers.GetAIDriverData(currentEvent.AIDriver);
        //string text = aIDriverData.GetDisplayName();
        //if (currentEvent.IsRandomRelay())
        //{
        //    text = GameDatabase.Instance.Names.GetName(Mathf.Abs(text.GetHashCode()));
        //}
        //return text;
	    if (m_cachedName == null)
	        m_cachedName = GameDatabase.Instance.Names.GetRandomName();
	    return m_cachedName;
	}

	protected override void RequestAvatar()
	{
		RaceEventData currentEvent = RaceEventInfo.Instance.CurrentEvent;
		if (currentEvent.IsAIDriverAvatarAvailable())
		{
            //TexturePack.RequestTextureFromBundle(RaceEventInfo.Instance.CurrentEvent.AIDriverCrew + ".Avatar" + RaceEventInfo.Instance.CurrentEvent.AIDriverCrewNumber, delegate(Texture2D texture)
            //{
            //    this.OnAvatarLoaded(texture);
            //});
		}
		else if (currentEvent.UseCsrAvatarForAI())
		{
			base.LoadCsrAvatarFromResources(currentEvent.AICsrAvatar);
		}
		else
		{
			base.LoadDefaultCsrAvatarFromResources();
		}
	}

	public override string GetNumberPlate()
	{
		PinDetail worldTourPinPinDetail=null;// = RaceEventInfo.Instance.CurrentEvent.GetWorldTourPinPinDetail();
		if (worldTourPinPinDetail.WorldTourScheduledPinInfo != null)
		{
			PinSchedulerAIDriverOverrides aIDriverOverrides = worldTourPinPinDetail.WorldTourScheduledPinInfo.AIDriverOverrides;
			if (!string.IsNullOrEmpty(aIDriverOverrides.NumberPlateString))
			{
				return aIDriverOverrides.NumberPlateString;
			}
		}
		AIDriverData aIDriverData = RaceEventInfo.Instance.AIDriverData;
		return GameDatabase.Instance.AIPlayers.GetDriverNumberPlateString(aIDriverData);
	}

	public override void SerialiseFromJson(JsonDict jsonDict)
	{
		throw new NotImplementedException();
	}

	public override void SerialiseToJson(JsonDict jsonDict)
	{
		throw new NotImplementedException();
	}
}
