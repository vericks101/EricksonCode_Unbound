using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerSprite
{
    public string name;
    public Sprite sprite;
}

[System.Serializable]
public class CurrentSprite
{
    public string name;
    public Image sprite;
    public SpriteRenderer spriteRenderer;
}

public class CharacterSpriteManager : MonoBehaviour
{
    public CurrentSprite[] curHairSprites;
    public CurrentSprite[] curHeadSprites;
    public CurrentSprite[] curBodySprites;
    public CurrentSprite[] curFeetSprites;

    public PlayerSprite[] hairSprites;
    public PlayerSprite[] headSprites;
    public PlayerSprite[] bodySprites;
    public PlayerSprite[] feetSprites;

    public void UpdateCurrentImage(int spriteType)
    {
        if (spriteType == 0)
        {
            for (int i = 0; i < curHairSprites.Length; i++)
                curHairSprites[i].sprite.sprite = hairSprites[(int)MainMenuManager.Instance.hairSlider.value].sprite;
        }
        else if (spriteType == 1)
        {
            for (int i = 0; i < curHeadSprites.Length; i++)
                curHeadSprites[i].sprite.sprite = headSprites[(int)MainMenuManager.Instance.headSlider.value].sprite;
        }
        else if (spriteType == 2)
        {
            for (int i = 0; i < curBodySprites.Length; i++)
                curBodySprites[i].sprite.sprite = bodySprites[(int)MainMenuManager.Instance.bodySlider.value].sprite;
        }
        else if (spriteType == 3)
        {
            for (int i = 0; i < curFeetSprites.Length; i++)
                curFeetSprites[i].sprite.sprite = feetSprites[(int)MainMenuManager.Instance.feetSlider.value].sprite;
        }
    }
}
