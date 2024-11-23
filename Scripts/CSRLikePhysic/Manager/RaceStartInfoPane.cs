using System.Collections.Generic;
using I2.Loc;
using UnityEngine;
using UnityEngine.UI;

public class RaceStartInfoPane : MonoBehaviour
{
	private const int PlayerPaneRenderQueueOffset = 7;

	public GameObject Background;

	public Text Name;

    public Text Car;

	public CarStatsElem CarStats;

	public Renderer RenderQueue3001;

	public Renderer RenderQueue3002;

	public Renderer RenderQueue3003;

	public Renderer RenderQueue3004;

	public GameObject HeadStart;

	public Image HeadStartGraphic;

	public Text HeadStartTimeDiff;

    public Text HeadStartText;

    //private List<SpriteRoot> carStatsSprites = new List<SpriteRoot>();

	private List<Renderer> childRenderers = new List<Renderer>();

	private List<Renderer> backgroundRenderers = new List<Renderer>();

	private int RenderQueueOffset;

	public void Populate(string zName, string zCarName, eCarTier zTier, int zPPIndex, float headStartTime, bool isPlayer = false)
	{
        //this.carStatsSprites.Clear();
		this.childRenderers.Clear();
		this.backgroundRenderers.Clear();
		if (isPlayer)
		{
			this.RenderQueueOffset = 7;
		}
		this.Name.text = zName;
        this.Car.text = zCarName;
		if (headStartTime == 0f)
		{
			this.HeadStart.SetActive(false);
		}
		else
		{
            this.HeadStartTimeDiff.text = RelayManager.FormatRaceTime(headStartTime);
		}
		//int renderQueue = 3001 + this.RenderQueueOffset;
		int layer = LayerMask.NameToLayer("Default");
		foreach (Transform transform in this.CarStats.gameObject.transform)
		{
			transform.gameObject.layer = layer;
            //transform.renderer.material.SetFloat("_ClipPos", 10f);
            //this.carStatsSprites.Add(transform.GetComponent<SpriteRoot>());
		}
        //this.HeadStartTimeDiff.renderer.material.SetFloat("_ClipPos", 10f);
        //this.carStatsSprites.Add(this.HeadStartTimeDiff.GetComponent<SpriteRoot>());
		this.HeadStartText.text = LocalizationManager.GetTranslation("TEXT_HEADSTART");
        //this.HeadStartText.renderer.material.SetFloat("_ClipPos", 10f);
        //this.carStatsSprites.Add(this.HeadStartText.GetComponent<SpriteRoot>());
		foreach (Transform transform2 in this.Background.gameObject.transform)
		{
            //Material material = transform2.renderer.material;
            //material.renderQueue = renderQueue;
            //this.backgroundRenderers.Add(transform2.renderer);
		}
		this.RenderQueue3001.material.renderQueue = 3002 + this.RenderQueueOffset;
		this.RenderQueue3002.material.renderQueue = 3003 + this.RenderQueueOffset;
		this.RenderQueue3003.material.renderQueue = 3004 + this.RenderQueueOffset;
		this.RenderQueue3004.material.renderQueue = 3005 + this.RenderQueueOffset;
        //this.HeadStartGraphic.renderer.material.renderQueue = 3006 + this.RenderQueueOffset;
        //this.HeadStartTimeDiff.renderer.material.renderQueue = 3007 + this.RenderQueueOffset;
        //this.HeadStartText.renderer.material.renderQueue = 3007 + this.RenderQueueOffset;
        //this.Name.renderer.material.renderQueue = 3002 + this.RenderQueueOffset;
        //this.Car.renderer.material.renderQueue = 3002 + this.RenderQueueOffset;
		this.CarStats.Set(zTier, zPPIndex);
        //this.childRenderers.Add(this.Name.renderer);
        //this.childRenderers.Add(this.Car.renderer);
		this.SetAlpha(1f);
	}

	public void SetAlpha(float zAlpha)
	{
		Color color = new Color(1f, 1f, 1f, zAlpha);
		foreach (Renderer current in this.childRenderers)
		{
			current.material.SetColor("_Tint", color);
		}
		Color color2 = new Color(1f, 1f, 1f, zAlpha * 0.8f);
		foreach (Renderer current2 in this.backgroundRenderers)
		{
			current2.material.SetColor("_Tint", color2);
		}
		this.CarStats.SetAlpha(zAlpha);
        //this.HeadStartGraphic.SetAlpha(zAlpha);
		//Color color3 = new Color(0f, 0f, 0f, zAlpha);
        //this.HeadStartTimeDiff.renderer.material.SetColor("_Tint", color3);
        //this.HeadStartText.renderer.material.SetColor("_Tint", color3);
	}

	public void SetScale(float zScale)
	{
		base.transform.localScale = new Vector3(zScale, zScale, zScale);
	}
}
