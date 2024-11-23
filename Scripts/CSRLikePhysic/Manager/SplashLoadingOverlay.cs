using UnityEngine;
using UnityEngine.UI;

public class SplashLoadingOverlay : MonoBehaviour
{
    [SerializeField] private RawImage logo;
    [SerializeField] private Texture logoSprite_EN;
    [SerializeField] private Texture logoSprite_CH;
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (BuildType.IsAppTuttiBuild) {
            logo.texture = logoSprite_CH;
        } else {
            logo.texture = logoSprite_EN;
        }
        DontDestroyOnLoad(this);
    }

    public void Open()
    {
        animator.SetTrigger("open");
    }

    public void Close()
    {
        Kill();
        //animator.SetTrigger("close");
        //Invoke("Kill", 0.75f);
    }

    public void Kill()
    {
        DestroyImmediate(this.gameObject);
    }
}
