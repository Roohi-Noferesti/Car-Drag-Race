using System;
using UnityEngine;
using UnityEngine.UI;

public class FadeQuad : MonoBehaviour
{
	public AnimationCurve animCurve;

	protected Color _startColor;

	protected Color _endColor = Color.black;

	protected Color _currentColor;

	private float _currentDuration = 0.5f;

	private float _currentTime;

	private Action _completeCallBack;

	private bool _completed = true;

	private bool _scaleInitialized;

    private Image m_fadeImage;

	public static FadeQuad Instance
	{
		get;
		private set;
	}

	public bool Completed
	{
		get
		{
			return this._completed;
		}
	}

	protected virtual void Awake()
	{
		if (Instance != null)
		{
		}
		Instance = this;
		this.SetUp();
	}

	protected void SetUp()
	{
        this.m_fadeImage = base.GetComponent<Image>();
        this.m_fadeImage.enabled = false;
		this.CompleteCurrent();
	}

	public void ForceUpdateScale()
	{
		this._scaleInitialized = false;
	}

	protected virtual void Update()
	{
		if (!this._scaleInitialized)
		{
		    float pixelDensity = ResolutionManager.PixelDensity;
            //base.transform.localScale = new Vector3((float)BaseDevice.ActiveDevice.GetScreenWidth() / pixelDensity, (float)BaseDevice.ActiveDevice.GetScreenHeight() / pixelDensity, base.transform.localScale.z);
            this._scaleInitialized = true;
		}
		if (this._completed)
		{
			return;
		}
		this._currentTime += Time.deltaTime;
		if (this._currentTime > this._currentDuration)
		{
			this.CompleteCurrent();
			return;
		}
		float time = this._currentTime / this._currentDuration;
		this._currentColor = Color.Lerp(this._startColor, this._endColor, this.animCurve.Evaluate(time));
	    m_fadeImage.color = this._currentColor;
	}

	private void UpdateRenderer()
	{
		if (base.gameObject != null && this._endColor.a < 0.0002f)
		{
			m_fadeImage.enabled = false;
		}
	}

	private void CompleteCurrent()
	{
		this._completed = true;
		this._currentColor = this._endColor;
	    this.m_fadeImage.color = this._endColor;
		this.UpdateRenderer();
		if (this._completeCallBack != null)
		{
			this._completeCallBack();
		}
	}

	public void SetColor(Color color)
	{
		this._endColor = color;
		this._currentColor = color;
	    this.m_fadeImage.color = color;
		this._completed = true;
		this.UpdateRenderer();
	}

	public Color GetCurrentColor()
	{
        return this.m_fadeImage.color;
	}

	public void FadeTo(Color color, float duration)
	{
		this.FadeTo(color, duration, null);
	}

	public void FadeTo(Color color, float duration, Action callback)
	{
		this._completed = false;
		this._startColor = this._endColor;
		this._endColor = color;
		this._currentDuration = duration;
		this._currentTime = 0f;
		this._completeCallBack = callback;
        m_fadeImage.enabled = true;
		if (duration == 0f)
		{
			this.CompleteCurrent();
		}
	}
}
