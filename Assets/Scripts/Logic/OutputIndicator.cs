using UnityEngine;
using TMPro;

public class LogicOutputDisplay : MonoBehaviour
{
    public TMP_Text text;

    public void SetValue(bool value)
    {
        text.text = value ? "1" : "0";
        text.color = value ? Color.green : Color.gray;
    }
}