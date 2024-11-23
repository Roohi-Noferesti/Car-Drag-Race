using UnityEngine;

public class VersusGraphic : MonoBehaviour
{
	public Animation starLoop;

	private void Start()
	{
	}

	private void Update()
	{
	}

	public void Anim_Initialise()
	{
		this.starLoop.Stop();
        //AnimationUtils.PlayFirstFrame(base.GetComponent<Animation>());
	}

	public void Anim_Finish()
	{
        //AnimationUtils.PlayLastFrame(base.GetComponent<Animation>());
        //AnimationUtils.PlayFirstFrame(this.starLoop);
		this.starLoop.Play();
	}

	public void Anim_Play()
	{
        //AnimationUtils.PlayAnim(base.GetComponent<Animation>());
	}

	private void Anim_Begin_Star_Loop()
	{
        //AnimationUtils.PlayAnim(this.starLoop);
	}
}
