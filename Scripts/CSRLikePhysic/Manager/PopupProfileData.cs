using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class PopupProfileData:ISerializationCallbackReceiver
{
	public int PopupID;

    [SerializeField]
    private string m_firstSeenTime;

	public int SeenCount;

	public DateTime FirstSeenTime;

	public bool IsValid = true;

    public void OnBeforeSerialize()
    {
        m_firstSeenTime = FirstSeenTime.ToString(CultureInfo.InvariantCulture);
    }

    public void OnAfterDeserialize()
    {
        FirstSeenTime = Convert.ToDateTime(m_firstSeenTime);
    }
}
