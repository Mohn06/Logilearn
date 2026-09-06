using UnityEngine;

public class LogicGateClickable : MonoBehaviour
{
    public LogicGateInfoPages info;

    public void OnClicked()
    {
        LogicGatePagedPopup.Instance.Show(info);
    }
}
