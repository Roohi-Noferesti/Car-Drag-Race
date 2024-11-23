using System;
using UnityEngine;
using UnityEngine.UI;

public class FullScreenFlash : MonoBehaviour
{
	public float DurationOfProgressBarFlash;

	public AnimationCurve ProgressBarCurve;

	public AnimationCurve ExplosionCurve;

	public RawImage WhiteSprite;

	private float CurrentTime;

	private bool AnimationStoped = true;

	public CurveState CurrentState;

	private void Start()
	{
		this.SetSpriteSize();
	}

	private void Update()
	{
		if (!this.AnimationStoped)
		{
			this.CurrentTime += Time.deltaTime;
			float spriteAlpha = this.GiveAlphaFromCurve(this.CurrentTime);
			this.SetSpriteAlpha(spriteAlpha);
		}
		CurveState currentState = this.CurrentState;
		if (currentState != CurveState.ProgressBar)
		{
			if (currentState == CurveState.Explosion)
			{
				float time = this.ExplosionCurve[this.ExplosionCurve.length - 1].time;
				if (this.CurrentTime >= time)
				{
					this.StopFlashAnimation();
				}
			}
		}
		else if (this.CurrentTime >= this.DurationOfProgressBarFlash)
		{
			this.StopFlashAnimation();
		}
	}

	public void StartFlashAnimation()
	{
		this.AnimationStoped = false;
		base.gameObject.SetActive(true);
		this.CurrentTime = 0f;
	}

	public void StopFlashAnimation()
	{
		this.AnimationStoped = true;
		base.gameObject.SetActive(false);
	}

	public void SwitchToProgressBarState()
	{
		this.CurrentState = CurveState.ProgressBar;
	}

	public void SwitchToExplosionState()
	{
		this.CurrentState = CurveState.Explosion;
	}

	private float GiveAlphaFromCurve(float timeToRead)
	{
		float result = 0f;
		CurveState currentState = this.CurrentState;
		if (currentState != CurveState.ProgressBar)
		{
			if (currentState == CurveState.Explosion)
			{
				result = this.ExplosionCurve.Evaluate(timeToRead);
			}
		}
		else
		{
			result = this.ProgressBarCurve.Evaluate(timeToRead);
		}
		return result;
	}

	private void SetSpriteAlpha(float newAlphaValue)
	{
		Color white = Color.white;
		white.a = newAlphaValue;
        //this.WhiteSprite.renderer.material.SetColor("_Color", white);
	}

	public void SetSpritePosition(Vector3 spritePosition)
	{
		this.WhiteSprite.gameObject.transform.localPosition = spritePosition;
	}

	public void SetSpriteSize()
	{
        //float screenWidth = GUICamera.Instance.ScreenWidth;
        //float screenHeight = GUICamera.Instance.ScreenHeight;
        //this.WhiteSprite.Setup(screenWidth, screenHeight, Vector2.zero, Vector2.zero);
	}
}
