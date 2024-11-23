using Metrics;
using System;
using System.Collections.Generic;
using UnityEngine;

public class PrizeListScreen : ZHUDScreen
{
    //public PrefabPlaceholder PipsElementPlaceholder;

    //public DummyTextButton continueButton;

    //private SinglePrizeListItem LastSelectedPrize;

    //private BubbleMessage emptySpaceTutorialBubble;

    //private Vector3 emptySpaceTutorialBubbleOffset = new Vector3(0f, -0.47f, 0f);

    //public override ScreenID ID
    //{
    //    get
    //    {
    //        return ScreenID.PrizeList;
    //    }
    //}

    //private HorizontalList PrizeSelectList
    //{
    //    get
    //    {
    //        return this.CarouselLists[0] as HorizontalList;
    //    }
    //}

    //private PipsElement Pips
    //{
    //    get
    //    {
    //        return this.PipsElementPlaceholder.GetBehaviourOnPrefab<PipsElement>();
    //    }
    //}

    //public override void OnActivate(bool zAlreadyOnStack)
    //{
    //    this.SetUpCarMenuForCurrentRes();
    //    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
    //    List<MultiplayerCarPrize> partiallyWonMultiplayerCars = activeProfile.GetPartiallyWonMultiplayerCars();
    //    if (!PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_PrizeScreenCompleted)
    //    {
    //        PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZE_LIST_TITLE", "TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZE_LIST_BODY", new PopUpButtonAction(this.OnTutorialPopupDismissed), false);
    //        PlayerProfileManager.Instance.ActiveProfile.MultiplayerTutorial_PrizeScreenCompleted = true;
    //        PlayerProfileManager.Instance.RequestImmediateSaveActiveProfile();
    //    }
    //    else if (partiallyWonMultiplayerCars.Count == 0)
    //    {
    //        PopUpDatabase.Common.ShowStargazerPopup("TEXT_POPUPS_MULTIPLAYER_TUTORIAL_PRIZE_LIST_TITLE", "TEXT_PRIZE_GALLERY_EMPTY", null, false);
    //    }
    //    this.ShowContinueButton(PrizePieceGiveScreen.AwardedNewPiece);
    //    this.continueButton.SetText(LocalizationManager.GetTranslation("TEXT_CAR_PART_CONTINUE"), true, true);
    //    PrizePieceGiveScreen.PiecesToAward = 0;
    //    base.OnActivate(zAlreadyOnStack);
    //}

    //public override void OnDeactivate()
    //{
    //    base.OnDeactivate();
    //    this.emptySpaceTutorialBubble = null;
    //    BubbleManager.Instance.KillAllMessages();
    //    PlayerProfileManager.Instance.ActiveProfile.UpdateCurrentPhysicsSetup();
    //}

    //protected override void Update()
    //{
    //    base.Update();
    //    if (this.emptySpaceTutorialBubble != null)
    //    {
    //        this.emptySpaceTutorialBubble.gameObject.transform.position = GameObjectHelper.MakeLocalPositionPixelPerfect(this.GetEmptySpaceBubblePosition());
    //    }
    //}

    //private void OnTutorialPopupDismissed()
    //{
    //    Vector3 emptySpaceBubblePosition = this.GetEmptySpaceBubblePosition();
    //    Log.AnEvent(Events.PrizeGalleryShown);
    //    this.emptySpaceTutorialBubble = BubbleManager.Instance.ShowMessage("TEXT_TUTORIAL_MESSAGE_PRIZE_LIST_EMPTY_SPACE", false, emptySpaceBubblePosition, BubbleMessage.NippleDir.UP, 0.5f, BubbleMessageConfig.ThemeStyle.SMALL, BubbleMessageConfig.PositionType.BOX_RELATIVE, 0.16f);
    //}

    //private Vector3 GetEmptySpaceBubblePosition()
    //{
    //    for (int i = 0; i < this.PrizeSelectList.NumItems; i++)
    //    {
    //        PrizePageListItem prizePageListItem = this.PrizeSelectList.GetItem(i) as PrizePageListItem;
    //        for (int j = 0; j < 3; j++)
    //        {
    //            SinglePrizeListItem prizeItem = prizePageListItem.GetPrizeItem(j);
    //            if (prizeItem.NullElement || prizeItem.LockedElement)
    //            {
    //                return prizeItem.gameObject.transform.position + this.emptySpaceTutorialBubbleOffset;
    //            }
    //        }
    //    }
    //    return new Vector3(0f, 0f, 0f);
    //}

