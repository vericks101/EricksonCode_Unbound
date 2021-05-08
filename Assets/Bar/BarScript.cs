using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BarScript : MonoBehaviour 
{
	private float fillAmount;

	[SerializeField] private Image content;
	[SerializeField] private Text valueText;
	[SerializeField] private Color fullColor;
	[SerializeField] private Color lowColor;
	[SerializeField] private bool lerpColors;
    [SerializeField] private float lerpSpeed;

	public float MaxValue { get; set; }

	public float FillAmount 
	{
		set 
		{
			valueText.text = value.ToString();
			fillAmount = Map (value, 0, MaxValue, 0, 1); 
		}
	}

	private void Start()
	{
		if (lerpColors)
            content.color = fullColor;
	}

	void Update () 
	{
		HandleBar ();
	}

	private void HandleBar()
	{
		if (fillAmount != content.fillAmount)
            content.fillAmount = Mathf.Lerp(content.fillAmount, fillAmount, Time.deltaTime * lerpSpeed);
		if (lerpColors)
            content.color = Color.Lerp (lowColor, fullColor, fillAmount);
	}

	private float Map(float value, float inMin, float inMax, float outMin, float outMax)
	{
		return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
	}
}
