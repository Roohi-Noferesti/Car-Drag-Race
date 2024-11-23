using System;
using System.Collections.Generic;

public class AwardCarPrize : AwardPrizeBase
{
	private Reward carTypeReward;

	private string carToAward;

	private Dictionary<Reward, Action> prizeRemoval;

	public AwardCarPrize(Reward carReward)
	{
		Dictionary<Reward, Action> dictionary = new Dictionary<Reward, Action>();
		dictionary.Add(Reward.SportCarPart, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfSportsCarPiecesRemaining--;
		});
		dictionary.Add(Reward.DesiribleCarPart1, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfDesiribleCarPiecesRemaining--;
		});
		dictionary.Add(Reward.DesiribleCarPart2, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfDesiribleCarPiecesRemaining--;
		});
		dictionary.Add(Reward.CommonCarPart1, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfCommonCarPiecesRemaining--;
		});
		dictionary.Add(Reward.CommonCarPart2, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfCommonCarPiecesRemaining--;
		});
		dictionary.Add(Reward.CommonCarPart3, delegate
		{
			PlayerProfileManager.Instance.ActiveProfile.NumberOfCommonCarPiecesRemaining--;
		});
		this.prizeRemoval = dictionary;
        //base..ctor();
		this.carTypeReward = carReward;
		PrizeomaticCarPieceChooser.CARSLOT slot = PrizeomaticCarPieceChooser.ConvertRewardToCarslot(this.carTypeReward);
		this.carToAward = PrizeomaticCarPieceChooser.GetCarKey(slot);
	}

	public override void AwardPrize()
	{
        //PlayerProfileManager.Instance.ActiveProfile.AwardPrizeomaticCarPart(this.carToAward);
	}

	public override string GetMetricsTypeString()
	{
		return "Car Part";
	}

    public override string GetPrizeString()
    {
        throw new NotImplementedException();
    }

    public override void TakePrizeAwayFromProfile()
	{
		this.prizeRemoval[this.carTypeReward]();
	}

	public override string GetMetricsCarToAwardString()
	{
		return this.carToAward;
	}
}
