using System;
using System.Collections;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = UnityEngine.Object;
#if UNITY_EDITOR
using PlayerSettings = UnityEditor.PlayerSettings;

#endif


public class ObbDownloader : MonoBehaviour
{
    public static string ApplicationIdentifier;
    [SerializeField] private Text m_text;
    [SerializeField] private Text m_percentageText;
    [SerializeField] private Slider m_slider;
    [SerializeField] private string m_downloadStringText;
    [SerializeField] private string m_downloadStringTextEn;
    [SerializeField] private string m_errorStringText;
    [SerializeField] private string m_errorStringTextEn;
    [SerializeField] private string m_errorAccessStringText;
    [SerializeField] private string m_errorAccessStringTextEn;
    [SerializeField] private string m_corruptedFileErrorStringText;
    [SerializeField] private string m_corruptedFileErrorStringTextEn;
    [SerializeField] private string m_loadingGameText;
    [SerializeField] private string m_loadingGameTextEn;
    [SerializeField] private Button m_retryButton;
    [SerializeField] private bool m_downloadMainObb;
    [SerializeField] private string m_publicKey;
    private bool fileReady;
    private int screenTimeout;
    private UnityWebRequest request;
    private ulong m_lastByteRecived;
    private float m_freezing;
    private bool m_downloadTried;
#pragma warning disable 649
    //[SerializeField] private string mainFileChecksums;
    //[SerializeField] private string patchFileChecksums;
    private SystemLanguage systemLanguage;
    private IGooglePlayObbDownloader m_obbDownloader;
    private static string expPath;
    private bool m_insideCountry;
    private UnityWebRequestAsyncOperation async;

#if UNITY_ANDROID


#pragma warning restore 649

    private void Awake()
    {
        ApplicationIdentifier = Application.identifier;
        systemLanguage = Application.systemLanguage;
#if !UNITY_EDITOR && UNITY_ANDROID
        AndroidSpecific.Initialise();
#endif

        //StartCoroutine(TestAssetBundle());
        screenTimeout = Screen.sleepTimeout;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        m_text.gameObject.SetActive(false);
        m_percentageText.gameObject.SetActive(false);
        m_slider.gameObject.SetActive(false);
        m_retryButton.gameObject.SetActive(false);
        m_insideCountry = BasePlatform.ActivePlatform.InsideCountry;
        //Debug.Log("inside iran is : " + m_insideCountry);

        try
        {
            TryDownload();
        }
        catch (Exception)
        {
            m_text.gameObject.SetActive(true);
            m_text.text = systemLanguage == SystemLanguage.Unknown?m_errorAccessStringText:m_errorAccessStringTextEn;
        }

    }


    /*
    private IEnumerator TestAssetBundle()
    {
        var path = AndroidSpecific.GetObbRootPath();

        Debug.Log("Obb Root Path : " + path);

        var uri = "jar:file://" + path + "/testbundle";

        Debug.Log("test Bundl Path : " + uri);

        UnityWebRequest webRequest = UnityWebRequestAssetBundle.GetAssetBundle(new Uri(uri), 1);

        yield return webRequest.SendWebRequest();

        Debug.Log("Web Request is done : " + webRequest.downloadedBytes);

        AssetBundle bundle = DownloadHandlerAssetBundle.GetContent(webRequest);
        if (bundle == null)
        {
            Debug.Log("Asset Bundle is null for : " + webRequest.url);
        }
        else
        {
            var bundleObject = bundle.LoadAsset<Object>(bundle.GetAllAssetNames()[0]);
            Debug.Log("Bundle " + bundleObject.name + " loaded from asset bundle");
        }
    }
    */


    public void TryDownload()
    {
        StartCoroutine(DownloadCoroutine());
    }

    private void InitializeObbDownloader()
    {
        m_obbDownloader = GooglePlayObbDownloadManager.GetGooglePlayObbDownloader();
        m_obbDownloader.PublicKey = m_publicKey;
        expPath = m_obbDownloader.GetExpansionFilePath();
    }

