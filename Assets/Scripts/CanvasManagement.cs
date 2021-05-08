using UnityEngine;
using System.Collections;

public class CanvasManagement : MonoBehaviour
{
    public static CanvasManagement Instance;

    void Awake()
    {
        if (Instance != null)
        {
            if (Instance != this)
            {
                Destroy(this.gameObject);
            }
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this);
        }
    }
}
