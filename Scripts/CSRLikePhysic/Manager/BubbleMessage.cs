using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using I2.Loc;
using RTLTMPro;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BubbleMessage : MonoBehaviour
{
	private enum FlipCase
	{
		NONE,
		XAXIS,
		YAXIS,
		BOTH
	}

    public enum AnimState
	{
		BEFORE,
		APPEAR,
		DISPLAY,
		HIDE,
		FINISHED
	}

	public enum NippleDir
	{
		UP,
		DOWN
	}

	public delegate void OnDestroyDelegate(BubbleMessage zMessage);

	private const float ScreenEdgeGap = 0.15f;

	private const float AnimDuration = 0.2f;

	private const float YPosOffset = -0.07f;

	private float height;

	//private float width;

	public TextMeshProUGUI Text;

    public Image Backdrop;

    public AnimState CurrentState;

    public bool isGlowAnimating;

	//private float animGlowPosition;

	//private float glowFrequency = 1f;

	public bool ShouldGlowLoopColours;

	public OnDestroyDelegate OnDestroyEvent;

    public RectTransform Nipple;
	public AnimationCurve AnimCurve;

	private float AnimTimePos;

	private Vector3 FinalPosition;

	public bool PersistThroughGarageFade;

	public Vector3 GarageFadeZoomInPos;

	public Animator Animator;

	public float GlowXScaleFactor;

	public float GlowYScaleFactor;

	//private float glowAnimationZ = 0.04f;

    private List<TransformParentHolder> m_targeTransformParentHolders;

    private Transform _placeHolderTransform;
    //private bool m_useBackdrop;
	private void OnDestroy()
	{
		if (this.OnDestroyEvent != null)
		{
			this.OnDestroyEvent(this);
		}
		this.AnimCurve = null;
	}

	public void Create(string str, bool alreadyLocalised, Vector3 position, NippleDir nippleDir, float nipplePos, BubbleMessageConfig config,bool useBackdrop = false,
        bool showNipple = true, params RectTransform[] targetTransforms)
	{
        //if (config.PlayGlowSwipeAnimation)
        //{
        //    this.Animator.Play("glow");
        //}
        //else
        //{
        //    this.Animator.gameObject.SetActive(false);
        //}

	    //m_useBackdrop = useBackdrop;

        m_targeTransformParentHolders = new List<TransformParentHolder>();
	    for (int i = 0; i < targetTransforms.Length; i++)
	    {
	        var targetTransform = targetTransforms[i];
	        if (targetTransform != null)
	        {
	            var holder = new TransformParentHolder(targetTransform,i>0);
	            m_targeTransformParentHolders.Add(holder);
	            var layout = targetTransform.GetComponentInParent<LayoutGroup>();
	            if (layout != null)
	            {
	                layout.enabled = false;
	            }



	            holder.SetParent(transform.parent, true);

	            if (i == 0)
	            {
	                var button = targetTransform.GetComponentInChildren<Button>();
	                if (button != null)
	                    button.onClick.AddListener(InvokeTutorialBubbleManager);
	            }
	        }
	    }

	    Backdrop.gameObject.SetActive(useBackdrop);
		Backdrop.raycastTarget = useBackdrop;

		this.Text.fontSize = config.FontSize;
        this.Text.text = (!alreadyLocalised ? LocalizationManager.GetTranslation(str) : str);
        //this.width = GameObjectHelper.MakeLocalPositionPixelPerfect(this.Text.TotalWidth + 0.25f);
        //this.height = GameObjectHelper.MakeLocalPositionPixelPerfect((this.Text.BaseHeight + 0.05f) * (float)this.Text.GetDisplayLineCount() + 0.2f);
        nipplePos = this.AutoFitNippleToScreen(nipplePos, position);
        //this.PositionStuffForCurrentSize(nipplePos, nippleDir, config.PosType);
        //base.gameObject.transform.position = GameObjectHelper.MakeLocalPositionPixelPerfect(position);
        //this.FinalPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(position);

	    if (!gameObject.activeInHierarchy)
	        gameObject.SetActive(true);
	    CoroutineManager.Instance.StartCoroutine(_delayedPosition(nipplePos, position, nippleDir));
        //UnityEngine.Debug.Log("Set "+position);
		this.CurrentState = AnimState.APPEAR;
		this.SetForFade(0f);
		this.SetDefaultOneShot(0.9f);
        Nipple.gameObject.SetActive(showNipple);
        transform.SetAsLastSibling();
        Nipple.SetAsLastSibling();
	}

    //private void SetupActiveUIElements()
    //{
    //    m_targeTransformParentHolders = new List<TransformParentHolder>();
    //    for (int i = 0; i < targetTransforms.Length; i++)
    //    {
    //        var targetTransform = targetTransforms[i];
    //        if (targetTransform != null)
    //        {
    //            var holder = new TransformParentHolder(targetTransform, i > 0);
    //            m_targeTransformParentHolders.Add(holder);
    //            var layout = targetTransform.GetComponentInParent<LayoutGroup>();
    //            if (layout != null)
    //            {
    //                layout.enabled = false;
    //            }

    //            holder.SetParent(transform.parent, true);

    //            if (i == 0)
    //            {
    //                var button = targetTransform.GetComponentInChildren<Button>();
    //                if (button != null)
    //                    button.onClick.AddListener(InvokeTutorialBubbleManager);
    //            }
    //        }
    //    }
    //}


    //private IEnumerator KillPopupWhenAnimationEnd(PopUpDialogue popUpDialogue)
    //{
    //    var animator = popUpDialogue.GetComponent<Animator>();
    //    animator.Play("Close");
    //    var endStateReached = false;
    //    while (!endStateReached)
    //    {
    //        if (animator == null)
    //            break;
    //        if (!animator.IsInTransition(0))
    //        {
    //            var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
    //            endStateReached =
    //                stateInfo.IsName("Close")
    //                && stateInfo.normalizedTime >= 1;
    //        }
    //        yield return 0;
    //    }

    //    this.KillPopUp();
    //}

    private IEnumerator _delayedPosition(float nipplePos,Vector3 position
        , NippleDir nippleDir)
    {
        var recTransform = transform as RectTransform;
        var canvas = recTransform.GetComponentInParent<Canvas>();
        var scaleFactor = canvas.transform.localScale.x;
        yield return new WaitForEndOfFrame();
        var rectWidth = recTransform.rect.width * scaleFactor;
        var rectHeight = recTransform.rect.height * scaleFactor;
        var nippleHeight = Nipple.rect.height*scaleFactor;
        var x = position.x + rectWidth * (0.5F - nipplePos);
        float y;
        int sign = 1;
        if (nippleDir == NippleDir.UP)
        {
            Nipple.localScale = new Vector3(1, -1, 1);
            y = position.y - (rectHeight/2)-nippleHeight;
        }
        else
        {
            Nipple.localScale = new Vector3(1, 1, 1);
            y = position.y + (rectHeight/2)+nippleHeight;
            sign = -1;
        }

        var nipplePosition = recTransform.position = new Vector3(x, y, 0);
        nipplePosition.x += rectWidth * (nipplePos - 0.5F);
        nipplePosition.y += rectHeight/2*sign*0.8F;
        Nipple.position = nipplePosition;
        yield return new WaitForEndOfFrame();
        if (Animator != null)
        {
            Animator.Play("Bubble@Open");
        }
    }


    private void PositionBubble()
    {
        
    }

	public void MoveToGaragePosition()
	{
        //base.gameObject.transform.position = this.GarageFadeZoomInPos;
	}

	public void ResetToDefaultPosition()
	{
        //base.gameObject.transform.position = this.FinalPosition;
	}

	public Transform GetParentTransform()
	{
		return base.gameObject.transform.parent;
	}


    //private void MatchSizePositionAnchor(Image spr, Image target, bool ignoreSize = false)
    //{
    //    if (!ignoreSize)
    //    {
    //        spr.SetSize(target.width, target.height);
    //    }
    //    spr.SetAnchor(target.anchor);
    //    spr.transform.localPosition = target.transform.localPosition + new Vector3(0f, 0f, -0.01f);
    //}

	private float AutoFitNippleToScreen(float nipplePos, Vector3 position)
	{
	    return Mathf.Clamp(nipplePos, 0.1F, 0.9F);
	    //float x = GUICamera.Instance.ScreenToWorldSpace(new Vector2((float)Screen.width, 0f)).x;
	    //float x2 = GUICamera.Instance.ScreenToWorldSpace(Vector2.zero).x;
	    //float num = position.x + this.width * (1f - nipplePos) - x + 0.15f;
	    //float num2 = position.x - this.width * nipplePos - x2 - 0.15f;
	    //if (num > 0f)
	    //{
	    //    nipplePos += num / this.width;
	    //}
	    //else if (num2 < 0f)
	    //{
	    //    nipplePos += num2 / this.width;
	    //}
	    //return Mathf.Clamp01(nipplePos);
	}

	private void PositionStuffForCurrentSize(float nipplePos, NippleDir nippleDir, BubbleMessageConfig.PositionType position)
	{
	}

	private void Update()
	{
        //UnityEngine.Debug.Log(transform.position);
		switch (this.CurrentState)
		{
		case AnimState.APPEAR:
		{
			this.AnimTimePos += ((Time.deltaTime <= 0f) ? Time.fixedDeltaTime : Time.deltaTime);
			float num = Mathf.Min(this.AnimTimePos / 0.2f, 1f);
			if (this.AnimTimePos >= 0.2f)
			{
				num = 1f;
				this.AnimTimePos = 0f;
				this.CurrentState = AnimState.DISPLAY;
			}
			this.SetForFade(num);
			break;
		}
		case AnimState.DISPLAY:
			if (this.isGlowAnimating)
			{
                //this.UpdateGlowTint(1f);
			}
			break;
		case AnimState.HIDE:
		{
			this.AnimTimePos += ((Time.deltaTime <= 0f) ? Time.fixedDeltaTime : Time.deltaTime);
			float num = Mathf.Min(this.AnimTimePos / 0.2f, 1f);
			if (this.AnimTimePos >= 0.2f)
			{
				this.AnimTimePos = 0f;
				this.CurrentState = AnimState.FINISHED;
				return;
			}
			this.SetForFade(1f - num);
			break;
		}
		case AnimState.FINISHED:
			this.KillNow();
			break;
		}
	}

	public void SetPulsing(float frequency = 1f)
	{
		this.SetListOfGlowColours(new List<Color>
		{
			new Color(0.6039f, 0.9961f, 1f, 1f),
			new Color(0f, 0f, 0f, 0f)
		}, frequency);
		this.ShouldGlowLoopColours = true;
	}

	public void SetForRAINBOW(float frequency = 2f)
	{
		this.SetListOfGlowColours(new List<Color>
		{
			new Color(1f, 0f, 0f, 1f),
			new Color(0f, 1f, 0f, 1f),
			new Color(0f, 0f, 1f, 1f)
		}, frequency);
		this.ShouldGlowLoopColours = true;
	}

	public void SetDefaultOneShot(float frequency = 0.9f)
	{
		this.SetListOfGlowColours(new List<Color>
		{
			new Color(0.6039f, 0.9961f, 1f, 0.2f),
			new Color(0.6039f, 0.9961f, 1f, 1f),
			new Color(0.6039f, 0.9961f, 1f, 1f),
			new Color(0f, 0f, 0f, 0.5f)
		}, frequency);
		this.ShouldGlowLoopColours = false;
	}

	public void SetForFlatGlow(float frequency = 0f)
	{
		this.SetListOfGlowColours(new List<Color>
		{
			new Color(0.6039f, 0.9961f, 1f, 1f)
		}, frequency);
		this.ShouldGlowLoopColours = false;
	}

	public void SetListOfGlowColours(List<Color> ColorList, float frequency = 1f)
	{
		//this.animGlowPosition = 0f;
		this.isGlowAnimating = true;
		//this.glowFrequency = frequency;
	}

	public void SetForFade(float interp)
	{
		//float alphaValue = this.AnimCurve.Evaluate(interp);
		//Vector3 b = new Vector3(0f, (1f - interp) * -0.07f, 0f);
        //base.gameObject.transform.localPosition = this.FinalPosition + b;
	}

	public bool HasFinished()
	{
		return this.CurrentState == AnimState.FINISHED;
	}

	public void ShowNow()
	{
		this.CurrentState = AnimState.DISPLAY;
		base.gameObject.SetActive(true);
	}

	public void KillNow()
	{
	    if (m_targeTransformParentHolders != null)
	    {
	        for (int i = 0; i < m_targeTransformParentHolders.Count; i++)
	        {
	            var targeTransformParentHolder = m_targeTransformParentHolders[i];
	            targeTransformParentHolder.Reset(i>0);

	            var layout = targeTransformParentHolder.Target.GetComponentInParent<LayoutGroup>();
	            if (layout != null)
	            {
	                layout.enabled = true;
	            }

	            var button = targeTransformParentHolder.Target.GetComponentInChildren<Button>();
	            if (button != null)
	            {
	                button.onClick.RemoveListener(InvokeTutorialBubbleManager);
	            }
	        }
	    }

	    Destroy(base.gameObject.transform.parent.gameObject);
	}

	public void Dismiss()
	{
		if (this.CurrentState == AnimState.HIDE || this.CurrentState == AnimState.FINISHED)
		{
			return;
		}
		this.CurrentState = AnimState.HIDE;
		this.isGlowAnimating = false;
		this.AnimTimePos = 0f;
	}

    public void InvokeTutorialBubbleManager()
    {
        TutorialBubblesManager.Instance.TriggerEvent(TutorialBubblesEvent.ScreenTap);
    }

	[Conditional("UNITY_EDITOR")]
	private void OnDrawGizmos()
	{
		//Gizmos.color = new Color(1f, 0.6f, 0f);
		//Vector3 center = base.gameObject.transform.position + new Vector3(0f, this.height / 2f, 0f);
		//Gizmos.DrawWireCube(center, new Vector3(this.width, this.height, 0f));
	}

    public void SetActive(bool value)
    {
        transform.parent.gameObject.SetActive(value);
    }
}

