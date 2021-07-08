
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    private static DontDestroy instance = null;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
    }

    private void Update()
    {
        if(GetComponent<Canvas>().worldCamera == null)
            GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
