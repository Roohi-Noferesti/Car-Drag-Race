using System;
using UnityEngine;

public class MeshFadeQude : MonoBehaviour
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

    public Renderer FadeQuadeRenderer;

    private Collider m_collider;

    public FadeQuadLoad.FadeState FadeState { get; private set; }

	public bool Completed
	{
		get
		{
			return this._completed;
		}
	}

	protected virtual void Awake()
	{
		this.SetUp();
	}

	protected void SetUp()
	{
        //this.FadeQuadeRenderer.enabled = false;
		this.CompleteCurrent();
        FadeState = FadeQuadLoad.FadeState.fadeOut;
	    m_collider = GetComponent<Collider>();
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
	    FadeQuadeRenderer.material.color = this._currentColor;
	}

	private void UpdateRenderer()
	{
		if (base.gameObject != null && this._endColor.a < 0.0002f)
		{
            FadeQuadeRenderer.enabled = false;
            m_collider.enabled = false;
		}
	}

	private void CompleteCurrent()
	{
		this._completed = true;
		this._currentColor = this._endColor;
	    this.FadeQuadeRenderer.material.color = this._endColor;
        FadeState = FadeQuadLoad.FadeState.fadeComplete;
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
	    this.FadeQuadeRenderer.material.color = color;
        //this._completed = true;
		this.UpdateRenderer();
	}

	public Color GetCurrentColor()
	{
        return this.FadeQuadeRenderer.material.color;
	}

	public void FadeTo(Color color, float duration)
	{
	    if (color.a <0.5F)
	    {
	        FadeState = FadeQuadLoad.FadeState.fadeOut;
	    }
        else if (color.a > 0.5F)
        {
            FadeState = FadeQuadLoad.FadeState.fadeToBlack;
        }
	    this.FadeTo(color, duration, null);
	}

	public void FadeTo(Color color, float duration, Action callback)
	{
	    if (!gameObject.activeSelf)
	        gameObject.SetActive(true);
		this._completed = false;
		this._startColor = this._endColor;
		this._endColor = color;
		this._currentDuration = duration;
		this._currentTime = 0f;
		this._completeCallBack = callback;
        FadeQuadeRenderer.enabled = true;
        m_collider.enabled = true;
		if (duration == 0f)
		{
			this.CompleteCurrent();
		}
	}

    public void SetToBlack()
    {
        gameObject.SetActive(true);
        this._endColor = new Color(0, 0, 0, 1);
        FadeQuadeRenderer.enabled = true;
        if (m_collider!=null)
            m_collider.enabled = true;
        FadeQuadeRenderer.material.color = _endColor;
    }
}
