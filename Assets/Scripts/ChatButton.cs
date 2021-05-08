using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChatButton : MonoBehaviour
{
    public Button button;
    public Text chatContent;
    public Text timeContent;

    private CItem chatLine;

    public void Setup(CItem currentItem, ChatScrollList currentScrollList)
    {
        chatLine = currentItem;
        chatContent.text = chatLine.lineContent;
        timeContent.text = string.Empty;
    }
}