using UnityEngine;
using System.Collections;
using UnityStandardAssets._2D;

public class SpawnpointManager : MonoBehaviour
{
    private static SpawnpointManager instance;

    public static SpawnpointManager Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<SpawnpointManager>();
            return instance;
        }
    }

    void Start()
    {
        float tempDamping = 0f;
        if (Camera.main != null)
        {
            tempDamping = Camera.main.GetComponentInParent<Camera2DFollow>().damping;
            Camera.main.GetComponentInParent<Camera2DFollow>().damping = 0f;
        }

        Player.Instance.spawnPoint = transform;
        Player.Instance.transform.position = Player.Instance.spawnPoint.position;
        Player.Instance.GetComponent<FallDamageManager>().enabled = true;
        FindObjectOfType<RestrictTransform>().UpdateInit();

        StartCoroutine("WaitToDamp");
    }

    IEnumerator WaitToDamp()
    {
        yield return new WaitForSeconds(1f);
        if (Camera.main != null)
            Camera.main.GetComponentInParent<Camera2DFollow>().damping = 0.3f;
    }
}
