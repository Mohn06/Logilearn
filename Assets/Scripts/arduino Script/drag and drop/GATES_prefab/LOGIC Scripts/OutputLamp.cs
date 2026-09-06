using UnityEngine;

public class OutputLamp : MonoBehaviour
{
    public GateSocket inputSocket;

    [Header("Lamp Visual")]
    public SpriteRenderer sr;

    public Sprite offSprite;
    public Sprite onSprite;

    public Color offColor = Color.gray;
    public Color onColor = Color.yellow;

    private bool lastValue = false;

    void Update()
    {
        bool value = GetInputValue();

        if (value != lastValue)
        {
            UpdateVisual(value);
            lastValue = value;
        }
    }

    void UpdateVisual(bool state)
    {
        if (sr == null) return;

        // Sprite change
        if (offSprite != null && onSprite != null)
        {
            sr.sprite = state ? onSprite : offSprite;
        }

        // Color change (optional)
        sr.color = state ? onColor : offColor;
    }

    bool GetInputValue()
    {
        if (inputSocket == null)
            return false;

        if (inputSocket.connectedSocket == null)
            return false;

        if (inputSocket.connectedSocket.parentGate == null)
            return false;

        GameObject sourceGate = inputSocket.connectedSocket.parentGate.gameObject;

        ILogicValue logicSource = sourceGate.GetComponent<ILogicValue>();

        if (logicSource == null)
            return false;

        return logicSource.GetValue();
    }
}