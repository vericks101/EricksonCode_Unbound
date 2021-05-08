using UnityEngine;
using System.Collections;

public class Money : MonoBehaviour
{
    public int amount;
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Player.Instance.Gold += amount;
            MenuManager.Instance.overallIncome += amount;
            AudioManager.instance.PlaySound("Pickup_Coin");
            Destroy(gameObject);
        }
    }
}
