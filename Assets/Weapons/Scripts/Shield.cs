using UnityEngine;
using System.Collections;

public class Shield : MonoBehaviour
{
    public float baseShieldDef;
    [HideInInspector] public float shieldDef;
    public float fuelCost;
    [SerializeField] private float shieldKnock;

    void OnEnable()
    {
        GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>(CharacterPanel.Instance.OffHandSlot.CurrentItem.Item.ItemSprite);
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            KnockbackManager.Instance.ApplyKnockback(other.gameObject.GetComponent<Rigidbody2D>(), shieldKnock, -other.contacts[0].normal);
        }
    }
}