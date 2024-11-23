using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class NetworkReplayCompress
{
	private const string GRID_DATA_MAGIC_START = "G";

	private const string GRID_DATA_MAGIC_END = "g";

	private const string RACE_DATA_MAGIC_START = "R";

	private const string RACE_DATA_MAGIC_END = "r";

	private const string REPLAY_META_DATA_START = "M";

	private const string REPLAY_META_DATA_END = "m";

	private const string REPLAY_MAGIC = "Q";

	private const int GEAR_UP = 1;

	private const int GEAR_DOWN = 2;

	private const int NITROUS_PRESSED = 4;

	private const int CONSUMABLE_ENG = 1;

	private const int CONSUMABLE_NITROUS = 2;

	private const int CONSUMABLE_TYRE = 4;

	private const int CONSUMABLE_BLOGGER = 8;

	public static string CreateStringFromReplayData(PlayerReplay playerReplay)
	{
		string str = "Q|";
		str += CreateReplayMetaDataHeader(playerReplay);
		str += CreateReplayGridData(playerReplay.replayData);
		return str + CreateReplayRaceData(playerReplay.replayData);
	}

	public static PlayerReplay CreateReplayFromString(string replayString, out bool validReplay, PlayerInfo playerInfo)
	{
		string[] array = replayString.Split(new char[]
		{
			'|'
		});
		validReplay = true;
        if (playerInfo == null)
        {
            playerInfo = new RTWPlayerInfo();
        }
		PlayerReplay playerReplay = new PlayerReplay(playerInfo);
		if (!string.Equals(array[0], "Q"))
		{
			validReplay = false;
			return playerReplay;
		}
		if (array.Length != 13)
		{
			validReplay = false;
			return playerReplay;
		}
		bool flag = ParseReplayMetadata(ref playerReplay, array, 1);
		validReplay &= flag;
		flag = ParseReplayGridData(ref playerReplay.replayData, array, 7);
		validReplay &= flag;
		flag = ParseReplayRaceData(ref playerReplay.replayData, array, 10);
		validReplay &= flag;
		return playerReplay;
	}

	private static bool ParseReplayMetadata(ref PlayerReplay playerReplay, string[] replayChunks, int startIndex)
	{
		string a = replayChunks[startIndex++];
		if (!string.Equals(a, "M"))
		{
			return false;
		}
		string text = replayChunks[startIndex++];
		text = Base64ToHexString(text);
		if (text.Length != 16)
		{
			return false;
		}
		string value = text.Substring(0, 8);
		byte[] bytes = BitConverter.GetBytes(Convert.ToInt32(value, 16));
		int num = (int)bytes[0];
		RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
        if (playerReplay.playerInfo.DoesComponentExist<ConsumablePlayerInfoComponent>())
        {
            ConsumablePlayerInfoComponent component2 = playerReplay.playerInfo.GetComponent<ConsumablePlayerInfoComponent>();
            component2.ConsumableEngine = (((num & 1) <= 0) ? 0 : 1);
            component2.ConsumableTyre = (((num & 4) <= 0) ? 0 : 1);
            component2.ConsumableN2O = (((num & 2) <= 0) ? 0 : 1);
            component2.ConsumablePRAgent = (((num & 8) <= 0) ? 0 : 1);
        }
		if (playerReplay.playerInfo.DoesComponentExist<MechanicPlayerInfoComponent>())
		{
			MechanicPlayerInfoComponent component3 = playerReplay.playerInfo.GetComponent<MechanicPlayerInfoComponent>();
			component3.MechanicEnabled = ((num & 1) > 0);
		}
		Dictionary<eUpgradeType, CarUpgradeStatus> dictionary = new Dictionary<eUpgradeType, CarUpgradeStatus>();
		CarUpgradeSetup.SetDefaultUpgradeStatus(dictionary);
		dictionary[eUpgradeType.BODY].levelFitted = CarUpgradeStatus.Convert((int)bytes[1]);
		dictionary[eUpgradeType.INTAKE].levelFitted = CarUpgradeStatus.Convert((int)bytes[2]);
		dictionary[eUpgradeType.TURBO].levelFitted = CarUpgradeStatus.Convert((int)bytes[3]);
		string value2 = text.Substring(8, 8);
		byte[] bytes2 = BitConverter.GetBytes(Convert.ToInt32(value2, 16));
		dictionary[eUpgradeType.TYRES].levelFitted = CarUpgradeStatus.Convert((int)bytes2[0]);
		dictionary[eUpgradeType.TRANSMISSION].levelFitted = CarUpgradeStatus.Convert((int)bytes2[1]);
		dictionary[eUpgradeType.NITROUS].levelFitted = CarUpgradeStatus.Convert((int)bytes2[2]);
		dictionary[eUpgradeType.ENGINE].levelFitted = CarUpgradeStatus.Convert((int)bytes2[3]);
		component.CarUpgradeStatus = dictionary;
		string s = replayChunks[startIndex++];
		if (!float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out playerReplay.replayData.finishTime))
		{
			return false;
		}
		string s2 = replayChunks[startIndex++];
		if (!float.TryParse(s2, NumberStyles.Float, CultureInfo.InvariantCulture, out playerReplay.replayData.finishSpeed))
		{
			return false;
		}
		string carDBKey = replayChunks[startIndex++];
		component.CarDBKey = carDBKey;
		string a2 = replayChunks[startIndex];
		return string.Equals(a2, "m");
	}

	private static bool ParseReplayGridData(ref NetworkReplayData replayData, string[] replayChunks, int startIndex)
	{
		replayData.GridReplayData = new List<ReplayEvent>();
		string a = replayChunks[startIndex++];
		if (!string.Equals(a, "G"))
		{
			return false;
		}
		string text = replayChunks[startIndex++];
		if (string.IsNullOrEmpty(text))
		{
			return true;
		}
		text = Base64ToHexString(text);
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i += 4)
		{
			string value = text.Substring(i, 4);
			ushort num = Convert.ToUInt16(value, 16);
			bool throttle = (num & 32768) > 0;
			int frameIndex = (int)num & -32769;
			ReplayEvent item = default(ReplayEvent);
			item.Throttle = throttle;
			item.FrameIndex = frameIndex;
			item.GearUp = false;
			item.GearDown = false;
			item.NitrousDown = false;
			string text3 = text2;
			text2 = string.Concat(new object[]
			{
				text3,
				"Frame ",
				item.FrameIndex,
				": ",
				(!item.Throttle) ? "Throttle Off, " : "Throttle On, "
			});
			replayData.GridReplayData.Add(item);
		}
		string a2 = replayChunks[startIndex];
		return string.Equals(a2, "g");
	}

	private static bool ParseReplayRaceData(ref NetworkReplayData replayData, string[] replayChunks, int startIndex)
	{
		string a = replayChunks[startIndex++];
		if (!string.Equals(a, "R"))
		{
			return false;
		}
		string text = replayChunks[startIndex++];
		if (string.IsNullOrEmpty(text))
		{
			return false;
		}
		text = Base64ToHexString(text);
		if (text.Length % 6 != 0)
		{
			return false;
		}
		replayData.RaceReplayData = new List<ReplayEvent>(text.Length);
		string text2 = string.Empty;
		for (int i = 0; i < text.Length; i += 6)
		{
			ReplayEvent item = default(ReplayEvent);
			string value = text.Substring(i, 2);
			ushort num = Convert.ToUInt16(value, 16);
			item.Throttle = true;
			item.GearUp = ((num & 1) > 0);
			item.GearDown = ((num & 2) > 0);
			item.NitrousDown = ((num & 4) > 0);
			string value2 = text.Substring(i + 2, 4);
			ushort frameIndex = Convert.ToUInt16(value2, 16);
			item.FrameIndex = (int)frameIndex;
			string text3 = text2;
			text2 = string.Concat(new object[]
			{
				text3,
				"Frame ",
				item.FrameIndex,
				" : "
			});
			if (item.GearUp)
			{
				text2 += "Gear Up, ";
			}
			if (item.GearDown)
			{
				text2 += "Gear Down, ";
			}
			if (item.NitrousDown)
			{
				text2 += "Nitrous pressed, ";
			}
			replayData.RaceReplayData.Add(item);
		}
		string a2 = replayChunks[startIndex];
		return string.Equals(a2, "r");
	}

	private static string CreateReplayGridData(NetworkReplayData replayData)
	{
		string text = "G|";
		string text2 = string.Empty;
		foreach (ReplayEvent current in replayData.GridReplayData)
		{
			ushort num = (ushort)current.FrameIndex;
			if (current.Throttle)
			{
				num |= 32768;
			}
			text2 += string.Format("{0:X4}", num);
		}
		text2 = HexStringToBase64(text2);
		text = text + text2 + "|g|";
		return text;
	}

	private static string CreateReplayRaceData(NetworkReplayData replayData)
	{
		string text = "R|";
		string text2 = string.Empty;
		foreach (ReplayEvent current in replayData.RaceReplayData)
		{
			byte b = 0;
			if (current.GearUp)
			{
				b |= 1;
			}
			if (current.GearDown)
			{
				b |= 2;
			}
			if (current.NitrousDown)
			{
				b |= 4;
			}
			string str = string.Format("{0:X2}", b);
			text2 += str;
			ushort num = (ushort)current.FrameIndex;
			text2 += string.Format("{0:X4}", num);
		}
		text2 = HexStringToBase64(text2);
		text = text + text2 + "|r";
		return text;
	}

	private static string CreateReplayMetaDataHeader(PlayerReplay playerReplay)
	{
		string str = "M|";
		int num = 0;
		RacePlayerInfoComponent component = playerReplay.playerInfo.GetComponent<RacePlayerInfoComponent>();
        if (playerReplay.playerInfo.DoesComponentExist<ConsumablePlayerInfoComponent>())
        {
            ConsumablePlayerInfoComponent component2 = playerReplay.playerInfo.GetComponent<ConsumablePlayerInfoComponent>();
            if (component2.ConsumableEngine > 0)
            {
                num |= 1;
            }
            if (component2.ConsumableTyre > 0)
            {
                num |= 4;
            }
            if (component2.ConsumableN2O > 0)
            {
                num |= 2;
            }
            if (component2.ConsumablePRAgent > 0)
            {
                num |= 8;
            }
        }
		if (playerReplay.playerInfo.DoesComponentExist<MechanicPlayerInfoComponent>())
		{
			MechanicPlayerInfoComponent component3 = playerReplay.playerInfo.GetComponent<MechanicPlayerInfoComponent>();
			if (component3.MechanicEnabled)
			{
				num |= 1;
			}
		}
		Dictionary<eUpgradeType, CarUpgradeStatus> carUpgradeStatus = component.CarUpgradeStatus;
		byte[] value = new byte[]
		{
			(byte)num,
			carUpgradeStatus[eUpgradeType.BODY].levelFitted,
			carUpgradeStatus[eUpgradeType.INTAKE].levelFitted,
			carUpgradeStatus[eUpgradeType.TURBO].levelFitted
		};
		int num2 = BitConverter.ToInt32(value, 0);
		string text = string.Format("{0:X8}", num2);
		byte[] value2 = new byte[]
		{
			carUpgradeStatus[eUpgradeType.TYRES].levelFitted,
			carUpgradeStatus[eUpgradeType.TRANSMISSION].levelFitted,
			carUpgradeStatus[eUpgradeType.NITROUS].levelFitted,
			carUpgradeStatus[eUpgradeType.ENGINE].levelFitted
		};
		num2 = BitConverter.ToInt32(value2, 0);
		text += string.Format("{0:X8}", num2);
		text = HexStringToBase64(text);
		str = str + text + "|";
		str += playerReplay.replayData.finishTime.ToString(CultureInfo.InvariantCulture);
		str += "|";
		str += playerReplay.replayData.finishSpeed.ToString(CultureInfo.InvariantCulture);
		str += "|";
		str += component.CarDBKey;
		return str + "|m|";
	}

	public static string HexStringToBase64(string hexString)
	{
		int num = Mathf.CeilToInt((float)hexString.Length / 2f);
		byte[] array = new byte[num];
		string str = string.Empty;
		int i;
		for (i = 0; i < hexString.Length - 1; i += 2)
		{
			int value = Convert.ToInt32(hexString.Substring(i, 2), 16);
			array[i / 2] = Convert.ToByte(value);
		}
		if (i == hexString.Length - 1)
		{
			int num2 = Convert.ToInt32(hexString.Substring(i, 1), 16);
			array[(i + 1) / 2] = Convert.ToByte(num2 << 4);
			str = ".";
		}
		return Convert.ToBase64String(array) + str;
	}

	public static string Base64ToHexString(string stringb64)
	{
		string result;
		try
		{
			bool flag = false;
			if (stringb64.Substring(stringb64.Length - 1) == ".")
			{
				flag = true;
				stringb64 = stringb64.Substring(0, stringb64.Length - 1);
			}
			byte[] array = Convert.FromBase64String(stringb64);
			string text = string.Empty;
			for (int i = 0; i < array.Length; i++)
			{
				text += string.Format("{0:X2}", Convert.ToInt32(array[i]));
			}
			if (flag)
			{
				text = text.Substring(0, text.Length - 1);
			}
			result = text;
		}
		catch (ArgumentOutOfRangeException)// var_4_94)
		{
			result = "error";
		}
		return result;
	}
}
