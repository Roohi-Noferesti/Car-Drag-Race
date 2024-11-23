using UnityEngine;
using UnityEngine.UI;

public class MenuGradientBackground : MonoBehaviour
{
	public AnimationCurve EaseCurve;

	public Shader OpaqueShader;

	public Shader BlendShader;

	private Image _sprite;

	private Color _startColor;

	private float _duration;

	private float _timer;

	private Color _endColor;

	private bool dimensionsSet;

	private void Awake()
	{
		this.dimensionsSet = false;
		this.Hide();
	}

	public void Start()
	{
		this.SetupDimensions();
	}

	private void SetupDimensions()
	{
		if (!this.dimensionsSet)
		{
            //this._sprite = base.GetComponent<Image>();
            //this._sprite.Setup(GUICamera.Instance.ScreenWidth, GUICamera.Instance.ScreenHeight, new Vector2(0f, (float)BaseDevice.ActiveDevice.GetScreenHeight() - 1f), new Vector2((float)BaseDevice.ActiveDevice.GetScreenWidth(), (float)BaseDevice.ActiveDevice.GetScreenHeight()));
			this.dimensionsSet = true;
		}
	}

	public void ForceSetupDimensions()
	{
		this.dimensionsSet = false;
		this.SetupDimensions();
	}

	public void Show(bool useBlendShader = false)
	{
		this.SetupDimensions();
		base.enabled = false;
		base.GetComponent<Renderer>().enabled = true;
		if (useBlendShader)
		{
			base.GetComponent<Renderer>().material.shader = this.BlendShader;
		}
		else
		{
			base.GetComponent<Renderer>().material.shader = this.OpaqueShader;
		}
	}

	public void Hide()
	{
		base.enabled = false;
		base.GetComponent<Renderer>().enabled = false;
		this._timer = float.PositiveInfinity;
		base.GetComponent<Renderer>().material.shader = this.OpaqueShader;
	}

	public void SetColour(Color backgroundColour)
	{
		base.GetComponent<Renderer>().material.SetColor("_Tint", backgroundColour);
	}

	public void SetFade(float backgroundFade = 0.5f)
	{
		base.GetComponent<Renderer>().material.SetFloat("_Fade", backgroundFade);
	}

	public void FadeIn(float duration, float delay)
	{
		base.enabled = true;
		base.GetComponent<Renderer>().enabled = true;
		base.GetComponent<Renderer>().material.shader = this.BlendShader;
		this._startColor = base.GetComponent<Renderer>().material.GetColor("_Tint");
		this._startColor.a = 0f;
		this._endColor = new Color(this._startColor.r, this._startColor.g, this._startColor.b, 1f);
		this._duration = duration;
		this._timer = -delay;
		this.UpdateAnim(0f);
	}

	public void FadeOut(float duration, float delay)
	{
		base.enabled = true;
		base.GetComponent<Renderer>().enabled = true;
		base.GetComponent<Renderer>().material.shader = this.BlendShader;
		this._startColor = base.GetComponent<Renderer>().material.GetColor("_Tint");
		this._startColor.a = 1f;
		this._endColor = new Color(this._startColor.r, this._startColor.g, this._startColor.b, 0f);
		this._duration = duration;
		this._timer = -delay;
		this.UpdateAnim(0f);
	}

	public void FadeOutIfRequired(float duration, float delay)
	{
		if (this.CurrentColor().a > 0f)
		{
			this.FadeOut(duration, delay);
		}
	}

	private void Update()
	{
		if (this._timer < this._duration)
		{
			this.UpdateAnim(Time.deltaTime);
		}
	}

	private Color CurrentColor()
	{
		base.GetComponent<Renderer>().material.shader = this.BlendShader;
		return base.GetComponent<Renderer>().material.GetColor("_Tint");
	}

	private Color DesiredColor()
	{
		float num = this._timer / this._duration;
		if (num < 0f)
		{
			num = 0f;
		}
		float t = this.EaseCurve.Evaluate(num);
		return Color.Lerp(this._startColor, this._endColor, t);
	}

	private void UpdateAnim(float deltaTime)
	{
		this._timer += deltaTime;
		if (this._timer > this._duration)
		{
			this._timer = this._duration;
		}
		Color color = this.DesiredColor();
		base.GetComponent<Renderer>().material.SetColor("_Tint", color);
		if (this._timer == this._duration)
		{
			base.enabled = false;
			if (color.a == 0f)
			{
				base.GetComponent<Renderer>().enabled = false;
				base.GetComponent<Renderer>().material.shader = this.OpaqueShader;
			}
			else if (color.a == 1f)
			{
				base.GetComponent<Renderer>().material.shader = this.OpaqueShader;
			}
			else
			{
				base.GetComponent<Renderer>().material.shader = this.BlendShader;
			}
		}
	}
}
