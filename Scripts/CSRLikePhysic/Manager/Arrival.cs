using System;
using System.Globalization;
using UnityEngine;

[Serializable]
public class Arrival:ISerializationCallbackReceiver
{
    [SerializeField] private string m_dueTimeString;


    public int AssociatedPushNotification = -1;

    public float deliveryTimeSecs;

    public DateTime dueTime;

    public ArrivalType arrivalType;

    public bool doesAutoTickDown = true;

    public string carId = string.Empty;

    public int ColourIndex;

    public eUpgradeType upgradeType;
    public bool hasArrived;

    public int GetRemainingSeconds()
    {
        return (int)(this.dueTime - GTDateTime.Now).TotalSeconds;
    }

    public int GetRemainingMinutes()
    {
        float num = Mathf.Max((float)(this.dueTime - GTDateTime.Now).TotalSeconds, 0f);
        return Mathf.CeilToInt(num/60f);
    }

    public void GetTimeUntilDelivery(out int minutes, out int seconds)
    {
        int num = Mathf.Max((int)(this.dueTime - GTDateTime.Now).TotalSeconds, 0);
        minutes = (int) Mathf.Floor((float) (num/60));
        seconds = num - minutes*60;
    }

    public bool Equals(Arrival other)
    {
        return this.arrivalType == other.arrivalType && String.Compare(this.carId, other.carId, StringComparison.Ordinal) == 0 &&
               (this.arrivalType != ArrivalType.Upgrade || this.upgradeType == other.upgradeType);
    }

    public int GetGoldCostForSkip(float secondGainedPerGold)
    {
        return GetGoldCostForSkip((float) this.GetRemainingSeconds(), secondGainedPerGold);
    }

    public static int GetGoldCostForSkip(float seconds, float secondGainedPerGold)
    {
        if (seconds <= 30f)
        {
            return 0;
        }
        int b = Mathf.CeilToInt(seconds/secondGainedPerGold);
        return Mathf.Max(0, b);
    }

    public void OnBeforeSerialize()
    {
        m_dueTimeString = dueTime.ToString(CultureInfo.InvariantCulture);
    }

    public void OnAfterDeserialize()
    {
        dueTime = Convert.ToDateTime(m_dueTimeString);
    }
}
