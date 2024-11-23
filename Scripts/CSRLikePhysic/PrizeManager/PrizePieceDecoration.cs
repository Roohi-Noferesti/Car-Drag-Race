using System;
using UnityEngine;

public class PrizePieceDecoration : MonoBehaviour
{
    //public float Width;

    //public float Height;

    //public float BlueBorderSize = 0.1f;

    //public PackedSprite whiteCorner;

    //public PackedSprite whiteVertical;

    //public PackedSprite whiteHorizontal;

    //public global::Sprite whiteMid;

    //public global::Sprite verticalDivider;

    //public global::Sprite horizontalDivider;

    //private float FlipX = 1f;

    //private float FlipY = 1f;

    //private bool IsCorner;

    //public void SetSize(float width, float height, float Xflip, float Yflip, bool isCorner)
    //{
    //    this.Width = width;
    //    this.Height = height;
    //    this.FlipX = Xflip;
    //    this.FlipY = Yflip;
    //    this.IsCorner = isCorner;
    //    this.UpdateSize();
    //}

    //private void UpdateSize()
    //{
    //    if (this.IsCorner)
    //    {
    //        this.PositionSpritesCorner(this.BlueBorderSize, Mathf.Abs(this.whiteCorner.width), 0f, this.whiteVertical, this.whiteHorizontal, this.whiteCorner, this.whiteMid);
    //    }
    //    else
    //    {
    //        this.PositionSpritesEdge(this.BlueBorderSize, Mathf.Abs(this.whiteCorner.width), 0f, this.whiteVertical, this.whiteHorizontal, this.whiteCorner, this.whiteMid);
    //    }
    //    if (this.horizontalDivider)
    //    {
    //        float num = Mathf.Abs(this.horizontalDivider.height);
    //        this.horizontalDivider.transform.localPosition = new Vector3(this.FlipX * (this.Width / 2f), this.FlipY * (this.Height / 2f - num / 2f), -0.25f);
    //        this.horizontalDivider.SetSize(this.FlipX * -this.Width, this.FlipY * num);
    //    }
    //    if (this.verticalDivider)
    //    {
    //        float num2 = Mathf.Abs(this.verticalDivider.height);
    //        this.verticalDivider.transform.localPosition = new Vector3(this.FlipX * (this.Width / 2f - num2 / 2f), this.FlipY * (this.Height / 2f), -0.25f);
    //        this.verticalDivider.SetSize(this.FlipY * this.Height, this.FlipX * num2);
    //    }
    //}

    //private void PositionSpritesCorner(float sizeAdd, float cornerSize, float zOffset, SpriteBase left, SpriteBase bottom, SpriteBase corner, SpriteBase mid)
    //{
    //    float num = this.Width + sizeAdd;
    //    float num2 = this.Height + sizeAdd;
    //    float num3 = num / 2f;
    //    float num4 = num2 / 2f;
    //    float w = this.FlipX * (num - sizeAdd / 2f - cornerSize);
    //    float h = this.FlipY * (num2 - sizeAdd / 2f - cornerSize);
    //    float x = this.FlipX * -num3;
    //    float y = this.FlipY * -num4;
    //    if (corner != null)
    //    {
    //        corner.transform.localPosition = new Vector3(x, y, zOffset);
    //        corner.SetSize(this.FlipX * cornerSize, this.FlipY * cornerSize);
    //    }
    //    if (left != null)
    //    {
    //        left.transform.localPosition = new Vector3(x, this.FlipY * (-num4 + cornerSize), zOffset);
    //        left.SetSize(this.FlipX * cornerSize, h);
    //    }
    //    if (bottom != null)
    //    {
    //        bottom.transform.localPosition = new Vector3(this.FlipX * (-num3 + cornerSize), y, zOffset);
    //        bottom.SetSize(w, this.FlipY * cornerSize);
    //    }
    //    if (mid != null)
    //    {
    //        mid.transform.localPosition = new Vector3(this.FlipX * (-num3 + cornerSize), this.FlipY * (-num4 + cornerSize), zOffset);
    //        mid.SetSize(w, h);
    //    }
    //}

    //private void PositionSpritesEdge(float sizeAdd, float cornerSize, float zOffset, SpriteBase left, SpriteBase bottom, SpriteBase corner, SpriteBase mid)
    //{
    //    float width = this.Width;
    //    float num = this.Height + sizeAdd;
    //    float num2 = width / 2f;
    //    float num3 = num / 2f;
    //    float w = this.FlipX * width;
    //    float h = this.FlipY * (num - sizeAdd / 2f - cornerSize);
    //    float y = this.FlipY * -num3;
    //    if (corner != null)
    //    {
    //        corner.gameObject.SetActive(false);
    //    }
    //    if (left != null)
    //    {
    //        left.gameObject.SetActive(false);
    //    }
    //    if (bottom != null)
    //    {
    //        bottom.transform.localPosition = new Vector3(this.FlipX * -num2, y, zOffset);
    //        bottom.SetSize(w, this.FlipY * cornerSize);
    //    }
    //    if (mid != null)
    //    {
    //        mid.transform.localPosition = new Vector3(this.FlipX * -num2, this.FlipY * (-num3 + cornerSize), zOffset);
    //        mid.SetSize(w, h);
    //    }
    //}
}
