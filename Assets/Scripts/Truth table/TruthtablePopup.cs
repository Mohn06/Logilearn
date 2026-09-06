using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TruthTablePopup : MonoBehaviour
{
    [Header("Popup")]
    public GameObject popupPanel;
    public TextMeshProUGUI resultText;

    [Header("Cells")]
    public TextMeshProUGUI[] texts;   // output cell texts
    public Button[] textButtons;      // clickable cell buttons

    [Header("Correct Answers")]
    public string[] correctAnswers = new string[8] { "0", "1", "0", "1", "0", "1", "1", "1" };

    [Header("Door")]
    public TruthTableDoor door;

    private bool solved = false;
    private int selectedIndex = -1;

    void Start()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);

        if (resultText != null)
            resultText.text = "";

        // optional default text
        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] != null)
                texts[i].text = "?";
        }
    }

    public void OpenPopup()
    {
        if (solved) return;

        if (popupPanel != null)
            popupPanel.SetActive(true);

        if (resultText != null)
            resultText.text = "";
    }

    public void ClosePopup()
    {
        if (popupPanel != null)
            popupPanel.SetActive(false);
    }

    // Called when player clicks a cell
    public void SelectCell(int index)
    {
        if (solved) return;

        if (index < 0 || index >= texts.Length)
            return;

        selectedIndex = index;

        Debug.Log("Selected cell: " + index);
    }

    // Called by 0 button
    public void InputZero()
    {
        SetSelectedValue("0");
    }

    // Called by 1 button
    public void InputOne()
    {
        SetSelectedValue("1");
    }

    void SetSelectedValue(string value)
    {
        if (solved) return;
        if (selectedIndex == -1) return;
        if (selectedIndex < 0 || selectedIndex >= texts.Length) return;
        if (texts[selectedIndex] == null) return;

        texts[selectedIndex].text = value;
    }

    public void SubmitAnswers()
    {
        if (solved) return;

        if (texts == null || correctAnswers == null)
        {
            if (resultText != null)
                resultText.text = "Setup error.";
            return;
        }

        if (texts.Length != correctAnswers.Length)
        {
            if (resultText != null)
                resultText.text = "Cells and answers do not match.";
            return;
        }

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i] == null || texts[i].text.Trim() != correctAnswers[i])
            {
                if (resultText != null)
                    resultText.text = "Wrong answer, Try Again.";
                return;
            }
        }

        solved = true;

        if (resultText != null)
            resultText.text = "Correct!";

        // disable all cell buttons
        for (int i = 0; i < textButtons.Length; i++)
        {
            if (textButtons[i] != null)
                textButtons[i].interactable = false;
        }

        if (door != null)
            door.OpenDoor();

        ClosePopup();
    }
}