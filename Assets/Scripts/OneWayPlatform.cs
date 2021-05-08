using UnityEngine;

public class OneWayPlatform : MonoBehaviour
{
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.S))
        {
            gameObject.GetComponent<Collider2D>().enabled = false;
        }
        else if (Player.Instance != null)
        {
            if (Player.Instance.transform.Find("GroundCheck").transform.position.y < transform.position.y)
                gameObject.GetComponent<Collider2D>().enabled = false;
            else
                gameObject.GetComponent<Collider2D>().enabled = true;
        }
    }
}
