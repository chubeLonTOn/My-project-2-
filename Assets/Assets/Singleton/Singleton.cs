using NUnit.Framework.Interfaces;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<T>();
            }

            if (_instance == null)
            {
                var resource = Resources.Load<T>(typeof(T).Name);
                if (resource != null)
                {
                    _instance = Instantiate(resource);
                }
                else
                {
                    Debug.LogError($"Singleton of type {typeof(T).Name} not found in Resources.");
                }
            }
            return _instance;
        }
    }

    public bool DontDestroyOnLoad = true;

    protected virtual void Awake()
    {
        if (_instance != null && _instance != this as T)
        {
            Destroy(this);
            return;
        }
        _instance = this as T;
        if (DontDestroyOnLoad == true)
        {
            DontDestroyOnLoad(gameObject);
        }
    }
    public static bool isSet => _instance != null;

    public static void ForceSetInstance() => _ = Instance;
}
