using System;
using UnityEngine;
using UnityEngine.UI;

public class CarouselTransition : MonoBehaviour
{
	public Image[] TransitionSprites;

	public Material MaterialCareer;

	public Material MaterialMultiplayer;

	public Material MaterialCareerToMulti;

	public Material MaterialMultiToCareer;

	private void SetMaterial(bool inMultiplayer, Material multiplayerMaterial, Material careerMaterial)
	{
		//if (inMultiplayer)
		//{
  //          Image[] transitionSprites = this.TransitionSprites;
		//	for (int i = 0; i < transitionSprites.Length; i++)
		//	{
  //              Image packedSprite = transitionSprites[i];
  //              //packedSprite.renderer.sharedMaterial = multiplayerMaterial;
		//	}
		//}
		//else
		//{
  //          Image[] transitionSprites2 = this.TransitionSprites;
		//	for (int j = 0; j < transitionSprites2.Length; j++)
		//	{
  //              Image packedSprite2 = transitionSprites2[j];
  //              //packedSprite2.renderer.sharedMaterial = careerMaterial;
		//	}
		//}
	}

	public void SetToDefaultMaterial(bool inMultiplayer)
	{
		this.SetMaterial(inMultiplayer, this.MaterialMultiplayer, this.MaterialCareer);
	}

	public void SetToDefaultMaterial()
	{
		this.SetMaterial(MultiplayerUtils.GarageInMultiplayerMode, this.MaterialMultiplayer, this.MaterialCareer);
	}

	public void SetToTransitionMaterial(bool inMultiplayer)
	{
		this.SetMaterial(inMultiplayer, this.MaterialMultiToCareer, this.MaterialCareerToMulti);
	}

	public void SetToTransitionMaterial()
	{
		this.SetMaterial(MultiplayerUtils.GarageInMultiplayerMode, this.MaterialMultiToCareer, this.MaterialCareerToMulti);
	}
}
