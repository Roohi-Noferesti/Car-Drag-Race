using System;
using UnityEngine;
using UnityEngine.UI;

public class CarPiece : MonoBehaviour
{
	public delegate void OnBuyPressDelegate(CarPiece piece);

	public Image CarSprite;

    //public PackedSprite Blocker;

	public PrizePieceDecoration Background;

	public int GridPositionX;

	public int GridPositionY;

	public CarPiece.OnBuyPressDelegate OnBuyPress;

	public int ID;

	public void SetBlockerColor(Color color)
	{
        //this.Blocker.renderer.material.SetColor("_Color", color);
	}

	public void SetBorderSize(float borderSide)
	{
        //this.Background.BlueBorderSize = borderSide;
	}

	public void SetSize(float width, float height, int GridSizeX, int GridSizeY)
	{
        //this.CarSprite.SetSize(width, height);
        //this.Blocker.SetSize(width, height);
        //float xflip = 1f;
        //float yflip = 1f;
        //if (this.GridPositionX > 0)
        //{
        //    xflip = -1f;
        //}
        //if (this.GridPositionY > 0)
        //{
        //    yflip = -1f;
        //}
        //bool isCorner = false;
        //if ((this.GridPositionX == 0 || this.GridPositionX == GridSizeX - 1) && (this.GridPositionY == 0 || this.GridPositionY == GridSizeY - 1))
        //{
        //    isCorner = true;
        //}
        //this.Background.SetSize(width, height, xflip, yflip, isCorner);
	}

	public void DisableBlocker()
	{
        //this.Blocker.Hide(true);
	}

	public bool IsCovered()
	{
	    return false;//!this.Blocker.IsHidden();
	}

	public void Setup(Texture2D carTexture, int x, int y, Rect uv, Color blockerColor, float borderSize)
	{
        //this.CarSprite.SetTexture(carTexture);
        //this.CarSprite.SetUVs(uv);
        //this.GridPositionX = x;
        //this.GridPositionY = y;
        //this.SetBlockerColor(blockerColor);
        //this.SetBorderSize(borderSize);
	}
}
