using DataSerialization;
using System;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class ProgressCrewMember : MonoBehaviour
{
	public RawImage memberSprite;

	public GameObject crossGraphic;

	//private readonly float unitConversion = 200f;

	public CompletionBar completionBar;

	//private GameObject CompletionBarGO;

	public void SetupCrewMember(CrewMemberLayout layout)
	{
		if (TierXManager.Instance.ProgressScreenMemberPanels.ContainsKey(layout.Texture))
		{
			Texture2D tex = TierXManager.Instance.ProgressScreenMemberPanels[layout.Texture];
			this.SetupSprite(tex);
			this.SetCompletionBar(layout);
		}
	}

	private void SetupSprite(Texture2D tex)
	{
	    this.memberSprite.texture = tex;

  //      this.memberSprite.renderer.material.SetTexture("_MainTex", tex);
		//this.memberSprite.Setup((float)tex.width / this.unitConversion, (float)tex.height / this.unitConversion, new UnityEngine.Vector2(0f, (float)tex.height - 1f), new UnityEngine.Vector2((float)tex.width, (float)tex.height));
	}

	private void SetupCross()
    {
        crossGraphic.gameObject.SetActive(true);
        //Texture2D texture2D = (Texture2D)Resources.Load("Career/Small_X");
        //   RawImage component = this.crossGraphic.GetComponent<RawImage>();
        //   component.texture = texture2D;
        //component.renderer.material.mainTexture = texture2D;
        //component.Setup((float)texture2D.width / 200f, (float)texture2D.height / 200f, new UnityEngine.Vector2(0f, (float)(texture2D.height - 1)), new UnityEngine.Vector2((float)texture2D.width, (float)texture2D.height));
        //component.gameObject.transform.localPosition = new UnityEngine.Vector3(0f, 0f, -0.05f);
        //component.renderer.material.SetFloat("_Greyness", 1f);
    }

	private void SetCompletionBar(CrewMemberLayout layout)
	{
		IGameState gameState = new GameStateFacade();
		Fraction progression = TierXManager.Instance.PinSchedule.GetProgression(gameState, layout.ScheduleID);
		//this.LoadCompletionBarPrefab();
		if (progression.ToPercent() > 0.99f)
		{
			//this.memberSprite.renderer.material.SetFloat("_Greyness", 1f);
			this.SetupCross();
			this.completionBar.SetupTierX(LocalizationManager.GetTranslation(layout.Name),null, ProgressBarStyle.None, default(Fraction));
		}
		else
		{
            crossGraphic.gameObject.SetActive(false);
            completionBar.SetupTierX(LocalizationManager.GetTranslation(layout.Name),null, ProgressBarStyle.Bar, progression);
		}
		//this.completionBar.Text.renderer.material.SetColor("_Tint", UnityEngine.Color.black);
	}

	//private void LoadCompletionBarPrefab()
	//{
	//	if (this.CompletionBarGO != null)
	//	{
	//		UnityEngine.Object.Destroy(this.CompletionBarGO);
	//		this.CompletionBarGO = null;
	//	}
	//	GameObject original = Resources.Load("Career/CompletionBar/CompletionBar") as GameObject;
	//	this.CompletionBarGO = (GameObject)UnityEngine.Object.Instantiate(original);
	//	this.CompletionBarGO.transform.parent = base.gameObject.transform;
	//	this.CompletionBarGO.transform.localPosition = new UnityEngine.Vector3(0f, -0.2f, -0.15f);
	//	this.completionBar = this.CompletionBarGO.GetComponent<CompletionBar>();
	//}
}
