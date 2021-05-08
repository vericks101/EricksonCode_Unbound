using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class CombatTextManager : MonoBehaviour 
{
	[SerializeField] private GameObject textPrefab;
	[SerializeField] private RectTransform canvasTransform;

	[SerializeField] private float speed;
	[SerializeField] private float fadeTime;
	[SerializeField] private Vector3 direction;

    public Hashtable curObjectTable = new Hashtable();

	private static CombatTextManager instance;
	public static CombatTextManager Instance
	{
		get 
		{
			if (instance == null) instance = GameObject.FindObjectOfType<CombatTextManager> ();

			return CombatTextManager.instance;
		}
	}

	public void CreateText(Vector3 position, string text, Color color, bool crit, bool optimize)
	{
        if (!curObjectTable.ContainsKey(text) || !optimize)
        {
            if (optimize)
                curObjectTable.Add(text, null);

            var sct = (GameObject)Instantiate(textPrefab, position, Quaternion.identity);
            sct.transform.SetParent(canvasTransform);
            sct.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
            sct.GetComponent<CombatText>().Initialize(text, speed, direction, fadeTime, crit);
            sct.GetComponent<Text>().text = text;
            sct.GetComponent<Text>().color = color;
        }
	}
}