using UnityEngine;
using UnityEngine.UI;

public class HUDHitState : MonoBehaviour
{
	public Button button;

	public GameObject hit;

	public GameObject normal;

	public bool MoveLeftWhenPressed;

	public bool MoveRightWhenPressed;

	public bool ScaleWhenPressed;

	private bool isHitActive;

	private bool isNormalActive;

	private Vector3 normalPos;

	private Vector3 hitPos;

	private Vector3 normalScale;

	private Vector3 hitScale;

	private void Awake()
	{
		if (this.MoveLeftWhenPressed)
		{
			this.normal.SetActive(true);
			this.normalPos = this.normal.transform.localPosition;
			this.hitPos = new Vector3(this.normalPos.x - 0.025f, this.normalPos.y - 0.05f, this.normalPos.z);
		}
		else if (this.MoveRightWhenPressed)
		{
			this.normal.SetActive(true);
			this.normalPos = this.normal.transform.localPosition;
			this.hitPos = new Vector3(this.normalPos.x + 0.025f, this.normalPos.y - 0.05f, this.normalPos.z);
		}
		else if (this.ScaleWhenPressed)
		{
			this.normal.SetActive(true);
			this.normalScale = this.normal.transform.localScale;
			this.hitScale = new Vector3(this.normalScale.x - 0.1f, this.normalScale.y - 0.1f, this.normalScale.z);
		}
		else
		{
			if (this.hit != null)
			{
				this.hit.SetActive(false);
			}
			this.normal.SetActive(false);
		}
		this.UpdateHitState();
	}

	private void Update()
	{
		this.UpdateHitState();
	}

	private void UpdateHitState()
	{
		if (PauseGame.isGamePaused && PauseGame.hasPopup)
		{
			return;
		}
		bool flag = false;
		bool flag2 = false;
		if (!button.interactable)
		{
			flag2 = true;
		}
		else if (this.hit != null)
		{
			flag = true;
		}
		else
		{
			flag2 = true;
		}
		if (flag != this.isHitActive)
		{
			this.isHitActive = flag;
			if (this.MoveLeftWhenPressed || this.MoveRightWhenPressed)
			{
				if (flag)
				{
					this.normal.transform.localPosition = this.hitPos;
				}
				else
				{
					this.normal.transform.localPosition = this.normalPos;
				}
			}
			else if (this.ScaleWhenPressed)
			{
				if (flag)
				{
					this.normal.transform.localScale = this.hitScale;
				}
				else
				{
					this.normal.transform.localScale = this.normalScale;
				}
			}
			else
			{
				this.hit.SetActive(flag);
			}
		}
		if (flag2 != this.isNormalActive)
		{
			this.isNormalActive = flag2;
			if (this.MoveLeftWhenPressed || this.MoveRightWhenPressed)
			{
				if (flag2)
				{
					this.normal.transform.localPosition = this.normalPos;
				}
				else
				{
					this.normal.transform.localPosition = this.hitPos;
				}
			}
			else if (this.ScaleWhenPressed)
			{
				if (flag2)
				{
					this.normal.transform.localScale = this.normalScale;
				}
				else
				{
					this.normal.transform.localScale = this.hitScale;
				}
			}
			else
			{
				this.normal.SetActive(flag2);
			}
		}
	}
}
