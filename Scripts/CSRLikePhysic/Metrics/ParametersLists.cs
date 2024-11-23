using System.Collections.Generic;

namespace Metrics
{
	public class ParametersLists
	{
		public static HashSet<Parameters> ValidForNMOnly = new HashSet<Parameters>
		{
			Parameters.Rlength,
			Parameters.Platform,
			Parameters.OpCr,
			Parameters.OpCrPP,
			Parameters.QNam,
			Parameters.Rwd
		};

		public static HashSet<Parameters> ValidForFlurry = new HashSet<Parameters>
		{
			Parameters.baid,
			Parameters.Platform,
			Parameters.openUDID,
			Parameters.mac,
			Parameters.bckt,
			Parameters.brnch,
			Parameters.tst,
			Parameters.lgdin,
			Parameters.Rname,
			Parameters.RDiff,
			Parameters.PFin,
			Parameters.OpFin,
			Parameters.PResult,
			Parameters.Pntrs,
			Parameters.Itm,
			Parameters.ItmClss,
			Parameters.Shortcut,
			Parameters.Upsl,
			Parameters.RecVer,
			Parameters.QStrt,
			Parameters.QCmp,
			Parameters.Anm,
			Parameters.frm,
			Parameters.to,
			Parameters.tme,
			Parameters.CmlCshPur,
			Parameters.CmlGldPur,
			Parameters.InvtTyp,
			Parameters.InvtP
		};

		public static List<Parameters> Empty = new List<Parameters>();

		public static List<Parameters> NotEnoughEventParameters = new List<Parameters>
		{
			Parameters.Ver,
			Parameters.Snapshot,
			Parameters.Platform,
			Parameters.brnch,
			Parameters.bckt,
			Parameters.tst,
			Parameters.Currency,
			Parameters.MlstRce,
			Parameters.Itm,
			Parameters.ItmClss,
			Parameters.YesNo,
			Parameters.CmlRCmp,
			Parameters.IAPBought,
			Parameters.PLvl,
			Parameters.StoreType,
			Parameters.StoreOption,
			Parameters.BGld,
			Parameters.BCsh,
			Parameters.DGld
		};

		public static List<Parameters> TutorialShowroomEventsParameters = new List<Parameters>
		{
			Parameters.PCr,
			Parameters.CostCash,
			Parameters.CostGold
		};

		public static List<Parameters> TutorialRacesEventsParameters = new List<Parameters>
		{
			Parameters.CmlRCmp,
			Parameters.PResult,
			Parameters.LaunchRPM,
			Parameters.TotalShifts,
			Parameters.Shift0,
			Parameters.Shift1,
			Parameters.Shift2,
			Parameters.Shift3,
			Parameters.Shift4,
			Parameters.Shift5,
			Parameters.Shift6,
			Parameters.Shift7,
			Parameters.Shift8,
			Parameters.Shift9
		};

		public static List<Parameters> CarDealEventsParameters = new List<Parameters>
		{
			Parameters.DealCar,
			Parameters.DealCarPrice,
			Parameters.DealCashback,
			Parameters.DealDiscount,
			Parameters.DealFreeUpgradeLevel,
			Parameters.BGld,
			Parameters.MlstRce
		};

		public static List<Parameters> ClickSwipeIAP = new List<Parameters>
		{
			Parameters.Store,
			Parameters.IAP,
			Parameters.DealCar,
			Parameters.DealCarPrice,
			Parameters.DealCashback,
			Parameters.DealDiscount,
			Parameters.DealFreeUpgradeLevel
		};

		public static List<Parameters> Profile = new List<Parameters>
		{
			Parameters.CSRID,
			Parameters.ProfileUDID,
			Parameters.GeneratedUDID,
			Parameters.DeviceModel,
			Parameters.DeviceSerial,
			Parameters.DeviceAndroidID,
			Parameters.DeviceTelephonyID
		};

		public static List<Parameters> AdsDefault = new List<Parameters>
		{
			Parameters.AdNwk,
			Parameters.AdLoc,
			Parameters.AdRwType,
			Parameters.AdRwAmt,
			Parameters.AdCfg,
			Parameters.MlstRce,
			Parameters.ItmClss,
			Parameters.Itm
		};
	}
}
