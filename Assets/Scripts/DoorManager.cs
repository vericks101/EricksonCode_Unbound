using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorManager : MonoBehaviour
{
    private bool inRange = false;
    private bool notClosed = false;

    [SerializeField] private Sprite doorClosed;
    [SerializeField] private Sprite doorOpen;

    [SerializeField] private BoxCollider2D doorCollider;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
            inRange = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Player")
            inRange = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && inRange)
        {
            if (!notClosed)
            {
                GetComponent<SpriteRenderer>().sprite = doorOpen;
                doorCollider.enabled = false;
                gameObject.layer = 25;
                notClosed = true;
            }
            else
            {
                GetComponent<SpriteRenderer>().sprite = doorClosed;
                doorCollider.enabled = true;
                gameObject.layer = 0;
                notClosed = false;
            }
        }
    }
}