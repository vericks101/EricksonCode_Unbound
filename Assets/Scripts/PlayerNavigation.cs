using UnityEngine;

public class PlayerNavigation : MonoBehaviour
{
    public static GameObject destinationHolder;

    void OnEnable()
    {
        destinationHolder = GameObject.Find("DestinationNavigatorHolder");
    }

    public void OnButtonPress()
    {
        destinationHolder.GetComponent<Animator>().SetTrigger("FadeIn");
        //destinationHolder.GetComponent<CanvasGroup>().alpha = 1f;
        destinationHolder.GetComponent<CanvasGroup>().interactable = true;
        destinationHolder.GetComponent<CanvasGroup>().blocksRaycasts = true;
    }
}