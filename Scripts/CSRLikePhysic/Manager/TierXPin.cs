using DataSerialization;
using System;
using System.Collections.Generic;
using System.Linq;
using KingKodeStudio;
using UnityEngine;

public class TierXPin : MonoBehaviour
{
	[NonSerialized]
	public PinDetail pinDetails;

	public UnityEngine.Vector2 PinSpacePosition;

	public UnityEngine.Vector2 PinSpaceOffset;

	public UnityEngine.Vector2 PositionOffset;

	public UnityEngine.Vector3 WorldOffset;

	private List<GameObject> attachedGameObjects = new List<GameObject>();

	private List<Component> attachedComponents = new List<Component>();

	private List<string> animations = new List<string>();

	private bool interactionsEnabled = true;

	public bool InteractionsEnabled
	{
		get
		{
			return this.interactionsEnabled;
		}
	}

	public void DisableInteractions()
	{
		this.interactionsEnabled = false;
	}

	public void EnableInteractions()
	{
		this.interactionsEnabled = true;
	}

	public void SetPinDetails(PinDetail details)
	{
		this.pinDetails = details;
        //this.PinSpacePosition = this.pinDetails.Position.AsUnityVector2();
		this.PinSpaceOffset = UnityEngine.Vector2.zero;
        //this.PositionOffset = this.pinDetails.PositionOffset.AsUnityVector2();
		this.WorldOffset = UnityEngine.Vector3.zero;
	}

	public void OnActiveState()
	{
		base.gameObject.SetActive(true);
	}

	public void OnInActiveState()
	{
		base.gameObject.SetActive(false);
	}

	public void LoadTheme(Action onThemeLoad=null)
	{
		if (!string.IsNullOrEmpty(this.pinDetails.ClickAction.themeToLoad))
		{
			if (this.pinDetails.ClickAction.themeToLoad == "TierX_International_CarChoice" && !PlayerProfileManager.Instance.ActiveProfile.HasSeenInternationalIntroScreen)
			{
                ScreenManager.Instance.PushScreen(ScreenID.InternationalUnlock);
            }
            else
			{
				TierXManager.Instance.LoadTheme(this.pinDetails.ClickAction.themeToLoad, delegate
                {
                    onThemeLoad?.Invoke();
                }, this.pinDetails.ClickAction.themeOption);
			}
		}
	}

	public void SetupPinAnimation(PinAnimationDetail pinAnimationDetail, GameObject pinAnimationGO)
	{
		foreach (Transform transform in pinAnimationGO.transform)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(transform.gameObject, UnityEngine.Vector3.zero, Quaternion.identity) as GameObject;
			gameObject.transform.parent = base.transform;
			gameObject.transform.localPosition = transform.transform.localPosition;
			gameObject.transform.localScale = transform.transform.localScale;
			gameObject.transform.localRotation = transform.transform.localRotation;
			gameObject.name = transform.name;
			this.attachedGameObjects.Add(gameObject);
		}
		IEnumerable<Type> source = from c in base.gameObject.GetComponents<Component>()
		select c.GetType();
		Component[] components = pinAnimationGO.GetComponents<Component>();
        for (int i = 0; i < components.Length; i++)
		{
            Component component = components[i];
			if (!source.Contains(component.GetType()))
			{
				this.attachedComponents.Add(base.gameObject.AddComponent(component.GetType()));
			}
		}
		base.GetComponent<Animation>().AddClip(pinAnimationGO.GetComponent<Animation>().clip, pinAnimationDetail.Name);
		this.animations.Add(pinAnimationDetail.Name);
	}

	public void RemoveAttachments()
	{
		foreach (string current in this.animations)
		{
			base.GetComponent<Animation>().RemoveClip(current);
		}
		this.animations.Clear();
		foreach (Component current2 in this.attachedComponents)
		{
			UnityEngine.Object.Destroy(current2);
		}
		this.attachedComponents.Clear();
		foreach (GameObject current3 in this.attachedGameObjects)
		{
			UnityEngine.Object.Destroy(current3);
		}
		this.attachedGameObjects.Clear();
	}

    //private void LateUpdate()
    //{
    //    if (TierXManager.Instance.IsJsonLoaded)
    //    {
    //        CareerModeMapEventSelect.PositionPinOnScreen(base.gameObject, this.PinSpacePosition + this.PinSpaceOffset, this.PositionOffset);
    //        base.gameObject.transform.Translate(this.WorldOffset);
    //    }
    //}
}
