using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class Jetpack : MonoBehaviour
{
    public float boostForce;
    public float jetpackCost;
    [HideInInspector] public float remainingTime;
    //public float timeToFloat;
    public float effectCooldownTime;
    private float effectCooldownTimer;
    [SerializeField] private GameObject jetpackParticles;
    [SerializeField] private Transform particlePosition;

    public void ChangeDirection(bool right)
    {
        if (right)
            transform.localPosition = new Vector3((transform.localPosition.x * -1f), transform.localPosition.y, transform.localPosition.z);
        else
            transform.localPosition = new Vector3((transform.localPosition.x * -1f), transform.localPosition.y, transform.localPosition.z);
    }

    private void ApplyForce()
    {
        Player.Instance.GetComponent<Rigidbody2D>().AddForce(new Vector2(0f, boostForce));
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) && !Player.Instance.GetComponent<PlatformerCharacter2D>().m_Grounded && Player.Instance.mana.CurrentVal >= jetpackCost /*&& remainingTime > 0f*/)
        {
            //remainingTime -= Time.deltaTime;
            if (effectCooldownTimer <= Time.time)
            {
                ApplyForce();
                effectCooldownTimer = Time.time + effectCooldownTime;
                Player.Instance.mana.CurrentVal -= jetpackCost;
                AudioManager.instance.PlaySound("Jetpack");
                GameObject particles = Instantiate(jetpackParticles, particlePosition.position, transform.rotation);
                Destroy(particles, 1f);
            }
        }
    }
}
