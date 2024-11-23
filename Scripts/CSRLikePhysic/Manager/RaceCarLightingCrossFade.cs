using System;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class RaceCarLightingCrossFade : MonoBehaviour
{
	private CarVisuals _targetCar;

	public bool PlayFromAnimation = true;

	public Color AmbientLightColor;

	public CarVisuals TargetCar
	{
		get
		{
			return this._targetCar;
		}
		set
		{
			this._targetCar = value;
			foreach (RaceCarLightingProcessor current in this.LightingProcessors)
			{
				current.TargetCar = value;
			}
			base.enabled = (this._targetCar != null);
		}
	}

	public List<RaceCarLightingProcessor> LightingProcessors
	{
		get;
		private set;
	}

	private void Awake()
	{
		this.FindLightingProcessors();
		base.enabled = false;
	}

	private void OnEnable()
	{
		if (this.TargetCar == null)
		{
			base.enabled = false;
		}
	}

	private void Start()
	{
		if (RaceEnvironmentSettings.Instance != null)
		{
			this.AmbientLightColor = this.TargetCar.AmbientLight.color;
		}
		if (base.GetComponent<Animation>())
		{
			AnimationState animationState = base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name];
			animationState.enabled = true;
			animationState.speed = 0f;
			animationState.time = 0f;
			base.GetComponent<Animation>().Play();
		}
	}

	private void FindLightingProcessors()
	{
		RaceCarLightingProcessor[] components = base.GetComponents<RaceCarLightingProcessor>();
		if (this.LightingProcessors == null)
		{
			this.LightingProcessors = new List<RaceCarLightingProcessor>(components);
			if (this.LightingProcessors.Count > 0)
			{
				this.LightingProcessors[0].Weight = 1f;
			}
			return;
		}
		if (this.LightingProcessors.Count != components.Length)
		{
			this.LightingProcessors = new List<RaceCarLightingProcessor>(components);
		}
	}

	private void Update()
	{
		if (this.TargetCar == null)
		{
			return;
		}
		if (base.GetComponent<Animation>() != null)
		{
			if (this.PlayFromAnimation && !base.GetComponent<Animation>().isPlaying)
			{
				base.GetComponent<Animation>().Play();
			}
			else if (!this.PlayFromAnimation && base.GetComponent<Animation>().isPlaying)
			{
				AnimationState animationState = base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name];
				animationState.speed = 1f;
				base.GetComponent<Animation>().Stop();
			}
			if (this.PlayFromAnimation)
			{
				float num = this.TargetCar.transform.position.z / 800f;
				AnimationState animationState2 = base.GetComponent<Animation>()[base.GetComponent<Animation>().clip.name];
				animationState2.time = 8f * num;
			}
		}
	}

	private void LateUpdate()
	{
		if (this.TargetCar == null)
		{
			return;
		}
		Vector3 vector = Vector3.zero;
		Vector3 vector2 = Vector3.zero;
		Color color = Color.black;
		float num = 0f;
		foreach (RaceCarLightingProcessor current in this.LightingProcessors)
		{
			vector += current.ResultLight.transform.position * current.Weight;
			vector2 += current.ResultLight.transform.eulerAngles * current.Weight;
			color += current.ResultLight.color * current.Weight;
			num += current.ResultLight.intensity * current.Weight;
		}
		this.TargetCar.DirectionalLight.transform.position = vector;
		this.TargetCar.DirectionalLight.transform.localEulerAngles = vector2;
		this.TargetCar.DirectionalLight.color = color;
		this.TargetCar.DirectionalLight.intensity = num;
		this.TargetCar.AmbientLight.color = this.AmbientLightColor;
	}

	public void UpdateWeight(RaceCarLightingProcessor processor, float weight)
	{
		float num = 1f - processor.Weight;
		float num2 = processor.Weight - weight;
		processor.Weight = weight;
		foreach (RaceCarLightingProcessor current in this.LightingProcessors)
		{
			if (!(current == processor))
			{
				float num3 = 1f;
				if (num != 0f)
				{
					num3 = current.Weight / num;
				}
				current.Weight = Mathf.Clamp01(current.Weight + num3 * num2);
			}
		}
	}
}