    //public void GoViewPrize(string carID)
    //{
    //    for (int i = 0; i < this.PrizeSelectList.NumItems; i++)
    //    {
    //        PrizePageListItem prizePageListItem = this.PrizeSelectList.GetItem(i) as PrizePageListItem;
    //        for (int j = 0; j < 3; j++)
    //        {
    //            SinglePrizeListItem prizeItem = prizePageListItem.GetPrizeItem(j);
    //            if (!prizeItem.NullElement && !prizeItem.LockedElement)
    //            {
    //                if (prizeItem.Key == carID)
    //                {
    //                    this.PrizeSelectList.SelectedIndex = i;
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}

    //private void SetUpCarMenuForCurrentRes()
    //{
    //    this.PrizeSelectList.DesiredMenuItemWidth = GUICamera.Instance.ScreenWidth;
    //    this.PrizeSelectList.Height = GUICamera.Instance.ScreenHeight - 0.3f - CommonUI.Instance.NavBar.Background.height;
    //    float zNewY = (0.3f - CommonUI.Instance.NavBar.Background.height) / 2f + 0.15f;
    //    GameObjectHelper.SetLocalY(this.PrizeSelectList.gameObject, zNewY);
    //}

    //private int GetPurchaseOrder(CarInfo carInfo)
    //{
    //    List<CarGarageInstance> carsOwned = PlayerProfileManager.Instance.ActiveProfile.CarsOwned;
    //    for (int i = 0; i < carsOwned.Count; i++)
    //    {
    //        if (carsOwned[i].CarDBKey == carInfo.Key)
    //        {
    //            return i;
    //        }
    //    }
    //    return -1;
    //}

    //private int Comparison(CarInfo a, CarInfo b)
    //{
    //    int num = this.GetPurchaseOrder(a);
    //    int num2 = this.GetPurchaseOrder(b);
    //    bool flag = ArrivalManager.Instance.isCarOnOrder(a.Key);
    //    bool flag2 = ArrivalManager.Instance.isCarOnOrder(b.Key);
    //    if (flag)
    //    {
    //        num = 2147483647;
    //    }
    //    if (flag2)
    //    {
    //        num2 = 2147483647;
    //    }
    //    if (num != -1)
    //    {
    //        if (num2 != -1)
    //        {
    //            return num.CompareTo(num2);
    //        }
    //        return -1;
    //    }
    //    else
    //    {
    //        if (num2 != -1)
    //        {
    //            return 1;
    //        }
    //        return a.BasePerformanceIndex.CompareTo(b.BasePerformanceIndex);
    //    }
    //}

    //private PrizePageListItem AddPrizePageItemToList(int zListIndex, List<string> CarNames)
    //{
    //    GameObject gameObject = UnityEngine.Object.Instantiate(this.ListItemPrefabs[zListIndex]) as GameObject;
    //    gameObject.transform.parent = this.CarouselLists[zListIndex].transform;
    //    PrizePageListItem component = gameObject.GetComponent<PrizePageListItem>();
    //    if (component != null)
    //    {
    //        Vector2 pageSize = new Vector2(this.CarouselLists[zListIndex].DesiredMenuItemWidth, this.CarouselLists[zListIndex].Height);
    //        component.Create(CarNames, pageSize);
    //        this.CarouselLists[zListIndex].AddItem(component);
    //    }
    //    return component;
    //}

    //protected override void PopulateItemLists()
    //{
    //    base.PopulateItemLists();
    //    PlayerProfile activeProfile = PlayerProfileManager.Instance.ActiveProfile;
    //    List<MultiplayerCarPrize> partiallyWonMultiplayerCars = activeProfile.GetPartiallyWonMultiplayerCars();
    //    List<CarInfo> list = new List<CarInfo>();
    //    foreach (MultiplayerCarPrize current in partiallyWonMultiplayerCars)
    //    {
    //        list.Add(CarDatabase.Instance.GetCar(current.CarDBKey));
    //    }
    //    list.Sort((CarInfo a, CarInfo b) => this.Comparison(a, b));
    //    List<string> list2 = null;
    //    for (int i = 0; i < list.Count; i++)
    //    {
    //        if (list2 == null)
    //        {
    //            list2 = new List<string>();
    //        }
    //        list2.Add(list[i].Key);
    //        if (list2.Count == 3)
    //        {
    //            this.AddPrizePageItemToList(0, list2);
    //            list2 = null;
    //        }
    //    }
    //    if (list2 != null)
    //    {
    //        this.AddPrizePageItemToList(0, list2);
    //    }
    //    base.MarkAllItemsAddedToCarousels();
    //    this.Pips.SetNumPips(this.PrizeSelectList.NumItems);
    //}

    //private void OnItemClick(PrizePageListItem itm)
    //{
    //    if (itm.LastTappedWasLocked)
    //    {
    //        return;
    //    }
    //    this.LastSelectedPrize = itm.LastTappedPrize;
    //    MenuAudio.Instance.playSound(MenuBleeps.MenuClickForward);
    //    this.Pips.SetActivePip(this.PrizeSelectList.SelectedIndex);
    //    PrizePieceGiveScreen.OnLoadPrizeGained = this.LastSelectedPrize.Key;
    //    PrizePieceGiveScreen.PiecesToAward = 0;
    //    ScreenManager.Instance.PushScreen(ScreenID.PrizePieceGive);
    //}

    //protected override void OnItemClick(ListItem zItem)
    //{
    //    PrizePageListItem prizePageListItem = zItem as PrizePageListItem;
    //    if (prizePageListItem != null)
    //    {
    //        this.OnItemClick(prizePageListItem);
    //    }
    //}

    //protected override void SelectedItemChanged(ListItem zItem)
    //{
    //    PrizePageListItem x = zItem as PrizePageListItem;
    //    if (x != null)
    //    {
    //        this.Pips.SetActivePip(this.PrizeSelectList.SelectedIndex);
    //    }
    //}

    //public override void RequestBackup()
    //{
    //    if (MultiplayerUtils.InPrizeCarSequence)
    //    {
    //        MultiplayerUtils.InPrizeCarSequence = false;
    //        MultiplayerUtils.OfferStreakChainFrontend();
    //    }
    //    else
    //    {
    //        base.RequestBackup();
    //    }
    //}

    //public void ShowContinueButton(bool shouldShow)
    //{
    //    if (shouldShow)
    //    {
    //        this.continueButton.Runtime.CurrentState = BaseRuntimeControl.State.Active;
    //    }
    //    else
    //    {
    //        this.continueButton.Runtime.CurrentState = BaseRuntimeControl.State.Hidden;
    //    }
    //}
}
