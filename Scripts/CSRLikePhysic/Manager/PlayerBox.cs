using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerBox : MonoBehaviour
{
	public Text PlayerNameText;

    public Text CarNameText;

	public GameObject RPNode;

    public Text RPText;

	public GameObject BestTimeNode;

    public Text BestTimeText;

    //public CarStatsElem CarStatsElem;

	public Image CarSnapshotSprite;

	public GameObject PlayerPanel;

	public GameObject Consumable_Mechanic;

	public GameObject Consumable_Engine;

	public GameObject Consumable_PRAgent;

	public GameObject Consumable_Tyre;

	public GameObject Consumable_N2O;

	public GameObject ConsumablesBackground;

	public GameObject ConsumablesBackgroundFriendMode;

    //public ConsumableIconAnim ConsumableIcons;

	private GameObject eliteGameObject;

    //public StarAvatarFrame StarFrame;

	public bool HasBeenSetup
	{
		get;
		private set;
	}

	public void SetBoxProfile(PlayerInfo playerInfo, CarGarageInstance playerCar, Texture2D snapshotTexture, Type loadingType, bool localPlayer)
	{
        //if (playerInfo == null)
        //{
        //    return;
        //}
        //this.PlayerNameText.Text = playerInfo.DisplayName.ToUpper();
        //this.CarSnapshotSprite.renderer.material.SetTexture("_MainTex", snapshotTexture);
        //this.CarSnapshotSprite.SetPixelDimensions(new Vector2((float)snapshotTexture.width, (float)snapshotTexture.height));
        //this.StarFrame.Picture.ShowBadges = !RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent();
        //if (RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent())
        //{
        //    RWFPlayerInfoComponent component = playerInfo.GetComponent<RWFPlayerInfoComponent>();
        //    if (!component.HasSetTimeInCurrentCar)
        //    {
        //        this.BestTimeNode.SetActive(false);
        //    }
        //    this.RPNode.SetActive(false);
        //}
        //else if (loadingType != typeof(InternationalVersusScreen))
        //{
        //    RTWPlayerInfoComponent component2 = playerInfo.GetComponent<RTWPlayerInfoComponent>();
        //    if (component2 != null)
        //    {
        //        this.RPText.Text = CurrencyUtils.GetRankPointsString(component2.RankPoints, true, true);
        //    }
        //    this.BestTimeNode.SetActive(false);
        //}
        //RacePlayerInfoComponent component3 = playerInfo.GetComponent<RacePlayerInfoComponent>();
        //CarInfo car = CarDatabase.Instance.GetCar(component3.CarDBKey);
        //string text = LocalizationManager.GetTranslation(car.ShortName);
        //this.CarNameText.Text = text.ToUpper();
        //this.CarStatsElem.Set(car.BaseCarTier, (!localPlayer) ? component3.PPIndex : playerCar.CurrentPPIndex);
        //bool flag = !MultiplayerUtils.FakeNoComsumablesActive && (!MultiplayerUtils.FakeAComsumableActive || localPlayer);
        //bool flag2 = MultiplayerUtils.FakeAComsumableActive && !localPlayer;
        //bool active = false;
        //bool active2 = false;
        //bool active3 = false;
        //bool active4 = false;
        //bool active5 = false;
        //bool flag3 = RaceEventInfo.Instance.CurrentEvent.IsFriendRaceEvent();
        //bool flag4 = RaceEventInfo.Instance.CurrentEvent.IsRelay || loadingType == typeof(InternationalVersusScreen);
        //if (!flag3 && !flag4)
        //{
        //    if (playerInfo.DoesComponentExist<ConsumablePlayerInfoComponent>())
        //    {
        //        ConsumablePlayerInfoComponent component4 = playerInfo.GetComponent<ConsumablePlayerInfoComponent>();
        //        active2 = ((component4.ConsumableEngine > 0 && flag) || (flag2 && MultiplayerUtils.FakeComsumable == eCarConsumables.EngineTune));
        //        active3 = ((component4.ConsumablePRAgent > 0 && flag) || (flag2 && MultiplayerUtils.FakeComsumable == eCarConsumables.PRAgent));
        //        active4 = ((component4.ConsumableTyre > 0 && flag) || (flag2 && MultiplayerUtils.FakeComsumable == eCarConsumables.Tyre));
        //        active5 = ((component4.ConsumableN2O > 0 && flag) || (flag2 && MultiplayerUtils.FakeComsumable == eCarConsumables.Nitrous));
        //    }
        //}
        //else if (playerInfo.DoesComponentExist<MechanicPlayerInfoComponent>())
        //{
        //    MechanicPlayerInfoComponent component5 = playerInfo.GetComponent<MechanicPlayerInfoComponent>();
        //    active = component5.MechanicEnabled;
        //}
        //this.Consumable_Mechanic.gameObject.SetActive(active);
        //this.Consumable_Engine.gameObject.SetActive(active2);
        //this.Consumable_PRAgent.gameObject.SetActive(active3);
        //this.Consumable_Tyre.gameObject.SetActive(active4);
        //this.Consumable_N2O.gameObject.SetActive(active5);
        //this.ConsumableIcons.SetToPlayerInfo(playerInfo, !localPlayer, flag4 || flag3);
        //bool isStar = playerInfo.IsStar;
        //playerInfo.Persona.SetupAvatarPicture(this.StarFrame.Picture);
        //if (playerInfo.DoesComponentExist<RWFPlayerInfoComponent>())
        //{
        //    this.StarFrame.Create(StarsManager.GetStarFromPlayerInfo(playerInfo), false, isStar);
        //}
        //if (this.eliteGameObject != null)
        //{
        //    UnityEngine.Object.Destroy(this.eliteGameObject);
        //    this.eliteGameObject = null;
        //}
        //bool flag5 = component3.IsEliteCar || (!localPlayer && RaceEventInfo.Instance.CurrentEvent.IsAIDriverAvatarAvailable());
        //if (flag5)
        //{
        //    EliteCarOverlay eliteCarOverlay = EliteCarOverlay.Create(false);
        //    this.eliteGameObject = eliteCarOverlay.gameObject;
        //    eliteCarOverlay.Setup(new Vector3(-0.785f, -0.075f, -0.34f), this.PlayerPanel.transform, 0.4f, 0.32f);
        //}
        //if (flag3 || flag4)
        //{
        //    this.ConsumablesBackground.SetActive(false);
        //    this.ConsumablesBackgroundFriendMode.SetActive(RaceEventInfo.Instance.CurrentEvent.IsMechanicAllowed());
        //}
        //else
        //{
        //    this.ConsumablesBackground.SetActive(true);
        //    this.ConsumablesBackgroundFriendMode.SetActive(false);
        //}
        //this.HasBeenSetup = true;
	}

	private void OnDestroy()
	{
        //Material material = this.CarSnapshotSprite.renderer.material;
        //Texture mainTexture = material.mainTexture;
        //if (mainTexture != null)
        //{
        //    UnityEngine.Object.Destroy(mainTexture);
        //}
        //UnityEngine.Object.Destroy(material);
	}
}
