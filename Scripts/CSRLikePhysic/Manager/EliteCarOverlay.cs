using System;
using I2.Loc;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EliteCarOverlay : MonoBehaviour
{
	public TextMeshProUGUI TextMeshProUGUI;

	public Image Start;

	public Image Middle;

	public Image End;

	public void Setup(Transform zParent)
	{
		base.transform.parent = zParent.transform;
	}

	public void Setup(Vector3 zOffset, Transform zParent, float width = 0.48f, float height = 0.32f)
	{
		base.transform.parent = zParent.transform;
		base.transform.localPosition = zOffset;
        //this.Middle.width = width;
        //this.Start.height = height;
        //this.Middle.height = height;
        //this.End.height = height;
		Vector3 localPosition = this.End.gameObject.transform.localPosition;
		localPosition.x = width;
		this.End.gameObject.transform.localPosition = localPosition;
	}

	public static EliteCarOverlay Create(bool extendStart = false)
	{
		UnityEngine.Object original = Resources.Load("Misc/EliteCarOverlay");
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		EliteCarOverlay component = gameObject.GetComponent<EliteCarOverlay>();
		component.TextMeshProUGUI.text = LocalizationManager.GetTranslation("TEXT_ELITE_CAR");
        //component.Start.renderer.enabled = extendStart;
        //component.Middle.renderer.enabled = true;
        //component.End.renderer.enabled = true;
		return component;
	}

	public float GetLeftXOffset()
	{
	    //float x;
        //if (this.Start.enabled)
        //{
        //    x = this.Start.renderer.bounds.min.x;
        //}
        //else
        //{
        //    x = this.Middle.renderer.bounds.min.x;
        //}
        //float x2 = base.gameObject.transform.position.x;
        //return x2 - x;
	    return 0;
	}

    public float GetTopYOffset()
    {
        //float y = this.Middle.renderer.bounds.min.y;
        //float y2 = base.gameObject.transform.position.y;
        //return y2 - y;
        return 0;
    }
}
