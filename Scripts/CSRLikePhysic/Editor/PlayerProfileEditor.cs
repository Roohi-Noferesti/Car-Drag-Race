using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEditor;
using UnityEngine;

public class PlayerProfileEditor : EditorWindow
{
    private Vector2 m_scrollPosition;
    public PlayerProfileData profileData;
    private PlayerProfile playerProfile;
    private Account m_account;
    private string m_searchValue;

    private SerializedObject serializedObject;
    private SerializedProperty serializedProperty;
    private long m_userid;
    private long m_destUserID;
    private long m_sourceUserID;
    private GameObject m_serverHelperObject;
    private WebRequestQueueEditor m_WebRequestQueueEditor;
    
    [MenuItem("Tools/Player Profile/Reveal in Finder")]
    static void RevealInFinder()
    {
        var path = Application.persistentDataPath;
        EditorUtility.RevealInFinder(path);
    }

    [MenuItem("Tools/Player Profile/Edit ...")]
    static void Init()
    {
        GetWindow(typeof(PlayerProfileEditor)).Show();
    }

    [MenuItem("Tools/Player Profile/Log DeviceID")]
    static void LogDeviceID()
    {
        var id = SystemInfo.deviceUniqueIdentifier;
        GUIUtility.systemCopyBuffer = id;
        UnityEngine.Debug.Log("DeviceID(clipboard) : "+id);
    }
    
    [MenuItem("Tools/Player Profile/Clear Profile Data")]
    private static void ClearProfileData()
    {
        if (!EditorUtility.DisplayDialog("Clear profile data", "Are you sure you want to clear profile data", "Yes", "No"))
        {
            return;
        }

        if (Directory.Exists(Application.persistentDataPath))
        {
            Directory.Delete(Application.persistentDataPath,true);
        }

        if (Directory.Exists(Application.temporaryCachePath))
        {
            Directory.Delete(Application.temporaryCachePath, true);
        }

        PlayerPrefs.DeleteAll();

        EditorUtility.DisplayDialog("Profiel data", "Profiel data cleared", "Ok");
    }

    void OnEnable()
    {
        m_serverHelperObject = new GameObject("SHO (EditorOnly)");
        var webclient = m_serverHelperObject.AddComponent<WebClient>();
        m_WebRequestQueueEditor = new WebRequestQueueEditor(webclient);
    }

    void OnDisable()
    {
        if (m_serverHelperObject != null)
        {
            DestroyImmediate(m_serverHelperObject);
        }
        m_WebRequestQueueEditor = null;
    }

    private void UserDataResponseCallback(string content, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            Debug.LogError("failed to get profile data :" + zerror);
            return;
        }

        JsonDict parameters = new JsonDict();
        if (!parameters.Read(content))
        {
            Debug.LogError("error getting profile : server send malformed json in response");
            return;
        }

        var username = parameters.GetString("username");
        var profileDataJson = parameters.GetString("profile_data");
        profileData = PlayerProfileMapper.FromJson(profileDataJson);
        playerProfile = new PlayerProfile(username);
        playerProfile.SetProfileData(profileData);
        serializedObject = new SerializedObject(this);

