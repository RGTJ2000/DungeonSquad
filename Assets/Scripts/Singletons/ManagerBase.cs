using UnityEngine;

public abstract class ManagerBase<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<T>();

                if (_instance == null)
                {
                    // Ensure [Managers] parent exists
                    GameObject managersParent = GameObject.Find("[Managers]");
                    if (managersParent == null)
                    {
                        managersParent = new GameObject("[Managers]");
                        DontDestroyOnLoad(managersParent);
                    }

                    // Create manager under [Managers]
                    GameObject go = new GameObject(typeof(T).Name + " (AutoCreated)");
                    go.transform.SetParent(managersParent.transform);
                    _instance = go.AddComponent<T>();

                    DontDestroyOnLoad(go);
                }
            }

            return _instance;
        }
    }

 

    protected virtual void Awake()
    {
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }
}
