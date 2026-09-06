using UnityEngine;

public class ArduinoConsoleInteract : MonoBehaviour, IInteractable
{
    [Header("Popup")]
    public ArduinoPuzzlePopup puzzlePopup;

    [Header("Audio")]
    public AudioClip interactSFX;

    public void Interact()
    {
        OpenConsole();
    }

    public void OpenConsole()
    {
        if (interactSFX != null && AudioManager.Instance != null)
            AudioManager.Instance.PlaySFX(interactSFX);

        if (puzzlePopup != null)
            puzzlePopup.OpenPopup();
    }
}