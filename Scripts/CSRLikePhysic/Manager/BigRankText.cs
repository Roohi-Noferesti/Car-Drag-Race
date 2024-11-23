using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class BigRankText : MonoBehaviour
{
	public enum Alignment
	{
		Centre,
		Right
	}

	public TextMeshProUGUI RankText;

	public RawImage RPBarRight;

	public RawImage RPBarLeft;

	public RawImage RPBarMid;

	public RawImage RPTail;

	public Image RPBarGlow;

	public bool HideRPSymbol;

	public float RPBarHeight;

	public float FontSize;

	private string _prevText;

	private void Start()
	{
        //if (!this.RankText.fontHookedUp)
        //{
        //    this.RankText.text = string.Empty;
        //}
        //this.RankText.SetCharacterSize(this.FontSize);
        //this.RPBarLeft.SetSize(this.RPBarHeight, this.RPBarHeight);
        //this.RPBarRight.SetSize(this.RPBarHeight, this.RPBarHeight);
	}

	public void SetRank(int rank, BigRankText.Alignment RPBarAlighnment)
	{
		this.RankText.text = CurrencyUtils.GetRankPointsString(rank, !this.HideRPSymbol, true);
		this.RepositionRpBar(RPBarAlighnment);
	}

	private void RepositionRpBar(BigRankText.Alignment RPBarAlignment)
	{
		if (RPBarAlignment == BigRankText.Alignment.Centre)
		{
			this.AlignBarCentre();
		}
		else if (RPBarAlignment == BigRankText.Alignment.Right)
		{
			this.AlignBarRight();
		}
	}

	private void AlignBarRight()
	{
        //float width = this.RankText.GetWidth(this.RankText.text);
        //this.RPBarMid.transform.localPosition = new Vector3(this.RPBarRight.transform.localPosition.x - width, 0f);
        //this.RPBarMid.SetSize(width, this.RPBarHeight);
        //this.RPBarMid.UpdateUVs();
		this.RPBarLeft.transform.localPosition = this.RPBarMid.transform.localPosition;
		if (this.RPBarGlow)
		{
            //this.RPBarGlow.SetSize(width + this.FontSize * 3f, this.FontSize * 2.2f);
            //this.RPBarGlow.UpdateUVs();
		}
		if (this.RPTail)
		{
            //this.RPTail.SetSize(width + 0.7f, this.RPTail.height);
            //this.RPTail.UpdateUVs();
		}
	}

	private void AlignBarCentre()
	{
        //float width = this.RankText.GetWidth(this.RankText.text);
        //this.RPBarRight.transform.localPosition = new Vector3(width / 2f, 0f);
        //this.RPBarMid.transform.localPosition = new Vector3(-width / 2f, 0f);
        //this.RPBarMid.SetSize(width, this.RPBarHeight);
        //this.RPBarMid.UpdateUVs();
        //this.RPBarLeft.transform.localPosition = this.RPBarMid.transform.localPosition;
        //if (this.RPBarGlow)
        //{
        //    this.RPBarGlow.SetSize(width + this.FontSize * 3f, this.FontSize * 2.2f);
        //    this.RPBarGlow.UpdateUVs();
        //}
        //if (this.RPTail)
        //{
        //    this.RPTail.SetSize(width + 0.7f, this.RPTail.height);
        //    this.RPTail.UpdateUVs();
        //}
	}

	public static BigRankText InstantiatePrefab()
	{
		GameObject original = Resources.Load("Multiplayer/RespectScreen/BigRankText") as GameObject;
		GameObject gameObject = UnityEngine.Object.Instantiate(original) as GameObject;
		return gameObject.GetComponent<BigRankText>();
	}

	public void CentreEverythingToAbsXPos(float absoluteXPos)
	{
        //float num = this.RPBarLeft.transform.position.x - this.RPBarLeft.width;
        //float num2 = this.RPBarRight.transform.position.x + this.RPBarRight.width;
        //float num3 = Mathf.Abs(num2 - num) / 2f + num;
        //float x = absoluteXPos - num3;
        //base.gameObject.transform.position = base.gameObject.transform.position + new Vector3(x, 0f, 0f);
	}
}