public class TransformParentHolder
{
    public Transform Target { get; private set; }
    public int TargetSiblingIndex { get; private set; }
    public Vector2 TargetPosition { get; private set; }
    public Transform TargetParent { get; private set; }

    public Transform PlaceHolder { get; private set; }

    public TransformParentHolder(Transform target,bool setButtonInActive)
    {
        Target = target;
        TargetParent = Target.parent;
        TargetPosition = Target.rectTransform().anchoredPosition;
        TargetSiblingIndex = Target.GetSiblingIndex();
        //PlaceHolder = new GameObject(target.name + "_PlaceHolder").AddComponent<RectTransform>();

        if (setButtonInActive)
        {
            var button = Target.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.interactable = false;
            }
        }
    }

    public void SetParent(Transform parent, bool value)
    {
        //PlaceHolder.transform.SetParent(TargetParent,false);
        //PlaceHolder.transform.localPosition = Target.localPosition; 
        Target.SetParent(parent, true);
        Target.SetAsLastSibling();
    }

    public void Reset(bool setButtonActive)
    {
        Target.SetParent(TargetParent, true);
        Target.SetSiblingIndex(TargetSiblingIndex);
        Target.rectTransform().anchoredPosition = TargetPosition;
        if (setButtonActive)
        {
            var button = Target.GetComponentInChildren<Button>();
            if (button != null)
            {
                button.interactable = true;
            }
        }

        //if (PlaceHolder != null)
        //{
        //    GameObject.Destroy(PlaceHolder.gameObject);
        //}
    }
}
