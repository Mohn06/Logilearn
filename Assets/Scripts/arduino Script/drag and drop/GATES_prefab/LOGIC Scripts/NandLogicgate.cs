using UnityEngine;

public class NandGateLogic : MonoBehaviour, ILogicValue
{
    public GateSocket inputA;
    public GateSocket inputB;

    public Sprite zeroSprite;
    public Sprite oneSprite;

    private SpriteRenderer sr;
    private bool lastOutput = false;

    void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        ForceOff();
    }

    public bool GetValue()
    {
        if (!HasValidConnection(inputA) || !HasValidConnection(inputB))
        {
            ForceOff();
            return false;
        }

        bool a = GetConnectedValue(inputA);
        bool b = GetConnectedValue(inputB);

        bool result = !(a && b);
        UpdateIfChanged(result);
        return result;
    }

    bool HasValidConnection(GateSocket socket)
    {
        return socket != null &&
               socket.connectedSocket != null &&
               socket.connectedSocket.parentGate != null;
    }

    bool GetConnectedValue(GateSocket socket)
    {
        GameObject sourceGate = socket.connectedSocket.parentGate.gameObject;
        ILogicValue logicSource = sourceGate.GetComponent<ILogicValue>();
        if (logicSource == null) return false;
        return logicSource.GetValue();
    }

    void ForceOff()
    {
        if (lastOutput != false)
        {
            UpdateVisual(false);
            lastOutput = false;
        }
        else
        {
            UpdateVisual(false);
        }
    }

    void UpdateIfChanged(bool value)
    {
        if (value != lastOutput)
        {
            UpdateVisual(value);
            lastOutput = value;
        }
    }

    void UpdateVisual(bool on)
    {
        if (sr == null) return;
        sr.sprite = on ? oneSprite : zeroSprite;
    }
}