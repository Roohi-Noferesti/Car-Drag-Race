using System;
using System.Collections.Generic;

[Serializable]
public class StreakChainRP
{
	public class RPMultiplier
	{
		public int ChainLength;

		public float Multiplier;

		public float IncrementAmount;

		public int ChainLengthLimit = -1;

		public bool LimitIncrement = true;

		public int MaxChainLength
		{
			get
			{
				return (this.ChainLengthLimit >= 0) ? this.ChainLengthLimit : this.ChainLength;
			}
		}

		public bool HasRewardForChain(int chainlength)
		{
			return chainlength >= this.ChainLength && (chainlength <= this.MaxChainLength || !this.LimitIncrement);
		}

		public float GetMultiplierForChain(int requestedChainLength)
		{
			return this.Multiplier + this.IncrementAmount * (float)(requestedChainLength - this.ChainLength);
		}
	}

	public TimeSpan MultiplierDuration = TimeSpan.Zero;

	public List<RPMultiplier> ChainMultiplers;

	public float GetMultiplier(int chainLength)
	{
		if (this.ChainMultiplers != null)
		{
			RPMultiplier rPMultiplier = this.ChainMultiplers.Find((RPMultiplier x) => x.HasRewardForChain(chainLength));
			if (rPMultiplier != null)
			{
				return rPMultiplier.GetMultiplierForChain(chainLength);
			}
		}
		return 0f;
	}
}
