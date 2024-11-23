using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EventPaneRestrictionBubble : MonoBehaviour
{
	private class RestrictionGraphic
	{
        public string TranslatedBubbleMessage;

		public GameObject RestrictionSprite;

        public bool RestrictionActive;
	}

	private const float SECONDS_TO_DISPLAY_EACH_RESTRICTION_FOR = 2f;

	public GameObject RestrictionSpritePrefab;

    public NavBarInfoPane InfoPane;

    public Transform RestrictionIconOffset;

    public TextMeshProUGUI RestrictionText;

	private List<EventPaneRestrictionBubble.RestrictionGraphic> activeRestrictions = new List<EventPaneRestrictionBubble.RestrictionGraphic>();

    private EventPaneRestrictionBubble.RestrictionGraphic currentHighlightedRestriction;

	private float elapsedTimeOnRestriction;

	public void ClearRestrictions()
	{
        //this.activeRestrictions.ForEach(delegate(EventPaneRestrictionBubble.RestrictionGraphic q)
        //{
        //    UnityEngine.Object.Destroy(q.RestrictionSprite.gameObject);
        //});
		this.activeRestrictions.Clear();
	}

	public void AddRestriction(string translatedBubbleMessage, string textureName, bool active)
	{
        this.activeRestrictions.Add(new EventPaneRestrictionBubble.RestrictionGraphic
        {
            TranslatedBubbleMessage = translatedBubbleMessage,
            //RestrictionSprite = this.CreateRestrictionSprite(textureName, active),
            RestrictionActive = active
        });
    }

	public void Finalise()
	{
        //if (!this.InfoPane.BeenCreated)
        //{
        //    this.InfoPane.Create(true);
        //}
        //this.InfoPane.gameObject.SetActive(false);
        //float num = (float)this.activeRestrictions.Count * 0.25f;
        //float num2 = -num / 2f;
        //foreach (RestrictionGraphic current in this.activeRestrictions)
        //{
        //    var restrictionSprite = current.RestrictionSprite;
        //    //restrictionSprite.transform.localPosition = new Vector3(num2 + restrictionSprite.width / 2f, 0f);
        //    num2 += 0.25f;
        //}
        //this.HighlightFirstActiveRestriction();
	}

    private GameObject CreateRestrictionSprite(string textureName, bool active)
	{
        GameObject gameObject = UnityEngine.Object.Instantiate(this.RestrictionSpritePrefab) as GameObject;
        gameObject.transform.parent = transform;//this.RestrictionIconOffset;
        gameObject.transform.localPosition = Vector3.zero;
        RawImage rawImage = gameObject.GetComponent<RawImage>();
        rawImage.color = (!active) ? Color.white : Color.red;
		string path = "Map_Screen/" + textureName;
		Texture2D texture2D = (Texture2D)Resources.Load(path);
        if (texture2D == null)
        {
            return null;
        }
        rawImage.texture = texture2D;
        //component.renderer.material.SetTexture("_MainTex", texture2D);
        //EventPane.DoEPSpriteSetup(component, texture2D);
        return RestrictionSpritePrefab;
	}

	private void HighlightFirstActiveRestriction()
	{
        EventPaneRestrictionBubble.RestrictionGraphic highlightedRestriction = this.activeRestrictions.First((EventPaneRestrictionBubble.RestrictionGraphic q) => q.RestrictionActive);
        this.SetHighlightedRestriction(highlightedRestriction);
	}

	private void SetHighlightedRestriction(RestrictionGraphic restriction)
	{
        this.InfoPane.SetText(restriction.TranslatedBubbleMessage);
        this.InfoPane.MoveNipple(restriction.RestrictionSprite.transform.localPosition.x);
        this.currentHighlightedRestriction = restriction;
		this.elapsedTimeOnRestriction = 0f;
	}

	public void ShowRestrictionBubbleMessage()
	{
        RestrictionGraphic restrictionGraphic = this.activeRestrictions.First(q => q.RestrictionActive);
        //Vector3 offset = new Vector3(0f, 0.05f, -0.2f);
        //BubbleMessage bubble = null;
        //base.StartCoroutine(BubbleManager.Instance.ShowMessageDelayed(0.25f, restrictionGraphic.TranslatedBubbleMessage,
        //    true, restrictionGraphic.RestrictionSprite.transform, offset, BubbleMessage.NippleDir.DOWN, 0.75f,
        //    BubbleMessageConfig.ThemeStyle.SMALL, false, delegate(BubbleMessage b)
        //    {
        //        bubble = b;
        //    }));
        //return bubble;
        RestrictionText.text = restrictionGraphic.TranslatedBubbleMessage;
        RestrictionText.gameObject.SetActive(true);
    }

    public void AnimateRestriction()
	{
		if (this.activeRestrictions.Count <= 1)
		{
			return;
		}
		this.elapsedTimeOnRestriction += Time.deltaTime;
		if (this.elapsedTimeOnRestriction < 2f)
		{
			return;
		}
        bool flag = false;
        RestrictionGraphic restrictionGraphic = null;
        foreach (RestrictionGraphic current in this.activeRestrictions)
        {
            if (current == this.currentHighlightedRestriction)
            {
                flag = true;
            }
            else if (flag && current.RestrictionActive)
            {
                restrictionGraphic = current;
                break;
            }
        }
        if (restrictionGraphic == null)
        {
            this.HighlightFirstActiveRestriction();
        }
        else
        {
            this.SetHighlightedRestriction(restrictionGraphic);
        }
	}

	public void ChangeAllTints(float alpha)
	{
		this.activeRestrictions.ForEach(delegate(RestrictionGraphic q)
		{
			EventPane.ChangeRendererTint(q.RestrictionSprite.GetComponent<Renderer>(), alpha);
		});
        this.InfoPane.ChangeAllTints(alpha);
	}

    public void HideRestrictionBubble()
    {
        RestrictionText.gameObject.SetActive(false);
    }
}
