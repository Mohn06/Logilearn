using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class TutorialButtonUI : MonoBehaviour, IPointerClickHandler
{
    public TMP_Text titleText;
    public GameObject newIndicator;

    public Image background;
    public AudioClip prefabbuttonSFX;

    private TutorialSO tutorial;

    private static TutorialButtonUI currentlySelected;

    Color normalBG = new Color(1, 1, 1, 0);
    Color selectedBG = Color.white;

    Color normalText = Color.white;
    Color selectedText = Color.black;
    void PlaySFX()
    {
        AudioManager.Instance.PlaySFX(prefabbuttonSFX);
    }
    public void Setup(TutorialSO tutorialData)
    {
        tutorial = tutorialData;

        titleText.text = tutorial.title;

        bool isNew = TutorialManager.Instance.IsNew(tutorial.tutorialID);
        newIndicator.SetActive(isNew);

        background.color = normalBG;
        titleText.color = normalText;
    }

    public void HideNew()
    {
        newIndicator.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PlaySFX();
        SelectButton();
    }

    public void SelectButton()
    {
        if (currentlySelected != null)
        {
            currentlySelected.background.color = normalBG;
            currentlySelected.titleText.color = normalText;
        }

        background.color = selectedBG;
        titleText.color = selectedText;

        currentlySelected = this;
    }
}