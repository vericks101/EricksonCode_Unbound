using UnityEngine;
using System;
using System.Collections;

[Serializable]
public class Stat 
{
	[SerializeField] private BarScript bar;
	[SerializeField] private float maxVal;
	[SerializeField] private float currentVal;

	public float CurrentVal
	{
		get { return currentVal; }

		set 
		{
			this.currentVal = Mathf.Clamp (value, 0, MaxVal);
			bar.FillAmount = currentVal;
		}
	}

	public float MaxVal
	{
		get { return maxVal; }

		set 
		{ 
			this.maxVal = value;
			bar.MaxValue = maxVal;
		}
	}

	public void Initialize()
	{
		this.MaxVal = maxVal;
		this.CurrentVal = currentVal;
	}
}