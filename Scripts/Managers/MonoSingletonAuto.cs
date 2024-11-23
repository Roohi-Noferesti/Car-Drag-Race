using UnityEngine;

public abstract class MonoSingletonAuto<T> : MonoBehaviour where T:MonoSingletonAuto<T>
{
    private bool m_persistent = true;
    private static T m_instance;

    public static T Instance
    {
        get
        {
            if (m_instance == null)
            {
                var objs = FindObjectsOfType<T>();
                if (objs.Length > 0)
                {
                    m_instance = objs[0];
                }
                else
                {
                    m_instance = new GameObject(typeof(T).Name).AddComponent<T>();
                }
            }
            return m_instance;
        }
    }

    public static bool HasInstance
    {
        get { return m_instance != null; }
    }

    protected virtual bool Awake()
    {
        return checkInstance();
    }

    protected bool checkInstance()
    {
        if (m_instance != null && m_instance != this)
        {
            Destroy(this);
            return false;
        }

        m_instance = (T)this;
        if (m_persistent)
            DontDestroyOnLoad(gameObject);
        return true;
    }



    protected virtual void OnDestroy()
    {
        m_instance = null;
    }
}
