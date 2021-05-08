using UnityEngine;
using UnityEngine.UI;

public class StatusIndicator : MonoBehaviour
{
    [SerializeField] private RectTransform healthBarRect;
    [SerializeField] private Text healthText;
    [SerializeField] private Text enemyNameText;

    private void Awake()
    {
        enemyNameText.text = GetComponentInParent<Enemy>().enemyName;
    }

    public void SetHealth(int cur, int max)
    {
        float value = (float)cur / max;

        healthBarRect.localScale = new Vector3(value, healthBarRect.localScale.y, healthBarRect.localScale.z);
        healthText.text = cur + "/" + max + " HP";
    }
}
