using UnityEngine;

public abstract class ParentAwareSingleton<T> : MonoBehaviour where T : MonoBehaviour
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
    }

    // Called by editor only
    public void EditorReset()
    {
        Instance = null;
    }
}
