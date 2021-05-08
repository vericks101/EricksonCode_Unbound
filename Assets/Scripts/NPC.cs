using UnityEngine;

public enum MoveMode { WALKING, IDLE}

public class NPC : MonoBehaviour
{
    private MoveMode moveMode = MoveMode.IDLE;
    private bool hittingWall;
    public float inverseScale = -1;
    public float speed;
    private Transform target;
    public float wallCheckRadius;
    public LayerMask whatIsGround;
    public LayerMask whatIsWall;
    public float turnTimeDelta;
    private float turnTime;
    private bool turn = true;

    private Animator npcAnimator;

    private void Awake()
    { 
        target = transform.Find("Target");
        npcAnimator = GetComponent<Animator>();
    }

    private void FixedUpdate()
    {
        Move();
        npcAnimator.SetFloat("velocity", Mathf.Abs(GetComponent<Rigidbody2D>().velocity.x));
    }

    private void Move()
    {
        if (turn)
        {
            inverseScale *= -1;
            turnTime = Time.time + Random.Range(1f, turnTimeDelta);
            turn = false;

            int random = Random.Range(0, 2);
            if (random == 0)
                moveMode = MoveMode.IDLE;
            else if (random == 1)
                moveMode = MoveMode.WALKING;
        }
        else if (Time.time > turnTime)
            turn = true;

        if (moveMode == MoveMode.WALKING)
        {
            if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(inverseScale, transform.localScale.y, 1f);
                GetComponent<Rigidbody2D>().velocity = new Vector2(speed, GetComponent<Rigidbody2D>().velocity.y);
            }
            else if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(inverseScale, transform.localScale.y, 1f);
                GetComponent<Rigidbody2D>().velocity = new Vector2(-speed, GetComponent<Rigidbody2D>().velocity.y);
            }
        }
    }
}
