using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Tutorial
{
    public string tutorialTitle;
    public string tutorialText;
    public Sprite tutorialImage;
}

public class TutorialManager : MonoBehaviour
{
    public Tutorial[] tutorials;

    private int currentIndex;
    public Text currentIndexText;
    public Image currentImage;
    public Text currentTitle;
    public Text currentText;

    private void Start()
    {
        currentTitle.text = tutorials[0].tutorialTitle;
        currentText.text = tutorials[0].tutorialText;
        currentIndexText.text = (currentIndex + 1).ToString();
        currentImage.sprite = tutorials[0].tutorialImage;
    }

    public void ChangeCurrentIndex(int increment)
    {
        currentIndex += increment;
        if (currentIndex <= 0)
            currentIndex = 0;
        else if (currentIndex >= tutorials.Length - 1)
            currentIndex = tutorials.Length - 1;

        UpdateCurrentTutorial();
    }

    private void UpdateCurrentTutorial()
    {
        currentTitle.text = tutorials[currentIndex].tutorialTitle;
        currentText.text = tutorials[currentIndex].tutorialText;
        currentIndexText.text = (currentIndex + 1).ToString();
        currentImage.sprite = tutorials[currentIndex].tutorialImage;
    }
}
