using System;
using TMPro;
using UnityEngine;

public class RankingEntryYou : ListItem
{
	public AvatarPicture Avatar;

	public GameObject AvatarBackground;

	public GameObject RPText;

	public TextMeshProUGUI RankText;

	public TextMeshProUGUI RespectPointText;

	public TextMeshProUGUI UserName;

	public Transform RespectPointTransform;

	public Transform ContentsTransform;

	public float RespectPointsOffset;

	public float UsernameOffset;

	public void ShowFeatures(bool show)
	{
		this.Avatar.gameObject.SetActive(show);
		this.AvatarBackground.SetActive(show);
		this.RPText.gameObject.SetActive(show);
		this.RankText.gameObject.SetActive(show);
		this.UserName.gameObject.SetActive(show);
	}

	public void SetWidth(float width)
	{
        //WindowPaneBatched component = base.GetComponent<WindowPaneBatched>();
        //component.Width = width;
        //this.UserName.transform.localPosition = new Vector3(this.UsernameOffset / 100f * width, this.UserName.transform.localPosition.y, this.UserName.transform.localPosition.z);
        //this.RespectPointTransform.localPosition = new Vector3(this.RespectPointsOffset / 100f * width, this.RespectPointTransform.localPosition.y, this.RespectPointTransform.localPosition.z);
        //component.UpdateSize();
	}
}
