using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColorPickerTester : MonoBehaviour 
{

    public new Image renderer;
    public ColorPicker picker;

	// Use this for initialization
	void Start () 
    {
        picker.onValueChanged.AddListener(color =>
        {
            renderer.color = color;
        });
		renderer.color = picker.CurrentColor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
