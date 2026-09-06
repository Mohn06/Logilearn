using System.Collections.Generic;
using UnityEngine;

public class WireSnapPoint : MonoBehaviour
{
    public static readonly List<WireSnapPoint> All = new();

    [Header("Snap Rules")]
    public bool allowSource;
    public bool allowTarget;

    [Header("Logic")]
    public MonoBehaviour logicBehaviour;
    public int inputIndex;

    public ILogicInteract Logic => logicBehaviour as ILogicInteract;

    void OnEnable()
    {
        if (!All.Contains(this))
            All.Add(this);
    }

    void OnDisable()
    {
        All.Remove(this);
    }
}
