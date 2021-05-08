using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tip
{
    public string title;
    public string desc;
}

public class TipManager : MonoBehaviour
{
    [SerializeField] private Text tipTitle;
    [SerializeField] private Text tipDesc;

    [SerializeField] private Tip[] tips;

    private void Start()
    {
        ChooseTip();
    }

    private void ChooseTip()
    {
        for (int i = 0; i < tips.Length; i++)
        {
            int index = Random.Range(0, tips.Length);
            tipTitle.text = tips[index].title;
            tipDesc.text = tips[index].desc;
        }
    }
}
