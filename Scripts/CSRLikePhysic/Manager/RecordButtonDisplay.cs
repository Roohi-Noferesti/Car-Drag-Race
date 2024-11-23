using UnityEngine;
using UnityEngine.UI;

public class RecordButtonDisplay : MonoBehaviour
{
	public Image RecordButton;

	public bool Hide;

	private Vector2 spritePixelSize;

	private Vector2 spriteSize;

	//private bool recordingAvailable;

	public void Awake()
	{
        //this.spritePixelSize = this.RecordButton.pixelDimensions;
        //this.spriteSize = new Vector2(this.RecordButton.width, this.RecordButton.height);
		this.Hide = this.ShouldHideButton();
		//this.recordingAvailable = VideoCapture.CanStartRecording();
	}

	public void Reset()
	{
		VideoCapture.ClearRecording();
		base.transform.parent.gameObject.SetActive(true);
		//this.recordingAvailable = VideoCapture.CanStartRecording();
		this.Hide = this.ShouldHideButton();
	}

	private bool ShouldHideButton()
	{
	    return true;//(!RelayManager.IsCurrentEventRelay()) ? (!PlayerProfileManager.Instance.ActiveProfile.HasBoughtFirstCar) : (RelayManager.GetRacesDone() + 1 < RelayManager.GetRaceCount());
	}

	private void Update()
	{
        //base.transform.parent.gameObject.SetActive(!this.Hide && this.recordingAvailable);
		//float y = this.spritePixelSize.y - 1f;
        //this.RecordButton.Setup(this.RecordButton.width, this.spriteSize.y, new Vector2(0f, y), new Vector2(this.spritePixelSize.x, this.spritePixelSize.y));
	}
}
