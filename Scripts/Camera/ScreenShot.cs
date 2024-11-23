using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ScreenShot : MonoSingleton<ScreenShot>
{
    private Camera m_camera;
    [SerializeField] private int m_size = 512;
    [SerializeField] private float m_aspectRatio = 1.86f;
    [SerializeField] private Transform m_spawnPoint;
    [SerializeField]
    private Image m_logoImage;
    [SerializeField]
    private Sprite[] m_logoSprites;

    private float m_initialLogoScale;


    private Dictionary<CarVisuals, Texture2D> m_cachedScreenShots = new Dictionary<CarVisuals, Texture2D>();

    public Transform SpawnPoint
    {
        get { return m_spawnPoint; }
    }

    public Texture2D this[CarVisuals CarVisual]
    {
        get { return m_cachedScreenShots[CarVisual]; }
    }

    protected override void Awake()
    {
        base.Awake();
        m_camera = GetComponentInChildren<Camera>();
        m_camera.enabled = false;
        m_initialLogoScale = m_logoImage.transform.localScale.x;
    }

    private Vector2 GetTextureSize()
    {
        int height = (int)(m_size / m_aspectRatio);
        return new Vector2(m_size, height);
    }
    public static Texture2D TakeScreenshot(string carID,bool saveToDisk=true)
    {
        if (Instance != null)
            return Instance.TakeScreenShot(carID, saveToDisk);
        return null;
    }

    public Texture2D TakeScreenShot(string carID, bool saveToDisk=true)
    {
        if (m_camera == null)
        {
            m_camera = GetComponentInChildren<Camera>();
        }
        // capture the virtuCam and save it as a square PNG.
        m_camera.enabled = true;

        m_camera.aspect = m_aspectRatio;
        // recall that the height is now the "actual" size from now on
        var textureSize = GetTextureSize();
        var tempRT = new RenderTexture((int)textureSize.x, (int)textureSize.y, 32);
        //// the 24 can be 0,16,24, formats like
        //// RenderTextureFormat.Default, ARGB32 etc.

        m_camera.targetTexture = tempRT;
        m_camera.Render();

        RenderTexture.active = tempRT;
        var virtualPhoto =
            new Texture2D((int)textureSize.x, (int)textureSize.y, TextureFormat.ARGB32, false);
        //// false, meaning no need for mipmaps
        virtualPhoto.ReadPixels(new Rect(0, 0, textureSize.x, textureSize.y), 0, 0);
        //Debug.Log("here");
        RenderTexture.active = null; //can help avoid errors 
        m_camera.targetTexture = null;
        // consider ... Destroy(tempRT);
        if (Application.isPlaying)
            Destroy(tempRT);

        //Debug.Log(Application.dataPath);

        if (saveToDisk)
        {
            if (Application.isPlaying)
                ResourceManager.SaveScreenshot(virtualPhoto, carID);
            else
            {
                string postfix = "";
                try
                {
                    postfix = GameDatabase.Instance.CarsConfiguration.postfix;
                } catch {}
                ResourceManager.SaveScreenshotInAssets(virtualPhoto, Application.dataPath +
                                                                     "/Resources/cars/" + carID + "/thumbnail/" + carID + postfix + ".png");
            }
        }


        // virtualCam.SetActive(false); ... no great need for this.

        // now use the image, 
        //UseFileImageAt(OurTempSquareImageLocation());
        if(Application.isPlaying)
        m_camera.enabled = false;

        m_camera.aspect = 1.777f;

        virtualPhoto.Apply();

        return virtualPhoto;
    }



    public static void TakeGarageScreenShot(string carID, bool saveToDisk, Action<Texture2D> callback)
    {
        Instance.StartCoroutine(_takeGarageScreenshotDelayed(carID, true,callback));
    }

    private static IEnumerator _takeGarageScreenshotDelayed(string carID, bool saveToDisk, Action<Texture2D> callback)
    {
        int width = Screen.width;
        int height = Screen.height;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        // Read screen contents into the texture
        yield return new WaitForEndOfFrame();
        tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        tex.Apply();

        if (saveToDisk)
        {
            if (Application.isPlaying)
                ResourceManager.SaveScreenshot(tex, carID + "_garage");
        }
        callback(tex);
    }

    public void LoadScreenShot(string carID,bool custom,Action<Texture2D> loadCompleted)
    {
        ResourceManager.GetCarThumbnail(carID, custom, loadCompleted);
    }

    public Texture2D PlaceCarAndRender(CarVisuals carVisual,CarInfo carinfo,bool cacheTexture = true,bool saveToDisk=true
        ,bool reverseLogo = false)
    {
        carVisual.CacheChildNodes();
        var position = carVisual.transform.position;
        var rotation = carVisual.transform.rotation;
        //var pos = new Vector3(m_spawnPoint.position.x, carModel.transform.position.y, m_spawnPoint.position.z);
        carVisual.transform.position = m_spawnPoint.position;
        carVisual.transform.rotation = m_spawnPoint.rotation;
        LoadLogo(carinfo, reverseLogo);
        var snapShot = TakeScreenshot(carinfo.Key, saveToDisk);

        //Get back car to its original position
        carVisual.transform.position = position;
        carVisual.transform.rotation = rotation;

        if (cacheTexture)
        {
            if (!m_cachedScreenShots.ContainsKey(carVisual))
            {
                m_cachedScreenShots.Add(carVisual, snapShot);
            }
            m_cachedScreenShots[carVisual] = snapShot;
        }
        return snapShot;
    }

    public void SetActive(bool b)
    {
        gameObject.SetActive(b);
    }

    public bool SnapshotCacheExists(CarVisuals carVisual)
    {
        return m_cachedScreenShots.ContainsKey(carVisual);
    }

    public void ClearSnapshotFromCache(CarVisuals carVisual)
    {
        m_cachedScreenShots.Remove(carVisual);
    }

    public void LoadLogo(CarInfo carinfo,bool reverse = false)
    {
        bool showCarsLogo = true;

        try {
            showCarsLogo = GameDatabase.Instance.CarsConfiguration.showCarsLogo;
        } catch {}
        
        if (showCarsLogo) {
            m_logoImage.gameObject.SetActive(true);
            m_logoImage.sprite = m_logoSprites.FirstOrDefault(s => "id_"+s.name == carinfo.ManufacturerID.ToString().ToLower());
            //m_logoImage.SetNativeSize();
            if (m_initialLogoScale == 0)
            {
                m_initialLogoScale = m_logoImage.transform.localScale.x;
            }
            m_logoImage.transform.localScale = new Vector3(reverse ? -m_initialLogoScale : m_initialLogoScale,
                m_initialLogoScale, m_initialLogoScale);
        } else {
            m_logoImage.gameObject.SetActive(false);
        }
    }
}
