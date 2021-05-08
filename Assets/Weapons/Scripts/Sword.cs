using UnityEngine;
using UnityStandardAssets._2D;
using System.Collections;

public class Sword : MonoBehaviour
{
    [SerializeField] private Animator swordAnimator;

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && (!swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("WeaponSwing") && !swordAnimator.GetCurrentAnimatorStateInfo(0).IsName("WeaponSwingLeft"))
            && !Player.Instance.isDead && PlayerNavigation.destinationHolder.GetComponent<CanvasGroup>().alpha == 0f && GameManager.canUseWeapons && !Player.Instance.inBed)
        {
            if (swordAnimator.speed != CharacterPanel.Instance.WeaponSlot.CurrentItem.Item.AttackSpeed)
                swordAnimator.speed = CharacterPanel.Instance.WeaponSlot.CurrentItem.Item.AttackSpeed;
            if (GetComponent<SpriteRenderer>().sprite != CharacterPanel.Instance.WeaponSlot.CurrentItem.itemSprite)
                GetComponent<SpriteRenderer>().sprite = CharacterPanel.Instance.WeaponSlot.CurrentItem.itemSprite;

            if (Player.Instance.GetComponent<PlatformerCharacter2D>().m_FacingRight)
            {
                swordAnimator.Play("WeaponSwing", 0);
                AudioManager.instance.PlaySound("ToolSwing");
            }
            else
            {
                swordAnimator.Play("WeaponSwingLeft", 0);
                AudioManager.instance.PlaySound("ToolSwing");
            }
        }
    }
}