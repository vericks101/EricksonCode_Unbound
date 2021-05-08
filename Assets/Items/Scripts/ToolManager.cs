using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections;

public class ToolManager : MonoBehaviour
{
    [SerializeField] private Animator toolAnimator;
    [SerializeField] private float swingSpeed;
    [SerializeField] private string toolName;

    public void SwingTool()
    {
        if (!Player.Instance.isDead && (!toolAnimator.GetCurrentAnimatorStateInfo(0).IsName(toolName) && !toolAnimator.GetCurrentAnimatorStateInfo(0).IsName(toolName + "Left")) 
            && MenuManager.Instance.GetComponent<CanvasGroup>().alpha <= 0f)
        {
            if (toolAnimator.speed != InventoryManager.Instance.SelectedSlot.CurrentItem.Item.AttackSpeed 
                && (InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Elementa == CurrentSceneManager.Instance.planetElement || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Elementa == Element.LUMINANT
                || InventoryManager.Instance.SelectedSlot.CurrentItem.Item.Elementa == Element.COMMON))
                toolAnimator.speed = Mathf.Pow(InventoryManager.Instance.SelectedSlot.CurrentItem.Item.AttackSpeed, 0.5f);
            else
                toolAnimator.speed = Mathf.Pow(1f, 0.5f);
            if (GetComponent<SpriteRenderer>().sprite != InventoryManager.Instance.SelectedSlot.CurrentItem.itemSprite)
                GetComponent<SpriteRenderer>().sprite = InventoryManager.Instance.SelectedSlot.CurrentItem.itemSprite;

            if (Player.Instance.GetComponent<PlatformerCharacter2D>().m_FacingRight)
                toolAnimator.Play(toolName, 0);
            else
                toolAnimator.Play(toolName + "Left", 0);
            AudioManager.instance.PlaySound("ToolSwing");
        }
    }
}