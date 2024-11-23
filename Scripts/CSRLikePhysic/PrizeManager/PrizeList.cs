using System;
using I2.Loc;
using UnityEngine;

public class PrizeList : MonoBehaviour
{
	public PrizeEntry[] Prizes;

	public Color PrizeCounterActiveColor;

	public Color PrizeCounterInactiveColor;

	public Color PrizeWonColor;

	public Color PrizeInactiveColor;

	public Color PrizeTextColorInactive;

	public Color PrizeTextColorWon;

	public Color PrizeCounterFlareColor;

	public Color PrizeListTintRGB = Color.white;

	public Color PrizeListGlowTintRGB = Color.white;

	public ParticleSystem PrizeParticleEmitter;

	public Transform WinCounterLocalObject;

	public ParticleSystem WinCounterEmitter;

	public PrizeLocal PrizeLocalObject;

	public float PrizeFlareAlpha;

	private int prizesWon;

	private bool didWin;

	private Color _prevPrizeListTint = Color.white;

	private Color _prevPrizeListGlowTint = Color.white;

	public void Initialize(int prizesWon, bool didWin)
	{
		this.prizesWon = prizesWon;
		this.didWin = didWin;
		int num = 0;
		for (int i = 0; i < this.Prizes.Length; i++)
		{
			PrizeEntry.eState state = PrizeEntry.eState.WonAlready;
			if (didWin)
			{
				if (i >= prizesWon)
				{
					state = PrizeEntry.eState.ToWin;
				}
				else if (i == prizesWon - 1 && didWin)
				{
					state = PrizeEntry.eState.Winning;
				}
			}
			else if (i >= prizesWon)
			{
				state = PrizeEntry.eState.Inactive;
			}
			else
			{
				state = PrizeEntry.eState.LosePrize;
			}
			int num2 = StreakManager.CashBonusForTier(i);
			num += StreakManager.CardBonusForTier(i);
			this.Prizes[i].Setup(state, num2, num);
			if (didWin && i == prizesWon - 1)
			{
				this.PrizeLocalObject.transform.localPosition = this.Prizes[i].transform.localPosition;
				this.PrizeLocalObject.SetPrize(num2);
				this.PrizeLocalObject.SetWidth(this.Prizes[i].Width);
			}
			string text = string.Format(LocalizationManager.GetTranslation("TEXT_MULTIPLAYER_RPSCREEN_WINCOUNT"), i + 1);
			this.Prizes[i].Count.SetText(text);
		}
	}

	public void Update()
	{
		if (this._prevPrizeListTint != this.PrizeListTintRGB)
		{
			PrizeEntry[] prizes = this.Prizes;
			for (int i = 0; i < prizes.Length; i++)
			{
				PrizeEntry prizeEntry = prizes[i];
				prizeEntry.SetRGBTint(this.PrizeListTintRGB);
			}
			this._prevPrizeListTint = this.PrizeListTintRGB;
		}
		if (this._prevPrizeListGlowTint != this.PrizeListGlowTintRGB)
		{
			PrizeEntry[] prizes2 = this.Prizes;
			for (int j = 0; j < prizes2.Length; j++)
			{
				PrizeEntry prizeEntry2 = prizes2[j];
				prizeEntry2.SetRGBGlowTint(this.PrizeListGlowTintRGB);
			}
			this._prevPrizeListGlowTint = this.PrizeListGlowTintRGB;
		}
	}

	public void PlayPrizeParticles()
	{
		this.PrizeParticleEmitter.Stop();
		this.PrizeParticleEmitter.Play();
	}

	public void PlayPrizeCounterPartilces()
	{
		this.WinCounterEmitter.Stop();
		this.WinCounterEmitter.Play();
	}

	public void DoPrizeRevealAnimation(int prize, bool WonPrize)
	{
		if (WonPrize)
		{
			this.PlayImpactNoise(prize);
		}
		this.Prizes[prize].Anim_Play();
		if (prize == this.prizesWon - 1 && this.didWin)
		{
            //AnimationUtils.PlayAnim(this.PrizeLocalObject.GetComponent<Animation>(), "RespectRankingScreen_RevealPrizeBar");
		}
	}

	public void Anim_Initialize()
	{
		PrizeEntry[] prizes = this.Prizes;
		for (int i = 0; i < prizes.Length; i++)
		{
			PrizeEntry prizeEntry = prizes[i];
			prizeEntry.Anim_Initialize();
		}
		this.PrizeLocalObject.Anim_Initialize();
	}

	public void Anim_Finish()
	{
		PrizeEntry[] prizes = this.Prizes;
		for (int i = 0; i < prizes.Length; i++)
		{
			PrizeEntry prizeEntry = prizes[i];
			prizeEntry.Anim_Finish();
		}
        //AnimationUtils.PlayLastFrame(this.PrizeLocalObject.GetComponent<Animation>(), "RespectRankingScreen_RevealPrizeBar");
		this.PrizeLocalObject.Anim_Finish();
	}

	private void PlayImpactNoise(int prize)
	{
        //prize++;
        //if (prize < this.prizesWon)
        //{
        //    string soundName = "WinStreakImpact" + prize.ToString();
        //    AudioManager.PlaySound(soundName, null);
        //}
        //if (prize == this.prizesWon)
        //{
        //    AudioManager.PlaySound("FinalImpact", null);
        //    AudioManager.FadeOut("Audio_SFXAudio_WinStreakStart", 1f, 0.5f);
        //}
	}
}
