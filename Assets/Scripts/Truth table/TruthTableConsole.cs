using UnityEngine;

public class TruthTableConsole : MonoBehaviour
{
    public TruthTablePopup truthTablePopup; // 👈 THIS MUST EXIST

    public void Interact()
    {
        if (truthTablePopup != null)
            truthTablePopup.OpenPopup();
    }
}