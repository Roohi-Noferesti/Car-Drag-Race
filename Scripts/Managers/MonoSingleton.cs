using UnityEngine;

public abstract class MonoSingleton<T> : MonoBehaviour where T:MonoSingleton<T>
{
    public static T Instance { get; private set; }
    [SerializeField] private bool _dontDestroyOnLoad;

    protected virtual void Awake()
    {
        var objs = FindObjectsOfType<T>();
        if (objs.Length > 1)
        {
            Destroy();
        }
        else
        {
            Instance = (T)this;
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }
        }
    }

    protected virtual void Destroy()
    {
        Destroy(this);
    }

    protected virtual void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }
}
