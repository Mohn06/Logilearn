using UnityEngine;

public class GateSocket : MonoBehaviour
{
    public enum SocketType
    {
        Input,
        Output
    }

    [Header("Socket Setup")]
    public SocketType socketType;
    public MonoBehaviour parentGate;
    public GateSocket connectedSocket;
    public WireConnection currentWire;

    [Header("Visual")]
    public SpriteRenderer socketRenderer;
    public Color offColor = Color.black;
    public Color onColor = Color.cyan;

    void Awake()
    {
        if (socketRenderer == null)
            socketRenderer = GetComponent<SpriteRenderer>();

        AutoAssignParentGate();
    }

    void Update()
    {
        UpdateVisual();
    }

    void AutoAssignParentGate()
    {
        // Keep manually assigned valid logic script
        if (parentGate != null &&
            (parentGate is ILogicValue || parentGate is ILogicInteract))
        {
            return;
        }

        parentGate = null;

        MonoBehaviour[] allScripts = GetComponentsInParent<MonoBehaviour>(true);

        for (int i = 0; i < allScripts.Length; i++)
        {
            MonoBehaviour script = allScripts[i];
            if (script == null) continue;

            if (script is ILogicValue || script is ILogicInteract)
            {
                parentGate = script;
                return;
            }
        }
    }

    public bool GetSignalValue()
    {
        if (socketType == SocketType.Output)
        {
            if (parentGate == null)
                return false;

            if (parentGate is ILogicValue logicValue)
                return logicValue.GetValue();

            if (parentGate is ILogicInteract logicInteract)
                return logicInteract.Output;

            return false;
        }
        else
        {
            if (connectedSocket == null)
                return false;

            return connectedSocket.GetSignalValue();
        }
    }

    void UpdateVisual()
    {
        if (socketRenderer == null) return;

        bool signal = GetSignalValue();
        socketRenderer.color = signal ? onColor : offColor;
    }
}