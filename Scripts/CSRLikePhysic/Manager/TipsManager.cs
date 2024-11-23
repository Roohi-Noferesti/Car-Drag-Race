using I2.Loc;
using UnityEngine;

public class TipsManager
{
	private enum TipType
	{
		SINGLEPLAYER,
		MULTIPLAYER,
		FRIENDS
	}

	private class TipIds
	{
		private int _idMax;

		private string _namingConvention;

		public TipIds(string namingConvention)
		{
			this._namingConvention = namingConvention;
			this._idMax = 0;
			bool flag = true;
			while (flag)
			{
				flag = false;
				//string textID = string.Format(this._namingConvention, this._idMax);
                //if (LocalisationManager.DoesTranslationExist(textID))
                //{
                //    flag = true;
                //    this._idMax++;
                //}
			}
			if (this._idMax == 0)
			{
			}
		}

		public string randomId()
		{
			if (this._idMax > 0)
			{
				int num = Random.Range(0, this._idMax - 1);
				return string.Format(this._namingConvention, num);
			}
			return string.Empty;
		}
	}

	private static TipsManager _instance;

	private TipIds[] _tips;

	public static TipsManager Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new TipsManager();
			}
			return _instance;
		}
	}

	public TipsManager()
	{
		this._tips = new TipIds[3];
		this._tips[0] = new TipIds("TEXT_LOADING_SCREEN_TIPS_{0:D3}");
		this._tips[1] = new TipIds("TEXT_LOADING_SCREEN_MULTIPLAYER_TIPS_{0:D3}");
		this._tips[2] = new TipIds("TEXT_LOADING_SCREEN_RYF_TIPS_{0:D3}");
	}

	private string GetRandomTip(TipType type)
	{
		if ((TipType)this._tips.Length > type)
		{
			TipIds tipIds = this._tips[(int)type];
			return LocalizationManager.GetTranslation(tipIds.randomId());
		}
		return string.Empty;
	}

	public string GetRandomSingleplayerTip()
	{
		return this.GetRandomTip(TipType.SINGLEPLAYER);
	}

	public string GetRandomMultiplayerTip()
	{
		return this.GetRandomTip(TipType.MULTIPLAYER);
	}

	public string GetRandomFriendsTip()
	{
		return this.GetRandomTip(TipType.FRIENDS);
	}

	public string GetRandomTipForCurrentEvent()
	{
		if (RaceEventInfo.Instance == null || RaceEventInfo.Instance.CurrentEvent == null)
		{
			return this.GetRandomSingleplayerTip();
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsRaceTheWorldOrClubRaceEvent())
		{
			return this.GetRandomMultiplayerTip();
		}
		if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
		{
			return this.GetRandomFriendsTip();
		}
		return this.GetRandomSingleplayerTip();
	}
}
