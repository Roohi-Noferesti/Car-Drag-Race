using System.Linq;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ScreenShot))]
public class ScreenshotInspector : Editor
{
    private static CarInfo[] m_cars;
    private static int m_selectedCarIndex = -1;
    void OnEnable()
    {
        if (Application.isPlaying)
            return;
        if (m_cars == null || m_cars.Length == 0)
        {
            m_cars = new CarInfo[CarsList.Cars.Length];
            for (int i = 0; i < CarsList.Cars.Length; i++)
            {
                m_cars[i] = ResourceManager.GetCarAsset<CarInfo>(CarsList.Cars[i], ServerItemBase.AssetType.spec);
                //while (!rr.isDone)
                //{
                //    yield return 0;
                //}
                //cars[i] = (CarInfo) rr.asset;
                if (m_cars[i] == null)
                {
                    Debug.Log(CarsList.Cars[i] + "  not found , you may change resources folder name");
                }
            }
        }

        if (m_selectedCarIndex == -1)
        {
            var carVisual = FindObjectOfType<CarVisuals>();
            if (carVisual != null)
            {
                var key = carVisual.name;
                m_selectedCarIndex = ArrayUtility.IndexOf(m_cars, m_cars.FirstOrDefault(c => c.Key == key));
            }
        }
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (Application.isPlaying)
            return;
        GUI.enabled = true;
        m_selectedCarIndex = EditorGUILayout.IntSlider("Index", m_selectedCarIndex, 0, m_cars.Length-1);

        var car = m_cars[(m_selectedCarIndex)];

        if (car == null)
        {
            car = ResourceManager.GetCarAsset<CarInfo>(CarsList.Cars[m_selectedCarIndex], ServerItemBase.AssetType.spec);
        }
        
        if (GUILayout.Button("LoadNextCar:" + car.Key))
        {
            LoadAndPlaceCar(car);
        }
        if (GUILayout.Button("Render"))
        {
            Render(car);
            AssetDatabase.Refresh();
            Selection.activeObject =
                AssetDatabase.LoadMainAssetAtPath("Assets/Resources/cars/" + car.Key + "/thumbnail/" + car.Key+".png");
            ProjectWindowUtil.ShowCreatedAsset(Selection.activeObject);
        }


        EditorGUILayout.Separator();

        if (GUILayout.Button("BatchRenderAllCars"))
        {
            BatchRender();
        }

        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }

    private void LoadAndPlaceCar(CarInfo carinfo)
    {
        var carViuals = FindObjectOfType<CarVisuals>();
        if (carViuals != null)
        {
            DestroyImmediate(carViuals.gameObject);
        }

        //var carModel = ResourceManager.GetCarAsset<GameObject>(carinfo.Key, ServerItemBase.AssetType.garage_model);
        var carModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Prefabs/cars/" + carinfo.Key + "/garage_model/" + carinfo.Key + ".prefab");
        if (carModel == null)
        {
            Debug.LogError(string.Format("'{0}' model not found", carinfo.Key));
            carModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/_Prefabs/cars/" + carinfo.Key + "/garage_model/" + carinfo.Key + ".prefab");
            if (carModel == null)
            {
                Debug.LogError(string.Format("'{0}' model not found", carinfo.Key));
                return;
            }
        }
        var screenShotManager = (target as ScreenShot);
        //var pos = new Vector3(m_spawnPoint.position.x, carModel.transform.position.y, m_spawnPoint.position.z);
        var instance = (GameObject)Instantiate(carModel, screenShotManager.SpawnPoint.position, screenShotManager.SpawnPoint.rotation);
        instance.name = carModel.name;
        instance.gameObject.SetActive(true);

        var carVisual = instance.GetComponent<CarVisuals>();

        if (carVisual!=null)
        {
            carVisual.CacheChildNodes();
            carVisual.SetProperty(carinfo.Key, ServerItemBase.AssetType.body_shader, null);
            carVisual.SetProperty(carinfo.Key, ServerItemBase.AssetType.ring_shader, null);
            carVisual.SetProperty(carinfo.Key, ServerItemBase.AssetType.headlight_shader, null);
            carVisual.SetProperty(carinfo.Key, ServerItemBase.AssetType.sticker, null);
            carVisual.SetProperty(carinfo.Key, ServerItemBase.AssetType.spoiler, null);
        }

        screenShotManager.LoadLogo(carinfo);
    }

    private void Render(CarInfo carInfo)
    {
        (target as ScreenShot).TakeScreenShot(carInfo.Key);
    }

    private void BatchRender()
    {
        int i = 0;
        foreach (var carInfo in m_cars)
        {
            i++;
            LoadAndPlaceCar(carInfo);
            Render(carInfo);
            EditorUtility.DisplayProgressBar("Rendering", "rendering " + carInfo.Key + "...", (float)i / m_cars.Length);
        }

        EditorUtility.ClearProgressBar();
        AssetDatabase.Refresh();

        if (m_cars.Length > 0)
        {
            LoadAndPlaceCar(m_cars[m_selectedCarIndex]);
        }
    }
}
