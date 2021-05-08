using UnityEngine;
using System.Collections;

public class CameraShake : MonoBehaviour 
{
	public Camera mainCam;
	private static CameraShake instance;

	private float shakeAmount = 0f;

	public static CameraShake Instance
	{
		get 
		{
			if (instance == null) 
				instance = GameObject.Find("GameManagement").GetComponentInChildren<CameraShake> ();

			return CameraShake.instance;
		}
	}

	void Awake()
	{
		if (mainCam == null) 
			mainCam = Camera.main;
	}

	public void Shake(float amt, float length)
	{
		shakeAmount = amt;

		InvokeRepeating ("DoShake", 0, 0.01f);
		Invoke ("StopShake", length);
	}

	void DoShake()
	{
		if (shakeAmount > 0) 
		{
			Vector3 camPos = mainCam.transform.position;

			float offsetX = Random.value * shakeAmount * 2f - shakeAmount;
			float offsetY = Random.value * shakeAmount * 2f - shakeAmount;
			camPos.x += offsetX;
			camPos.y += offsetY;
            camPos.z = mainCam.transform.position.z;

			mainCam.transform.position = camPos;
		}
	}

	void StopShake()
	{
		CancelInvoke ("DoShake");
		mainCam.transform.localPosition = Vector3.zero;
	}
}