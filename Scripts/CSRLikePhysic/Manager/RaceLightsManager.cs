using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("GT/Logic/RaceLightsManager")]
public class RaceLightsManager : MonoBehaviour
{
	public static RaceLightsManager instance;

	public float lightIntensity = 0.7f;

	public float maxLightIntensity = 1.5f;

	public float minLightIntensity = 0.3f;

	public float desaturation = 1f;

	public List<Vector3> humanStreamPositions;

	public Color humanStreamColor;

	public List<Vector3> aiStreamPositions;

	public Color aiStreamColor;

	//private GameObject humanCar;

	//private GameObject aiCar;

	//private int currentHumanLightIdx;

	//private int currentAILightIdx;

	public void Start()
	{
		instance = this;
		base.gameObject.SetActive(false);
		//this.currentHumanLightIdx = -1;
		//this.currentAILightIdx = -1;
	}

	private void Update()
	{
		//if (this.humanCar != null)
		//{
		//	Vector3 position = default(Vector3);
		//	float num = 1f;
		//	this.currentHumanLightIdx = this.FindBestLight(this.humanCar.transform.position, this.humanStreamPositions, this.currentHumanLightIdx);
		//	this.StreamUpdate(this.humanCar.transform.position, this.humanStreamPositions, this.currentHumanLightIdx, out position, out num);
		//	RaceEnvironmentSettings.Instance.CarHumanLight.transform.position = position;
		//	RaceEnvironmentSettings.Instance.CarHumanLight.intensity = num * this.lightIntensity;
		//	RaceEnvironmentSettings.Instance.CarHumanLight.intensity = Mathf.Clamp(RaceEnvironmentSettings.Instance.CarHumanLight.intensity, this.minLightIntensity, this.maxLightIntensity);
		//	RaceEnvironmentSettings.Instance.CarHumanLight.transform.LookAt(this.humanCar.transform.position);
		//}
		//if (this.aiCar != null)
		//{
		//	Vector3 position2 = default(Vector3);
		//	float num2 = 1f;
		//	this.currentAILightIdx = this.FindBestLight(this.aiCar.transform.position, this.aiStreamPositions, this.currentAILightIdx);
		//	this.StreamUpdate(this.aiCar.transform.position, this.aiStreamPositions, this.currentAILightIdx, out position2, out num2);
		//	RaceEnvironmentSettings.Instance.CarAiLight.transform.position = position2;
		//	RaceEnvironmentSettings.Instance.CarAiLight.intensity = num2 * this.lightIntensity;
		//	RaceEnvironmentSettings.Instance.CarAiLight.intensity = Mathf.Clamp(RaceEnvironmentSettings.Instance.CarAiLight.intensity, this.minLightIntensity, this.maxLightIntensity);
		//	RaceEnvironmentSettings.Instance.CarAiLight.transform.LookAt(this.aiCar.transform.position);
		//}
	}

	public void ResetScene()
	{
        //this.humanCar = CompetitorManager.Instance.LocalCompetitor.CarPhysics.gameObject;
        //this.currentHumanLightIdx = this.FindBestLight(this.humanCar.transform.position, this.humanStreamPositions, -1);
        //RaceEnvironmentSettings.Instance.CarHumanLight.color = this.humanStreamColor;
        //RaceCompetitor otherCompetitor = CompetitorManager.Instance.OtherCompetitor;
        //if (otherCompetitor != null)
        //{
        //    this.aiCar = otherCompetitor.CarPhysics.gameObject;
        //    this.currentAILightIdx = this.FindBestLight(this.aiCar.transform.position, this.aiStreamPositions, -1);
        //    RaceEnvironmentSettings.Instance.CarAiLight.color = this.aiStreamColor;
        //}
        //if (this.humanStreamPositions.Count > 0 && this.aiStreamPositions.Count > 0)
        //{
        //    base.gameObject.SetActive(true);
        //}
	}

	private int FindBestLight(Vector3 position, List<Vector3> stream, int currentLightIdx)
	{
		int result = -1;
		float num = 3.40282347E+38f;
		int num2 = 0;
		int num3 = stream.Count;
		if (currentLightIdx >= 0)
		{
			num2 = Mathf.Max(0, currentLightIdx - 3);
			num3 = Mathf.Min(stream.Count, currentLightIdx + 3);
		}
		for (int i = num2; i < num3; i++)
		{
			Vector3 vector = stream[i] - position;
			if (vector.magnitude < num)
			{
				result = i;
				num = vector.magnitude;
			}
		}
		return result;
	}

	private void StreamUpdate(Vector3 position, List<Vector3> stream, int bestLightIdx, out Vector3 streamPos, out float intensity)
	{
		Vector3 vector = stream[bestLightIdx];
		streamPos = vector;
		intensity = 1f;
		int num = stream.Count;
		int num2;
		if (position.z < vector.z)
		{
			num2 = bestLightIdx - 1;
			num = bestLightIdx;
		}
		else
		{
			num2 = bestLightIdx;
			num = bestLightIdx + 1;
		}
		if (num2 < 0 || num >= stream.Count)
		{
			return;
		}
		Vector3 vector2 = stream[num2];
		Vector3 vector3 = stream[num];
		float num3 = vector3.z - vector2.z;
		float num4 = position.z - vector2.z;
		float num5 = num4 / num3;
		Vector3 vector4 = vector2 * (1f - num5);
		vector4 += vector3 * num5;
		streamPos = vector4;
		if ((double)num5 < 0.5)
		{
			intensity = 1f - 2f * num5;
			streamPos = vector2;
		}
		else
		{
			intensity = 2f * num5 - 1f;
			streamPos = vector3;
		}
		float f = 1f - intensity;
		float num6 = Mathf.Pow(f, 2.5f);
		intensity = 1f - num6;
	}
}
