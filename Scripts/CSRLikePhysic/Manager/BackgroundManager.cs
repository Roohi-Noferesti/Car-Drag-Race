using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundManager
{
	public enum BackgroundType
	{
		None,
		Garage,
		Showroom,
		Gradient,
		Smoke_THISHASBEENDELETED,
		ColourGradient,
		Monitors,
		Black,
		CSRN
	}

	private static GameObject showroomInstance;

	private static GameObject garageInstance;

	private static BackgroundType _currentbgID;

	public static bool IsValid
	{
		get;
		private set;
	}

	public static void Load()
	{
		if (SceneManager.GetActiveScene().name != "Frontend")
		{
			return;
		}
		if (showroomInstance != null || garageInstance != null)
		{
			return;
		}
		LoadGarage();
        CleanDownManager.Instance.OnBackgroundManagerLoadFinished();
		IsValid = true;
	}

	public static void UnloadAll()
	{
		UnloadShowroom();
		UnloadGarage();
		IsValid = false;
        _currentbgID = BackgroundType.None;
	}

	private static void LoadGarage()
	{
		GameObject gameObject = Resources.Load("Prefabs/Environments/Garage") as GameObject;
		garageInstance = (Object.Instantiate(gameObject) as GameObject);
		garageInstance.name = gameObject.name;
		garageInstance.SetActive(false);
	}

	private static void UnloadGarage()
	{
		if (garageInstance != null)
		{
			Object.Destroy(garageInstance);
			garageInstance = null;
		}
	}

	private static void DisableGarageScene()
	{
		if (garageInstance && garageInstance.activeInHierarchy)
		{
			garageInstance.SetActive(false);
		}
	}

	private static void EnableGarageScene()
	{
		if (garageInstance == null)
		{
			LoadGarage();
		}
		if (!garageInstance.activeInHierarchy)
		{
			garageInstance.SetActive(true);
		}
        if (!SceneManagerGarage.Instance.isActiveAndEnabled)
        {
            SceneManagerGarage.Instance.ActivateAndEnable();
        }
	}

	private static void EnableShowroomScene()
	{
		GameObject gameObject = Resources.Load("Prefabs/Environments/Showroom") as GameObject;
		showroomInstance = (Object.Instantiate(gameObject) as GameObject);
		showroomInstance.name = gameObject.name;
		if (!showroomInstance.activeInHierarchy)
		{
			showroomInstance.SetActive(true);
		}
        if (!SceneManagerShowroom.Instance.gameObject.activeInHierarchy)
        {
            SceneManagerShowroom.Instance.gameObject.SetActive(true);
        }
        //if (!ShowroomCameraManager.Instance.gameObject.activeInHierarchy)
        //{
        //    ShowroomCameraManager.Instance.gameObject.SetActive(true);
        //}
	}

	private static void UnloadShowroom()
	{
		if (showroomInstance != null)
		{
			Object.Destroy(showroomInstance);
			showroomInstance = null;
		}
		ClearShowroomSlot();
	}

	private static void ClearShowroomSlot()
	{
        AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICar);
        AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.AICarLivery);
        AsyncSwitching.Instance.ClearSlot(AsyncBundleSlotDescription.ManufacturerLogo);
        CleanDownManager.Instance.OnShowroomSlotClear();
	}

	private static void SetBackground()
	{
		BackgroundType currentbgID = _currentbgID;
		if (currentbgID != BackgroundType.Garage)
		{
			if (currentbgID != BackgroundType.Showroom)
			{
				SetMeshBackground();
			}
			else
			{
				SetMeshShowroom();
			}
		}
		else
		{
			SetMeshGarage();
		}
	}

	private static void SetMeshGarage()
	{
		UnloadShowroom();
		EnableGarageScene();
	}

	private static void SetMeshShowroom()
	{
		DisableGarageScene();
		EnableShowroomScene();
	}

	private static void SetMeshBackground()
	{
		DisableGarageScene();
		UnloadShowroom();
	}

	public static void ShowBackground(BackgroundType bgType)
	{
		ShowBackground(bgType, false);
	}

	public static void ShowBackground(BackgroundType bgType, bool force)
	{
		if (!IsValid)
		{
			return;
		}
		if (SceneLoadManager.Instance.CurrentScene != SceneLoadManager.Scene.Frontend)
		{
			return;
		}
		if (_currentbgID != bgType || force)
		{
			_currentbgID = bgType;
			SetBackground();
		}
	}
}
