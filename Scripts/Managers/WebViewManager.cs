using System;
using System.Threading.Tasks;
using UnityEngine;



public class WebViewManager : MonoBehaviour
{
    public UniWebView m_webView;
    public static WebViewManager Instance;
    private Action<string> actOnReceiveMessage;
    private Action actOnWebViewClose;
    private bool _isOpen;
    void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        m_webView.AddUrlScheme("gt-club");
        m_webView.AddUrlScheme("zarinpalpaymant");
        m_webView.AddUrlScheme("zarinpalpaymant");
    //    m_webView.AddUrlScheme("gt-club://zarinpalpaymant");
        m_webView.OnMessageReceived += DidReceiveMessage;
        m_webView.OnShouldClose += DidHideEvent;
        m_webView.OnOrientationChanged += OnOrientationChanged;

    }

   

    private  void OnOrientationChanged(UniWebView webView, ScreenOrientation orientation)
    {
  
        m_webView.Frame = new Rect(0, 0, Screen.width / EnvQualitySettings.ScreenScale, Screen.height / EnvQualitySettings.ScreenScale);
       
        
        Debug.Log(Screen.width + "//" + Screen.height + "//" + m_webView.Frame.width + "//" + m_webView.Frame.height);


    }

    public void SetWebView(UniWebView uniWebView)
    {
        m_webView = uniWebView;


    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            var url = "https://docs.uniwebview.com/guide/using-prefab.html#adding-to-scene";
            Show(url, (value) => { Debug.Log("OnReciveMessae"); }, () => { Debug.Log("OnClosed!"); });
        }

        if (Input.GetKeyDown(KeyCode.Escape) && _isOpen)
        {
            Finish();
            if (actOnWebViewClose != null)
            {
                actOnWebViewClose.Invoke();
                actOnWebViewClose = null;
            }
        }
    }
    public void Show(string url, Action<string> actOnReceiveMessage = null, Action actOnWebViewClose = null)
    {

#if UNITY_EDITOR
        Application.OpenURL(url);
        return;
#endif
        _isOpen = true;
        Screen.orientation = ScreenOrientation.Portrait;

        this.actOnReceiveMessage = actOnReceiveMessage;
        this.actOnWebViewClose = actOnWebViewClose;
        m_webView.Load(url);
        m_webView.Show();
    }

    public void Finish()
    {
        _isOpen = false;
        Screen.orientation = ScreenOrientation.Landscape;
        Screen.orientation = ScreenOrientation.AutoRotation;
        m_webView.Hide();
    }
    private bool DidHideEvent(UniWebView webView)
    {
        return false;
    }
    private void DidReceiveMessage(UniWebView _webview, UniWebViewMessage _message)
    {
        Debug.Log("WebView ReceivedMessageEvent: " + _message.ToString());
        foreach (var item in _message.Args)
        {
            Debug.Log("WebView : " + item.Key+"//"+item.Value);
        }
        if (actOnReceiveMessage != null)
        {
            actOnReceiveMessage.Invoke(_message.Args["Status"] + "," + _message.Args["Authority"]);
            actOnReceiveMessage = null;
        }
    }


}
