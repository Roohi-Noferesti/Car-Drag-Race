using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class PrizePageListItem //: ListItem
{
    //public const int NUM_PRIZES_ON_PAGE = 3;

    //private const int NUM_ROWS = 1;

    //private const int NUM_COLUMNS = 3;

    //private const float PADDING = 0.06f;

    //public PrefabPlaceholder[] PrizePlaceholders;

    //public GameObject LeftCenter;

    //private int _lastTappedIndex;

    //private string _lastTappedKey;

    //private bool _lastTappedWasLocked;

    //private bool _lastTappedWasNull;

    //private Vector2 _pageSize;

    //public bool LastTappedWasLocked
    //{
    //    get
    //    {
    //        return this._lastTappedWasLocked;
    //    }
    //}

    //public bool LastTappedWasNull
    //{
    //    get
    //    {
    //        return this._lastTappedWasNull;
    //    }
    //}

    //public int LastTappedIndex
    //{
    //    get
    //    {
    //        return this._lastTappedIndex;
    //    }
    //}

    //public string LastTappedKey
    //{
    //    get
    //    {
    //        return this._lastTappedKey;
    //    }
    //}

    //public SinglePrizeListItem LastTappedPrize
    //{
    //    get
    //    {
    //        return this.GetPrizeItem(this.LastTappedIndex);
    //    }
    //}

    //public SinglePrizeListItem GetPrizeItem(int i)
    //{
    //    return this.PrizePlaceholders[i].GetBehaviourOnPrefab<SinglePrizeListItem>();
    //}

    //public void Create(List<string> carNames, Vector2 pageSize)
    //{
    //    this._ignoreThePressLock = true;
    //    this._pageSize = pageSize;
    //    int i;
    //    for (i = 0; i < 3; i++)
    //    {
    //        this.CreatePrizeItem(carNames, pageSize, 0f, i);
    //    }
    //    for (int j = i; j < this.PrizePlaceholders.Length; j++)
    //    {
    //        SinglePrizeListItem prizeItem = this.GetPrizeItem(j);
    //        prizeItem.SetupInvisible();
    //    }
    //}

    //private void CreatePrizeItem(List<string> carNames, Vector2 pageSize, float offset, int index)
    //{
    //    Vector2 idealItemSizeForCurrentRes = this.GetIdealItemSizeForCurrentRes();
    //    SinglePrizeListItem prizeItem = this.GetPrizeItem(index);
    //    if (index < carNames.Count)
    //    {
    //        prizeItem.Setup(carNames[index]);
    //        prizeItem.Tap += new SinglePrizeListItem.TapEventHandler(this.OnChildTap);
    //    }
    //    else
    //    {
    //        prizeItem.SetupClear();
    //    }
    //    float num = (float)(index % 3);
    //    float num2 = 0f;
    //    float num3 = (this._pageSize.x - 3f * idealItemSizeForCurrentRes.x) / 4f;
    //    float num4 = (this._pageSize.y - 1f * idealItemSizeForCurrentRes.y) / 2f;
    //    float x = -(this._pageSize.x / 2f) + num * (idealItemSizeForCurrentRes.x + num3) + idealItemSizeForCurrentRes.x / 2f + num3;
    //    float y = -(this._pageSize.y / 2f) + num2 * (idealItemSizeForCurrentRes.y + num4) + idealItemSizeForCurrentRes.y / 2f + num4;
    //    Vector3 zPosition = new Vector3(x, y, 0f);
    //    this.PrizePlaceholders[index].transform.localPosition = GameObjectHelper.MakeLocalPositionPixelPerfect(zPosition);
    //}

    //protected override void Show()
    //{
    //    this._thisIsGreyedOut = false;
    //    this._thisIsDisabled = false;
    //}

    //protected override void Hide()
    //{
    //}

    //public override void Shutdown()
    //{
    //    for (int i = 0; i < this.PrizePlaceholders.Length; i++)
    //    {
    //        this.GetPrizeItem(i).Tap -= new SinglePrizeListItem.TapEventHandler(this.OnChildTap);
    //    }
    //}

    //private void OnChildTap(SinglePrizeListItem item)
    //{
    //    this._lastTappedIndex = -1;
    //    this._lastTappedWasLocked = item.IsLocked;
    //    this._lastTappedWasNull = item.IsNull;
    //    for (int i = 0; i < this.PrizePlaceholders.Length; i++)
    //    {
    //        if (this.GetPrizeItem(i) == item)
    //        {
    //            this._lastTappedKey = item.Key;
    //            this._lastTappedIndex = i;
    //            break;
    //        }
    //    }
    //    base.InvokeTapEvent();
    //}

    //private Vector2 GetIdealItemSizeForCurrentRes()
    //{
    //    float x = this._pageSize.x / 3f - 0.24f;
    //    float y = this._pageSize.y / 1f - 0.12f;
    //    Vector2 result = new Vector2(x, y);
    //    return result;
    //}

    //[Conditional("UNITY_EDITOR")]
    //private void OnDrawGizmos()
    //{
    //    Gizmos.color = Color.cyan;
    //    Gizmos.DrawWireCube(base.gameObject.transform.position, new Vector3(this._pageSize.x, this._pageSize.y, 0f));
    //}
}
