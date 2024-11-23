using System;

public class SimpleLensFlare : FlareManager
{
	public float AnimatedOffset;

	public float AnimatedAlpha;

	private float _prevOffset = -1f;

	private float _prevAlpha = -1f;

	public void Startup()
	{
		base.FadeColorsMultiply(0f);
		base.UpdateColors();
	}

	public void Update()
	{
		if (this._prevOffset != this.AnimatedOffset)
		{
			base.UpdateAnimation(this.AnimatedOffset);
			this._prevOffset = this.AnimatedOffset;
		}
		if (this._prevAlpha != this.AnimatedAlpha)
		{
			base.FadeColorsMultiply(this.AnimatedAlpha);
			base.UpdateColors();
			this._prevAlpha = this.AnimatedAlpha;
		}
	}
}
