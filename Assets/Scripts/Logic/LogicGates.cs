using System.Data;
using UnityEngine;
using UnityEngine.Events;

public abstract class LogicGates : MonoBehaviour, ILogicInteract
{
    public bool Output { get; private set; }
    protected bool[] inputs;
    public UnityEvent<bool> OnOutputChange;
    
    protected virtual void Awake()
    {
        inputs = new bool[2];
    }

    public void SetInput(bool value, int inputIndex)
    {
        inputs[inputIndex] = value;
        Evaluate();
    }
    protected void SetOutput(bool value)
    {
        if (Output == value) return;
        
            Output = value;
            OnOutputChange?.Invoke(Output);
        
    }
    protected abstract void Evaluate();
}
