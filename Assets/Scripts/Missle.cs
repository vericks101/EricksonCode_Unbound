using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missle : MonoBehaviour
{
    public float speed = 5f;
    public float blastRadius = 1f;
    public LayerMask layerMask;
    public float rotatingSpeed = 200f;
    public GameObject target;
    public GameObject explosionPrefab;
    public float selfDestTime;
    private float selfDestTimer;

    public float damage;
    public float knockPow;

    private Rigidbody2D rb;

    private void Start()
    {
        if (target == null)
            target = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();

        selfDestTimer = Time.time + selfDestTime;
    }

    private void FixedUpdate()
    {
        if (Time.time > selfDestTimer)
            Destroy(gameObject);

        Vector2 point2Target = (Vector2)transform.position - (Vector2)target.transform.position;
        point2Target.Normalize();

        float value = Vector3.Cross(point2Target, transform.right).z;
        if (value > 0)
            rb.angularVelocity = rotatingSpeed;
        else if (value < 0)
            rb.angularVelocity = -rotatingSpeed;
        else
            rotatingSpeed = 0f;

        rb.velocity = transform.right * speed;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject tmp = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        AudioManager.instance.PlaySound("Explosion");
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, blastRadius, layerMask);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].GetComponent<Enemy>() != null)
                colliders[i].GetComponent<Enemy>().DamageEnemy(damage, collision.contacts[0].normal, knockPow, false);
        }

        Destroy(gameObject);
        Destroy(tmp, 3f);
    }
}
