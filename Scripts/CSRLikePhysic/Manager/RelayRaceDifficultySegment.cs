using UnityEngine;
using UnityEngine.UI;

public class RelayRaceDifficultySegment : MonoBehaviour
{
	public Image[] FilledParts;

    public Image Mask;

    public Image Glow;

	public GameObject SlantOffset;

	public float GlowAlpha;

	public float Value
	{
		set
		{
            //this.Mask.transform.localScale = new Vector3(1f - value, 1f, 1f);
            //float num = this.FilledParts.Sum((Image x) => x.width);
            //float y = this.SlantOffset.transform.localPosition.y;
            //float z = this.SlantOffset.transform.localPosition.z;
            //this.SlantOffset.transform.localPosition = new Vector3(value * num - 0.5f * num, y, z);
            //this.SlantOffset.gameObject.SetActive(value > 0f && value < 1f);
            //this.Glow.transform.localScale = new Vector3(value, 1f, 1f);
            //this.Glow.transform.localPosition = new Vector3(0.5f * value * num - 0.5f * num, 0f, 0f);
		}
	}

	public Color Color
	{
		set
		{
            //Image[] filledParts = this.FilledParts;
            //for (int i = 0; i < filledParts.Length; i++)
            //{
            //    Image packedSprite = filledParts[i];
            //    packedSprite.renderer.material.SetColor("_Tint", value);
            //}
            //value.a = this.GlowAlpha;
            //this.Glow.GetComponent<Renderer>().material.SetColor("_Tint", value);
		}
	}

	private void Awake()
	{
		this.Value = 0f;
	}
}
