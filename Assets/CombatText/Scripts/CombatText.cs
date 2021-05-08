using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CombatText : MonoBehaviour 
{
    public new string name;
	private float speed;
	private Vector3 direction;
	private float fadeTime;

	[SerializeField] private AnimationClip critAnim;

	private void Update()
	{
		float translation = speed * Time.deltaTime;
		transform.Translate (direction * translation);
	}

	public void Initialize(string name, float speed, Vector3 direction, float fadeTime, bool crit)
	{
        this.name = name;
		this.speed = speed;
		this.direction = direction;
		this.fadeTime = fadeTime;

		if (crit) 
		{
			GetComponent<Animator> ().SetTrigger ("Critical");
			crit = false;
			StartCoroutine (Critical ());
		}
		else 
		{
			StartCoroutine (FadeOut ());
		}
    }

	private IEnumerator Critical()
	{
		yield return new WaitForSeconds (critAnim.length);
		StartCoroutine (FadeOut ());
	}

	private IEnumerator FadeOut()
	{
		float startAlpha = GetComponent<Text> ().color.a;
		float rate = 1f / fadeTime;
		float progress = 0f;

		while (progress < 1f) 
		{
			Color tmpColor = GetComponent<Text> ().color;
			GetComponent<Text> ().color = new Color (tmpColor.r, tmpColor.g, tmpColor.b, Mathf.Lerp (startAlpha, 0f, progress));

			progress += rate * Time.deltaTime;

			yield return null;
		}

        if (CombatTextManager.Instance.curObjectTable.ContainsKey(name))
            CombatTextManager.Instance.curObjectTable.Remove(name);
        Destroy (gameObject);
	}
}