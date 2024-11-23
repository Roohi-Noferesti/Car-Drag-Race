using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MapPinSelected : MonoBehaviour
{
	private enum eState
	{
		NotInitialised,
		In,
		Out,
		Looping
	}

	public enum eVisibility
	{
		VisibleWithoutArrow,
		Visible,
		Invisible
	}

	private const float crossFadeDuration = 0.2f;

	public List<Image> AllSprites;

	public GameObject Arrow;

	private float timeInState;

	private MapPinSelected.eState state;

	private Vector2 nextEventPosition;

	private MapPinSelected.eState State
	{
		get
		{
			return this.state;
		}
		set
		{
			this.timeInState = 0f;
			this.state = value;
		}
	}

	public void OnActivate()
	{
		this.SetState(MapPinSelected.eState.NotInitialised);
	}

	public void OnEventSelected(Vector2 zPosition)
	{
		this.nextEventPosition = zPosition;
		if (this.State == MapPinSelected.eState.NotInitialised)
		{
			this.SwitchToNextEvent();
			this.SetState(MapPinSelected.eState.In);
		}
		else
		{
			this.SetState(MapPinSelected.eState.Out);
		}
	}

	public void SetVisible(MapPinSelected.eVisibility visibility)
	{
		//bool enabled = visibility == MapPinSelected.eVisibility.Visible || visibility == MapPinSelected.eVisibility.VisibleWithoutArrow;
        //foreach (SpriteRoot current in this.AllSprites)
        //{
        //    current.renderer.enabled = enabled;
        //}
		if (this.Arrow != null)
		{
			this.ShowArrow(visibility == MapPinSelected.eVisibility.Visible);
		}
	}

	public void ShowArrow(bool show)
	{
		this.Arrow.SetActive(show);
        //this.Arrow.renderer.enabled = show;
	}

	private void SwitchToNextEvent()
	{
		base.transform.localPosition = new Vector3(this.nextEventPosition.x, this.nextEventPosition.y + -0.21f, -0.5f);
        //base.transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(base.transform.localPosition);
	}

	private void Update()
	{
		this.timeInState += Time.deltaTime;
		switch (this.state)
		{
		case MapPinSelected.eState.In:
		{
            //float num = base.animation["Pin_Selected_FadeIn"].length - base.animation["Pin_Selected_FadeIn"].time;
            //if (num <= 0.2f)
            //{
            //    this.SetState(MapPinSelected.eState.Looping);
            //}
			break;
		}
		case MapPinSelected.eState.Out:
            //if (!base.animation.isPlaying)
            //{
            //    this.SetState(MapPinSelected.eState.In);
            //}
			break;
		case MapPinSelected.eState.Looping:
            //if (!base.animation.isPlaying)
            //{
            //    base.animation.Play("Pin_Selected_Loop");
            //}
			break;
		}
        //foreach (SpriteRoot current in this.AllSprites)
        //{
        //    current.renderer.material.SetColor("_Tint", current.color);
        //}
	}

	private void SetState(MapPinSelected.eState zState)
	{
		switch (zState)
		{
		case MapPinSelected.eState.In:
			this.SwitchToNextEvent();
            //base.animation.Play("Pin_Selected_FadeIn");
            //base.animation["Pin_Selected_FadeIn"].speed = 1.6f;
			break;
		case MapPinSelected.eState.Out:
            //base.animation.CrossFade("Pin_Selected_FadeOut", 0.2f);
            //base.animation["Pin_Selected_FadeOut"].speed = 1.6f;
			break;
		case MapPinSelected.eState.Looping:
            //base.animation.CrossFade("Pin_Selected_Loop", 0.2f);
			break;
		}
		this.State = zState;
	}
}
