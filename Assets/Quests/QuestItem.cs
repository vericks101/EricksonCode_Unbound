using UnityEngine;
using System.Collections;

public class QuestItem : MonoBehaviour
{
    public int questIndex;
    public string itemName;

    void Start()
    {
        if (GetComponent<ItemScript>() != null)
            itemName = GetComponent<ItemScript>().Item.ItemName;
        else if (GetComponent<SpriteManager>() != null)
            itemName = GetComponent<SpriteManager>().ItemName;
        else
        { }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            //if (!QuestManager.Instance.questCompleted[questIndex] && QuestManager.Instance.quests[questIndex].gameObject.activeSelf)
            //{
                QuestManager.Instance.itemCollected = itemName;
                //gameObject.SetActive(false);
            //}
        }
    }
}