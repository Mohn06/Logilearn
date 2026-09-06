using UnityEngine;

public class Port : MonoBehaviour
{
    public enum PortType
    {
        Input,
        Output
    }

    [Header("Port Settings")]
    public PortType portType;

    [Tooltip("Index of the input on the gate (only used for Input ports)")]
    public int inputIndex = 0;

    [Header("Logic Node")]
    public MonoBehaviour logicNodeComponent;

    [HideInInspector]
    public ILogicInteract logicNode;

    [Header("Connection")]
    public bool occupied = false;
    public GameObject connectedWire;

    void Awake()
    {
        if (logicNodeComponent != null)
            logicNode = logicNodeComponent as ILogicInteract;

        if (logicNode == null)
        {
            Debug.LogWarning($"{name} Port has no ILogicInteract assigned.");
        }
    }

    public bool CanConnect()
    {
        // Input ports allow only ONE wire
        if (portType == PortType.Input && occupied)
            return false;

        return true;
    }

    public void Connect(GameObject wire)
    {
        occupied = true;
        connectedWire = wire;
    }

    public void Disconnect()
    {
        occupied = false;
        connectedWire = null;
    }
}