using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    public static T Instance { get; private set; }

    protected virtual void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;
        transform.SetParent(null);
        DontDestroyOnLoad(gameObject); // keep between scenes
    }

    // Called by editor only
    public void EditorReset()
    {
        Instance = null;
    }
}

public interface ISingleton
{
    void EditorReset();
}
