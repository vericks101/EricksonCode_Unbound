using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UnityStandardAssets._2D
{
    public class Camera2DFollow : MonoBehaviour
    {
        public Transform target;
        public float damping = 1;
        public float lookAheadFactor = 3;
        public float lookAheadReturnSpeed = 0.5f;
        public float lookAheadMoveThreshold = 0.1f;

        private float m_OffsetZ;
        private Vector3 m_LastTargetPosition;
        private Vector3 m_CurrentVelocity;
        private Vector3 m_LookAheadPos;

        public Camera cam;

        public static Camera2DFollow Instance;

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

        private void Start()
        {
            m_LastTargetPosition = target.position;
            m_OffsetZ = (transform.position - target.position).z;
            transform.parent = null;
        }

        private void OnLevelWasLoaded()
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
            {
                if (cam != null)
                    cam.enabled = false;
                if (CanvasManagement.Instance != null)
                {
                    CanvasManagement.Instance.GetComponent<CanvasGroup>().alpha = 0f;
                    CanvasManagement.Instance.GetComponent<CanvasGroup>().interactable = false;
                    CanvasManagement.Instance.GetComponent<CanvasGroup>().blocksRaycasts = false;
                }
            }
            else
            {
                if (cam != null)
                    cam.enabled = true;
                if (CanvasManagement.Instance != null)
                {
                    CanvasManagement.Instance.GetComponent<CanvasGroup>().alpha = 1f;
                    CanvasManagement.Instance.GetComponent<CanvasGroup>().interactable = true;
                    CanvasManagement.Instance.GetComponent<CanvasGroup>().blocksRaycasts = true;
                }
            }
        }

        // Update is called once per frame
        private void Update()
        {
            if (target != null)
            {
                // only update lookahead pos if accelerating or changed direction
                float xMoveDelta = (target.position - m_LastTargetPosition).x;

                bool updateLookAheadTarget = Mathf.Abs(xMoveDelta) > lookAheadMoveThreshold;

                if (updateLookAheadTarget)
                {
                    m_LookAheadPos = lookAheadFactor * Vector3.right * Mathf.Sign(xMoveDelta);
                }
                else
                {
                    m_LookAheadPos = Vector3.MoveTowards(m_LookAheadPos, Vector3.zero, Time.deltaTime * lookAheadReturnSpeed);
                }

                Vector3 aheadTargetPos = target.position + m_LookAheadPos + Vector3.forward * m_OffsetZ;
                Vector3 newPos = Vector3.SmoothDamp(transform.position, aheadTargetPos, ref m_CurrentVelocity, damping);
                if (GameManager.Instance != null /*&& TerrainManager.Instance != null && TerrainManager.Instance.enableBorders*/)
                {
                    if (GameManager.Instance.restrictX)
                    {
                        transform.position = new Vector3(transform.position.x, newPos.y, -1f);
                    }
                    else if (GameManager.Instance.restrictY)
                    {
                        transform.position = new Vector3(newPos.x, transform.position.y, -1f);
                    }
                    else
                        transform.position = newPos;
                    if (GameManager.Instance.restrictY)
                    {
                        transform.position = new Vector3(newPos.x, transform.position.y, -1f);
                    }
                    else if (GameManager.Instance.restrictX)
                    {
                        transform.position = new Vector3(transform.position.x, newPos.y, -1f);
                    }
                    else
                        transform.position = newPos;
                }
                else
                    transform.position = newPos;

                m_LastTargetPosition = target.position;
            }
        }
    }
}