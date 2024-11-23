using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RelayRaceDifficulty : MonoBehaviour
{
	public GameObject[] SegmentParents;

	public GameObject SegmentPrefab;

	public Color GoodColor;

	public Color BadColor;

	public Text RatingOld;

    public Text RatingNew;

	public RaceEventDifficulty.Rating OldDifficultyRating;

	public RaceEventDifficulty.Rating DifficultyRating;

	public float DifficultyInterp;

	public float ColorInterp;

	private float initialDifficulty;

	private float targetDifficulty;

	private Color baseColor;

	private Color overrideColor;

	private List<RelayRaceDifficultySegment> Segments = new List<RelayRaceDifficultySegment>();

	private static float cachedTimeDifference;

	private void UpdateDifficulty()
	{
		float num = Mathf.Clamp01(Mathf.Lerp(this.initialDifficulty, this.targetDifficulty, this.DifficultyInterp));
		float num2 = 1f / (float)this.Segments.Count;
		float num3 = num;
		foreach (RelayRaceDifficultySegment current in this.Segments)
		{
			current.Value = Mathf.Clamp01(num3 / num2);
			num3 = Mathf.Clamp01(num3 - num2);
		}
	}

	private void UpdateColor()
	{
		Color color = (1f - this.ColorInterp) * this.baseColor + this.ColorInterp * this.overrideColor;
		foreach (RelayRaceDifficultySegment current in this.Segments)
		{
			current.Color = color;
		}
	}

	private void SetBaseColor(float difficulty)
	{
		this.baseColor = RaceEventDifficulty.Instance.GetColour(difficulty);
		this.UpdateColor();
	}

	public void SetBaseColorToInitial()
	{
		this.SetBaseColor(this.initialDifficulty);
	}

	public void SetBaseColorToTarget()
	{
		this.SetBaseColor(this.targetDifficulty);
	}

	public void Setup(RaceEventData relayEvent, int racesDone, float initialInterp = 0f, bool skipAnimation = false, bool useCachedDifficulty = false)
	{
		//GameObject[] segmentParents = this.SegmentParents;
		//for (int i = 0; i < segmentParents.Length; i++)
		//{
		//	//GameObject parent = segmentParents[i];
  //          //RelayRaceDifficultySegment item = GameObjectHelper.InstantiatePrefab<RelayRaceDifficultySegment>(this.SegmentPrefab, parent);
  //          //this.Segments.Add(item);
		//}
		int racesDone2 = Mathf.Max(0, racesDone - 1);
		float timeDifference = (!useCachedDifficulty) ? RelayManager.CalculateExpectedTimeDifference(relayEvent, racesDone2) : cachedTimeDifference;
		float timeDifference2 = RelayManager.CalculateExpectedTimeDifference(relayEvent, racesDone);
		if (racesDone == 0 && !useCachedDifficulty)
		{
			this.initialDifficulty = 0f;
		}
		else
		{
			this.initialDifficulty = RelayManager.ConvertTimeDifferenceToPercentage(timeDifference);
		}
		this.targetDifficulty = RelayManager.ConvertTimeDifferenceToPercentage(timeDifference2);
		this.DifficultyInterp = initialInterp;
		this.UpdateDifficulty();
		this.ColorInterp = 0f;
		this.SetBaseColorToInitial();
		if (this.initialDifficulty > this.targetDifficulty)
		{
			this.overrideColor = this.GoodColor;
		}
		else
		{
			this.overrideColor = this.BadColor;
		}
		RaceEventDifficulty.Rating difficultyForTimeDifference = RelayManager.GetDifficultyForTimeDifference(timeDifference);
		RaceEventDifficulty.Rating difficultyForTimeDifference2 = RelayManager.GetDifficultyForTimeDifference(timeDifference2);
        //this.RatingOld.Text = RaceEventDifficulty.Instance.GetString(difficultyForTimeDifference);
        //this.RatingNew.Text = RaceEventDifficulty.Instance.GetString(difficultyForTimeDifference2);
		this.RatingOld.gameObject.SetActive(racesDone > 0 && !skipAnimation);
		this.RatingNew.gameObject.SetActive(skipAnimation);
		this.DifficultyRating = difficultyForTimeDifference2;
		this.OldDifficultyRating = difficultyForTimeDifference;
		if (skipAnimation)
		{
			this.SetBaseColorToTarget();
            //AnimationUtils.PlayLastFrame(base.GetComponent<Animation>(), "RelayDifficultyIntro");
		}
		cachedTimeDifference = timeDifference2;
	}

	public void PlayDifficultyAnim(string animationName)
	{
        //AnimationUtils.PlayAnim(base.GetComponent<Animation>(), animationName);
	}

	public void InitDifficultyAnim(string animationName)
	{
        //AnimationUtils.PlayFirstFrame(base.GetComponent<Animation>(), animationName);
	}

	public void FinishDifficultyAnim(string animationName)
	{
        //AnimationUtils.PlayLastFrame(base.GetComponent<Animation>(), animationName);
	}

	public bool NoChangeInRating()
	{
		return this.OldDifficultyRating == this.DifficultyRating;
	}

	public bool NoChangeInDifficulty()
	{
		return this.initialDifficulty == this.targetDifficulty;
	}

	private void Update()
	{
		this.UpdateDifficulty();
		this.UpdateColor();
	}
}
