using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(LandGenerator))]
public class HillGeneratorEditor : Editor {

	public override void OnInspectorGUI(){
		base.OnInspectorGUI();
		
		LandGenerator h = (LandGenerator)target;

		
		if (GUILayout.Button("Generate")){
			h.Generate();
		}
		if (GUILayout.Button("Append")){
			h.Append();
		}
		if (GUILayout.Button("Randomise")){
			h.yNoise = Random.Range(0f,1f);
			h.Generate();
		}
	}
	
}