    private void HandleGoogleObbDownloader()
    {
        InitializeObbDownloader();

        if (!GooglePlayObbDownloadManager.IsDownloaderAvailable())
        {
            GTDebug.Log(GTLogChannel.Android, "download is not available");
            m_text.text = m_errorStringText;
            return;
        }

        if (expPath == null)
        {
            m_text.text = m_errorStringText;
            //GUI.Label(new Rect(10, 10, Screen.width - 10, 20), "External storage is not available!");
        }
        else
        {
            //var mainPath = m_obbDownloader.GetMainOBBPath();
            //var patchPath = m_obbDownloader.GetPatchOBBPath();

            //GUI.Label(new Rect(10, 10, Screen.width - 10, 20), "Main = ..." + (mainPath == null ? " NOT AVAILABLE" : mainPath.Substring(expPath.Length)));
            //GUI.Label(new Rect(10, 25, Screen.width - 10, 20), "Patch = ..." + (patchPath == null ? " NOT AVAILABLE" : patchPath.Substring(expPath.Length)));
            if (!OBBStatusOK())
            {
                GTDebug.Log(GTLogChannel.Android,"fetch obb");
                m_obbDownloader.FetchOBB();
            }
        }
    }

    public bool OBBStatusOK()
    {
        if (m_obbDownloader == null)
            return false;
        string mainOBBPath = m_obbDownloader.GetMainOBBPath();
        //string patchOBBPath = m_obbDownloader.GetPatchOBBPath();
        if (mainOBBPath == null)// || patchOBBPath == null)
        {
            return false;
        }
        return true;
    }


    
    private IEnumerator DownloadCoroutine()
    {
        yield return new WaitForEndOfFrame();
#if !UNITY_EDITOR && UNITY_ANDROID
        yield return StartCoroutine(PermissionManager.CheckRequiredPermission());
#endif
        if (m_downloadMainObb)
        {
            if (!m_insideCountry)
            {
                //This means that our game is published on play store so we handle download by google obb downloader
                HandleGoogleObbDownloader();
            }
            else
            {

                EnsureDirectoryExists(GetObbRoot());

                fileReady = true;
                // var patchObbPath = GetObbFilePath(false);
                var isHashValid = false;


                var mainObbPath = GetObbFilePath(true);
                var obbFileExists = File.Exists(mainObbPath);
                if (!obbFileExists)
                {
                    GTDebug.LogWarning(GTLogChannel.Android,"mainObbPath: " + mainObbPath + "not found");
                }
                else
                {
                    yield return IsHashValid(mainObbPath, true, result => { isHashValid = result; });
                }

                if (!isHashValid)
                {
                    GTDebug.Log(GTLogChannel.Android,"obb File does not exists");
                    DeleteOldObbFile(true);
                    fileReady = false;
                    m_downloadTried = true;
                    GTDebug.Log(GTLogChannel.Android,"trying to download obb file");
                    yield return StartCoroutine(DownloadFile(mainObbPath, true));
                }
            }
        }
        else
        {
            
            fileReady = true;
        }
        //if (!File.Exists(patchObbPath) || !IsFileValid(patchObbPath, patchFileChecksums))
            //{
            //    DeleteOldObbFile(false);
            //    fileReady = false;
            //    yield return StartCoroutine(DownloadFile(patchObbPath, false));
            //}

            if (fileReady)
            {
                GTDebug.Log(GTLogChannel.Other,"Files ready . Loading scene 1");
                if (m_downloadTried)
                {
#if UNITY_ANDROID
                    var title = systemLanguage == SystemLanguage.Unknown ? "دانلود تکمیل شد" : "Download Completed!";
                    var body = systemLanguage == SystemLanguage.Unknown ? m_loadingGameText : m_loadingGameTextEn;
                    AndroidSpecific.ShowDialog(title, body);
#endif
                }
#if !UNITY_EDITOR && UNITY_ANDROID
                // AndroidSpecific.ObbMount();
#endif
                yield return new WaitForEndOfFrame();
                Screen.sleepTimeout = screenTimeout;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);;
            }
        
    }

    private void DeleteOldObbFile(bool isMain)
    {
        var obbRoot = GetObbRoot();
        if (Directory.Exists(obbRoot))
        {
            DirectoryInfo directory = new DirectoryInfo(obbRoot);
            var files = directory.GetFiles(isMain ? "main*.obb" : "patch*.obb", SearchOption.TopDirectoryOnly);
            foreach (var fileInfo in files)
            {
                try
                {
                    fileInfo.Delete();
                }
                catch (Exception e)
                {
                    GTDebug.LogWarning(GTLogChannel.Android, "Can't delete file:" + e.Message);
                }
            }
        }
    }

    private IEnumerator DownloadFile(string localFilePath, bool isMain)
    {
        m_text.text = string.Empty;
        m_slider.value = 0;
        m_percentageText.text = "0";
        m_retryButton.gameObject.SetActive(false);
        m_text.gameObject.SetActive(true);
        m_slider.gameObject.SetActive(true);
        m_percentageText.gameObject.SetActive(true);
        m_lastByteRecived = 0;
        m_freezing = 0;

        if (Application.internetReachability == NetworkReachability.NotReachable)
        {
            GTDebug.Log(GTLogChannel.Android,"No internet");
            m_text.text = systemLanguage == SystemLanguage.Unknown?m_errorStringText:m_errorStringTextEn;
            m_retryButton.gameObject.SetActive(true);
            yield break;
        }

        GTDebug.Log(GTLogChannel.Android,"getting file path");
        var fileName = Path.GetFileName(localFilePath);

        var path = Endpoint.GetS3URL("obb/"+Application.version+"/"+ fileName);
        
        request = UnityWebRequest.Get(path);
        request.downloadHandler = new ToFileDownloadHandler(new byte[64 * 1024], localFilePath);
        async = request.SendWebRequest();

        while (!async.isDone)
        {
            yield return new WaitForEndOfFrame();

            if (m_lastByteRecived == request.downloadedBytes)
            {
                m_freezing += Time.deltaTime;
            }
            else
            {
                m_freezing = 0;
            }

            m_lastByteRecived = request.downloadedBytes;
            var mbDownloaded = (float)request.downloadedBytes / 1024;
            var mbDownloadedString = string.Format("{0:###} kb", mbDownloaded);
            if (mbDownloaded > 900)
            {
                mbDownloaded /= 1024;
                mbDownloadedString = string.Format("{0:#0.0}", mbDownloaded);
            }

            var totalSizeByte = Convert.ToInt64(request.GetResponseHeader("Content-Length"));
            var totalSize = totalSizeByte / ((float)1024*1024);
            var totalSizeString = string.Format("{0:#0.0}", totalSize);
            m_text.text = string.Format(systemLanguage == SystemLanguage.Unknown ? m_downloadStringText : m_downloadStringTextEn, mbDownloadedString, totalSizeString);
            var progressValue = (float)request.downloadedBytes / totalSizeByte;
            m_percentageText.text = string.Format("{0:p1}", progressValue);
            m_slider.value = progressValue;//async.progress;

            if (request.isNetworkError || (m_freezing > 60 && !request.isDone))
            {
                GTDebug.LogError(GTLogChannel.Android,request.error);
                m_text.text = systemLanguage == SystemLanguage.Unknown ? m_errorStringText:m_errorStringTextEn;
                m_retryButton.gameObject.SetActive(true);
                Cancel();
                yield break;
            }
        }

        GTDebug.LogError(GTLogChannel.Android,"Checking error : "+ request.error);
        if (string.IsNullOrEmpty(request.error))
        {
            var mainObbPath = GetObbFilePath(true);
            bool isHashValid = false;
            GTDebug.Log(GTLogChannel.Android,"Checking Is Hash valid...");
            yield return IsHashValid(mainObbPath,false, result=>
            {
                isHashValid = result;
            });
            try
            {
                //var expectedChecksum = isMain ? mainFileChecksums : patchFileChecksums;
                //if (isMain || IsFileValid(localFilePath, expectedChecksum))
                //{
                //    fileReady = true;
                //}
               
                    if (isHashValid)
                    {
                        fileReady = true;
                    }
                    else
                    {
                        fileReady = false;
                        var error = systemLanguage == SystemLanguage.Unknown ? m_corruptedFileErrorStringText:m_corruptedFileErrorStringTextEn;
                        GTDebug.LogError(GTLogChannel.Android,error);
                        m_text.text = error;
                        m_retryButton.gameObject.SetActive(true);
                        m_slider.value = 0;
                        m_percentageText.text = "0";
                    }

            }
            catch (Exception e)
            {
                Debug.LogError("Error checking fies : "+e.Message);
                m_text.text = e.Message;
            }
        }
        else
        {
            fileReady = false;
            var error = (systemLanguage == SystemLanguage.Unknown ? m_errorStringText : m_errorStringTextEn) + "\n" +
                        request.error;
            GTDebug.LogError(GTLogChannel.Android,error);
            m_text.text = error;
            m_retryButton.gameObject.SetActive(true);
        }
    }



    private IEnumerator IsHashValid(string path, bool obbFileExist, Action<bool> onComplete)
    {
        var version = Application.version;


        if (obbFileExist && Application.internetReachability == NetworkReachability.NotReachable)
        {
            onComplete(true);
            yield break;
        }

        var obbHashPrefKey = "OBB_HASH_STATUS" + version;
        if (obbFileExist && PlayerPrefs.HasKey(obbHashPrefKey))
        {
            if (PlayerPrefs.GetInt(obbHashPrefKey) == 1)
            {
                onComplete(true);
                yield break;
            }
        }

        // todo: change URI
        var uri = (!Endpoint.GetEC2ServerSecure() ? "http://" : "https://") +
                  Endpoint.ServerEndpointCollection.Current.AccountsApiUrl + "/getHash?version=" + Application.version;
        //Debug.LogError(uri);
        var getObbHashReq = UnityWebRequest.Get(uri).SendWebRequest();
        yield return getObbHashReq;
        // if there is network error for example resolving host , then we consider hash as valid
        if (getObbHashReq.webRequest.isNetworkError || getObbHashReq.webRequest.isHttpError)
        {
            Debug.LogError("GetHashError: " + getObbHashReq.webRequest.error);
            onComplete(true);
            yield break;
        }

        var expectedHash = getObbHashReq.webRequest.downloadHandler.text;
        var computedHash = MathTools.ComputeHashForFile(path);

        if (expectedHash == computedHash)
        {
            PlayerPrefs.SetInt(obbHashPrefKey, 1);
        }

        GTDebug.Log(GTLogChannel.Android, "expectedHash : " + expectedHash + "    ,  computedHash : " + computedHash);
        onComplete(expectedHash == computedHash);
    }


    //private bool IsFileValid(string path, string expectedChecksums)
    //{
    //    return expectedChecksums == MathTools.ComputeHashForFile(path);
    //}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if (DownloadedFromGooglePlay && OBBStatusOK())
        {
#if UNITY_ANDROID
            // AndroidSpecific.ObbMount();
#endif
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
        }
    }

    public bool DownloadedFromGooglePlay
    {
        get { return m_downloadMainObb && !m_insideCountry; }
    }

    private void Cancel()
    {
        if (!fileReady && request != null && request.downloadHandler != null)
        {
            (request.downloadHandler as ToFileDownloadHandler).Cancel();
        }
    }

    private void OnDestroy()
    {
        Cancel();
    }

    private void EnsureDirectoryExists(string dir)
    {
        if (!Directory.Exists(dir))
        {
           Directory.CreateDirectory(dir);
        }
    }

    private string GetObbFilePath(bool main)
    {
#if UNITY_EDITOR
        return GetObbFilePathEditor(main);
#elif UNITY_ANDROID
        return AndroidSpecific.GetObbFullPath(main);
#else
        return null;
#endif
    }


#if UNITY_EDITOR
    private string GetObbFilePathEditor(bool main)
    {
        var version = PlayerSettings.Android.bundleVersionCode;//main ? PlayerSettings.Android.bundleVersionCode : AndroidSpecific.obbPatchVersion;
        var path = Application.dataPath + "/../BuiltBundles/Android/";
        path += main ? "main." : "patch.";
        path += version + "." +
                PlayerSettings.applicationIdentifier + ".obb";
        return path;
    }
#endif



    private string GetObbRoot()
    {
#if UNITY_EDITOR
        return GetObbRootEditor();
#elif UNITY_ANDROID
        return AndroidSpecific.GetObbRoot();
#else
        return null;
#endif
    }


#if UNITY_EDITOR
    private string GetObbRootEditor()
    {
        var path = Application.dataPath + "/../BuiltBundles/Android/";
        return path;
    }
#endif

#endif

}