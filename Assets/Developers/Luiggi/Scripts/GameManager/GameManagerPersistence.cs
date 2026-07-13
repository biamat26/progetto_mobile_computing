using UnityEngine;

public class GameManagerPersistence : MonoBehaviour
{
    private static GameManagerPersistence istanza;

    void Awake()
    {
        if (istanza != null && istanza != this)
        {
            Destroy(gameObject);
            return;
        }
        istanza = this;

        transform.SetParent(null);
        DontDestroyOnLoad(gameObject);
    }
}