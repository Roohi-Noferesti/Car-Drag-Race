using System;
using UnityEngine;

public class WorldTourAnimationClips
{
	public string PreIntroClip = string.Empty;

	public string IntroClip = string.Empty;

	public Vector3 IntroOffset = Vector3.zero;

	public WorldTourAnimationClips(string preIntro, string intro, Vector3 offset)
	{
		this.PreIntroClip = preIntro;
		this.IntroClip = intro;
		this.IntroOffset = offset;
	}
}
