using UnityEngine;

public class NitrousButtonDisplay : MonoBehaviour
{
	//private float displayedNitrous;

	public float currentNitrous;

	public bool anyNitrousAvailable;

	public GameObject NitrousAvailable;

    public GameObject NitrousDepleted;

	private Vector2 SpritePixelSize;

	private Vector2 SpriteSize;

	public void Awake()
	{
        //this.SpritePixelSize = this.NitrousAvailable.pixelDimensions;
        //this.SpriteSize = new Vector2(this.NitrousAvailable.width, this.NitrousAvailable.height);
	}

	public void Reset()
	{
		this.SetNitrousAmount(0f);
	}

	private void Update()
	{
        bool flag = false;
        if (this.currentNitrous <= 0f)
        {
            flag = false;
        }
        else if (this.anyNitrousAvailable)
        {
            flag = true;
        }
        if (this.NitrousAvailable.activeInHierarchy != flag)
        {
            this.NitrousAvailable.SetActive(flag);
        }
        //if (this.NitrousDepleted.activeInHierarchy != flag)
        //{
        //    this.NitrousDepleted.SetActive(flag);
        //}
        if (flag)// && this.displayedNitrous != this.currentNitrous)
        {
            this.SetNitrousAmount(this.currentNitrous);
        }
	}

	private void SetNitrousAmount(float amount)
	{
        //this.displayedNitrous = amount;
        //float num = Mathf.Floor(this.SpritePixelSize.y * amount);
        //float num2 = this.SpriteSize.y * amount;
        //float y = this.SpritePixelSize.y * 2f - 1f;
        ////this.NitrousAvailable.Setup(this.NitrousAvailable.width, num2, new Vector2(0f, y), new Vector2(this.SpritePixelSize.x, num));
        //float y2 = this.SpritePixelSize.y - num;
        //float h = this.SpriteSize.y - num2;
        ////this.NitrousDepleted.Setup(this.NitrousDepleted.width, h, new Vector2(0f, y2), new Vector2(this.SpritePixelSize.x, y2));
	}
}
