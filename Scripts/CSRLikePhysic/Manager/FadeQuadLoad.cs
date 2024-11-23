using UnityEngine;

public class FadeQuadLoad : MonoBehaviour
{
	public enum FadeState
	{
		fadeToBlack,
		fadeOut,
		fadeComplete
	}

	public string ColourName = "_Alpha";

	public string SaturationName = "_Value";

	public string OuterFadeValue = "_Fade";

	private FadeState state;

	private float fadeVal;

	private float fadeOver;

	private float fadeTime;

	private Color fadeColor = Color.black;

	public bool IsFading
	{
		get
		{
			return base.gameObject.activeInHierarchy && this.state != FadeState.fadeComplete;
		}
	}

	private void Update()
	{
		this.DoFade();
	}

	public void DoFade()
	{
		switch (this.state)
		{
		case FadeState.fadeToBlack:
			this.fadeTime += ((Time.deltaTime <= 0f) ? Time.fixedDeltaTime : Time.deltaTime);
			this.fadeVal = Mathf.Clamp01(this.fadeTime / this.fadeOver);
			if (this.fadeTime > this.fadeOver)
			{
				this.state = FadeState.fadeComplete;
			}
			goto IL_F1;
		case FadeState.fadeOut:
			this.fadeTime += ((Time.deltaTime <= 0f) ? Time.fixedDeltaTime : Time.deltaTime);
			this.fadeVal = 1f - Mathf.Clamp01(this.fadeTime / this.fadeOver);
			if (this.fadeTime > this.fadeOver)
			{
				base.gameObject.SetActive(false);
				this.state = FadeState.fadeComplete;
			}
			goto IL_F1;
		case FadeState.fadeComplete:
			return;
		}
		return;
		IL_F1:
		this.fadeColor.a = this.fadeVal;
		Material material = base.GetComponent<MeshRenderer>().material;
		material.SetColor(this.ColourName, this.fadeColor);
		material.SetFloat(this.SaturationName, Mathf.Clamp01((this.fadeVal - 0.5f) * 2f));
		material.SetFloat(this.OuterFadeValue, 0.5f);
	}

	public void StartFade(float inFadeOver, FadeState inState)
	{
		this.state = inState;
		this.fadeVal = ((this.state != FadeState.fadeToBlack) ? 1f : 0f);
		this.fadeOver = inFadeOver;
		this.fadeTime = 0f;
		if (inFadeOver == 0f)
		{
			this.fadeVal = 1f - this.fadeVal;
		}
		base.GetComponent<Renderer>().material.SetFloat("_Fade", this.fadeVal);
		base.gameObject.SetActive(true);
		this.DoFade();
	}

	public void Transparent()
	{
		this.state = FadeState.fadeComplete;
		this.fadeColor.a = 0f;
		base.GetComponent<Renderer>().material.SetColor(this.ColourName, this.fadeColor);
	}

	public void Opaque()
	{
		this.state = FadeState.fadeComplete;
		this.fadeColor.a = 1f;
		base.GetComponent<Renderer>().material.SetColor(this.ColourName, this.fadeColor);
	}

	public void OpaquePattern()
	{
		this.state = FadeState.fadeComplete;
		this.fadeColor.a = 1f;
		base.GetComponent<Renderer>().material.SetColor(this.ColourName, this.fadeColor);
		base.GetComponent<Renderer>().material.SetFloat(this.SaturationName, 1f);
	}

	public void ForceEndFade()
	{
		base.gameObject.SetActive(false);
		this.state = FadeState.fadeComplete;
	}
}
