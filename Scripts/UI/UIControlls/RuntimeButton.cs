using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class RuntimeButton : BaseRuntimeControl
{
    private Button m_button;
    private Animator m_animator;
    public UnityEvent ButtonAction;
    public List<GameObject> extraStuff = new List<GameObject>();


	public override Vector2 Size
	{
		get
		{
            var rect = this.rectTransform().rect;
            return new Vector2(rect.width, rect.height);
		}
	}

    public Button Button
    {
        get
        {
            if (m_button == null)
            {
                m_button = GetComponentInChildren<Button>();
            }
            return m_button;
        }
    }

    public Animator Animator
    {
        get
        {
            if (m_animator == null)
            {
                m_animator = GetComponent<Animator>();
            }
            return m_animator;
        }
    }

    public override void OnActivate()
	{
		base.CurrentState = BaseRuntimeControl.State.Active;
	}

    protected override void Awake()
    {
        base.Awake();
        Button.onClick.AddListener(OnPress);
        m_animator = GetComponent<Animator>();
    }

    public void AddValueChangedDelegate(UnityAction action)
    {
        this.ButtonAction.AddListener(action);
        //this.Button.onClick.AddListener(action);
    }

    public void RemoveValueChangedDelegate(UnityAction action)
    {
        this.ButtonAction.RemoveListener(action);
    }

	public override void OnDestroy()
	{

	}

	private void SetExtraStuffActive(bool state)
	{
		if (this.extraStuff == null)
		{
			return;
		}
		this.extraStuff.ForEach(delegate(GameObject go)
		{
			go.SetActive(state);
		});
	}

    protected override void Update()
    {

    }

    protected override void AdjustToBe(BaseRuntimeControl.State zState)
    {
        switch (zState)
        {
            case BaseRuntimeControl.State.Active:
                this.gameObject.SetActive(true);
                this.Button.interactable = true;
                this.SetExtraStuffActive(true);
                ToggleChanged("Normal");
                break;
            case BaseRuntimeControl.State.Pressed:
                this.gameObject.SetActive(true);
                this.Button.interactable = true;
                this.SetExtraStuffActive(true);
                ToggleChanged("Normal");
                break;
            case BaseRuntimeControl.State.Disabled:
                this.gameObject.SetActive(true);
                this.Button.interactable = false;
                this.SetExtraStuffActive(true);
                ToggleChanged("Disabled");
                break;
            case BaseRuntimeControl.State.Hidden:
                this.gameObject.SetActive(false);
                this.Button.interactable = false;
                this.SetExtraStuffActive(false);
                break;
            case BaseRuntimeControl.State.Highlight:
                this.gameObject.SetActive(true);
                this.Button.interactable = true;
                this.SetExtraStuffActive(true);
                ToggleChanged("Highlighted");
                break;
        }
    }

    protected void ToggleChanged(string value)
    {
        if (m_animator != null && m_animator.isInitialized)
        {
            m_animator.SetTrigger(value);
        }
    }

    public void SetCallback(UnityAction action)
	{
        ButtonAction.RemoveAllListeners();
	    ButtonAction.AddListener(action);
	}

	public void OnPress()
	{
        if (this.ButtonAction == null)
		{
			return;
		}
        ButtonAction.Invoke();

        //moeen just for debug. you can remove it for performance
  //   UnityEngine.Debug.Log("moeen button click function:::::>>>>>>" +  ButtonAction.GetPersistentMethodName(0));
 //    UnityEngine.Debug.Log("moeen button click target::::::>>>>>>>"+ ButtonAction.GetPersistentTarget(0).name);
    }

	public void Init(UnityAction action,List<GameObject> zExtras)
	{
		this.extraStuff = zExtras;
        ButtonAction.RemoveAllListeners();
        ButtonAction.AddListener(action);
		this.gameObject.SetActive(true);
	}

	public void Show(bool doShow)
	{
		base.gameObject.SetActive(doShow);
		if (doShow)
		{
			this.AdjustToBe(base.CurrentState);
		}
	}
}
