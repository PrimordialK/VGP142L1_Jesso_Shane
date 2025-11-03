using UnityEngine;

[DefaultExecutionOrder(-1000)]
public abstract class Singleton<T> : MonoBehaviour where T : Component
{
    protected static T _instance;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
   public static T Instance
    {
        get
        {
            try
            {
                _instance = FindFirstObjectByType<T>();
            }
            catch (System.Exception e)
            {
                Debug.LogException(e);
                GameObject obj = new GameObject(typeof(T).Name);
                _instance = obj.AddComponent<T>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
       
    }

    // Update is called once per frame
    private void Awake()
    {
        if (!_instance)
        {
            _instance = this as T;
            DontDestroyOnLoad(_instance);
            return;
        }
    }
}
