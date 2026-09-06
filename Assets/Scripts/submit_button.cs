using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Submit_button : MonoBehaviour
{
    public TextMeshProUGUI[] texts;
    public Button[] textButtons; // buttons ng bawat clickable text
    public GameObject truthTable;

    public void Submit()
    {
        string[] correct = { "0", "1", "0", "1", "0", "1", "0", "1" };

        for (int i = 0; i < texts.Length; i++)
        {
            if (texts[i].text.Trim() != correct[i])
            {
                Debug.Log("Wrong pattern!");
                return;
            }
        }

        truthTable.SetActive(false);

        for (int i = 0; i < textButtons.Length; i++)
        {
            textButtons[i].interactable = false;
        }
    }
}