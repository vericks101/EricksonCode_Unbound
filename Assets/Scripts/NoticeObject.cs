using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticeObject : MonoBehaviour
{
    [HideInInspector] public Image noticeImage;
    [HideInInspector] public Text noticeTitle;
    [HideInInspector] public Text noticeDesc;
    [SerializeField] private float destructTime;

    private void Awake()
    {
        destructTime += Time.time;
        noticeImage = transform.Find("IconImage").GetComponent<Image>();
        noticeTitle = transform.Find("TitleText").GetComponent<Text>();
        noticeDesc = transform.Find("NoticeText").GetComponent<Text>();
    }

    private void Update()
    {
        if (Time.time > destructTime)
            Destroy(gameObject);
    }
}