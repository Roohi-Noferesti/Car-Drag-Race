using System;
using UnityEngine;
using UnityEngine.UI;

public class Buckle : MonoBehaviour
{
	private const float buckleWidth = 0.14f;

	public Image topLeft;

    public Image topRight;

    public Image topArrow;

    public Image left;

    public Image right;

    public Image bottom;

	public void SetWidth(float zWidth)
	{
		zWidth -= 0.14f;
        //GameObjectHelper.SetLocalX(this.left.gameObject, -zWidth / 2f);
        //GameObjectHelper.SetLocalX(this.right.gameObject, zWidth / 2f);
        //this.bottom.SetSize(zWidth, this.bottom.height);
        //float w = (zWidth - this.topArrow.width) / 2f;
        //this.topLeft.SetSize(w, this.topLeft.height);
        //this.topRight.SetSize(w, this.topRight.height);
	}
}