        var accountdata = parameters.GetString("account_data");
        if (!string.IsNullOrEmpty(accountdata))
        {
            m_account = new Account();
            m_account = Account.LoadFromJson(accountdata);
        }
    }

    void OnGUI()
    {
        ServerSideOperationsGUI();

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Client side operations");
        if (GUILayout.Button("Load local data",GUILayout.Width(200)))
        {
            LoadGameData();
            return;
        }


        if (profileData != null && serializedObject!=null)
        {
            serializedProperty = serializedObject.FindProperty("profileData");
            if (GUILayout.Button("Save to local data", GUILayout.Width(200)))
            {
                if (EditorUtility.DisplayDialog("Saving local",
                    "Are you sure you want to save this data to local?", "Yes", "No"))
                {
                    SaveGameData();
                }
            }

            if (GUILayout.Button("Close data", GUILayout.Width(200)))
            {
                profileData = null;
                serializedObject = null;
                serializedProperty = null;
                return;
            }

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("1st tutorial", GUILayout.Width(100)))
            {
                SetupFor1stTutorial();
            }
            if (GUILayout.Button("2nd tutorial", GUILayout.Width(100)))
            {
                SetupFor2ndTutorial();
            }
            if (GUILayout.Button("3th tutorial", GUILayout.Width(100)))
            {
                SetupFor3thTutorial();
            }
            if (GUILayout.Button("4th tutorial", GUILayout.Width(100)))
            {
                SetupFor4thTutorial();
            }
            if (GUILayout.Button("5th tutorial", GUILayout.Width(100)))
            {
                SetupFor5thTutorial();
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal(GUILayout.Width(400));
            m_searchValue = EditorGUILayout.TextField("Find what", m_searchValue);
            if (GUILayout.Button("x", GUILayout.Width(20)))
            {
                m_searchValue = null;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUIUtility.labelWidth = 300;
            m_scrollPosition = EditorGUILayout.BeginScrollView(m_scrollPosition);

            bool hasChildren = true;
            while (serializedProperty.NextVisible(hasChildren))
            {
                if (string.IsNullOrEmpty(m_searchValue) || serializedProperty.name.ToLower().Contains(m_searchValue.ToLower()))
                {
                    EditorGUILayout.PropertyField(serializedProperty,
                        serializedProperty.hasVisibleChildren && serializedProperty.isExpanded);
                }

                hasChildren = serializedProperty.hasVisibleChildren && serializedProperty.isExpanded;
            }


            serializedObject.ApplyModifiedProperties();
            EditorGUILayout.EndScrollView();
        }
    }


    private void ServerSideOperationsGUI()
    {
        EditorGUILayout.LabelField("Server side operations");

        EditorGUILayout.BeginVertical();

        if (GUI.changed)
        {
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Load Server Data", GUILayout.Width(200)))
        {
            var username = "user" + m_userid;
            JsonDict parameters = new JsonDict();
            parameters.Set("username", username);
            parameters.Set("is_binary", false.ToString());
            WebRequestQueueEditor.Instance.StartCall("get_full_user_data", "get full user Data", parameters, UserDataResponseCallback,
                null, UsernameHashCode(username));
        }

        //if (GUILayout.Button("Load Server Data Binary", GUILayout.Width(200)))
        //{
        //    var username = "user" + m_userid;
        //    JsonDict parameters = new JsonDict();
        //    parameters.Set("username", username);
        //    parameters.Set("is_binary", true.ToString());
        //    WebRequestQueueEditor.Instance.StartCall("get_full_user_data", "get full user Data", parameters, UserDataResponseCallback,
        //        null, UsernameHashCode(username));
        //}
        m_userid = EditorGUILayout.LongField("UserID", m_userid,GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save server data (userid:" + m_userid+")", GUILayout.Width(200)))
        {
            if (EditorUtility.DisplayDialog("Saving to server",
                "Are you sure you want to save this data to server?", "Yes", "No"))
            {
                if (playerProfile != null && m_userid > 0)
                {
                    var profileDataString = PlayerProfileMapper.ToJson(profileData);
                    var parameters = new JsonDict();
                    var username = "user" + m_userid;
                    parameters.Set("username", username);
                    
                    parameters.Set("profile_data", profileDataString);
                    parameters.Set("user_data", profileDataString);
                    WebRequestQueueEditor.Instance.StartCall("save_user_data", "Save user Data", parameters, SaveUserDataResponseCallback, null, UsernameHashCode(username));
                }
            }
        }

        m_userid = EditorGUILayout.LongField("UserID", m_userid, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Transfer Profile Data", GUILayout.Width(200)))
        {
            if (m_destUserID == 0 || m_destUserID == m_sourceUserID)
            {
                EditorUtility.DisplayDialog("Error", "Destination must not zero and not equal to source id", "Ok");
                return;
            }

            if (!EditorUtility.DisplayDialog("Transfer",
                string.Format(
                    "Are you sure you want to transfer UserID:{0} profile data to UserID:{1}?This operation can not be undone.",
                    m_sourceUserID, m_destUserID), "Yes", "No"))
            {
                return;
            }

            var srcUsername = "user" + m_sourceUserID;
            var destUsername = "user" + m_destUserID;
            JsonDict parameters = new JsonDict();
            parameters.Set("source_username", srcUsername);
            parameters.Set("dest_username", destUsername);
            WebRequestQueueEditor.Instance.StartCall("transfer_profile_data", "transfer profile Data", parameters, TransferProfileDataResponse,
                null, UsernameHashCode(srcUsername));
        }
        m_sourceUserID = EditorGUILayout.LongField("From", m_sourceUserID, GUILayout.Width(200));
        m_destUserID = EditorGUILayout.LongField("To", m_destUserID, GUILayout.Width(200));
        EditorGUILayout.EndHorizontal();

        GUI.enabled = true;
    }

    private void SaveUserDataResponseCallback(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            Debug.LogError("Error saving user data :" + zerror);
        }
        else
        {
            Debug.Log("saving user data successfully .");
        }
    }

    private void TransferProfileDataResponse(string zhttpcontent, string zerror, int zstatus, object zuserdata)
    {
        if (zstatus != 200 || !string.IsNullOrEmpty(zerror))
        {
            Debug.LogError("Error transfer profile data :"+zerror);
        }
        else
        {
            Debug.Log("profileData tranfer successfully .");
        }
    }

    private string UsernameHashCode(string username)
    {
        var activePlatform = new StandalonePlatform();
        return activePlatform.HMACSHA1_Hash(username, BasePlatform.eSigningType.Server_Accounts);
    }

    private void SetupFor1stTutorial()
    {
        profileData.HasBoughtFirstCar = false;
        profileData.HasBoughtFirstUpgrade = false;
        profileData.HasChoosePlayerName = false;
        profileData.CarsOwned.Clear();
        profileData.GoldSpent = 0;
        profileData.RacesEntered = 0;
        profileData.RacesWon = 0;
        profileData.NewCars.Clear();
    }

    private void SetupFor2ndTutorial()
    {
        SetupFor1stTutorial();
        profileData.EventsCompleted.Clear();
        profileData.EventsCompleted.Add(1001);
    }

    private void SetupFor3thTutorial()
    {
        SetupFor2ndTutorial();
        profileData.HasBoughtFirstCar = true;
        profileData.EventsCompleted.Add(1002);

        var cargarage = new CarGarageInstance();
        var carInfo = ResourceManager.GetCarAsset<CarInfo>(CarsList.DefaultCar, ServerItemBase.AssetType.spec, null);
        cargarage.SetupNewGarageInstance(carInfo);
        profileData.CarsOwned.Add(cargarage);
        profileData.CurrentlySelectedCarDBKey = carInfo.ID;
    }

    private void SetupFor4thTutorial()
    {
        SetupFor3thTutorial();
        profileData.HasBoughtFirstUpgrade = true;
        profileData.EventsCompleted.Add(1003);
    }

    private void SetupFor5thTutorial()
    {
        SetupFor4thTutorial();
        profileData.RacesEntered = 5;
        profileData.RacesWon = 3;
    }

    private bool LoadGameData()
    {
        Accounts accounts = new Accounts();
        var defaultAccount = accounts.Default();
        if (defaultAccount == null)
        {
            defaultAccount = accounts.FindTemp();
        }
        if (defaultAccount != null)
        {
            playerProfile = new PlayerProfile(defaultAccount.Username);
            if (playerProfile.Load(EProfileFileType.account))
            {
                profileData = playerProfile.GetProfileData();
                serializedObject = new SerializedObject(this);
                return true;
            }
            else
            {
                Debug.LogError("Error loading profile data");
                return false;
            }
        }
        else
        {
            Debug.LogError("could not find default account");
            return false;
        }
    }

    private void SaveGameData()
    {
        if (playerProfile != null)
        {
            playerProfile.SetProfileData(profileData);
            playerProfile.Save();
        }

        if (m_account != null)
        {
            Accounts accounts = new Accounts();
            accounts.Save(m_account);
        }
    }

    private static string ApplyChecksum(string content)
    {
        return Hash(content) + "\n" + content;
    }

    static string Hash(string zSource)
    {
        byte[] key = Encoding.ASCII.GetBytes(BasePlatform.eSigningType.Client_Everything.ToString());
        HMACSHA1 myhmacsha1 = new HMACSHA1(key);
        byte[] byteArray = Encoding.ASCII.GetBytes(zSource);
        MemoryStream stream = new MemoryStream(byteArray);
        return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}", e), s => s);
    }

    private void Update()
    {
        if (m_WebRequestQueueEditor != null)
            m_WebRequestQueueEditor.Update();
    }
}